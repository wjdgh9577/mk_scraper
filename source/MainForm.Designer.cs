using System.Drawing;
using System.Windows.Forms;

namespace MkScraper.WinForms;

partial class MainForm
{
    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        datePicker = new DateTimePicker();
        label1 = new Label();
        label2 = new Label();
        comboSection = new ComboBox();
        btnDownload = new Button();
        txtLog = new RichTextBox();
        lblOutputRoot = new Label();
        txtOutputRoot = new TextBox();
        btnOpenDirectory = new Button();
        btnBrowse = new Button();
        SuspendLayout();
        // 
        // datePicker
        // 
        datePicker.Location = new Point(68, 15);
        datePicker.Name = "datePicker";
        datePicker.Size = new Size(200, 23);
        datePicker.TabIndex = 0;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(20, 19);
        label1.Name = "label1";
        label1.Size = new Size(31, 15);
        label1.TabIndex = 1;
        label1.Text = "Date";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(286, 19);
        label2.Name = "label2";
        label2.Size = new Size(45, 15);
        label2.TabIndex = 2;
        label2.Text = "Section";
        // 
        // comboSection
        // 
        comboSection.DropDownStyle = ComboBoxStyle.DropDownList;
        comboSection.FormattingEnabled = true;
        comboSection.Items.AddRange(new object[] { "A", "B" });
        comboSection.Location = new Point(337, 15);
        comboSection.Name = "comboSection";
        comboSection.Size = new Size(70, 23);
        comboSection.TabIndex = 3;
        comboSection.SelectedIndex = 0;
        // 
        // btnDownload
        // 
        btnDownload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnDownload.Location = new Point(480, 14);
        btnDownload.Name = "btnDownload";
        btnDownload.Size = new Size(110, 27);
        btnDownload.TabIndex = 4;
        btnDownload.Text = "Download";
        btnDownload.UseVisualStyleBackColor = true;
        btnDownload.Click += btnDownload_Click;
        // 
        // txtLog
        // 
        txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtLog.Location = new Point(20, 112);
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.Size = new Size(570, 256);
        txtLog.TabIndex = 5;
        txtLog.Text = "";
        // 
        // lblOutputRoot
        // 
        lblOutputRoot.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblOutputRoot.AutoSize = true;
        lblOutputRoot.Location = new Point(20, 54);
        lblOutputRoot.Name = "lblOutputRoot";
        lblOutputRoot.Size = new Size(83, 15);
        lblOutputRoot.TabIndex = 6;
        lblOutputRoot.Text = "Download root";
        // 
        // txtOutputRoot
        // 
        txtOutputRoot.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtOutputRoot.Location = new Point(20, 73);
        txtOutputRoot.Name = "txtOutputRoot";
        txtOutputRoot.PlaceholderText = "Choose folder...";
        txtOutputRoot.Size = new Size(330, 23);
        txtOutputRoot.TabIndex = 6;
        // 
        // btnOpenDirectory
        // 
        btnOpenDirectory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnOpenDirectory.Location = new Point(360, 72);
        btnOpenDirectory.Name = "btnOpenDirectory";
        btnOpenDirectory.Size = new Size(110, 25);
        btnOpenDirectory.TabIndex = 7;
        btnOpenDirectory.Text = "Open Directory";
        btnOpenDirectory.UseVisualStyleBackColor = true;
        btnOpenDirectory.Click += btnOpenDirectory_Click;
        // 
        // btnBrowse
        // 
        btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBrowse.Location = new Point(480, 72);
        btnBrowse.Name = "btnBrowse";
        btnBrowse.Size = new Size(110, 25);
        btnBrowse.TabIndex = 8;
        btnBrowse.Text = "Browse…";
        btnBrowse.UseVisualStyleBackColor = true;
        btnBrowse.Click += btnBrowse_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(614, 381);
        Controls.Add(btnOpenDirectory);
        Controls.Add(btnBrowse);
        Controls.Add(txtOutputRoot);
        Controls.Add(lblOutputRoot);
        Controls.Add(txtLog);
        Controls.Add(btnDownload);
        Controls.Add(comboSection);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(datePicker);
        MinimumSize = new Size(630, 420);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "MK Scraper";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.ComponentModel.IContainer? components;
    private DateTimePicker datePicker = null!;
    private Label label1 = null!;
    private Label label2 = null!;
    private ComboBox comboSection = null!;
    private Button btnDownload = null!;
    private RichTextBox txtLog = null!;
    private Label lblOutputRoot = null!;
    private TextBox txtOutputRoot = null!;
    private Button btnBrowse = null!;
    private Button btnOpenDirectory = null!;
}
