using eid.saml20.Schema.Core;
using eid.saml20.Schema.Protocol;

namespace eid.saml20.Validation
{
    internal interface ISaml20AttributeValidator
    {
        void ValidateAttribute(SamlAttribute samlAttribute);
        void ValidateEncryptedAttribute(EncryptedElement encryptedElement);
    }
}