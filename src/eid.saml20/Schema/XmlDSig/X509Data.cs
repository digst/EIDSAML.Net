using System;
using System.Xml.Serialization;

namespace eid.saml20.Schema.XmlDSig
{
    /// <summary>
    /// An X509Data element within KeyInfo contains one or more identifiers of keys or X509 certificates 
    /// (or certificates' identifiers or a revocation list). The content of X509Data is: 
    /// 
    /// At least one element, from the following set of element types; any of these may appear together or more 
    /// than once iff (if and only if) each instance describes or is related to the same certificate: 
    /// 
    /// *) The X509IssuerSerial element, which contains an X.509 issuer distinguished name/serial number pair that SHOULD be compliant with RFC2253 [LDAP-DN], 
    /// *) The X509SubjectName element, which contains an X.509 subject distinguished name that SHOULD be compliant with RFC2253 [LDAP-DN], 
    /// *) The X509SKI element, which contains the base64 encoded plain (i.e. non-DER-encoded) value of a X509 V.3 SubjectKeyIdentifier extension. 
    /// *) The X509Certificate element, which contains a base64-encoded [X509v3] certificate, and 
    /// *) Elements from an external namespace which accompanies/complements any of the elements above. 
    /// *) The X509CRL element, which contains a base64-encoded certificate revocation list (CRL) [X509v3]. 
    /// 
    /// Any X509IssuerSerial, X509SKI, and X509SubjectName elements that appear MUST refer to the certificate 
    /// or certificates containing the validation key. All such elements that refer to a particular individual 
    /// certificate MUST be grouped inside a single X509Data element and if the certificate to which they refer 
    /// appears, it MUST also be in that X509Data element. 
    /// 
    /// Any X509IssuerSerial, X509SKI, and X509SubjectName elements that relate to the same key but different 
    /// certificates MUST be grouped within a single KeyInfo but MAY occur in multiple X509Data elements. 
    /// 
    /// All certificates appearing in an X509Data element MUST relate to the validation key by either containing 
    /// it or being part of a certification chain that terminates in a certificate containing the validation key. 
    /// </summary>
    [Serializable]
    [XmlType(Namespace=Saml20Constants.XMLDSIG)]
    [XmlRoot(ELEMENT_NAME, Namespace=Saml20Constants.XMLDSIG, IsNullable=false)]
    public class X509Data
    {

        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ELEMENT_NAME = "X509Data";

        private ItemsChoiceType[] itemsElementNameField;
        private object[] itemsField;


        /// <summary>
        /// Gets or sets the items.
        /// Items can be of types: X509CRL, X509Certificate, X509IssuerSerial, X509SKI, X509SubjectName
        /// </summary>
        /// <value>The items.</value>
        [XmlAnyElement]
        [XmlElement("X509CRL", typeof (byte[]), DataType="base64Binary")]
        [XmlElement("X509Certificate", typeof (byte[]), DataType="base64Binary")]
        [XmlElement("X509IssuerSerial", typeof (X509IssuerSerial))]
        [XmlElement("X509SKI", typeof (byte[]), DataType="base64Binary")]
        [XmlElement("X509SubjectName", typeof (string))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }


        /// <summary>
        /// Gets or sets the name of the items element.
        /// </summary>
        /// <value>The name of the items element.</value>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsChoiceType[] ItemsElementName
        {
            get { return itemsElementNameField; }
            set { itemsElementNameField = value; }
        }
    }
}