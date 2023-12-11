using eid.saml20.Schema.Core;

namespace eid.saml20.Validation
{
    interface ISaml20SubjectValidator
    {
        void ValidateSubject(Subject subject);
    }
}