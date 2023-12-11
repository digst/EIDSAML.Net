﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;
using eid.saml20;
using eid.saml20.config;
using eid.saml20.protocol;
using IdentityProviderDemo.config;
using IdentityProviderDemo.Logic;
using eid.saml20.Schema.Protocol;
using eid.saml20.Schema.Core;
using eid.saml20.Bindings;
using eid.saml20.Bindings.SignatureProviders;
using eid.saml20.Utils;
using System.Linq;
using eid.saml20.Schema.Metadata;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using eid.saml20.Specification;

namespace IdentityProviderDemo
{
    public partial class SignonForm : Page
    {
        private AuthnRequest request;
        private string _loaLevel;

        protected Dictionary<string, User> Users { get { return UserData.Users; } }

        protected override void OnInit(EventArgs e)
        {
            request = Context.Session["authenticationrequest"] as AuthnRequest;

            if (request == null)
            {
                HandleRequestMissing();
                return;
            }

            if (request.RequestedAuthnContext != null)
            {
                for (int i = 0; i < request.RequestedAuthnContext.ItemsElementName.Length; i++)
                {
                    var elementName = request.RequestedAuthnContext.ItemsElementName[i];
                    if (elementName == ItemsChoiceType7.AuthnContextClassRef)
                    {
                        if (request.RequestedAuthnContext.Items.Length <= i)
                        {
                            Context.Response.Write(string.Format("The RequestedAuthnContext {0} could not be determined.", i));
                            Context.Response.End();
                            return;
                        }

                        SPDesiredContext.Text += request.RequestedAuthnContext.Items[i] + "<br/>";
                    }
                }

                if (!string.IsNullOrEmpty(SPDesiredContext.Text))
                    DemandArea.Visible = true;
            }

            User user = UserSessionsHandler.CurrentUser;
        }

        private void HandleRequestMissing()
        {
            Context.Response.Write("This page cannot be accessed directly.");
            Context.Response.End();
        }

        protected void Page_Load(object sender, EventArgs e)
        { }

        protected void AuthenticateUser(object sender, EventArgs e)
        {
            MessageLbl.Text = string.Empty;
            if (!UserData.Users.ContainsKey(UsernameTextbox.Text))
            {
                ErrorLabel.Text = "Unknown user";
                ErrorLabel.Visible = true;
                return;
            }

            User user = UserData.Users[UsernameTextbox.Text];

            if (user.Password != PasswordTextbox.Text)
            {
                ErrorLabel.Text = "Bad password";
                ErrorLabel.Visible = true;
                return;
            }
            else
            {
                SetLevelOfAssurance(user);
                UserSessionsHandler.CurrentUser = user;
                WriteCommonDomainCookie();
                CreateAssertionResponse(user);
                return;
            }
        }

        private void SetLevelOfAssurance(User user)
        {
            _loaLevel = LoaLow.Text;

            if (LoaHigh.Checked)
                _loaLevel = LoaHigh.Text;

            if (LoaSubstantial.Checked)
                _loaLevel = LoaSubstantial.Text;
        }

        private void CreateAuthenticationFailedResponse()
        {
            string entityId = request.Issuer.Value;
            Saml20MetadataDocument metadataDocument = IDPConfig.GetServiceProviderMetadata(entityId);
            IDPEndPointElement endpoint =
                metadataDocument.AssertionConsumerServiceEndpoints().Find(delegate (IDPEndPointElement e) { return e.Binding == SAMLBinding.POST; });

            if (endpoint == null)
            {
                Context.Response.Write(string.Format("'{0}' does not have a SSO endpoint that supports the POST binding.", entityId));
                Context.Response.End();
                return;
            }

            Response response = new Response();
            response.Destination = endpoint.Url;
            response.Status = new Status();
            response.Status.StatusCode = new StatusCode();
            response.Status.StatusCode.Value = Saml20Constants.StatusCodes.Requester;

            response.Status.StatusCode.SubStatusCode = new StatusCode();
            response.Status.StatusCode.SubStatusCode.Value = Saml20Constants.StatusCodes.AuthnFailed;
            response.Status.StatusMessage = "Authentication failed. Username and/or password was incorrect.";

            HttpPostBindingBuilder builder = new HttpPostBindingBuilder(endpoint);
            builder.Action = SAMLAction.SAMLResponse;
            builder.Response = Serialization.SerializeToXmlString(response);

            builder.GetPage().ProcessRequest(Context);
            Context.Response.End();
        }

