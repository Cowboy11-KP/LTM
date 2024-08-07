using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace UploadFileGoogleDrive
{
    public partial class Form1 : Form
    {
        private OpenFileDialog openFileDialog;
        private List<string> filePaths;
        private DriveService service;
        private bool isChangingAccount;
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            // Initialize OpenFileDialog
            openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true; // Allow multiple file selection
            openFileDialog.Filter = "All files (*.*)|*.*";

            // Initialize FolderBrowserDialog

            // Initialize file paths list
            filePaths = new List<string>();

            // Set button click event handlers
            buttonSelectFile.Click += new EventHandler(ButtonSelectFile_Click);
            buttonUpdate.Click += new EventHandler(ButtonUpload_Click);
            buttonRemove.Click += new EventHandler(ButtonRemove_Click);
            buttonChangeAccount.Click += new EventHandler(buttonChangeAccount_Click);

            // Set SelectionMode to MultiExtended
            listBoxFiles.SelectionMode = SelectionMode.MultiExtended;
            //Set enabled button
            buttonRemove.Enabled = listBoxFiles.SelectedIndices.Count > 0;

            listBoxFiles.SelectedIndexChanged += listBoxFiles_SelectedIndexChanged;

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Any initialization code
            await initializeDriveServiceAsync();
            // Display user email
            DisplayUserEmail();
        }

        private async Task initializeDriveServiceAsync()
        {
            UserCredential credential;
            string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

            if (System.IO.File.Exists(credPath + ".dat"))
            {
                using (var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        new[] { DriveService.Scope.DriveFile },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true));
                }
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

            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API .NET Quickstart",
            });
        }


        private void DisplayUserEmail()
        {
            try
            {
                var aboutRequest = service.About.Get();
                aboutRequest.Fields = "user";
                var about = aboutRequest.Execute();
                this.labelNameAddress.Text = about.User.EmailAddress;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching user info: " + ex.Message);
            }
        }

        // Method to handle file selection
        private void ButtonSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    filePaths.Add(fileName);
                    listBoxFiles.Items.Add(Path.GetFileName(fileName));
                }
                enableButtonUpdate();
            }
        }
        private void listBoxFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listBoxFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    filePaths.Add(file);
                    listBoxFiles.Items.Add(Path.GetFileName(file));
                }
            }
        }

        // Method to handle file upload

        private void enableButtonUpdate()
        {
            buttonUpdate.Enabled = listBoxFiles.Items.Count > 0;
        }


        private async void ButtonUpload_Click(object sender, EventArgs e)
{
    if (filePaths.Count == 0)
    {
        MessageBox.Show("Please select a file first.");
        return;
    }

    // Hiển thị ProgressBar và thiết lập trạng thái
    progressBar1.Visible = true;
    progressBar1.Maximum = filePaths.Count;
    progressBar1.Value = 0;

    // Tạo danh sách các nhiệm vụ tải lên
    var uploadTasks = filePaths.Select(filePath => Task.Run(() =>
    {
        UploadFileToGoogleDrive(filePath);
        // Cập nhật ProgressBar sau khi tải xong một tệp
        this.Invoke((Action)(() => UpdateProgressBar()));
    }));

    // Đợi tất cả các nhiệm vụ tải lên hoàn tất
    await Task.WhenAll(uploadTasks);
    this.Invoke((Action)(() =>
    {
        progressBar1.Value = progressBar1.Maximum;
        MessageBox.Show("Hoàn tất. Tất cả các tệp đã được tải lên thành công.");
    }));

    // Xóa các mục trong ListBox và danh sách filePaths
    progressBar1.Visible = false;
    listBoxFiles.Items.Clear();
    filePaths.Clear();
}

private void UpdateProgressBar()
{
    if (progressBar1.InvokeRequired)
    {
        progressBar1.Invoke(new Action(UpdateProgressBar));
    }
    else
    {
        // Đảm bảo rằng giá trị không vượt quá giá trị tối đa
        progressBar1.Value = Math.Min(progressBar1.Value + 1, progressBar1.Maximum);
    }
}


        private string UploadFileToGoogleDrive(string path)
        {
            try
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(path)
                };
                FilesResource.CreateMediaUpload request;
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    request = service.Files.Create(fileMetadata, stream, GetMimeType(path));
                    request.Fields = "id";
                    request.Upload();
                }

                var file = request.ResponseBody;
                return file.Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                return null;
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
        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check if any item is selected 
            buttonRemove.Enabled = listBoxFiles.SelectedIndices.Count > 0;
        }
        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            var selectedIndices = listBoxFiles.SelectedIndices.Cast<int>().ToList();

            selectedIndices.Sort();
            selectedIndices.Reverse();

            foreach (var selectedIndex in selectedIndices)
            {
                string selectedFile = listBoxFiles.Items[selectedIndex].ToString();

                // Remove from listBoxFiles
                listBoxFiles.Items.RemoveAt(selectedIndex);

                // Remove from filePaths
                string filePathToRemove = filePaths.FirstOrDefault(fp => Path.GetFileName(fp) == selectedFile);
                if (filePathToRemove != null)
                {
                    filePaths.Remove(filePathToRemove);
                }
            }
            enableButtonUpdate();
        }

        private void labelTextLUd_Click(object sender, EventArgs e)
        {

        }
        private void labelTextFile_Click(object sender, EventArgs e)
        {
            this.label2.Text = "FILE:";
        }
        private void labelTextAcc_Click(object sender, EventArgs e)
        {

        }

        private void labelNameAddress_Click(object sender, EventArgs e)
        {

        }

        private async void buttonChangeAccount_Click(object sender, EventArgs e)
        {
            {
                if (isChangingAccount)
                {
                    return;
                }
                try
                {
                    isChangingAccount = true;
                    string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                    if (Directory.Exists(credPath))
                    {
                        Directory.Delete(credPath, true);
                    }


                    await initializeDriveServiceAsync();
                    DisplayUserEmail();
                    isChangingAccount = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while changing accounts: " + ex.Message);
                }
                finally
                {
                    isChangingAccount = false;
                }
            }
        }
    }
}