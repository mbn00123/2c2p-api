using static InvoiceAPI.ModelViews.InvoiceXmlData;
using System.Xml.Serialization;

namespace InvoiceAPI.Helpers
{
    public class InvoiceXMLMapper
    {
        public TransactionList MapXmlContentToClass(string xmlContent)
        {
            var serializer = new XmlSerializer(typeof(TransactionList));
            using (var reader = new StringReader(xmlContent))
            {
                return (TransactionList)serializer.Deserialize(reader);
            }
        }
    }
}
