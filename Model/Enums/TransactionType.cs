using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Enums
{
    public enum TransactionType
    {
        // Exchange or something like that
        Trade,

        // When coin or fiat received
        In,

        // When coin or fiat sending
        Out,

        // Todo: To be defined
        //Mining,
    }
}
