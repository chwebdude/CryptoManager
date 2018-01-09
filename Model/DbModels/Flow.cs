using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DbModels
{
    public class Flow
    {
        public Guid Id { get; set; }
        public List<Flow> Parents { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public Guid ExchangeId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
