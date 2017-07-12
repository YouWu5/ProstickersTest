namespace ProStickers.ViewModel.Core
{
    public class CommonValidationMessage
    {
        public const string Required = "{PropertyName} is required.";

        public const string Max10 = "{PropertyName} should be of 10 digits.";

        public const string Max15 = "{PropertyName} should be of 15 digits.";

        public const string CodeInfo = "{PropertyName} is invalid.";

        public const string AlphaNumericMax100 = "{PropertyName} maximum length should be 100 characters.";

        public const string AspectRatio = "{PropertyName} should be greater than 0.00 and smaller than 100.00";

        public const string DesignerNote = "{PropertyName} maximum length should be 1500 characters.";
    }
}
