using Microsoft.WindowsAzure.Storage.Table;

namespace ProStickers.CloudDAL.Entity.Master
{
    public class DesignerAppointmentDetailEntity : TableEntity
    {
        public DesignerAppointmentDetailEntity()
        {

        }

        public DesignerAppointmentDetailEntity(string date, string userID)
        {
            PartitionKey = date;
            RowKey = userID;
        }
        public string UserName { get; set; }
        public int NoOfOrder{ get; set; }
        public double Amount { get; set; }
    }
}
