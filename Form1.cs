using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using UpLoadFileToGoogle;

namespace UploadFileGoogleDrive
{
    public partial class Form1 : Form
    {
        private GoogleDriveUploader upLoader;
        private OpenFileDialog openFileDialog;
        private List<string> filePaths;
        private DriveService service;
        private bool isChangingAccount;
        private ProgressForm progressForm;


        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            InitializeOpenFileDialog();
            InitializeFilePaths();
            InitializeProgressForm();
            InitializeButtonEvents();
        }

        private void InitializeOpenFileDialog()
        {
            openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "All files (*.*)|*.*"
            };
        }

        private void InitializeFilePaths()
        {
            filePaths = new List<string>();
        }

        private void InitializeProgressForm()
        {
            progressForm = new ProgressForm();
        }

        private void InitializeButtonEvents()
        {
            buttonSelectFile.Click += new EventHandler(ButtonSelectFile_Click);
            buttonUpdate.Click += new EventHandler(ButtonUpload_Click);
            buttonRemove.Click += new EventHandler(ButtonRemove_Click);
            buttonChangeAccount.Click += new EventHandler(buttonChangeAccount_Click);

            listBoxFiles.SelectionMode = SelectionMode.MultiExtended;
            listBoxFiles.SelectedIndexChanged += listBoxFiles_SelectedIndexChanged;
            listBoxFiles.DragEnter += listBoxFiles_DragEnter;
            listBoxFiles.DragDrop += listBoxFiles_DragDrop;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await InitializeDriveServiceAsync();
            upLoader = new GoogleDriveUploader(service);
            DisplayUserEmail();
        }

        private async Task InitializeDriveServiceAsync()
        {
            UserCredential credential;
            string credPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".credentials/drive-dotnet-quickstart.json");

            if (File.Exists(credPath + ".dat"))
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
                labelNameAddress.Text = about.User.EmailAddress;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching user info: " + ex.Message);
            }
        }

        private void ButtonSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                AddFilesToList(openFileDialog.FileNames);
                enableButtonUpdate();
            }
        }

        private void AddFilesToList(IEnumerable<string> files)
        {
            foreach (var fileName in files)
            {
                filePaths.Add(fileName);
                listBoxFiles.Items.Add(Path.GetFileName(fileName));
            }
        }

        private void listBoxFiles_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void listBoxFiles_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                AddFilesToList(files);
            }
        }

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
            // Tắt nút Upload để ngăn người dùng nhấp vào trong khi tải lên
            buttonUpdate.Enabled = false;

            progressForm.Show();
            progressForm.ProgressBar.Maximum = filePaths.Count;
            progressForm.ProgressBar.Value = 0;

            var semaphore = new SemaphoreSlim(4); // Limiting concurrent uploads to 4
            var uploadTasks = filePaths.Select(async filePath =>
            {
                await semaphore.WaitAsync(); // Chờ cho đến khi có chỗ trống trong semaphore
                try
                {
                    await upLoader.UploadFileAsync(filePath); // Tải lên tệp
                }
                finally
                {
                    semaphore.Release(); // Giải phóng chỗ trống trong semaphore
                    UpdateProgressBar();
                }
            });

            await Task.WhenAll(uploadTasks); // Đợi tất cả các tác vụ tải lên hoàn tất

            progressForm.Invoke((Action)(() =>
            {
                progressForm.ProgressBar.Value = progressForm.ProgressBar.Maximum;
                progressForm.StatusLabel.Text = "Hoàn tất. Tất cả các tệp đã được tải lên thành công.";
            }));

            await Task.Delay(2000);
            progressForm.Invoke((Action)(() => progressForm.Hide()));

            // Bật lại nút Upload sau khi tất cả các tệp đã được tải lên
            buttonUpdate.Enabled = true;

            listBoxFiles.Items.Clear();
            filePaths.Clear();
        }

        private async Task UploadFileWithSemaphore(string filePath, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                await upLoader.UploadFileAsync(filePath);
            }
            finally
            {
                semaphore.Release();
                UpdateProgressBar();
            }
        }

        private void UpdateProgressBar()
        {
            progressForm.ProgressBar.Value = Math.Min(progressForm.ProgressBar.Value + 1, progressForm.ProgressBar.Maximum);
        }

        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonRemove.Enabled = listBoxFiles.SelectedIndices.Count > 0;
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            var selectedIndices = listBoxFiles.SelectedIndices.Cast<int>().ToList();
            selectedIndices.Sort();
            selectedIndices.Reverse();

            foreach (var selectedIndex in selectedIndices)
            {
                var selectedFile = listBoxFiles.Items[selectedIndex].ToString();
                listBoxFiles.Items.RemoveAt(selectedIndex);
                var filePathToRemove = filePaths.FirstOrDefault(fp => Path.GetFileName(fp) == selectedFile);
                if (filePathToRemove != null)
                {
                    filePaths.Remove(filePathToRemove);
                }
            }
            enableButtonUpdate();
        }

        private async void buttonChangeAccount_Click(object sender, EventArgs e)
        {
            if (isChangingAccount)
            {
                return;
            }

            try
            {
                isChangingAccount = true;
                string credPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".credentials/drive-dotnet-quickstart.json");

                // Xóa thông tin xác thực cũ
                if (Directory.Exists(credPath))
                {
                    Directory.Delete(credPath, true);
                }

                // Khởi tạo lại dịch vụ
                await InitializeDriveServiceAsync();

                // Kiểm tra và cập nhật thông tin người dùng
                DisplayUserEmail();
                MessageBox.Show("Account has been successfully changed.");
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                MessageBox.Show($"An error occurred while changing accounts: {ex.Message}");
            }
            finally
            {
                isChangingAccount = false;
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void labelNameAddress_Click_1(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void labelNameAddress_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
