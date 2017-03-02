using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Webkit;

namespace OneDriveSimpleSample.Droid
{
    [Activity(Label = "Authentication")]
    public class AuthenticationActivity : Activity
    {
        private OneDriveService _service;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Authentication);

            _service = App.ServiceInstance;

            var webViewClient = new WebViewClientEx(_service);
            AuthWebView.SetWebViewClient(webViewClient);
            AuthWebView.Settings.JavaScriptEnabled = true;

            StartAuthentication();
        }

        private void StartAuthentication()
        {
            var uri = _service.GetStartUri();
            AuthWebView.LoadUrl(uri.AbsoluteUri);
        }

        protected override void OnResume()
        {
            App.CurrentActivity = this;
            base.OnResume();
        }

        private WebView _authWebView;

        public WebView AuthWebView => _authWebView
            ?? (_authWebView = FindViewById<WebView>(Resource.Id.AuthWebView));

        public class WebViewClientEx : WebViewClient
        {
            private readonly OneDriveService _service;

            public WebViewClientEx(OneDriveService service)
            {
                _service = service;
            }

            public override void OnPageFinished(WebView view, string url)
            {
                if (_service.CheckRedirectUrl(url))
                {
                    _service.ContinueGetTokens(new Uri(url));
                }
            }

            public override void OnReceivedError(WebView view, [GeneratedEnum] ClientError errorCode, string description, string failingUrl)
            {
                _service.ContinueGetTokens(null);
            }
        }
    }
}