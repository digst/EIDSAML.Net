using System;
using System.Xml.Serialization;

namespace eid.saml20.Schema.Core
{
    /// <summary>
    /// The KeyInfoConfirmationDataType complex type constrains a &lt;SubjectConfirmationData&gt;
    /// element to contain one or more &lt;ds:KeyInfo&gt; elements that identify cryptographic keys that are used in
    /// some way to authenticate an attesting entity. The particular confirmation method MUST define the exact
    /// mechanism by which the confirmation data can be used. The optional attributes defined by the
    /// SubjectConfirmationDataType complex type MAY also appear.
    /// </summary>
    [Serializable]
    [XmlType(Namespace=Saml20Constants.ASSERTION)]
    [XmlRoot(ELEMENT_NAME, Namespace = Saml20Constants.ASSERTION, IsNullable = false)]
    public class KeyInfoConfirmationData : SubjectConfirmationData
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ELEMENT_NAME = "KeyInfoConfirmationData";
    }
}