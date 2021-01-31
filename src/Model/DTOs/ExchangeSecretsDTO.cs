using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.DbModels
{
    public class ExchangeSecretsDTO
    {
        public string Comment { get; set; }
        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
        public string Passphrase { get; set; }
        public Enums.Exchange ExchangeId { get; set; }
    }
}