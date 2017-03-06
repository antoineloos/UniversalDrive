using System;
using System.Collections.Generic;
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
    public sealed partial class AuthPage : Page
    {
        private readonly OneDriveService _service;

        public AuthPage()
        {
            InitializeComponent();

            _service = Views.OneDriveFilePage._service;

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

