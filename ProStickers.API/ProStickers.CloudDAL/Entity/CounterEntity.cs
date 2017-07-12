using Microsoft.WindowsAzure.Storage.Table;

namespace ProStickers.CloudDAL.Entity
{
    public class CounterEntity:TableEntity
    {
        public CounterEntity()
        {

        }

        public CounterEntity(string counterID, string name)
        {
            RowKey = counterID;
            PartitionKey = name;
        }
        public int StartNo { get; set; }
        public long LastNo { get; set; }
        public string Date { get; set; }
    }
}
