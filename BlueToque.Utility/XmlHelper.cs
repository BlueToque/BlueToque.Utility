using System.Xml;
using System.Xml.Linq;

namespace BlueToque.Utility
{
    /// <summary>
    /// Helper methods to manipulate xml
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Convert from Linq XElement to XmlElement
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public static XmlElement? ToXmlElement(this XElement el)
        {
            var doc = new XmlDocument();
            doc.Load(el.CreateReader());
            return doc.DocumentElement;
        }

        /// <summary>
        /// Convert from XmlElement to Linq XElement
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public static XElement ToXElement(this XmlElement el) => XElement.Parse(el.OuterXml);

        /// <summary>
        /// Create an XmlElement from a string
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XmlElement? XmlStringToElement(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement;
        }
    }
}
