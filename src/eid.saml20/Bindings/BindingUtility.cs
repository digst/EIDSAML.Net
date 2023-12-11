using System;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using eid.saml20.config;
using eid.saml20.Properties;

namespace eid.saml20.Bindings
{
    /// <summary>
    /// Utility functions for use in binding implementations.
    /// </summary>
    public class BindingUtility
    {
        /// <summary>
        /// Validates the SAML20Federation configuration.
        /// </summary>
        /// <param name="errorMessage">The error message. If validation passes, it will be an empty string. Otherwise it will contain a userfriendly message.</param>
        /// <returns>True if validation passes, false otherwise</returns>
        public static bool ValidateConfiguration(out string errorMessage)
        {
            EidSAML20FederationConfig _config;

            try
            {
                _config = EidSAML20FederationConfig.GetConfig();
                if (_config == null)
                {
                    errorMessage = HttpUtility.HtmlEncode(Saml20Resources.MissingSaml20Federation);
                    return false;
                }
                if (_config.ServiceProvider == null)
                {
                    errorMessage =
                        HttpUtility.HtmlEncode(Saml20Resources.MissingServiceProvider);
                    return false;
                }
                if (string.IsNullOrEmpty(_config.ServiceProvider.ID))
                {
                    errorMessage =
                        HttpUtility.HtmlEncode(Saml20Resources.MissingServiceProviderId);
                    return false;
                }
                if (EidFederationConfig.GetConfig().SigningCertificate == null)
                {
                    errorMessage = HttpUtility.HtmlEncode(Saml20Resources.MissingSigningCertificate);
                    return false;
                }
                try
                {                    
                    X509Certificate2 signingCert = EidFederationConfig.GetConfig().SigningCertificate.GetCertificate();
                    if (!signingCert.HasPrivateKey)
                    {
                        errorMessage = Saml20Resources.SigningCertificateMissingPrivateKey;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = HttpUtility.HtmlEncode(Saml20Resources.SigningCertficateLoadError) + ex.Message;
                    return false;
                }

                if (EidFederationConfig.GetConfig().EncryptionCertificate == null)
                {
                    errorMessage = HttpUtility.HtmlEncode(Saml20Resources.MissingEncryptionCertificate);
                    return false;
                }
                try
                {
                    X509Certificate2 signingCert = EidFederationConfig.GetConfig().EncryptionCertificate.GetCertificate();
                    if (!signingCert.HasPrivateKey)
                    {
                        errorMessage = Saml20Resources.EncryptionCertificateMissingPrivateKey;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = HttpUtility.HtmlEncode(Saml20Resources.EncryptionCertficateLoadError) + ex.Message;
                    return false;
                }

                if (_config.IDPEndPoints == null)
                {
                    errorMessage = HttpUtility.HtmlEncode(Saml20Resources.MissingIDPEndpoints);
                    return false;
                }

                if (_config.Endpoints.MetadataLocation == null)
                {
                    errorMessage = HttpUtility.HtmlEncode(Saml20Resources.MissingMetadataLocation);
                    return false;
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}