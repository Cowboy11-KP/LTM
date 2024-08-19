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

            // Hiển thị form tiến trình và thiết lập trạng thái
            progressForm.Show();
            progressForm.ProgressBar.Maximum = filePaths.Count;
            progressForm.ProgressBar.Value = 0;
            progressForm.CancelButton.Enabled = true;
            progressForm.StatusLabel.Text = "Bắt đầu tải lên...";

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            var semaphore = new SemaphoreSlim(3); // Giới hạn 3 tệp tải lên cùng lúc

            var uploadTasks = filePaths.Select(async filePath =>
            {
                await semaphore.WaitAsync(token);
                try
                {
                    if (token.IsCancellationRequested)
                        return;

                    // Tải lên tệp và cập nhật tiến trình
                    await googleDriveService.UploadFileAsync(filePath, cancellationToken: token);
                    this.Invoke((Action)(() => UpdateProgressBar()));
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
                progressForm.Invoke((Action)(() => progressForm.StatusLabel.Text = "Tải lên đã bị hủy."));
            }
            finally
            {
                // Ẩn form tiến trình sau khi hoàn tất
                await Task.Delay(2000); // Thời gian chờ để người dùng thấy thông báo
                progressForm.Invoke((Action)(() => progressForm.Hide()));
                listBoxFiles.Invoke((Action)(() =>
                {
                    listBoxFiles.Items.Clear();
                    filePaths.Clear();
                }));
            }
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
