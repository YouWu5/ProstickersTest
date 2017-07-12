using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace ProStickers.CloudDAL.Entity.Designer
{
    public class DesignerAvailabilityEntity : TableEntity
    {
        public DesignerAvailabilityEntity()
        {

        }
        public DesignerAvailabilityEntity(string date, string userID)
        {
            PartitionKey = date;
            RowKey = userID;
        }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public int TimeSlot1 { get; set; }
        public int TimeSlot2 { get; set; }
        public int TimeSlot3 { get; set; }
        public int TimeSlot4 { get; set; }
        public int TimeSlot5 { get; set; }
        public int TimeSlot6 { get; set; }
        public int TimeSlot7 { get; set; }
        public int TimeSlot8 { get; set; }
        public int TimeSlot9 { get; set; }
        public int TimeSlot10 { get; set; }
        public int TimeSlot11 { get; set; }
        public int TimeSlot12 { get; set; }
        public int TimeSlot13 { get; set; }
        public int TimeSlot14 { get; set; }
        public int TimeSlot15 { get; set; }
        public int TimeSlot16 { get; set; }
        public int TimeSlot17 { get; set; }
        public int TimeSlot18 { get; set; }
        public int TimeSlot19 { get; set; }
        public int TimeSlot20 { get; set; }
        public int TimeSlot21 { get; set; }
        public int TimeSlot22 { get; set; }
        public int TimeSlot23 { get; set; }
        public int TimeSlot24 { get; set; }
        public int TimeSlot25 { get; set; }
        public int TimeSlot26 { get; set; }
        public int TimeSlot27 { get; set; }
        public int TimeSlot28 { get; set; }
        public int TimeSlot29 { get; set; }
        public int TimeSlot30 { get; set; }
        public int TimeSlot31 { get; set; }
        public int TimeSlot32 { get; set; }
        public int TimeSlot33 { get; set; }
        public int TimeSlot34 { get; set; }
        public int TimeSlot35 { get; set; }
        public int TimeSlot36 { get; set; }
        public int TimeSlot37 { get; set; }
        public int TimeSlot38 { get; set; }
        public int TimeSlot39 { get; set; }
        public int TimeSlot40 { get; set; }
        public int TimeSlot41 { get; set; }
        public int TimeSlot42 { get; set; }
        public int TimeSlot43 { get; set; }
        public int TimeSlot44 { get; set; }
        public int TimeSlot45 { get; set; }
        public int TimeSlot46 { get; set; }
        public int TimeSlot47 { get; set; }
        public int TimeSlot48 { get; set; }
        public int TimeSlot49 { get; set; }
        public int TimeSlot50 { get; set; }
        public int TimeSlot51 { get; set; }
        public int TimeSlot52 { get; set; }
        public int TimeSlot53 { get; set; }
        public int TimeSlot54 { get; set; }
        public int TimeSlot55 { get; set; }
        public int TimeSlot56{ get; set; }
        public int TimeSlot57{ get; set; }
        public int TimeSlot58{ get; set; }
        public int TimeSlot59{ get; set; }
        public int TimeSlot60 { get; set; }
        public int TimeSlot61 { get; set; }
        public int TimeSlot62 { get; set; }
        public int TimeSlot63 { get; set; }
        public int TimeSlot64 { get; set; }
        public int TimeSlot65 { get; set; }
        public int TimeSlot66 { get; set; }
        public int TimeSlot67 { get; set; }
        public int TimeSlot68 { get; set; }
        public int TimeSlot69 { get; set; }
        public int TimeSlot70 { get; set; }
        public int TimeSlot71 { get; set; }
        public int TimeSlot72 { get; set; }
        public int TimeSlot73 { get; set; }
        public int TimeSlot74 { get; set; }
        public int TimeSlot75 { get; set; }
        public int TimeSlot76 { get; set; }
        public int TimeSlot77 { get; set; }
        public int TimeSlot78 { get; set; }
        public int TimeSlot79 { get; set; }
        public int TimeSlot80 { get; set; }
        public int TimeSlot81 { get; set; }
        public int TimeSlot82 { get; set; }
        public int TimeSlot83 { get; set; }
        public int TimeSlot84 { get; set; }
        public int TimeSlot85 { get; set; }
        public int TimeSlot86 { get; set; }
        public int TimeSlot87 { get; set; }
        public int TimeSlot88 { get; set; }
        public int TimeSlot89 { get; set; }
        public int TimeSlot90 { get; set; }
        public int TimeSlot91 { get; set; }
        public int TimeSlot92 { get; set; }
        public int TimeSlot93 { get; set; }
        public int TimeSlot94 { get; set; }
        public int TimeSlot95 { get; set; }
        public int TimeSlot96 { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedTS { get; set; }
        public DateTime UpdatedTS { get; set; }
    }
}
