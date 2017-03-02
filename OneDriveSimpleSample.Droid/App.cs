using Android.App;

namespace OneDriveSimpleSample.Droid
{
    public static class App
    {
        static App()
        {
            // Client ID
            ServiceInstance = new OneDriveService("000000004C169646"); 
        }

        public static OneDriveService ServiceInstance
        {
            get;
            set;
        }

        public static Activity CurrentActivity
        {
            get;
            set;
        }
    }
}