using Model.DbModels;

namespace Model.DTOs
{
    public class FlowLinkDTO : FlowLink
    {
        #region Properties

        public new string FlowNodeSource { get; set; }
        public new string FlowNodeTarget { get; set; }

        #endregion
    }
}
