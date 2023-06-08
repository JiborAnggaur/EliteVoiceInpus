namespace SpeechRecognition
{
    partial class ParametersWindow
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
            this.SynthesizerMSOpt = new System.Windows.Forms.RadioButton();
            this.SynthesizerSileroOpt = new System.Windows.Forms.RadioButton();
            this.OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SynthesizerMSOpt
            // 
            this.SynthesizerMSOpt.AutoSize = true;
            this.SynthesizerMSOpt.Location = new System.Drawing.Point(39, 35);
            this.SynthesizerMSOpt.Name = "SynthesizerMSOpt";
            this.SynthesizerMSOpt.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SynthesizerMSOpt.Size = new System.Drawing.Size(103, 24);
            this.SynthesizerMSOpt.TabIndex = 0;
            this.SynthesizerMSOpt.TabStop = true;
            this.SynthesizerMSOpt.Text = "MS Synth";
            this.SynthesizerMSOpt.UseVisualStyleBackColor = true;
            this.SynthesizerMSOpt.CheckedChanged += new System.EventHandler(this.SynthesizerMSOpt_CheckedChanged);
            // 
            // SynthesizerSileroOpt
            // 
            this.SynthesizerSileroOpt.AutoSize = true;
            this.SynthesizerSileroOpt.Location = new System.Drawing.Point(39, 84);
            this.SynthesizerSileroOpt.Name = "SynthesizerSileroOpt";
            this.SynthesizerSileroOpt.Size = new System.Drawing.Size(119, 24);
            this.SynthesizerSileroOpt.TabIndex = 1;
            this.SynthesizerSileroOpt.TabStop = true;
            this.SynthesizerSileroOpt.Text = "Silero Synth";
            this.SynthesizerSileroOpt.UseVisualStyleBackColor = true;
            this.SynthesizerSileroOpt.CheckedChanged += new System.EventHandler(this.SynthesizerSileroOpt_CheckedChanged);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.Location = new System.Drawing.Point(360, 161);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(139, 52);
            this.OK.TabIndex = 2;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // ParametersWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 237);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.SynthesizerSileroOpt);
            this.Controls.Add(this.SynthesizerMSOpt);
            this.Name = "ParametersWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ParametersWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton SynthesizerMSOpt;
        private System.Windows.Forms.RadioButton SynthesizerSileroOpt;
        private System.Windows.Forms.Button OK;
    }
}