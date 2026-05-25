using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace CyberAwareness
{
    public partial class Form1 : Form
    {
        private Chatbot bot;

        private static readonly Color ColUser = Color.FromArgb(56, 189, 248);
        private static readonly Color ColBot = Color.FromArgb(0, 255, 135);
        private static readonly Color ColSystem = Color.FromArgb(125, 133, 144);
        private static readonly Color ColTitle = Color.FromArgb(255, 215, 0);

        public Form1()
        {
            InitializeComponent();
            bot = new Chatbot();
        }

        //  Form Load 
        private void Form1_Load(object sender, EventArgs e)
        {
            // Play sound from POE 1 - Check if the sound file exists before trying to play it
            string soundPath = @"C:\Users\zandi\source\repos\CyberAwareness\CyberAwareness\Sound.wav";
            if (File.Exists(soundPath))
            {
                try { new SoundPlayer(soundPath).Play(); }
                catch { }
            }

            // translate POE 1 into GUI)
            string name = PromptForName();
            if (string.IsNullOrWhiteSpace(name)) name = "User";
            bot.UserName = name;

            // ASCII art from POE 1 
            AppendColoured(
@"  ██████╗ ██████╗ ████████╗
 ██╔════╝██╔═══██╗╚══██╔══╝
 ██║     ██║   ██║   ██║   
 ██║     ██║   ██║   ██║   
 ╚██████╗╚██████╔╝   ██║   
  ╚═════╝ ╚═════╝    ╚═╝   
  +---------------------------------+
  |  [ CYBERSECURITY AWARENESS BOT ]|
  |  [ PROTECTING YOU ONLINE      ] |
  |  [ STAY SAFE. STAY AWARE.     ] |
  +---------------------------------+
  🔒 THINK SAFE  🛡️ ACT SMART  ⚠️ STAY ALERT", ColBot, bold: true);

            AppendSystem("═══════════════════════════════════════════════");
            AppendColoured($"  Hello, {name}! Welcome to the Cybersecurity Awareness Bot.", ColTitle, bold: true);
            AppendColoured($"  I am here to help you stay safe online.", ColBot);
            AppendSystem("═══════════════════════════════════════════════");

            // Menu from POE 1
            AppendSystem("\n  You can ask me about:");
            AppendSystem("  [1] How are you?");
            AppendSystem("  [2] What is your purpose?");
            AppendSystem("  [3] What can I ask you about?");
            AppendSystem("  [4] Password safety");
            AppendSystem("  [5] Phishing");
            AppendSystem("  [6] Safe browsing");
            AppendSystem("  [0] Goodbye\n");

            lblStatus.Text = $"Logged in as: {name}  •  Type a message or click a quick topic";
        }

        //  Name Prompt (replaces Console.ReadLine POE 1)
        private string PromptForName()
        {
            Form prompt = new Form();
            prompt.Text = "Welcome!";
            prompt.Size = new Size(350, 150);
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.BackColor = Color.FromArgb(22, 27, 34);
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.MaximizeBox = false;
            prompt.MinimizeBox = false;

            Label lbl = new Label();
            lbl.Text = "Please enter your name:";
            lbl.ForeColor = Color.FromArgb(230, 237, 243);
            lbl.Location = new Point(15, 18);
            lbl.AutoSize = true;

            TextBox txt = new TextBox();
            txt.Location = new Point(15, 42);
            txt.Size = new Size(300, 25);
            txt.BackColor = Color.FromArgb(13, 17, 23);
            txt.ForeColor = Color.FromArgb(230, 237, 243);
            txt.BorderStyle = BorderStyle.FixedSingle;

            Button btn = new Button();
            btn.Text = "OK";
            btn.Location = new Point(230, 75);
            btn.Size = new Size(85, 28);
            btn.BackColor = Color.FromArgb(0, 255, 135);
            btn.ForeColor = Color.FromArgb(13, 17, 23);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.DialogResult = DialogResult.OK;

            prompt.Controls.Add(lbl);
            prompt.Controls.Add(txt);
            prompt.Controls.Add(btn);
            prompt.AcceptButton = btn;

            return prompt.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : "User";
        }

        //  Send button
        private void btnSend_Click(object sender, EventArgs e) => ProcessInput();

        //  Enter key sends message 
        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ProcessInput();
            }
        }

        // Process the user input
        private void ProcessInput()
        {
            string userInput = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(userInput))
            {
                SystemSounds.Beep.Play();
                return;
            }

            // Map number menu choices from POE 1
            string mapped = MapMenuNumber(userInput);

            AppendColoured($"  {bot.UserName}: {userInput}", ColUser);
            txtInput.Clear();
            txtInput.Focus();

            string response = bot.GetResponse(mapped);
            AppendColoured("  Bot: " + response, ColBot);
            AppendSystem("");

            lblStatus.Text = $"Logged in as: {bot.UserName}  •  Last topic: {bot.LastTopic}";
        }

        //  Maps numbered menu options to keywords (POE 1) 
        private string MapMenuNumber(string input)
        {
            switch (input.Trim())
            {
                case "1": return "how are you";
                case "2": return "what is your purpose";
                case "3": return "what can i ask you about";
                case "4": return "password safety";
                case "5": return "phishing";
                case "6": return "safe browsing";
                case "0": return "goodbye";
                default: return input;
            }
        }

        //  Quick topic sidebar buttons 
        private void TopicButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            string label = btn.Text.Substring(btn.Text.IndexOf(' ') + 1).ToLower();
            txtInput.Text = label;
            ProcessInput();
        }

        // Clear button
        private void btnClear_Click(object sender, EventArgs e)
        {
            chatBox.Clear();
            bot.Reset();
            lblStatus.Text = $"Logged in as: {bot.UserName}  •  Conversation cleared.";
            AppendSystem("Conversation cleared. Ask me anything!\n");
        }

        // Append coloured text to chat box
        private void AppendColoured(string text, Color color, bool bold = false)
        {
            chatBox.SelectionStart = chatBox.TextLength;
            chatBox.SelectionLength = 0;
            chatBox.SelectionColor = color;
            chatBox.SelectionFont = bold
                ? new Font(chatBox.Font, FontStyle.Bold)
                : chatBox.Font;
            chatBox.AppendText(text + Environment.NewLine);
            chatBox.SelectionColor = chatBox.ForeColor;
            chatBox.ScrollToCaret();
        }

        private void AppendSystem(string text) => AppendColoured(text, ColSystem);
    }
}
