using System.Web;
using eid.saml20.Actions;
using eid.saml20.identity;
using eid.saml20.Logging;
using eid.saml20.protocol;

namespace WebsiteDemo
{
    public class LogAction: IAction
    {
        #region IAction Members

        public void LoginAction(eid.saml20.protocol.AbstractEndpointHandler handler, HttpContext context, eid.saml20.Saml20Assertion assertion)
        {
            // Since FormsAuthentication is used in this sample, the user name to log can be found in context.User.Identity.Name.
            // This user will not be set until after a new redirect, so unfortunately we cannot just log it here,
            // but will have to do in MyPage.Load in order to log the local user id
        }

        public void LogoutAction(eid.saml20.protocol.AbstractEndpointHandler handler, HttpContext context, bool IdPInitiated)
        {
            // Example of logging required by the requirements SLO1 ("Id of internal user account")
            // Since FormsAuthentication is used in this sample, the user name to log can be found in context.User.Identity.Name
            // The login will be not be cleared until next redirect due to the way FormsAuthentication works, so we will have to check Saml20Identity.IsInitialized() too
            AuditLogging.logEntry(Direction.IN, Operation.LOGOUT, "ServiceProvider logout",
                "SP local user id: " + (context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "none") + " login status: " + Saml20Identity.IsInitialized());
        }

        /// <summary>
        /// <see cref="IAction.SoapLogoutAction"/>
        /// </summary>
        public void SoapLogoutAction(AbstractEndpointHandler handler, HttpContext context, string userId)
        {
            AuditLogging.logEntry(Direction.IN, Operation.LOGOUT, "ServiceProvider SOAP logout",
                "IdP user id: " + userId + " login status: " + Saml20Identity.IsInitialized());
        }

        public string Name
        {
            get { return "LogAction"; }
            set
            {
                
            }
        }

        #endregion
    }
}
