using Microsoft.WindowsAzure.Storage.Table;

namespace ProStickers.CloudDAL.Entity.Master
{
    public class InsertMasterDataEntity : TableEntity
    {
        public InsertMasterDataEntity()
        {

        }

        public InsertMasterDataEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }
        public string MasterDataJSON { get; set; }
    }

    public class StateEntity
    {
        public int StateID { get; set; }
        public int CountryID { get; set; }
        public string Name { get; set; }
    }

    public class CountryEntity
    {
        public int CountryID { get; set; }
        public string Name { get; set; }
        public string PostalCodeRegax { get; set; }
    }

    public class MonthEntity
    {
        public int MonthID { get; set; }
        public string Name { get; set; }
    }

    public class YearEntity
    {
        public int YearID { get; set; }
    }

    public class TimeSlotEntity
    {
        public int TimeSlotID { get; set; }
        public string Name { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
    }

    public class SupportedFileExtensionEntity
    {
        public string Text { get; set; }
        public string  Value { get; set; }
    }
    public class ConfigurationEntity : TableEntity
    {
        public string Value { get; set; }
    }
}