        private void CreateAssertionResponse(User user)
        {
            string entityId = request.Issuer.Value;
            Saml20MetadataDocument metadataDocument = IDPConfig.GetServiceProviderMetadata(entityId);
            IDPEndPointElement endpoint =
                metadataDocument.AssertionConsumerServiceEndpoints().Find(delegate (IDPEndPointElement e) { return e.Binding == SAMLBinding.POST; });

            if (endpoint == null)
            {
                Context.Response.Write(string.Format("'{0}' does not have a SSO endpoint that supports the POST binding.", entityId));
                Context.Response.End();
                return;
            }

            UserSessionsHandler.AddLoggedInSession(entityId);

            Response response = new Response();
            response.Destination = endpoint.Url;
            response.InResponseTo = request.ID;
            response.Status = new Status();
            response.Status.StatusCode = new StatusCode();
            response.Status.StatusCode.Value = Saml20Constants.StatusCodes.Success;

            var nameIdFormat = metadataDocument.Entity.Items.OfType<SPSSODescriptor>().SingleOrDefault()?.NameIDFormat.SingleOrDefault() ?? Saml20Constants.NameIdentifierFormats.Persistent;
            List<SamlAttribute> requestedAttribtes = GetRequestedAttributes(metadataDocument);
            Assertion assertion = CreateAssertion(user, entityId, nameIdFormat, requestedAttribtes);

            var signatureProvider = SignatureProviderFactory.CreateFromShaHashingAlgorithmName(ShaHashingAlgorithm.SHA256);
            EncryptedAssertion encryptedAssertion = null;

            var keyDescriptors = metadataDocument.Keys.Where(x => x.use == KeyTypes.encryption);
            if (keyDescriptors.Any())
            {
                 foreach (KeyDescriptor keyDescriptor in keyDescriptors)
                {
                    KeyInfo ki = (KeyInfo)keyDescriptor.KeyInfo;

                    foreach (KeyInfoClause clause in ki)
                    {
                        if (clause is KeyInfoX509Data)
                        {
                            X509Certificate2 cert = XmlSignatureUtils.GetCertificateFromKeyInfo((KeyInfoX509Data)clause);

                            var spec = new DefaultCertificateSpecification();
                            string error;
                            if (spec.IsSatisfiedBy(cert, out error))
                            {
                                AsymmetricAlgorithm key = XmlSignatureUtils.ExtractKey(clause);
                                AssertionEncryptionUtility.AssertionEncryptionUtility encryptedAssertionUtil = new AssertionEncryptionUtility.AssertionEncryptionUtility((RSA)key, assertion);

                                // Sign the assertion inside the response message.
                                signatureProvider.SignAssertion(encryptedAssertionUtil.Assertion, assertion.ID, IDPConfig.IDPCertificate);

                                encryptedAssertionUtil.Encrypt();
                                encryptedAssertion = Serialization.DeserializeFromXmlString<EncryptedAssertion>(encryptedAssertionUtil.EncryptedAssertion.OuterXml);
                                break;
                            }
                        }
                    }
                    if (encryptedAssertion != null)
                    {
                        break;
                    }
                }

                if (encryptedAssertion == null)
                    throw new Exception("Could not encrypt. No valid certificates found.");
            }

            if(encryptedAssertion!= null)
            {
                response.Items = new object[] { encryptedAssertion };

            }
            else
            {
                response.Items = new object[] { assertion };
            }
         
            // Serialize the response.
            XmlDocument responseDoc = new XmlDocument();
            responseDoc.XmlResolver = null;
            responseDoc.PreserveWhitespace = true;
            responseDoc.LoadXml(Serialization.SerializeToXmlString(response));

            if (encryptedAssertion == null)
            {
                // Sign the assertion inside the response message.
                signatureProvider.SignAssertion(responseDoc, assertion.ID, IDPConfig.IDPCertificate);
            }

            HttpPostBindingBuilder builder = new HttpPostBindingBuilder(endpoint);
            builder.Action = SAMLAction.SAMLResponse;
         
            builder.Response = responseDoc.OuterXml;

            builder.GetPage().ProcessRequest(Context);
            Context.Response.End();
        }

        private void WriteCommonDomainCookie()
        {
            string cookieValue = HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.ASCII.GetBytes(IDPConfig.ServerBaseUrl)));

