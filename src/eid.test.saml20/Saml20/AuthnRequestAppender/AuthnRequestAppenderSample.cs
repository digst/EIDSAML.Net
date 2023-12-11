using System.Web;
using eid.saml20;
using eid.saml20.AuthnRequestAppender;

namespace eid.test.saml20.AuthnRequestAppender
{
    /// <summary>
    /// Just a sample implementation of IAuthnRequestAppender to verify that AuthnRequestAppenderFactory works
    /// </summary>
    public class AuthnRequestAppenderSample : IAuthnRequestAppender
    {
        public void AppendAction(Saml20AuthnRequest authnRequest, HttpRequest request)
        {
        }
    }
}