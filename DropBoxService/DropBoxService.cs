using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DropBoxService
{
    public class DropBoxService
    {


        public Dropbox.Api.DropboxClient Client { get; set; }

        public DropBoxService()
        {
            
            Dropbox.Api.DropboxClientConfig clt = new Dropbox.Api.DropboxClientConfig();
            
        }

        public Uri DropBoxStartUrl
        {
            get
            {
                return Dropbox.Api.DropboxOAuth2Helper.GetAuthorizeUri(Dropbox.Api.OAuthResponseType.Code, Uri.EscapeDataString("sdk8acwthc0l9g1"),"","",false,false);
            }
            private set { }
        }

        public string AccessToken
        {
            get;
            set;
        }

        public Object _service;

        private Action _errorCallback;
        private Action _successCallback;

        public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

        public bool CheckAuthenticate(Action successCallback, Action errorCallback)
        {
            if (IsAuthenticated)
            {
                successCallback();
                return true;
            }

            _successCallback = successCallback;
            _errorCallback = errorCallback;
            return false;
        }

        public void ContinueAfterGetToken()
        {
            if (AccessToken == null)
            {
                _errorCallback?.Invoke();
                return;
            }


            else
            {
                _successCallback?.Invoke();
            }
        }


        public async Task<Dropbox.Api.Files.Metadata> SaveFile(string parentPath, string fileName, Stream stream)
        {
            return await Client.Files.UploadAsync(parentPath + "/" + fileName, WriteMode.Add.Instance,false,DateTime.Now,false,stream);
        }

            public async Task SetAccessTokenAsync(String httpresponse)
        {
            


            if (httpresponse.Contains("<div id=\"auth-text\">"))
            {
                Regex myRegex = new Regex("data-token=\"(.*?)\"");
                var m = myRegex.Match(httpresponse);
                if (m.Success)
                {
                    var code = m.Groups[1];

                    Debug.WriteLine(code);

                    var response = await Dropbox.Api.DropboxOAuth2Helper.ProcessCodeFlowAsync(code.ToString(), "sdk8acwthc0l9g1", "ee9o0vxgab1j0yi");

                   

                    AccessToken = response.AccessToken;

                    this.Client = new DropboxClient(this.AccessToken, new DropboxClientConfig("VDesk"));

                   
                }


            }
        }
    }
}
