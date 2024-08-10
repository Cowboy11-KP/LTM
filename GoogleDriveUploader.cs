using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpLoadFileToGoogle
{
    internal class GoogleDriveUploader
    {
        private DriveService service;

        public GoogleDriveUploader(DriveService Service)
        {
            service = Service;
        }

        public async Task<string> UploadFileAsync(string filePath)
        {
            try
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(filePath)
                };
                FilesResource.CreateMediaUpload request;
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    request = service.Files.Create(fileMetadata, stream, GetMimeType(filePath));
                    request.Fields = "id";
                    await request.UploadAsync();
                }

                var file = request.ResponseBody;
                return file?.Id;
            }
            catch (Exception ex)
            {
                // Log exception and rethrow or handle accordingly
                throw new Exception("An error occurred during file upload: " + ex.Message, ex);
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
