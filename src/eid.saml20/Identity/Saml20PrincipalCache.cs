﻿using System.Security.Principal;
using eid.saml20.Session;
using eid.saml20.session;
using eid.saml20.identity;

namespace eid.saml20.Identity
{
    /// <summary>
    /// 
    /// </summary>
    internal class Saml20PrincipalCache
    {
        /// <summary>
        /// Gets the principal.
        /// </summary>
        /// <returns></returns>
        internal static IPrincipal GetPrincipal()
        {
            var saml20Assertion = GetSaml20AssertionLite();
            if (saml20Assertion != null)
                return Saml20Identity.InitSaml20Identity(saml20Assertion);
            return null;
        }

        /// <summary>
        /// Gets the principal.
        /// </summary>
        /// <returns></returns>
        internal static Saml20AssertionLite GetSaml20AssertionLite()
        {
            return SessionStore.CurrentSession?[SessionConstants.Saml20AssertionLite] as Saml20AssertionLite;
        }
    }
}
