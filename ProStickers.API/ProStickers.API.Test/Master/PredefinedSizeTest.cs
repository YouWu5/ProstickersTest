using Xunit;
using ProStickers.ViewModel.Master;
using ProStickers.ViewModel.Core;
using ProStickers.BL.Master;

namespace ProStickers.API.Test.Master
{
    public class PredefinedSizeTest
    {
        [Fact]
        public async void GetByIdAsync()
        {
            OperationResult operationResult = await PredefinedSizeBL.GetByIDAsync();
            PredefinedSizeViewModel expectedData = operationResult.ReturnedData as PredefinedSizeViewModel;
            Xunit.Assert.NotNull(expectedData);
        }
    }
}
