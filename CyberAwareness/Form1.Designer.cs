namespace CyberAwareness
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // Controls
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblAsciiArt;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel panelChat;
        private System.Windows.Forms.RichTextBox chatBox;
        private System.Windows.Forms.Panel panelInput;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Panel panelTopics;
        private System.Windows.Forms.Label lblTopicsTitle;
        private System.Windows.Forms.Button btnPassword;
        private System.Windows.Forms.Button btnPhishing;
        private System.Windows.Forms.Button btnScam;
        private System.Windows.Forms.Button btnPrivacy;
        private System.Windows.Forms.Button btnMalware;
        private System.Windows.Forms.Button btnFirewall;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            //  Panels
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelChat = new System.Windows.Forms.Panel();
            this.panelInput = new System.Windows.Forms.Panel();
            this.panelTopics = new System.Windows.Forms.Panel();

            //  Header controls 
            this.lblAsciiArt = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();

            //  Chat 
            this.chatBox = new System.Windows.Forms.RichTextBox();

            //  Input row 
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();

            //  Quick-topic buttons 
            this.lblTopicsTitle = new System.Windows.Forms.Label();
            this.btnPassword = new System.Windows.Forms.Button();
            this.btnPhishing = new System.Windows.Forms.Button();
            this.btnScam = new System.Windows.Forms.Button();
            this.btnPrivacy = new System.Windows.Forms.Button();
            this.btnMalware = new System.Windows.Forms.Button();
            this.btnFirewall = new System.Windows.Forms.Button();

            // Status strip 
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();

            
            // COLOURS
            
            var darkBg = System.Drawing.Color.FromArgb(13, 17, 23);
            var panelBg = System.Drawing.Color.FromArgb(22, 27, 34);
            var inputBg = System.Drawing.Color.FromArgb(30, 37, 46);
            var accentGreen = System.Drawing.Color.FromArgb(0, 255, 135);
            var accentBlue = System.Drawing.Color.FromArgb(56, 189, 248);
            var textMain = System.Drawing.Color.FromArgb(230, 237, 243);
            var textMuted = System.Drawing.Color.FromArgb(125, 133, 144);
            var btnTopicBg = System.Drawing.Color.FromArgb(33, 43, 54);
            var btnTopicFg = accentBlue;

            
            // HEADER PANEL  
            
            this.panelHeader.BackColor = darkBg;
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 160;
            this.panelHeader.Padding = new System.Windows.Forms.Padding(15, 8, 15, 8);

            // ASCII art
            this.lblAsciiArt.Text =
                "  ██████╗██╗   ██╗██████╗ ███████╗██████╗ \r\n" +
                " ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗\r\n" +
                " ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝\r\n" +
                " ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗\r\n" +
                " ╚██████╗   ██║   ██████╔╝███████╗██║  ██║\r\n" +
                "  ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝";
            this.lblAsciiArt.Font = new System.Drawing.Font("Consolas", 6.8f, System.Drawing.FontStyle.Bold);
            this.lblAsciiArt.ForeColor = accentGreen;
            this.lblAsciiArt.BackColor = System.Drawing.Color.Transparent;
            this.lblAsciiArt.AutoSize = false;
            this.lblAsciiArt.Size = new System.Drawing.Size(680, 90);
            this.lblAsciiArt.Location = new System.Drawing.Point(10, 5);
            this.lblAsciiArt.TextAlign = System.Drawing.ContentAlignment.TopLeft;

            // Title
            this.lblTitle.Text = "🔒  Cybersecurity Awareness Chatbot";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13f, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = textMain;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(10, 100);

            // Subtitle
            this.lblSubtitle.Text = "Ask me about passwords, phishing, scams, privacy, malware, or firewalls.";
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5f);
            this.lblSubtitle.ForeColor = textMuted;
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Location = new System.Drawing.Point(12, 128);

            this.panelHeader.Controls.Add(this.lblAsciiArt);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Controls.Add(this.lblSubtitle);

           
            // QUICK-TOPIC PANEL  
            
            this.panelTopics.BackColor = panelBg;
            this.panelTopics.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelTopics.Width = 140;
            this.panelTopics.Padding = new System.Windows.Forms.Padding(8);

            this.lblTopicsTitle.Text = "Quick Topics";
            this.lblTopicsTitle.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lblTopicsTitle.ForeColor = accentGreen;
            this.lblTopicsTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTopicsTitle.AutoSize = false;
            this.lblTopicsTitle.Size = new System.Drawing.Size(124, 22);
            this.lblTopicsTitle.Location = new System.Drawing.Point(8, 10);

            var topicButtons = new[]
            {
                (this.btnPassword, "🔑 Password"),
                (this.btnPhishing, "🎣 Phishing"),
                (this.btnScam,     "⚠️ Scam"),
                (this.btnPrivacy,  "🛡️ Privacy"),
                (this.btnMalware,  "🦠 Malware"),
                (this.btnFirewall, "🔥 Firewall"),
            };

            int ty = 40;
            foreach (var (btn, label) in topicButtons)
            {
                btn.Text = label;
                btn.Size = new System.Drawing.Size(124, 34);
                btn.Location = new System.Drawing.Point(8, ty);
                btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(50, 60, 72);
                btn.FlatAppearance.BorderSize = 1;
                btn.BackColor = btnTopicBg;
                btn.ForeColor = btnTopicFg;
                btn.Font = new System.Drawing.Font("Segoe UI", 8.5f);
                btn.Cursor = System.Windows.Forms.Cursors.Hand;
                btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                btn.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
                btn.Click += new System.EventHandler(this.TopicButton_Click);
                this.panelTopics.Controls.Add(btn);
                ty += 42;
            }
            this.panelTopics.Controls.Add(this.lblTopicsTitle);

            
            // INPUT PANEL  
            
            this.panelInput.BackColor = inputBg;
            this.panelInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInput.Height = 55;
            this.panelInput.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);

            this.txtInput.BackColor = System.Drawing.Color.FromArgb(13, 17, 23);
            this.txtInput.ForeColor = textMain;
            this.txtInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInput.Font = new System.Drawing.Font("Segoe UI", 10f);
            this.txtInput.Location = new System.Drawing.Point(10, 13);
            this.txtInput.Size = new System.Drawing.Size(390, 28);
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);

            this.btnSend.Text = "Send ➤";
            this.btnSend.Location = new System.Drawing.Point(410, 12);
            this.btnSend.Size = new System.Drawing.Size(85, 30);
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.BackColor = accentGreen;
            this.btnSend.ForeColor = darkBg;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);

            this.btnClear.Text = "Clear";
            this.btnClear.Location = new System.Drawing.Point(503, 12);
            this.btnClear.Size = new System.Drawing.Size(60, 30);
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(60, 70, 85);
            this.btnClear.FlatAppearance.BorderSize = 1;
            this.btnClear.BackColor = panelBg;
            this.btnClear.ForeColor = textMuted;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 8.5f);
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);

            this.panelInput.Controls.Add(this.txtInput);
            this.panelInput.Controls.Add(this.btnSend);
            this.panelInput.Controls.Add(this.btnClear);

           
            // CHAT PANEL  (fills remaining space)
            
            this.panelChat.BackColor = darkBg;
            this.panelChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelChat.Padding = new System.Windows.Forms.Padding(10);

            this.chatBox.BackColor = darkBg;
            this.chatBox.ForeColor = textMain;
            this.chatBox.Font = new System.Drawing.Font("Segoe UI", 10f);
            this.chatBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chatBox.ReadOnly = true;
            this.chatBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.chatBox.Dock = System.Windows.Forms.DockStyle.Fill;

            this.panelChat.Controls.Add(this.chatBox);

            
            // STATUS STRIP
            
            this.statusStrip.BackColor = panelBg;
            this.statusStrip.Items.Add(this.lblStatus);
            this.lblStatus.Text = "Ready  •  Type a message or click a quick topic";
            this.lblStatus.ForeColor = textMuted;

            
            // FORM
            
            this.Text = "CyberAwareness — Security Chatbot";
            this.BackColor = darkBg;
            this.ClientSize = new System.Drawing.Size(720, 620);
            this.MinimumSize = new System.Drawing.Size(700, 580);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font = new System.Drawing.Font("Segoe UI", 9f);

            
            this.Controls.Add(this.panelChat);    
            this.Controls.Add(this.panelTopics);  
            this.Controls.Add(this.panelInput);   
            this.Controls.Add(this.panelHeader);  
            this.Controls.Add(this.statusStrip);  

            this.Load += new System.EventHandler(this.Form1_Load);
        }
    }
}
