using System.Reflection;
using System.IO;
using Xunit;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using ProStickers.ViewModel.Master;
using ProStickers.ViewModel.Core;
using ProStickers.BL.Master;
using ProStickers.ViewModel.Security;

namespace ProStickers.API.Test.Master
{
    public class ColorChartTest
    {
        UserInfo ui = new UserInfo("Master.prosticker@gmail.com", "ProStickers");

        [Fact]
        public async void CreateTestAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var rName = "ProStickers.API.Test.JSON.ColorChartJSON.json";
            using (Stream stream = assembly.GetManifestResourceStream(rName))
            {
                if (stream == null)
                    return;
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    string _requestJson = streamReader.ReadToEnd();
                    ColorChartViewModel actualData = JsonConvert.DeserializeObject<ColorChartViewModel>(_requestJson) as ColorChartViewModel;
                    if (actualData != null)
                    {
                        OperationResult result = await ColorChartBL.CreateAsync(actualData, ui);
                        if (result.Result == Result.Success)
                        {
                            string id = result.ReturnedData.ToString();
                            result = await ColorChartBL.GetByIDAsync(id, ui);
                            ColorChartViewModel expectedData = (result.ReturnedData as ColorChartViewModel);
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

        [Fact]
        public async void GetByIDAsync()
        {
            OperationResult result = await ColorChartBL.GetByIDAsync("000001", ui);
            ColorChartViewModel vm = (result.ReturnedData as ColorChartViewModel);
            Xunit.Assert.NotNull(vm);
        }
    }
}
