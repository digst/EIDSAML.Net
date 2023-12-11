using eid.saml20.Schema.Core;

namespace eid.saml20.Profiles.Eidas.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class EidasSamlFamilyNameAttribute : EidasAttribute
    {
        /// <summary>
        /// Attribute name
        /// </summary>
        public const string NAME = "http://eidas.europa.eu/attributes/naturalperson/CurrentFamilyName";
        /// <summary>
        /// Friendly name
        /// </summary>
        public const string FRIENDLYNAME = "FamilyName";

        /// <summary>
        /// Creates an attribute with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static SamlAttribute Create(string value)
        {
            return Create(NAME, FRIENDLYNAME, value);
        }
    }
}