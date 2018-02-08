using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DbModels
{
    public class FlowNode
    {
        public FlowNode()
        {
        }
        public FlowNode(DateTime dateTime, decimal amount, string currency, Guid exchangeId, Guid transactionId, string comment = null)
        {
            DateTime = dateTime;
            Amount = amount;
            Currency = currency;
            ExchangeId = exchangeId;
            TransactionId = transactionId;
            Comment = comment;
            TransactionId = transactionId;

            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public Guid ExchangeId { get; set; }
        public string Comment { get; set; }
        public Guid TransactionId { get; set; }
    }
}
