using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using eid.saml20.session;
using eid.saml20.config;
using System.Web.SessionState;
using System.Xml;
using eid.saml20.Logging;
using eid.saml20.protocol.pages;
using eid.saml20.Session;
using Trace = eid.saml20.Utils.Trace;

namespace eid.saml20.protocol
{
    /// <summary>
    /// A base class for all endpoint handlers.
    /// </summary>
    public abstract class AbstractEndpointHandler : IHttpHandler, IRequiresSessionState
    {
        #region IHttpHandler Members

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public abstract void ProcessRequest(HttpContext context);

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Displays an error page.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="overrideConfigSetting">if set to <c>true</c> [override config setting].</param>
        /// <param name="exceptionCreatorFunc"></param>
        public void HandleError(HttpContext context, string errorMessage, bool overrideConfigSetting, Func<string, Saml20Exception> exceptionCreatorFunc)
        {
            Trace.TraceData(TraceEventType.Error, "Error: " + errorMessage);

            var showError = EidSAML20FederationConfig.GetConfig().ShowError;
            const string defaultMessage = "Unable to validate SAML message!";

            if (!string.IsNullOrEmpty(ErrorBehaviour) && ErrorBehaviour.Equals(eid.saml20.config.ErrorBehaviour.THROWEXCEPTION.ToString()))
            {
                var exception = showError ? exceptionCreatorFunc(errorMessage) : exceptionCreatorFunc(defaultMessage);
                throw exception;
            }
            else
            {
                var page = new ErrorPage();
                page.OverrideConfig = overrideConfigSetting;
                page.ErrorText = showError ? errorMessage?.Replace("\n", "<br />") : defaultMessage;
                page.ProcessRequest(context);
                context.Response.End();
            }
        }

        /// <summary>
        /// Invoked when a LoA validation has failed. Adds a log entry to the audit log and displays an error page.
        /// </summary>
        protected void HandleLoaValidationError(string errorMessageTemplate, string sourceLoa, string requiredMinLoa, 
            HttpContext context, XmlElement assertionXml)
        {
            var loaErrorMessage = string.Format(errorMessageTemplate, sourceLoa, requiredMinLoa);
            
            AuditLogging.logEntry(Direction.IN, Operation.AUTHNREQUEST_POST,
                loaErrorMessage + " Assertion: " + assertionXml.OuterXml);
            
            HandleError(context, loaErrorMessage, (m) => new Saml20LoaException(m));
        }

        /// <summary>
        /// Displays an error page.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="format">The format string of the error message</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void HandleError(HttpContext context, string format, params string[] args)
        {
            var htmlEncodedArguments = args.Select(WebUtility.HtmlEncode).Cast<object>().ToArray();
            var errorMessage = string.Format(format, htmlEncodedArguments);
            HandleError(context, errorMessage, false, m => new Saml20Exception(m));
        }
        
        /// <summary>
        /// Displays an error page.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="errorMessage">The error message.</param>
        public void HandleError(HttpContext context, string errorMessage)
        {
            HandleError(context, errorMessage, false, m => new Saml20Exception(m));
        }


        /// <summary>
        /// Displays an error page.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="overrideConfigSetting"></param>
        public void HandleError(HttpContext context, string errorMessage, bool overrideConfigSetting)
        {
            HandleError(context, errorMessage, overrideConfigSetting, m => new Saml20Exception(m));
        }

        /// <summary>
        /// Displays an error page.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="exceptionCreatorFunc"></param>
        public void HandleError(HttpContext context, string errorMessage, Func<string, Saml20Exception> exceptionCreatorFunc)
        {
            HandleError(context, errorMessage, false, exceptionCreatorFunc);
        }

        /// <summary>
        /// Displays an error page.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="e">The exception that caused the error.</param>
        public void HandleError(HttpContext context, Exception e)
        {
            // ThreadAbortException is just part of ASP.NET's slightly broken conditional logic, so don't react to it.
            if (e is ThreadAbortException)
                return;

            StringBuilder sb = new StringBuilder(1000);
            while (e != null)
            {
                sb.AppendLine(e.ToString());
                e = e.InnerException;
            }

            HandleError(context, sb.ToString());
        }

        private string _errorBehaviour;

        /// <summary>
        /// Gets or sets the error handling behaviour.
        /// </summary>
        /// <value>The error handling behaviour.</value>
        public string ErrorBehaviour
        {
            get { return _errorBehaviour; }
            set { _errorBehaviour = value; }
        }

        private string _redirectUrl;

        /// <summary>
        /// Gets or sets the default redirect URL.
        /// </summary>
        /// <value>The redirect URL.</value>
        public string RedirectUrl
        {
            get { return _redirectUrl; }
            set { _redirectUrl = value; }
        }

        /// <summary>
        /// Redirects the user.
        /// </summary>
        /// <param name="context">The context.</param>
        public void DoRedirect(HttpContext context)
        {
            var currentSession = SessionStore.CurrentSession;
            if (currentSession != null)
            {
                var redirectUrl = (string)currentSession[SessionConstants.RedirectUrl];
                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    currentSession[SessionConstants.RedirectUrl] = null;
                    context.Response.Redirect(redirectUrl);
                    return;
                }
            }

            // Use default redirect url
            context.Response.Redirect(string.IsNullOrEmpty(RedirectUrl) ? "~/" : RedirectUrl);
        }
    }
}