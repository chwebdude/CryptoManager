using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Plugins.Importers.Bity
{
    class BityAuthResponse
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
    }

    public class Orders
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("objects")]
        public Object[] Objects { get; set; }
    }

    public class Meta
    {
        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("previous")]
        public object Previous { get; set; }

        [JsonProperty("total_count")]
        public long TotalCount { get; set; }
    }

    public class Object
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("inputtransactions")]
        public Puttransaction[] Inputtransactions { get; set; }

        [JsonProperty("outputtransactions")]
        public Puttransaction[] Outputtransactions { get; set; }

        [JsonProperty("person")]
        public string Person { get; set; }

        [JsonProperty("resource_uri")]
        public string ResourceUri { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("timestamp_created")]
        public System.DateTime TimestampCreated { get; set; }
    }

    public class Puttransaction
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("order")]
        public string Order { get; set; }

        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("payment_processor_fee")]
        public string PaymentProcessorFee { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("resource_uri")]
        public string ResourceUri { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("payout_method")]
        public string PayoutMethod { get; set; }
    }

    public enum Status { Canc, Conf, Fill };
}
