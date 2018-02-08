using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DbModels
{
   public class FlowLink
    {
        public FlowLink(DateTime dateTime, decimal amount, string currency, Guid flowNodeSource, Guid flowNodeTarget, Guid exchangeId, string comment = null)
        {
            DateTime = dateTime;
            Amount = amount;
            Currency = currency;
            FlowNodeSource = flowNodeSource;
            FlowNodeTarget = flowNodeTarget;
            Comment = comment;
            ExchangeId = exchangeId;
            Id = Guid.NewGuid();
        }
        public FlowLink()
        {
        }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public Guid FlowNodeSource { get; set; }
        public Guid FlowNodeTarget { get; set; }
        public string Comment { get; set; }
        public Guid ExchangeId { get; set; }
    }
}
