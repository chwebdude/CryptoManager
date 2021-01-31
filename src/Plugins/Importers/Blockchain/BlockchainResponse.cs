using System.Collections.Generic;

namespace Plugins.Importers.Blockchain
{
    /// <summary>
    /// The Blockchain namespace. The normal NuGet Package is not used since it doesn't provide xPub information on
    /// transactions.
    /// </summary>
    internal class BlockchainResponse
    {
        #region Properties

        public Wallet Wallet { get; set; }
        public IEnumerable<Transaction> Txs { get; set; }

        #endregion
    }

    internal class Wallet
    {
        #region Properties

        public decimal Total_Sent { get; set; }
        public decimal Total_Received { get; set; }
        public decimal Final_Balance { get; set; }

        #endregion
    }

    internal class Transaction
    {
        #region Properties

        public string Hash { get; set; }
        public decimal Ver { get; set; }
        public decimal Fee { get; set; }
        public int Time { get; set; }
        public decimal Result { get; set; }
        public IEnumerable<OutObject> Out { get; set; }
        public IEnumerable<InObject> Inputs { get; set; }
        #endregion
    }

    internal class OutObject
    {
        #region Properties

        public decimal Value { get; set; }
        public string Addr { get; set; }
        public XPub XPub { get; set; }

        #endregion
    }

    internal class InObject
    {
        public OutObject Prev_Out { get; set; }
    }

    internal class XPub
    {
        #region Properties

        public string M { get; set; }
        public string Path { get; set; }

        #endregion
    }
}
