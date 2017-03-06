using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace OneDriveSimpleSample.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class AuthGooglePage : Page
    {
        private readonly GoogleDriveService.GoogleDriveService _service;

        public AuthGooglePage()
        {
            

            this.InitializeComponent();
            _service = GoogleDrivePage._service;
            Loaded += (s, e) =>
            {

                Web.Navigate(new Uri(_service.GoogleStartUrl));
            };

            Web.NavigationCompleted += async (s, e) =>
            {


                if (e.Uri.AbsoluteUri != "")
                {

                    string res = await Web.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
                    await _service.SetAccessTokenAsync(res);
                    if(_service.IsAuthenticated) _service.ContinueAfterGetToken();
                };
            };

            Web.NavigationFailed += (s, e) =>
            {
                Debug.WriteLine("web nav failed");
            };
        }
    }
}
