using System;
using System.Threading;
using System.Windows.Forms;

namespace UploadGoogleDrive
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        public ProgressBar ProgressBar => this.progressBar;
        public Label StatusLabel => this.statusLabel;
        public new Button CancelButton => this.cancelButton;

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancelRequested?.Invoke(); // Gọi sự kiện CancelRequested
        }

        public event Action OnCancelRequested;
    }
}
