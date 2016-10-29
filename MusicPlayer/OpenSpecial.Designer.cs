namespace MusicPlayer
{
    partial class OpenSpecial
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.addedOn = new System.Windows.Forms.DateTimePicker();
            this.addedOnChecked = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.search = new System.Windows.Forms.TextBox();
            this.Submit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(260, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start year added to library";
            // 
            // addedOn
            // 
            this.addedOn.Location = new System.Drawing.Point(413, 9);
            this.addedOn.Name = "addedOn";
            this.addedOn.Size = new System.Drawing.Size(363, 31);
            this.addedOn.TabIndex = 1;
            // 
            // addedOnChecked
            // 
            this.addedOnChecked.AutoSize = true;
            this.addedOnChecked.Location = new System.Drawing.Point(800, 13);
            this.addedOnChecked.Name = "addedOnChecked";
            this.addedOnChecked.Size = new System.Drawing.Size(28, 27);
            this.addedOnChecked.TabIndex = 2;
            this.addedOnChecked.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(358, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Contains the following text (Search):";
            // 
            // search
            // 
            this.search.Location = new System.Drawing.Point(413, 63);
            this.search.Name = "search";
            this.search.Size = new System.Drawing.Size(363, 31);
            this.search.TabIndex = 4;
            // 
            // Submit
            // 
            this.Submit.Location = new System.Drawing.Point(716, 267);
            this.Submit.Name = "Submit";
            this.Submit.Size = new System.Drawing.Size(112, 46);
            this.Submit.TabIndex = 5;
            this.Submit.Text = "Submit";
            this.Submit.UseVisualStyleBackColor = true;
            this.Submit.Click += new System.EventHandler(this.Submit_Click);
            // 
            // OpenSpecial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 336);
            this.Controls.Add(this.Submit);
            this.Controls.Add(this.search);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.addedOnChecked);
            this.Controls.Add(this.addedOn);
            this.Controls.Add(this.label1);
            this.Name = "OpenSpecial";
            this.Text = "OpenSpecial";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker addedOn;
        private System.Windows.Forms.CheckBox addedOnChecked;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox search;
        private System.Windows.Forms.Button Submit;
    }
}