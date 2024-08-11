namespace ConfigurationLib.Dashboard.Models
{
    public class HomeViewModel
    {
        public class CurrentEnvironmentViewModel
        {
            public string SiteName { get; set; }
            public bool IsBasketEnabled { get; set; }
            public int MaxItemCount { get; set; }
        }
      public  CurrentEnvironmentViewModel CurrentEnvironment { get; set; }
    }
}
