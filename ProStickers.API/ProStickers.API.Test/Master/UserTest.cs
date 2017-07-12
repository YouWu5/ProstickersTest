using System.IO;
using System.Reflection;
using Xunit;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using ProStickers.BL.Master;
using ProStickers.ViewModel.Core;
using ProStickers.ViewModel.Master;
using ProStickers.ViewModel.Security;

namespace ProStickers.API.Test.Master
{
    public class UserTest
    {
        UserInfo ui = new UserInfo("testuser0001@gmail.com", "Master");

        [Fact]
        public async void TestMethod1()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = "ProStickers.API.Test.JSON.UserJSON.json";
            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                using (StreamReader sReader = new StreamReader(stream))
                {
                    string json = sReader.ReadToEnd();
                    UserViewModel userVM = JsonConvert.DeserializeObject<UserViewModel>(json);
                    if (userVM != null)
                    {
                        OperationResult result = await UserBL.CreateAsync(userVM, ui);
                        if (result.Result == Result.Success)
                        {
                            OperationResult result1 = await UserBL.GetByIDAsync("10000001",result.ReturnedData.ToString());
                            UserViewModel expectedVM = result1.ReturnedData as UserViewModel;
                            userVM.UserID = expectedVM.UserID;
                            expectedVM.UpdatedTS = userVM.UpdatedTS;
                            CompareLogic cmprlgc = new CompareLogic();
                            ComparisonResult compareResult = cmprlgc.Compare(userVM, expectedVM);
                            Xunit.Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
                        }
                    }
                }
            }
        }
    }
}
