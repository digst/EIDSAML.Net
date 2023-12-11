using System;
using eid.saml20.Profiles.Eidas;
using eid.saml20.Schema.Core;
using eid.saml20.Schema.Protocol;

namespace eid.saml20.Validation
{
    internal class EidasSaml20AttributeValidator : ISaml20AttributeValidator
    {
        public void ValidateAttribute(SamlAttribute samlAttribute)
        {            
            if (!Uri.IsWellFormedUriString(samlAttribute.Name, UriKind.Absolute))
                throw new DKSaml20FormatException("The SAML profile requires that an attribute's \"Name\" is an URI.");

            if (samlAttribute.AttributeValue == null)
                return;

            foreach (object val in samlAttribute.AttributeValue)
            {
                if (val is string)
                    continue;

                throw new DKSaml20FormatException("The SAML profile requires that all attribute values are of type \"xs:string\".");
            }
        }

        public void ValidateEncryptedAttribute(EncryptedElement encryptedElement)
        {
            throw new DKSaml20FormatException("The SAML profile does not support the EncryptedAttribute element");
        }
    }
}