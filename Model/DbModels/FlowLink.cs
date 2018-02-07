using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DbModels
{
   public class FlowLink
    {
        public FlowLink() { }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public Guid FlowNodeSource { get; set; }
        public Guid FlowNodeTarget { get; set; }
    }
}
