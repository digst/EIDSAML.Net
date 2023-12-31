using eid.saml20.Profiles.Eidas;
using eid.saml20.Schema.Core;
using eid.saml20.Utils;

namespace eid.saml20.Validation
{
    internal class EidasSaml20SubjectConfirmationValidator : ISaml20SubjectConfirmationValidator
    {
        public void ValidateSubjectConfirmation(SubjectConfirmation subjectConfirmation)
        {
            if (subjectConfirmation.Method == SubjectConfirmation.BEARER_METHOD)
            {
                if (subjectConfirmation.SubjectConfirmationData == null)
                    throw new DKSaml20FormatException("The SAML profile requires that the bearer \"SubjectConfirmation\" element contains a \"SubjectConfirmationData\" element.");

                if (!Saml20Utils.ValidateRequiredString(subjectConfirmation.SubjectConfirmationData.Recipient))
                    throw new DKSaml20FormatException("The SAML profile requires that the \"SubjectConfirmationData\" element contains the \"Recipient\" attribute.");

                if (!subjectConfirmation.SubjectConfirmationData.NotOnOrAfter.HasValue)
                    throw new DKSaml20FormatException("The SAML profile requires that the \"SubjectConfirmationData\" element contains the \"NotOnOrAfter\" attribute.");

                if (subjectConfirmation.SubjectConfirmationData.NotBefore.HasValue)
                    throw new DKSaml20FormatException("The SAML profile disallows the use of the \"NotBefore\" attribute of the \"SubjectConfirmationData\" element.");
            }
        }
    }
}