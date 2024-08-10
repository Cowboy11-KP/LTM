using System;

namespace UploadFileGoogleDrive
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelNameAddress = new System.Windows.Forms.Label();
            this.buttonChangeAccount = new System.Windows.Forms.Button();
            this.panelControl = new System.Windows.Forms.Panel();
            this.panelControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.buttonSelectFile.ForeColor = System.Drawing.Color.White;
            this.buttonSelectFile.Location = new System.Drawing.Point(622, 223);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(120, 35);
            this.buttonSelectFile.TabIndex = 3;
            this.buttonSelectFile.Text = "Select Files";
            this.buttonSelectFile.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.label3.Location = new System.Drawing.Point(30, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 28);
            this.label3.TabIndex = 4;
            this.label3.Text = "Upload List:";
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.buttonUpdate.ForeColor = System.Drawing.Color.White;
            this.buttonUpdate.Location = new System.Drawing.Point(622, 284);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(120, 35);
            this.buttonUpdate.TabIndex = 6;
            this.buttonUpdate.Text = "Upload";
            this.buttonUpdate.UseVisualStyleBackColor = false;
            // 
            // listBoxFiles
            // 
            this.listBoxFiles.AllowDrop = true;
            this.listBoxFiles.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.ItemHeight = 23;
            this.listBoxFiles.Location = new System.Drawing.Point(30, 130);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.Size = new System.Drawing.Size(570, 303);
            this.listBoxFiles.TabIndex = 5;
            this.listBoxFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxFiles_DragDrop);
            this.listBoxFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBoxFiles_DragEnter);
            // 
            // buttonRemove
            // 
            this.buttonRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.buttonRemove.ForeColor = System.Drawing.Color.White;
            this.buttonRemove.Location = new System.Drawing.Point(622, 344);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(120, 35);
            this.buttonRemove.TabIndex = 7;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.label1.Location = new System.Drawing.Point(30, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 28);
            this.label1.TabIndex = 8;
            this.label1.Text = "Account:";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // labelNameAddress
            // 
            this.labelNameAddress.AutoSize = true;
            this.labelNameAddress.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelNameAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(114)))), ((int)(((byte)(164)))));
            this.labelNameAddress.Location = new System.Drawing.Point(153, 34);
            this.labelNameAddress.Name = "labelNameAddress";
            this.labelNameAddress.Size = new System.Drawing.Size(139, 28);
            this.labelNameAddress.TabIndex = 9;
            this.labelNameAddress.Text = "Name Address";
            this.labelNameAddress.Click += new System.EventHandler(this.labelNameAddress_Click);
            // 
            // buttonChangeAccount
            // 
            this.buttonChangeAccount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(87)))), ((int)(((byte)(140)))));
            this.buttonChangeAccount.ForeColor = System.Drawing.Color.White;
            this.buttonChangeAccount.Location = new System.Drawing.Point(337, 32);
            this.buttonChangeAccount.Name = "buttonChangeAccount";
            this.buttonChangeAccount.Size = new System.Drawing.Size(137, 35);
            this.buttonChangeAccount.TabIndex = 10;
            this.buttonChangeAccount.Text = "Change Account";
            this.buttonChangeAccount.UseVisualStyleBackColor = false;
            // 
            // panelControl
            // 
            this.panelControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelControl.Controls.Add(this.buttonSelectFile);
            this.panelControl.Controls.Add(this.label3);
            this.panelControl.Controls.Add(this.buttonUpdate);
            this.panelControl.Controls.Add(this.listBoxFiles);
            this.panelControl.Controls.Add(this.buttonRemove);
            this.panelControl.Controls.Add(this.label1);
            this.panelControl.Controls.Add(this.labelNameAddress);
            this.panelControl.Controls.Add(this.buttonChangeAccount);
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl.Location = new System.Drawing.Point(0, 0);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(780, 450);
            this.panelControl.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 450);
            this.Controls.Add(this.panelControl);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Upload File to Google Drive";
            this.panelControl.ResumeLayout(false);
            this.panelControl.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelNameAddress;
        private System.Windows.Forms.Button buttonChangeAccount;
        private System.Windows.Forms.Panel panelControl;
    }
}