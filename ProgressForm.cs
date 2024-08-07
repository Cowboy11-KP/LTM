using System;
using System.Windows.Forms;

namespace UploadFileGoogleDrive
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        // Expose the ProgressBar and StatusLabel to be accessible from outside
        public ProgressBar ProgressBar => this.progressBar;
        public Label StatusLabel => this.statusLabel;

        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }
    }
}
