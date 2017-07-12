using System.Reflection;
using System.IO;
using Xunit;
using Newtonsoft.Json;
using KellermanSoftware.CompareNetObjects;
using ProStickers.ViewModel.Customer;
using ProStickers.BL.Customer;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Security;

namespace ProStickers.API.Test.Customer
{
    public class FilesTest
    {
        UserInfo ui = new UserInfo("test.01@gmail.com", "TestNameTwo");

        [Fact]
        public async void CreateTestAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var rName = "ProStickers.API.Test.JSON.FilesJSON.json";
            using (Stream stream = assembly.GetManifestResourceStream(rName))
            {
                if (stream == null)
                    return;
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    string _requestJson = streamReader.ReadToEnd();
                    FilesViewModel actualData = JsonConvert.DeserializeObject<FilesViewModel>(_requestJson) as FilesViewModel;
                    if (actualData != null)
                    {
                        OperationResult result = await FilesBL.UploadFileAsync(ui, actualData);
                        if (result.Result == Result.Success)
                        {
                            string id = result.ReturnedData.ToString();
                           // result = await FilesBL.GetDefaultViewModelAsync();
                            FilesViewModel expectedData = (result.ReturnedData as FilesViewModel);
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
    }
}
