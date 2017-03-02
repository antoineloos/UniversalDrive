using Windows.UI.Xaml;

namespace OneDriveSimpleSample
{
    public sealed partial class AuthenticationPage
    {
        private readonly OneDriveService _service;

        public AuthenticationPage()
        {
            InitializeComponent();

            _service  = MainPage._service;

            Loaded += (s, e) =>
            {
                var uri = _service.GetStartUri();
                Web.Navigate(uri);
            };

            Web.NavigationCompleted += (s, e) =>
            {
                if (_service.CheckRedirectUrl(e.Uri.AbsoluteUri))
                {
                    _service.ContinueGetTokens(e.Uri);
                }
            };

            Web.NavigationFailed += (s, e) =>
            {
                _service.ContinueGetTokens(null);
            };
        }
    }
}