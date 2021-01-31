using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DbModels
{
 public   class Fund
    {
        public Guid Id { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public Guid ExchangeId { get; set; }
    }
}
