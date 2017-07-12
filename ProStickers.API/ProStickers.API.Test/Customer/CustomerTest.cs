using System.IO;
using System.Reflection;
using Xunit;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using ProStickers.BL.Customer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Customer;
using ProStickers.ViewModel.Security;

namespace ProStickers.API.Test.Customer
{
    public class CustomerTest
    {
        UserInfo ui = new UserInfo("test.01@gmail.com", "TestNameOne");

        [Fact]
        public async void CreateTestAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var rName = "ProStickers.API.Test.JSON.CustomerJSON.json";
            using (Stream stream = assembly.GetManifestResourceStream(rName))
            {
                if (stream == null)
                    return;
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    string _requestJson = streamReader.ReadToEnd();
                    CustomerViewModel actualData = JsonConvert.DeserializeObject<CustomerViewModel>(_requestJson) as CustomerViewModel;
                    if (actualData != null)
                    {
                        OperationResult result = await CustomerBL.CreateAsync(actualData, ui);
                        if (result.Result == Result.Success)
                        {
                            string id = result.ReturnedData.ToString();
                            result = await CustomerBL.GetByIDAsync(id, ui);
                            CustomerViewModel expectedData = (result.ReturnedData as CustomerViewModel);
                            if (expectedData != null)
                            {
                                actualData.UpdatedTS = expectedData.UpdatedTS;
                                CompareLogic cmprlgc = new CompareLogic();
                                ComparisonResult compareResult = cmprlgc.Compare(actualData, expectedData);
                                Xunit.Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
                            }
                        }
                    }

                }
            }
        }

        //[Fact]
        //public async void GetByIDAsync()
        //{
        //    OperationResult result = await CustomerBL.GetByIDAsync("000001", ui);
        //    CustomerViewModel vm = (result.ReturnedData as CustomerViewModel);
        //    Xunit.Assert.NotNull(vm);
        //}
    }
}
