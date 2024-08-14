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
        private void enableButtonUpdate()
        {
            buttonUpdate.Enabled = listBoxFiles.Items.Count > 0;
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
                enableButtonUpdate();
            }
        }

        private async void ButtonSelectFile_Click(object sender, EventArgs e)
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

            progressForm.Show();
            progressForm.ProgressBar.Maximum = filePaths.Count;
            progressForm.ProgressBar.Value = 0;

            var uploadTasks = filePaths.Select(filePath => Task.Run(() =>
            {
                googleDriveService.UploadFile(filePath);
                Invoke((Action)UpdateProgressBar);
            }));

            await Task.WhenAll(uploadTasks);

            progressForm.StatusLabel.Text = "Hoàn tất. Tất cả các tệp đã được tải lên thành công.";
            await Task.Delay(2000);
            progressForm.Hide();

            listBoxFiles.Items.Clear();
            filePaths.Clear();
        }

        private void UpdateProgressBar()
        {
            progressForm.ProgressBar.Value = Math.Min(progressForm.ProgressBar.Value + 1, progressForm.ProgressBar.Maximum);
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            var selectedFiles = listBoxFiles.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();
            foreach (int index in selectedFiles)
            {
                string fileName = listBoxFiles.Items[index].ToString();
                filePaths.Remove(filePaths.First(f => Path.GetFileName(f) == fileName));
                listBoxFiles.Items.RemoveAt(index);
            }
            UpdateButtonState();
        }

        private void ListBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonRemove.Enabled = listBoxFiles.SelectedIndices.Count > 0;
        }

        private void UpdateButtonState()
        {
            buttonUpdate.Enabled = listBoxFiles.Items.Count > 0;
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

        private void DisplayUserEmail()
        {
            try
            {
                var email = googleDriveService.GetUserEmail();
                labelNameAddress.Text = email;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching user info: " + ex.Message);
            }
        }
    }
}
