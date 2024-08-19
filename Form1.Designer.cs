using System;

namespace UploadGoogleDrive
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.labelUserEmail = new System.Windows.Forms.Label();
            this.buttonChangeAccount = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.buttonSelectFile.Location = new System.Drawing.Point(592, 195);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(131, 48);
            this.buttonSelectFile.TabIndex = 3;
            this.buttonSelectFile.Text = "Select";
            this.buttonSelectFile.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label3.Location = new System.Drawing.Point(29, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(194, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "LIST UPLOAD FILE:";
            this.label3.Click += new System.EventHandler(this.labelTextLUd_Click);
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.BackColor = System.Drawing.Color.Cyan;
            this.buttonUpdate.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.buttonUpdate.Location = new System.Drawing.Point(592, 249);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(131, 42);
            this.buttonUpdate.TabIndex = 6;
            this.buttonUpdate.Text = "Upload";
            this.buttonUpdate.UseVisualStyleBackColor = false;
            // 
            // listBoxFiles
            // 
            this.listBoxFiles.AllowDrop = true;
            this.listBoxFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.ItemHeight = 20;
            this.listBoxFiles.Location = new System.Drawing.Point(34, 147);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.Size = new System.Drawing.Size(545, 244);
            this.listBoxFiles.TabIndex = 5;
            this.listBoxFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListBoxFiles_DragDrop);
            this.listBoxFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListBoxFiles_DragEnter);
            // 
            // buttonRemove
            // 
            this.buttonRemove.BackColor = System.Drawing.Color.Red;
            this.buttonRemove.Location = new System.Drawing.Point(592, 301);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(131, 41);
            this.buttonRemove.TabIndex = 7;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = false;
            this.buttonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // labelUserEmail
            // 
            this.labelUserEmail.AutoSize = true;
            this.labelUserEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUserEmail.Location = new System.Drawing.Point(31, 50);
            this.labelUserEmail.Name = "labelUserEmail";
            this.labelUserEmail.Size = new System.Drawing.Size(139, 25);
            this.labelUserEmail.TabIndex = 9;
            this.labelUserEmail.Text = "Name address";
            this.labelUserEmail.Click += new System.EventHandler(this.labelNameAddress_Click);
            // 
            // buttonChangeAccount
            // 
            this.buttonChangeAccount.BackColor = System.Drawing.Color.AliceBlue;
            this.buttonChangeAccount.Location = new System.Drawing.Point(592, 147);
            this.buttonChangeAccount.Name = "buttonChangeAccount";
            this.buttonChangeAccount.Size = new System.Drawing.Size(131, 42);
            this.buttonChangeAccount.TabIndex = 10;
            this.buttonChangeAccount.Text = "Change Account";
            this.buttonChangeAccount.UseVisualStyleBackColor = false;
            this.buttonChangeAccount.Click += new System.EventHandler(this.ButtonChangeAccount_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 450);
            this.Controls.Add(this.buttonChangeAccount);
            this.Controls.Add(this.labelUserEmail);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonUpdate);
            this.Controls.Add(this.listBoxFiles);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonSelectFile);
            this.Name = "Form1";
            this.Text = "Upload File Google Drive";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void labelNameAddress_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void labelTextLUd_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void labelTextFile_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Label labelUserEmail;
        private System.Windows.Forms.Button buttonChangeAccount;
    }
}
