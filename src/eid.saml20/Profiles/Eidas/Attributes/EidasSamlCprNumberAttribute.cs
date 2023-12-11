using eid.saml20.Schema.Core;

namespace eid.saml20.Profiles.Eidas.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class EidasSamlCprNumberAttribute : EidasAttribute
    {
        /// <summary>
        /// Attribute name
        /// </summary>
        public const string NAME = "dk:gov:saml:attribute:CprNumberIdentifier";

        /// <summary>
        /// Creates an attribute with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static SamlAttribute Create(string value)
        {
            return Create(NAME, null, value);
        }
    }
}