            HttpCookie cdc = new HttpCookie(CommonDomainCookie.COMMON_DOMAIN_COOKIE_NAME, cookieValue);
            cdc.Domain = "." + Context.Request.Url.Host;
            Context.Response.Cookies.Add(cdc);
        }
        
        private Assertion CreateAssertion(User user, string receiver, string nameIdFormat, List<SamlAttribute> requestedAttribtes)
        {            
            Assertion assertion = new Assertion();

            { // Subject element                
                assertion.Subject = new Subject();
                assertion.ID = "id" + Guid.NewGuid().ToString("N");
                assertion.IssueInstant = DateTime.Now.AddMinutes(10);

                assertion.Issuer = new NameID();
                assertion.Issuer.Format = Saml20Constants.NameIdentifierFormats.Entity;
                assertion.Issuer.Value = IDPConfig.ServerBaseUrl;

                SubjectConfirmation subjectConfirmation = new SubjectConfirmation();
                subjectConfirmation.Method = SubjectConfirmation.BEARER_METHOD;
                subjectConfirmation.SubjectConfirmationData = new SubjectConfirmationData();
                subjectConfirmation.SubjectConfirmationData.NotOnOrAfter = DateTime.Now.AddHours(1);
                subjectConfirmation.SubjectConfirmationData.Recipient = receiver;

                NameID nameId = new NameID();
                nameId.Format = nameIdFormat;
                if (nameIdFormat == Saml20Constants.NameIdentifierFormats.Transient)
                    nameId.Value = $"EU/DK/" + Guid.NewGuid();
                else
                    nameId.Value = $"{user.NameId}";

                assertion.Subject.Items = new object[] { nameId, subjectConfirmation };
            }

            { // Conditions element
                assertion.Conditions = new Conditions();
                assertion.Conditions.Items = new List<ConditionAbstract>();

                assertion.Conditions.NotOnOrAfter = DateTime.Now.AddHours(1);

                AudienceRestriction audienceRestriction = new AudienceRestriction();
                audienceRestriction.Audience = new List<string>();
                audienceRestriction.Audience.Add(receiver);
                assertion.Conditions.Items.Add(audienceRestriction);
            }

            List<StatementAbstract> statements = new List<StatementAbstract>(2);
            { // AuthnStatement element
                AuthnStatement authnStatement = new AuthnStatement();
                authnStatement.AuthnInstant = DateTime.Now;
                authnStatement.SessionIndex = Convert.ToString(new Random().Next());

                authnStatement.AuthnContext = new AuthnContext();

                authnStatement.AuthnContext.Items =
                    new object[] { $"http://eidas.europa.eu/LoA/{_loaLevel.ToLowerInvariant()}" };

                // Wow! Setting the AuthnContext is .... verbose.
                authnStatement.AuthnContext.ItemsElementName =
                    new ItemsChoiceType5[] { ItemsChoiceType5.AuthnContextClassRef };

                statements.Add(authnStatement);
            }

            { // Generate attribute list.                
                AttributeStatement attributeStatement = new AttributeStatement();

                List<SamlAttribute> attributes = new List<SamlAttribute>(user.Attributes.Count);
                foreach (KeyValuePair<string, string> att in user.Attributes)
                {
                    var existingAttribute = attributes.FirstOrDefault(x => x.Name == att.Key);
                    if (existingAttribute != null)
                    {
                        var attributesValues = new List<string>();
                        attributesValues.AddRange(existingAttribute.AttributeValue);
                        attributesValues.Add(att.Value);
                        existingAttribute.AttributeValue = attributesValues.ToArray();
                    }
                    else
                    {
                        SamlAttribute attribute = new SamlAttribute();
                        attribute.Name = att.Key;
                        attribute.AttributeValue = new string[] { att.Value };
                        attribute.NameFormat = SamlAttribute.NAMEFORMAT_URI;

                        if(requestedAttribtes.Any(ra => ra.Name == attribute.Name))
                            attributes.Add(attribute);
                    }
                }


                attributeStatement.Items = attributes.ToArray();

                statements.Add(attributeStatement);
            }

            assertion.Items = statements.ToArray();

            return assertion;
        }

        private List<SamlAttribute> GetRequestedAttributes(Saml20MetadataDocument serviceProviderMetadata) {
            List<SamlAttribute> requestedAttributes = new List<SamlAttribute>();
            foreach (object item in serviceProviderMetadata.Entity.Items)
            {
                if (item is SPSSODescriptor)
                {
                    SPSSODescriptor descriptor = (SPSSODescriptor)item;
                    
                    descriptor.AttributeConsumingService.ToList().ForEach(acs => requestedAttributes.AddRange(acs.RequestedAttribute));
                }
            }

            return requestedAttributes;
        }
    }
}
