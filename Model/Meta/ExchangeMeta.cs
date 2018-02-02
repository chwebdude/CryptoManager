using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Model.Enums;

namespace Model.Meta
{
    public class ExchangeMeta
    {
        public Exchange ExchangeId { get; set; }
        public string Name { get; set; }
        public bool SupportsPublicKey => !string.IsNullOrEmpty(LabelPublicKey);
        public string LabelPublicKey { get; set; }
        public bool SupportsPrivateKey => !string.IsNullOrEmpty(LabelPrivateKey);
        public string LabelPrivateKey { get; set; }
        public bool SupportsPassphrase => !string.IsNullOrEmpty(LabelPassphrase);
        public string LabelPassphrase { get; set; }

    }
}
