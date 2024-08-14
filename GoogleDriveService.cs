using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace UploadGoogleDrive
{
    public class GoogleDriveService
    {
        private DriveService _service;

        public async Task InitializeAsync()
        {
            UserCredential credential;
            string credPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".credentials/drive-dotnet-quickstart.json");

            if (File.Exists(credPath + ".dat"))
            {
                credential = await AuthorizeAsync(credPath);
            }
            else
            {
                using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        new[] { DriveService.Scope.DriveFile },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true));
                }
            }

            _service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API .NET Quickstart"
            });
        }

        private async Task<UserCredential> AuthorizeAsync(string credPath)
        {
            using (var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read))
            {
                return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { DriveService.Scope.DriveFile },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }
        }

        public string UploadFile(string path)
        {
            try
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = Path.GetFileName(path)
                };

                using (var stream = new FileStream(path, FileMode.Open))
                {
                    var request = _service.Files.Create(fileMetadata, stream, GetMimeType(path));
                    request.Fields = "id";
                    request.Upload();

                    return request.ResponseBody?.Id;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during upload.", ex);
            }
        }

        public string GetUserEmail()
        {
            try
            {
                var aboutRequest = _service.About.Get();
                aboutRequest.Fields = "user";
                var about = aboutRequest.Execute();
                return about.User.EmailAddress;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while fetching user info.", ex);
            }
        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }
}
