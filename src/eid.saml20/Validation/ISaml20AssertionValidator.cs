using System;
using eid.saml20.Schema.Core;

namespace eid.saml20.Validation
{
    internal interface ISaml20AssertionValidator
    {
        void ValidateAssertion(Assertion assertion);
        void ValidateTimeRestrictions(Assertion assertion, TimeSpan allowedClockSkew, DateTime currentUtcTime);
    }
}