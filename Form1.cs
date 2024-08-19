using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UploadGoogleDrive
{
    public partial class Form1 : Form
    {
        private OpenFileDialog openFileDialog;
        private List<string> filePaths;
        private GoogleDriveService googleDriveService;
        private bool isChangingAccount;
        private ProgressForm progressForm;
        private CancellationTokenSource _cancellationTokenSource;

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            InitializeOpenFileDialog();
            InitializeFields();
            AttachEventHandlers();
        }

        private void InitializeOpenFileDialog()
        {
            openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "All files (*.*)|*.*"
            };
        }

        private void InitializeFields()
        {
            filePaths = new List<string>();
            googleDriveService = new GoogleDriveService();
            progressForm = new ProgressForm();
            buttonRemove.Enabled = false;
        }

        private void AttachEventHandlers()
        {
            buttonSelectFile.Click += ButtonSelectFile_Click;
            buttonUpdate.Click += ButtonUpload_Click;
            buttonRemove.Click += ButtonRemove_Click;
            buttonChangeAccount.Click += ButtonChangeAccount_Click;
            listBoxFiles.SelectedIndexChanged += ListBoxFiles_SelectedIndexChanged;
            listBoxFiles.SelectionMode = SelectionMode.MultiExtended;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await googleDriveService.InitializeAsync();
            DisplayUserEmail();
        }

        private void EnableButtonUpdate()
        {
            buttonUpdate.Enabled = listBoxFiles.Items.Count > 0;
        }

        private void ListBoxFiles_DragEnter(object sender, DragEventArgs e)
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

        private void ListBoxFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    filePaths.Add(file);
                    listBoxFiles.Items.Add(Path.GetFileName(file));
                }
                EnableButtonUpdate();
            }
        }

        private void ButtonSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    filePaths.Add(fileName);
                    listBoxFiles.Items.Add(Path.GetFileName(fileName));
                }
                UpdateButtonState();
            }
        }

        private async void ButtonUpload_Click(object sender, EventArgs e)
        {
            if (!filePaths.Any())
            {
                MessageBox.Show("Please select a file first.");
                return;
            }

            // Đăng ký sự kiện OnCancelRequested chỉ một lần
            progressForm.OnCancelRequested -= CancelUpload;
            progressForm.OnCancelRequested += CancelUpload;

            progressForm.Show();
            progressForm.ProgressBar.Maximum = filePaths.Count;
            progressForm.ProgressBar.Value = 0;
            progressForm.CancelButton.Enabled = true;
            progressForm.StatusLabel.Text = "Bắt đầu tải lên...";

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            var semaphore = new SemaphoreSlim(3); // Giới hạn 3 tệp tải lên cùng lúc
            var uploadedFilesCount = 0;
            var successfullyUploadedFiles = new List<string>(); // Danh sách các tệp đã tải lên thành công

            var uploadTasks = filePaths.Select(async filePath =>
            {
                await semaphore.WaitAsync(token);
                try
                {
                    if (token.IsCancellationRequested)
                        return;

                    await googleDriveService.UploadFileAsync(filePath, cancellationToken: token);
                    successfullyUploadedFiles.Add(filePath); // Thêm tệp đã tải lên thành công vào danh sách
                    this.Invoke((Action)(() =>
                    {
                        UpdateProgressBar();
                        uploadedFilesCount++;
                    }));
                }
                catch (Exception ex)
                {
                    if (!token.IsCancellationRequested)
                        this.Invoke((Action)(() => MessageBox.Show($"An error occurred while uploading {Path.GetFileName(filePath)}: {ex.Message}")));
                }
                finally
                {
                    semaphore.Release();
                }
            });

            try
            {
                await Task.WhenAll(uploadTasks);
                progressForm.Invoke((Action)(() => progressForm.StatusLabel.Text = "Hoàn tất. Tất cả các tệp đã được tải lên thành công."));
            }
            catch (OperationCanceledException)
            {
                progressForm.Invoke((Action)(() =>
                {
                    ShowCancelMessage(uploadedFilesCount);
                    progressForm.StatusLabel.Text = "Tải lên đã bị hủy.";
                }));
            }
            finally
            {
                await Task.Delay(2000);
                progressForm.Invoke((Action)(() => progressForm.Hide()));
                listBoxFiles.Invoke((Action)(() =>
                {
                    // Xóa các tệp đã tải lên thành công từ ListBox và filePaths
                    foreach (var file in successfullyUploadedFiles)
                    {
                        var fileName = Path.GetFileName(file);
                        int index = listBoxFiles.Items.IndexOf(fileName);
                        if (index != -1)
                        {
                            listBoxFiles.Items.RemoveAt(index);
                            filePaths.Remove(file);
                        }
                    }
                }));
            }
        }


        private void CancelUpload()
        {
            _cancellationTokenSource?.Cancel();
        }

        private void ShowCancelMessage(int uploadedFilesCount)
        {
            var remainingFilesCount = filePaths.Count - uploadedFilesCount;
            MessageBox.Show($"Tải lên đã bị hủy. Đã tải {uploadedFilesCount} tệp. Các tệp còn lại trong danh sách: {remainingFilesCount}.", "Thông báo");
        }

        private void UpdateProgressBar()
        {
            progressForm.ProgressBar.Value++;
            progressForm.StatusLabel.Text = $"Đã tải {progressForm.ProgressBar.Value} / {progressForm.ProgressBar.Maximum} tệp";
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedIndices.Count > 0)
            {
                foreach (var index in listBoxFiles.SelectedIndices.Cast<int>().OrderByDescending(i => i))
                {
                    filePaths.RemoveAt(index);
                    listBoxFiles.Items.RemoveAt(index);
                }
                UpdateButtonState();
            }
        }

        private async void ButtonChangeAccount_Click(object sender, EventArgs e)
        {
            if (isChangingAccount) return;

            try
            {
                isChangingAccount = true;
                string credPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".credentials/drive-dotnet-quickstart.json");

                if (Directory.Exists(credPath))
                {
                    Directory.Delete(credPath, true);
                }

                await googleDriveService.InitializeAsync();
                DisplayUserEmail();
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

        private void ListBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonRemove.Enabled = listBoxFiles.SelectedItems.Count > 0;
        }

        private void UpdateButtonState()
        {
            buttonUpdate.Enabled = filePaths.Any();
        }

        private void DisplayUserEmail()
        {
            try
            {
                string email = googleDriveService.GetUserEmail();
                labelUserEmail.Text = $"Tài khoản: {email}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi lấy thông tin tài khoản: {ex.Message}");
            }
        }
    }
}
