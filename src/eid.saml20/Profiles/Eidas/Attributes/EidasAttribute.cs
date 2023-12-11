using eid.saml20.Schema.Core;

namespace eid.saml20.Profiles.Eidas.Attributes
{
    /// <summary>
    /// The base class for all DK Saml 2.0 attributes.
    /// </summary>
    public abstract class EidasAttribute 
    {
        /// <summary>
        /// Creates a SamlAttribute with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="friendlyName">Friendly name.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns></returns>
        protected static SamlAttribute Create(string name, string friendlyName, string value)
        {
            SamlAttribute att = new SamlAttribute();
            att.NameFormat = SamlAttribute.NAMEFORMAT_URI;
            att.Name = name;
            att.FriendlyName = friendlyName;
            att.AttributeValue = new string[] { value };
            return att;   
        }
    }
}