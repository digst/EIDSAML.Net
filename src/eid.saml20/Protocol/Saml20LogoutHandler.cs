using System;
using System.Diagnostics;
using System.Threading;
using System.Web;
using eid.saml20.Session;
using eid.saml20.identity;
using eid.saml20.config;
using Trace = eid.saml20.Utils.Trace;
using eid.saml20.Actions;

namespace eid.saml20.protocol
{
    /// <summary>
    /// Handles logout for all SAML bindings.
    /// </summary>
    public class Saml20LogoutHandler : Saml20AbstractEndpointHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20LogoutHandler"/> class.
        /// </summary>
        public Saml20LogoutHandler()
        {
            // Read the proper redirect url from config
            try
            {
                RedirectUrl = EidSAML20FederationConfig.GetConfig().ServiceProvider.LogoutEndpoint.RedirectUrl;
                ErrorBehaviour = EidSAML20FederationConfig.GetConfig().ServiceProvider.LogoutEndpoint.ErrorBehaviour.ToString();
            }
            catch (Exception e)
            {
                if (Trace.ShouldTrace(TraceEventType.Error))
                    Trace.TraceData(TraceEventType.Error, e.ToString());
            }
        }

        #region IHttpHandler related

        /// <summary>
        /// Handles a request.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void Handle(HttpContext context)
        {
            Trace.TraceMethodCalled(GetType(), "Handle()");

            try
            {
                // Just logout locally since no sessions are stored on the gateway.
                DoLogout(context);
            }
            catch (Exception e)
            {
                //ThreadAbortException is thrown by response.Redirect so don't worry about it
                if (e is ThreadAbortException)
                    throw;

                HandleError(context, e.Message);
            }
        }

        #endregion

        #region Private utility functions

        private void DoLogout(HttpContext context)
        {
            DoLogout(context, false);
        }

        private void DoLogout(HttpContext context, bool IdPInitiated)
        {

            try
            {
                foreach (IAction action in Actions.Actions.GetActions())
                {
                    Trace.TraceMethodCalled(action.GetType(), "LogoutAction()");

                    action.LogoutAction(this, context, IdPInitiated);

                    Trace.TraceMethodDone(action.GetType(), "LogoutAction()");
                }
            }
            finally
            {
                if (SessionStore.CurrentSession != null)
                {
                    // Always end with abandoning the session.
                    Trace.TraceData(TraceEventType.Information, "Clearing session for userId: " + Saml20Identity.Current.Name);
                    SessionStore.AbandonAllSessions(Saml20Identity.Current.Name);
                    Trace.TraceData(TraceEventType.Verbose, "Session cleared.");
                }
                else
                {
                    Trace.TraceData(TraceEventType.Warning, "The user was logged out but the session had already expired. Distributed session could therefore not be abandoned");
                }
            }
        }        

        #endregion
    }
}
