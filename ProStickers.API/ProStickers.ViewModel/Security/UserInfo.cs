namespace ProStickers.ViewModel.Security
{
    public class UserInfo
    {
        public UserInfo(string userID, string userName)
        {
            this._userID = userID;
            this.UserName = userName;
        }

        private string _userID;
        public string UserID
        {
            get
            {
                return _userID;
            }
            set
            {

                _userID = value;

            }
        }

        public string UserName { get; set; }
    }
}
