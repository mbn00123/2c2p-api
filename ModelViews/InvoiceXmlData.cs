using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace InvoiceAPI.ModelViews
{
    public class InvoiceXmlData
    {
        [XmlRoot("Transactions")]
        public class TransactionList
        {
            [XmlElement("Transaction")]
            public List<Transaction> Transactions { get; set; }
        }

        public class Transaction
        {
            [XmlAttribute("id")]
            public string Id { get; set; }

            [XmlElement("TransactionDate")]
            public DateTime TransactionDate { get; set; }

            public PaymentDetails PaymentDetails { get; set; }

            [XmlElement("Status")]
            public string Status { get; set; }
        }

        public class PaymentDetails
        {
            [XmlElement("Amount")]
            public decimal Amount { get; set; }

            [XmlElement("CurrencyCode")]
            public string CurrencyCode { get; set; }
        }
    }
}
