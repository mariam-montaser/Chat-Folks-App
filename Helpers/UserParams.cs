namespace SocialApp.Helpers
{
    public class UserParams
    {
        private const int maxPageSize = 50;
        private int _pagesize = 10;

        public int PageSize
        {
            get { return _pagesize; }
            set { _pagesize = value > maxPageSize ? maxPageSize : value; }
        }

        public string CurrentUserName { get; set; }
        public string Gender { get; set; }
        public string OrderBy { get; set; } = "lastActive";
        public int currentPage { get; set; } = 1;
        public int minAge { get; set; } = 18;
        public int maxAge { get; set; } = 150;
    }
}
