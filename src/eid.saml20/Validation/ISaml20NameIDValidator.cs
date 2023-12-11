using eid.saml20.Schema.Core;
using eid.saml20.Schema.Protocol;

namespace eid.saml20.Validation
{
    internal interface ISaml20NameIDValidator
    {
        void ValidateNameID(NameID nameID);
        void ValidateEncryptedID(EncryptedElement encryptedID);
    }
}