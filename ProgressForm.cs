using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UploadGoogleDrive
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
