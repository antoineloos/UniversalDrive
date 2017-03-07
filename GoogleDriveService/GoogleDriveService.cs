using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoogleDriveService
{
    public class GoogleDriveService
    {
        public string GoogleStartUrl { get
            {
                var googleUrl = new System.Text.StringBuilder();
                googleUrl.Append("https://accounts.google.com/o/oauth2/v2/auth?");

                googleUrl.Append("scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fdrive&");
                googleUrl.Append("access_type=offline&");
                googleUrl.Append("include_granted_scopes=true&");
                googleUrl.Append("redirect_uri=");
                googleUrl.Append(Uri.EscapeDataString("urn:ietf:wg:oauth:2.0:oob:auto"));
                googleUrl.Append("&response_type=code&");
                googleUrl.Append("client_id=");
                googleUrl.Append(Uri.EscapeDataString("1012831291296-t3053kshv8o2u8fv22bv9e9uc9d4u6ah.apps.googleusercontent.com"));
                return googleUrl.ToString();
            }
            private set { }
        }

        public string AccessToken
        {
            get;
            set;
        }

        public DriveService _service;

        private Action _errorCallback;
        private Action _successCallback;

        public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);


        public async Task SetAccessTokenAsync(String httpresponse)
        {
            GoogleAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = new Google.Apis.Auth.OAuth2.ClientSecrets()
                {
                    ClientSecret = "sgeSQ9Tc9gdEOQznUJisa4dn",
                    ClientId = "1012831291296-t3053kshv8o2u8fv22bv9e9uc9d4u6ah.apps.googleusercontent.com"

                }
                   ,
                Scopes = new string[] { DriveService.Scope.Drive, DriveService.Scope.DriveMetadata }
            });

            
                if (httpresponse.Contains("<title>Success"))
                {
                    Regex myRegex = new Regex("code=(.*?)&");
                    var m = myRegex.Match(httpresponse);
                    if (m.Success)
                    {
                        var code = m.Groups[1];
                        var response = await flow.ExchangeCodeForTokenAsync("1012831291296-t3053kshv8o2u8fv22bv9e9uc9d4u6ah.apps.googleusercontent.com", code.ToString(), "urn:ietf:wg:oauth:2.0:oob:auto", new System.Threading.CancellationToken());

                        UserCredential credential = new UserCredential(flow, "1012831291296-t3053kshv8o2u8fv22bv9e9uc9d4u6ah.apps.googleusercontent.com", response);
                         _service = new DriveService(new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = credential,
                            ApplicationName = "VDesk",

                        });


                        AccessToken = response.AccessToken;

                    
                    
                }

                
            }
        }


        public Task<MemoryStream> DownloadFile(string fileId, string mimetype)
        {
            var stream = new System.IO.MemoryStream();
            return Task.Factory.StartNew(() => 
            {
                var request = _service.Files.Export(fileId, mimetype);

                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                request.MediaDownloader.ProgressChanged +=
                        (IDownloadProgress progress) =>
                        {
                            switch (progress.Status)
                            {
                                case DownloadStatus.Downloading:
                                    {
                                        Console.WriteLine(progress.BytesDownloaded);
                                        break;
                                    }
                                case DownloadStatus.Completed:
                                    {
                                        Console.WriteLine("Download complete.");
                                        break;
                                    }
                                case DownloadStatus.Failed:
                                    {
                                        Console.WriteLine("Download failed.");
                                        break;
                                    }
                            }
                        };
                 request.Download(stream);
                 return stream;
            });
            

        }

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

        public List<Google.Apis.Drive.v3.Data.File> GetRootFolderChildren()
        {
            var request = _service.Files.List();
            request.Fields = "files/thumbnailLink, files/name, files/mimeType, files/id ";
            request.Q = "'root' in parents and trashed = false";
            return request.Execute().Files.ToList();
        }

        public List<Google.Apis.Drive.v3.Data.File> GetSharedWithMeChildren()
        {
            var request = _service.Files.List();
            request.Fields = "files/thumbnailLink, files/name, files/mimeType, files/id ";
            request.Q = "sharedWithMe=true and trashed=false";
            return request.Execute().Files.ToList();
        }

        public List<Google.Apis.Drive.v3.Data.File> GetSubFolderChildren(string folderId)
        {
            var request = _service.Files.List();
            request.Fields = "files/thumbnailLink, files/name, files/mimeType, files/id ";
            request.Q = $"'{folderId}' in parents and trashed = false";
            return request.Execute().Files.ToList();
        }


        

        [Obsolete("pourquoi tu fais ça serieux ? c'est récursif ! tu cherche la merde c'est ça ? ")]
        public List<Google.Apis.Drive.v3.Data.File> RetrieveAllFiles()
        {
            
            List<Google.Apis.Drive.v3.Data.File> result = new List<Google.Apis.Drive.v3.Data.File>();
            var request = _service.Files.List();
            
            // request.Fields = "files/thumbnailLink, files/name, files/mimeType, files/id , files/kind";
            request.Fields = "files/thumbnailLink, files/name, files/mimeType, files/id ";
            result = request.Execute().Files.ToList();
            do
            {
                try
                {
                    FileList files = request.Execute();

                    result = files.Files.ToList();

                    request.PageToken = files.NextPageToken;
                    foreach (Google.Apis.Drive.v3.Data.File elem in result)
                    {


                        Debug.WriteLine(elem.Name + " thumbnail : " + elem.ThumbnailLink);

                    }
                }
                catch (IOException e)
                {
                    Debug.WriteLine("An error occurred: " + e);
                    request.PageToken = null;
                }
            } while (request.PageToken != null &&
                     request.PageToken.Length > 0);

            return result;
        }

      
    }
}
