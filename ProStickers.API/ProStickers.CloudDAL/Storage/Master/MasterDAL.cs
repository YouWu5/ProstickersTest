using Microsoft.WindowsAzure.Storage.Table;
using ProStickers.CloudDAL.Entity.Master;
using System.Threading.Tasks;

namespace ProStickers.CloudDAL.Storage.Master
{
    public static class MasterDAL
    {
        public static CloudTable masterDataTable;

        static MasterDAL()
        {
            masterDataTable = Utility.GetStorageTable("MasterData");
        }

        #region RetriveEntity Method
        public static async Task<string> RetriveEntity(string partitionKey)
        {
            TableOperation retrive = TableOperation.Retrieve<InsertMasterDataEntity>(partitionKey, partitionKey);
            TableResult result = await masterDataTable.ExecuteAsync(retrive);
            InsertMasterDataEntity masterData = result.Result as InsertMasterDataEntity;
            return masterData.MasterDataJSON.ToString();
        }
        #endregion

    }
}
