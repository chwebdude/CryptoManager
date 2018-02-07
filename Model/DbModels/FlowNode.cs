using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DbModels
{
    public class FlowNode
    {
        public FlowNode() { }
        public FlowNode(DateTime dateTime, decimal amount, string currency, Guid exchangeId, string comment = null)
        {
            DateTime = dateTime;
            Amount = amount;
            Currency = currency;
            ExchangeId = exchangeId;
            Comment = comment;
            Id = new Guid();
        }

        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public Guid ExchangeId { get; set; }
        public string Comment { get; set; }
    }
}
