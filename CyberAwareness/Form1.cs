using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;
using System.Collections.Generic;

namespace CyberAwareness
{
    public partial class Form1 : Form
    {
        private static readonly Color ColUser = Color.FromArgb(56, 189, 248);
        private static readonly Color ColBot = Color.FromArgb(0, 255, 135);
        private static readonly Color ColSystem = Color.FromArgb(125, 133, 144);
        private static readonly Color ColTitle = Color.FromArgb(255, 215, 0);
        private static readonly Color ColWarn = Color.FromArgb(248, 81, 73);
        private static readonly Color DarkBg = Color.FromArgb(13, 17, 23);
        private static readonly Color PanelBg = Color.FromArgb(22, 27, 34);
        private static readonly Color InputBg = Color.FromArgb(30, 37, 46);
        private static readonly Color AccentGreen = Color.FromArgb(0, 255, 135);
        private static readonly Color AccentBlue = Color.FromArgb(56, 189, 248);
        private static readonly Color TextMain = Color.FromArgb(230, 237, 243);
        private static readonly Color TextMuted = Color.FromArgb(125, 133, 144);

        private Chatbot bot = new Chatbot();

        //  Task tab controls
        private TextBox txtTaskTitle = new TextBox();
        private TextBox txtTaskDesc = new TextBox();
        private TextBox txtTaskReminder = new TextBox();
        private Panel taskListPanel = new Panel();
        private Label lblDbStatus = new Label();

        //  Quiz controls and state
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();
        private int quizIndex = -1;
        private int quizScore = 0;
        private bool quizAnswered = false;
        private Label lblQuestion = new Label();
        private Label lblQuizScore = new Label();
        private Label lblQuizNum = new Label();
        private Panel answerPanel = new Panel();
        private Label lblFeedback = new Label();
        private Button btnNextQuestion = new Button();

        // Activity log
        private RichTextBox logBox = new RichTextBox();

        public Form1()
        {
            InitializeComponent();
            BuildPart3Tabs();
        }

       
        // FORM LOAD
        
        private void Form1_Load(object sender, EventArgs e)
        {
            string soundPath = Path.Combine(Application.StartupPath, "Sound_.wav");
            if (File.Exists(soundPath))
            {
                try { new SoundPlayer(soundPath).Play(); }
                catch { }
            }

            string name = PromptForName();
            if (string.IsNullOrWhiteSpace(name)) name = "User";
            bot.UserName = name;

            AppendColoured(
                "  ####### ####### #######\r\n" +
                "  #       #    #  #      \r\n" +
                "  #       #    #  #####  \r\n" +
                "  #       #    #  #      \r\n" +
                "  ####### #    #  #######\r\n" +
                "  +---------------------------------+\r\n" +
                "  |  [ CYBERSECURITY AWARENESS BOT ]|\r\n" +
                "  |  [ PROTECTING YOU ONLINE      ] |\r\n" +
                "  |  [ STAY SAFE. STAY AWARE.     ] |\r\n" +
                "  +---------------------------------+\r\n" +
                "  THINK SAFE  |  ACT SMART  |  STAY ALERT",
                ColBot, true);

            AppendSystem("===================================================");
            AppendColoured("  Hello, " + name + "! Welcome to the Cybersecurity Awareness Bot.", ColTitle, true);
            AppendColoured("  I am here to help you stay safe online.", ColBot, false);
            AppendSystem("===================================================");
            AppendSystem("\n  You can ask me about:");
            AppendSystem("  [1] How are you");
            AppendSystem("  [2] What is your purpose");
            AppendSystem("  [3] What can I ask you about");
            AppendSystem("  [4] Password safety");
            AppendSystem("  [5] Phishing");
            AppendSystem("  [6] Safe browsing");
            AppendSystem("  [0] Goodbye\n");
            AppendSystem("  Tip: Say 'add task', 'view tasks', or 'quiz' to use new features!\n");

            lblStatus.Text = "Logged in as: " + name + "  |  Type a message or click a quick topic";
            ActivityLog.Add("Application started - user: " + name);

            string dbMsg;
            bool dbOk = DatabaseHelper.Initialise(out dbMsg);
            lblDbStatus.Text = dbMsg;
            lblDbStatus.ForeColor = dbOk ? AccentGreen : ColWarn;
            ActivityLog.Add("Database: " + dbMsg);
            if (dbOk) RefreshTaskList();
        }

        
        // CHAT TAB
       
        private void btnSend_Click(object sender, EventArgs e)
        {
            ProcessInput();
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ProcessInput();
            }
        }

        private void ProcessInput()
        {
            string userInput = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(userInput))
            {
                SystemSounds.Beep.Play();
                return;
            }

            string mapped = MapMenuNumber(userInput);
            AppendColoured("  " + bot.UserName + ": " + userInput, ColUser, false);
            txtInput.Clear();
            txtInput.Focus();

            ChatResponse chatResp = bot.GetResponse(mapped);
            AppendColoured("  Bot: " + chatResp.Response, ColBot, false);
            AppendSystem(string.Empty);

            ActivityLog.Add("User: " + userInput);
            ActivityLog.Add("Bot: " + chatResp.Intent.ToString());
            RefreshLogTab();

            lblStatus.Text = "Logged in as: " + bot.UserName + "  |  Last topic: " + bot.LastTopic;

            if (chatResp.Intent == ChatIntent.AddTask)
            {
                mainTabControl.SelectedIndex = 1;
                if (!string.IsNullOrWhiteSpace(chatResp.TaskHint))
                    txtTaskTitle.Text = chatResp.TaskHint;
                txtTaskTitle.Focus();
            }
            else if (chatResp.Intent == ChatIntent.ViewTasks)
            {
                mainTabControl.SelectedIndex = 1;
                RefreshTaskList();
            }
            else if (chatResp.Intent == ChatIntent.StartQuiz)
            {
                mainTabControl.SelectedIndex = 2;
                StartNewQuiz();
            }
        }

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

        private void TopicButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string label = btn.Text.Substring(btn.Text.IndexOf(' ') + 1).ToLower();
            txtInput.Text = label;
            ProcessInput();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            chatBox.Clear();
            bot.Reset();
            lblStatus.Text = "Logged in as: " + bot.UserName + "  |  Conversation cleared.";
            AppendSystem("Conversation cleared. Ask me anything!\n");
            ActivityLog.Add("Chat cleared");
            RefreshLogTab();
        }

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
            lbl.ForeColor = TextMain;
            lbl.Location = new Point(15, 18);
            lbl.AutoSize = true;

            TextBox txt = new TextBox();
            txt.Location = new Point(15, 42);
            txt.Size = new Size(300, 25);
            txt.BackColor = DarkBg;
            txt.ForeColor = TextMain;
            txt.BorderStyle = BorderStyle.FixedSingle;

            Button btn = new Button();
            btn.Text = "OK";
            btn.Location = new Point(230, 75);
            btn.Size = new Size(85, 28);
            btn.BackColor = AccentGreen;
            btn.ForeColor = DarkBg;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.DialogResult = DialogResult.OK;

            prompt.Controls.Add(lbl);
            prompt.Controls.Add(txt);
            prompt.Controls.Add(btn);
            prompt.AcceptButton = btn;

            return prompt.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : "User";
        }

        private void AppendColoured(string text, Color color, bool bold)
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

        private void AppendSystem(string text)
        {
            AppendColoured(text, ColSystem, false);
        }

        
        
        // This method adds tabs 1, 2 and 3 to mainTabControl.
       
        private void BuildPart3Tabs()
        {
            Font uiFont = new Font("Segoe UI", 9f);
            Font boldFont = new Font("Segoe UI", 9f, FontStyle.Bold);
            Color borderCol = Color.FromArgb(48, 54, 61);

            // Tab 1: Task Assistant
            TabPage tabTasks = new TabPage("Task Assistant");
            tabTasks.BackColor = DarkBg;
            tabTasks.Padding = new Padding(12);
            tabTasks.Controls.Add(BuildTaskTab(uiFont, boldFont, borderCol));
            mainTabControl.TabPages.Add(tabTasks);

            // Tab 2: Mini Game
            TabPage tabQuiz = new TabPage("Mini Game");
            tabQuiz.BackColor = DarkBg;
            tabQuiz.Padding = new Padding(12);
            tabQuiz.Controls.Add(BuildQuizTab(uiFont, boldFont));
            mainTabControl.TabPages.Add(tabQuiz);

            // Tab 3: Activity Log
            TabPage tabLog = new TabPage("Activity Log");
            tabLog.BackColor = DarkBg;
            tabLog.Padding = new Padding(12);
            tabLog.Controls.Add(BuildLogTab(uiFont));
            mainTabControl.TabPages.Add(tabLog);
        }

        
        // TASK TAB
        
        private Panel BuildTaskTab(Font uiFont, Font boldFont, Color borderCol)
        {
            Panel root = new Panel();
            root.Dock = DockStyle.Fill;
            root.BackColor = DarkBg;

            Panel formPanel = new Panel();
            formPanel.Width = 300;
            formPanel.Dock = DockStyle.Left;
            formPanel.BackColor = PanelBg;
            formPanel.Padding = new Padding(14);

            int y = 14;

            Label lblFormTitle = new Label();
            lblFormTitle.Text = "Add New Task";
            lblFormTitle.ForeColor = AccentGreen;
            lblFormTitle.Font = boldFont;
            lblFormTitle.AutoSize = true;
            lblFormTitle.Location = new Point(14, y);
            formPanel.Controls.Add(lblFormTitle);
            y += 30;

            Label lblTitle = new Label();
            lblTitle.Text = "Title *";
            lblTitle.ForeColor = TextMuted;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(14, y);
            formPanel.Controls.Add(lblTitle);
            y += 20;

            txtTaskTitle.Location = new Point(14, y);
            txtTaskTitle.Size = new Size(268, 26);
            txtTaskTitle.BackColor = InputBg;
            txtTaskTitle.ForeColor = TextMain;
            txtTaskTitle.BorderStyle = BorderStyle.FixedSingle;
            txtTaskTitle.Font = uiFont;
            formPanel.Controls.Add(txtTaskTitle);
            y += 36;

            Label lblDesc = new Label();
            lblDesc.Text = "Description";
            lblDesc.ForeColor = TextMuted;
            lblDesc.AutoSize = true;
            lblDesc.Location = new Point(14, y);
            formPanel.Controls.Add(lblDesc);
            y += 20;

            txtTaskDesc.Location = new Point(14, y);
            txtTaskDesc.Size = new Size(268, 70);
            txtTaskDesc.BackColor = InputBg;
            txtTaskDesc.ForeColor = TextMain;
            txtTaskDesc.BorderStyle = BorderStyle.FixedSingle;
            txtTaskDesc.Font = uiFont;
            txtTaskDesc.Multiline = true;
            txtTaskDesc.ScrollBars = ScrollBars.Vertical;
            formPanel.Controls.Add(txtTaskDesc);
            y += 80;

            Label lblReminder = new Label();
            lblReminder.Text = "Reminder (optional)";
            lblReminder.ForeColor = TextMuted;
            lblReminder.AutoSize = true;
            lblReminder.Location = new Point(14, y);
            formPanel.Controls.Add(lblReminder);
            y += 20;

            txtTaskReminder.Location = new Point(14, y);
            txtTaskReminder.Size = new Size(268, 26);
            txtTaskReminder.BackColor = InputBg;
            txtTaskReminder.ForeColor = TextMain;
            txtTaskReminder.BorderStyle = BorderStyle.FixedSingle;
            txtTaskReminder.Font = uiFont;
            formPanel.Controls.Add(txtTaskReminder);
            y += 30;

            Label lblHint = new Label();
            lblHint.Text = "e.g. in 7 days  or  2025-12-01";
            lblHint.ForeColor = TextMuted;
            lblHint.AutoSize = true;
            lblHint.Location = new Point(14, y);
            lblHint.Font = new Font("Segoe UI", 7.5f);
            formPanel.Controls.Add(lblHint);
            y += 22;

            Button btnAddTask = MakeButton("Add Task", AccentGreen, DarkBg,
                new Point(14, y), new Size(268, 34), boldFont);
            btnAddTask.Click += new EventHandler(BtnAddTask_Click);
            formPanel.Controls.Add(btnAddTask);
            y += 44;

            Button btnRefresh = MakeButton("Refresh List", PanelBg, TextMuted,
                new Point(14, y), new Size(268, 30), uiFont);
            btnRefresh.FlatAppearance.BorderColor = borderCol;
            btnRefresh.Click += delegate { RefreshTaskList(); };
            formPanel.Controls.Add(btnRefresh);
            y += 44;

            Label sep = new Label();
            sep.BorderStyle = BorderStyle.Fixed3D;
            sep.Location = new Point(14, y);
            sep.Size = new Size(268, 2);
            formPanel.Controls.Add(sep);
            y += 12;

            Label lblDbLbl = new Label();
            lblDbLbl.Text = "DB Status";
            lblDbLbl.ForeColor = TextMuted;
            lblDbLbl.AutoSize = true;
            lblDbLbl.Location = new Point(14, y);
            formPanel.Controls.Add(lblDbLbl);
            y += 20;

            lblDbStatus.Text = "Connecting...";
            lblDbStatus.ForeColor = TextMuted;
            lblDbStatus.Location = new Point(14, y);
            lblDbStatus.Size = new Size(268, 40);
            lblDbStatus.Font = uiFont;
            lblDbStatus.AutoSize = false;
            formPanel.Controls.Add(lblDbStatus);

            Panel listPanel = new Panel();
            listPanel.Dock = DockStyle.Fill;
            listPanel.BackColor = DarkBg;
            listPanel.Padding = new Padding(12, 8, 8, 8);

            Label listHeader = new Label();
            listHeader.Text = "My Cybersecurity Tasks";
            listHeader.ForeColor = AccentGreen;
            listHeader.Font = boldFont;
            listHeader.AutoSize = true;
            listHeader.Location = new Point(12, 8);
            listPanel.Controls.Add(listHeader);

            Panel scroller = new Panel();
            scroller.Dock = DockStyle.Fill;
            scroller.AutoScroll = true;
            scroller.BackColor = DarkBg;

            taskListPanel.AutoSize = true;
            taskListPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            taskListPanel.BackColor = DarkBg;
            taskListPanel.Padding = new Padding(12, 38, 12, 12);
            taskListPanel.MinimumSize = new Size(400, 0);
            scroller.Controls.Add(taskListPanel);
            listPanel.Controls.Add(scroller);

            root.Controls.Add(listPanel);
            root.Controls.Add(formPanel);
            return root;
        }

        private void BtnAddTask_Click(object sender, EventArgs e)
        {
            string title = txtTaskTitle.Text.Trim();
            string desc = txtTaskDesc.Text.Trim();
            string reminder = txtTaskReminder.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTaskTitle.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(desc))
                desc = "Cybersecurity task: " + title;

            string addErr;
            int newId = DatabaseHelper.AddTask(title, desc, reminder, out addErr);

            if (newId > 0)
            {
                string chatMsg = string.IsNullOrWhiteSpace(reminder)
                    ? "Task added: " + title + " - " + desc
                    : "Task added: " + title + ". Reminder set: " + reminder;

                mainTabControl.SelectedIndex = 0;
                AppendColoured("  Bot: " + chatMsg, ColBot, false);
                AppendSystem(string.Empty);
                mainTabControl.SelectedIndex = 1;

                ActivityLog.Add("Task added (ID " + newId + "): " + title);
                RefreshLogTab();

                txtTaskTitle.Clear();
                txtTaskDesc.Clear();
                txtTaskReminder.Clear();
                lblStatus.Text = "Task saved: " + title;
                RefreshTaskList();
            }
            else
            {
                MessageBox.Show("Could not save task:\n" + addErr, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ActivityLog.Add("Task add failed: " + addErr);
                RefreshLogTab();
            }
        }

        private void RefreshTaskList()
        {
            taskListPanel.Controls.Clear();

            string loadErr;
            List<TaskItem> tasks = DatabaseHelper.GetAllTasks(out loadErr);

            if (!string.IsNullOrEmpty(loadErr))
            {
                Label err = new Label();
                err.Text = "Could not load tasks: " + loadErr;
                err.ForeColor = ColWarn;
                err.AutoSize = true;
                err.Location = new Point(0, 0);
                taskListPanel.Controls.Add(err);
                return;
            }

            if (tasks.Count == 0)
            {
                Label empty = new Label();
                empty.Text = "No tasks yet. Add your first cybersecurity task!";
                empty.ForeColor = TextMuted;
                empty.AutoSize = true;
                empty.Location = new Point(0, 0);
                empty.Font = new Font("Segoe UI", 9f, FontStyle.Italic);
                taskListPanel.Controls.Add(empty);
                return;
            }

            int cardY = 0;
            foreach (TaskItem task in tasks)
            {
                Panel card = BuildTaskCard(task, cardY);
                taskListPanel.Controls.Add(card);
                cardY += card.Height + 8;
            }
        }

        private Panel BuildTaskCard(TaskItem task, int yPos)
        {
            bool done = task.IsComplete;

            Panel card = new Panel();
            card.Location = new Point(0, yPos);
            card.Size = new Size(taskListPanel.Width > 0 ? taskListPanel.Width - 4 : 480, 100);
            card.BackColor = PanelBg;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            Panel bar = new Panel();
            bar.Location = new Point(0, 0);
            bar.Size = new Size(4, 100);
            bar.BackColor = done ? TextMuted : AccentGreen;
            card.Controls.Add(bar);

            Label titleLbl = new Label();
            titleLbl.Text = (done ? "Done: " : "Todo: ") + task.Title;
            titleLbl.ForeColor = done ? TextMuted : TextMain;
            titleLbl.Font = new Font("Segoe UI", 10f, done
                ? FontStyle.Strikeout | FontStyle.Bold : FontStyle.Bold);
            titleLbl.Location = new Point(12, 8);
            titleLbl.Size = new Size(card.Width - 220, 20);
            titleLbl.AutoEllipsis = true;
            card.Controls.Add(titleLbl);

            Label descLbl = new Label();
            descLbl.Text = task.Description;
            descLbl.ForeColor = TextMuted;
            descLbl.Font = new Font("Segoe UI", 8.5f);
            descLbl.Location = new Point(12, 30);
            descLbl.Size = new Size(card.Width - 220, 18);
            descLbl.AutoEllipsis = true;
            card.Controls.Add(descLbl);

            string meta = task.CreatedAt.ToString("dd MMM yyyy HH:mm");
            if (!string.IsNullOrWhiteSpace(task.Reminder))
                meta += "   Reminder: " + task.Reminder;

            Label metaLbl = new Label();
            metaLbl.Text = meta;
            metaLbl.ForeColor = AccentBlue;
            metaLbl.Font = new Font("Segoe UI", 8f);
            metaLbl.Location = new Point(12, 52);
            metaLbl.Size = new Size(card.Width - 220, 18);
            metaLbl.AutoEllipsis = true;
            card.Controls.Add(metaLbl);

            TaskItem capturedTask = task;
            bool capturedDone = done;

            Button btnComplete = MakeButton(
                done ? "Undo" : "Complete",
                done ? InputBg : Color.FromArgb(35, 134, 54), TextMain,
                new Point(card.Width - 200, 32), new Size(90, 28),
                new Font("Segoe UI", 8.5f));
            btnComplete.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnComplete.Click += delegate
            {
                DatabaseHelper.SetComplete(capturedTask.Id, !capturedDone);
                ActivityLog.Add("Task " + capturedTask.Id + " marked " +
                    (!capturedDone ? "complete" : "incomplete") + ": " + capturedTask.Title);
                RefreshLogTab();
                RefreshTaskList();
            };
            card.Controls.Add(btnComplete);

            Button btnDelete = MakeButton(
                "Delete", Color.FromArgb(100, 0, 0), TextMain,
                new Point(card.Width - 104, 32), new Size(90, 28),
                new Font("Segoe UI", 8.5f));
            btnDelete.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnDelete.FlatAppearance.BorderColor = ColWarn;
            btnDelete.Click += delegate
            {
                DialogResult dr = MessageBox.Show(
                    "Delete task: " + capturedTask.Title + "?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    DatabaseHelper.DeleteTask(capturedTask.Id);
                    ActivityLog.Add("Task deleted: " + capturedTask.Title);
                    RefreshLogTab();
                    RefreshTaskList();
                }
            };
            card.Controls.Add(btnDelete);

            return card;
        }

        
        // QUIZ TAB
       
        private Panel BuildQuizTab(Font uiFont, Font boldFont)
        {
            Panel root = new Panel();
            root.Dock = DockStyle.Fill;
            root.BackColor = DarkBg;

            Label header = new Label();
            header.Text = "Cybersecurity Quiz";
            header.ForeColor = AccentGreen;
            header.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            header.AutoSize = true;
            header.Location = new Point(30, 16);
            root.Controls.Add(header);

            // Back to Chat button
            Button btnBackToChat = MakeButton("Back to Chat", PanelBg, AccentBlue,
                new Point(850, 16), new Size(120, 30), uiFont);
            btnBackToChat.FlatAppearance.BorderColor = AccentBlue;
            btnBackToChat.Click += delegate { mainTabControl.SelectedIndex = 0; };
            root.Controls.Add(btnBackToChat);

            lblQuizScore.Text = "Score: 0";
            lblQuizScore.ForeColor = AccentGreen;
            lblQuizScore.Font = boldFont;
            lblQuizScore.Location = new Point(30, 60);
            lblQuizScore.AutoSize = true;
            root.Controls.Add(lblQuizScore);

            lblQuizNum.Text = "Question: 0 / 0";
            lblQuizNum.ForeColor = AccentBlue;
            lblQuizNum.Font = boldFont;
            lblQuizNum.Location = new Point(200, 60);
            lblQuizNum.AutoSize = true;
            root.Controls.Add(lblQuizNum);

            Panel qBox = new Panel();
            qBox.Location = new Point(30, 95);
            qBox.Size = new Size(700, 70);
            qBox.BackColor = PanelBg;

            lblQuestion.Text = "Press Start Quiz to begin!";
            lblQuestion.ForeColor = TextMain;
            lblQuestion.Font = new Font("Segoe UI", 11f);
            lblQuestion.Location = new Point(10, 10);
            lblQuestion.Size = new Size(680, 50);
            lblQuestion.AutoEllipsis = false;
            qBox.Controls.Add(lblQuestion);
            root.Controls.Add(qBox);

            answerPanel.Location = new Point(30, 175);
            answerPanel.Size = new Size(700, 200);
            answerPanel.BackColor = DarkBg;
            root.Controls.Add(answerPanel);

            lblFeedback.Location = new Point(30, 385);
            lblFeedback.Size = new Size(700, 60);
            lblFeedback.ForeColor = AccentGreen;
            lblFeedback.Font = uiFont;
            lblFeedback.AutoSize = false;
            lblFeedback.Visible = false;
            root.Controls.Add(lblFeedback);

            Button btnStart = MakeButton("Start Quiz", AccentGreen, DarkBg,
                new Point(30, 460), new Size(130, 36), boldFont);
            btnStart.Click += delegate { StartNewQuiz(); };
            root.Controls.Add(btnStart);

            btnNextQuestion.Text = "Next";
            btnNextQuestion.Location = new Point(170, 460);
            btnNextQuestion.Size = new Size(100, 36);
            btnNextQuestion.BackColor = PanelBg;
            btnNextQuestion.ForeColor = TextMuted;
            btnNextQuestion.FlatStyle = FlatStyle.Flat;
            btnNextQuestion.FlatAppearance.BorderColor = Color.FromArgb(48, 54, 61);
            btnNextQuestion.Enabled = false;
            btnNextQuestion.Cursor = Cursors.Hand;
            btnNextQuestion.Click += delegate { ShowNextQuestion(); };
            root.Controls.Add(btnNextQuestion);

            Button btnRestart = MakeButton("Restart", PanelBg, TextMuted,
                new Point(280, 460), new Size(100, 36), uiFont);
            btnRestart.FlatAppearance.BorderColor = Color.FromArgb(48, 54, 61);
            btnRestart.Click += delegate { StartNewQuiz(); };
            root.Controls.Add(btnRestart);

            return root;
        }

        private void StartNewQuiz()
        {
            quizQuestions = QuizEngine.GetShuffledQuestions(10);
            quizIndex = -1;
            quizScore = 0;
            quizAnswered = false;
            lblFeedback.Visible = false;
            btnNextQuestion.Enabled = false;
            UpdateQuizScore();
            ActivityLog.Add("Quiz started");
            RefreshLogTab();
            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            quizIndex++;
            quizAnswered = false;
            lblFeedback.Visible = false;
            btnNextQuestion.Enabled = false;

            if (quizIndex >= quizQuestions.Count)
            {
                EndQuiz();
                return;
            }

            QuizQuestion q = quizQuestions[quizIndex];
            lblQuestion.Text = q.Question;
            lblQuizNum.Text = "Question: " + (quizIndex + 1) + " / " + quizQuestions.Count;

            answerPanel.Controls.Clear();

            for (int i = 0; i < q.Options.Length; i++)
            {
                int capturedI = i;

                Button btn = MakeButton(
                    ((char)('A' + i)) + ")  " + q.Options[i],
                    InputBg, TextMain,
                    new Point(0, capturedI * 46),
                    new Size(700, 38),
                    new Font("Segoe UI", 9f));
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.FlatAppearance.BorderColor = Color.FromArgb(48, 54, 61);
                btn.Tag = capturedI;
                btn.Click += delegate (object s2, EventArgs e2) { AnswerBtn_Click(s2, e2); };
                answerPanel.Controls.Add(btn);
            }

            ActivityLog.Add("Quiz Q" + (quizIndex + 1) + " shown");
            RefreshLogTab();
        }

        private void AnswerBtn_Click(object sender, EventArgs e)
        {
            if (quizAnswered) return;
            Button clicked = sender as Button;
            if (clicked == null) return;

            quizAnswered = true;
            int chosen = (int)clicked.Tag;
            QuizQuestion q = quizQuestions[quizIndex];
            bool correct = (chosen == q.CorrectIndex);

            foreach (Control c in answerPanel.Controls)
            {
                Button b = c as Button;
                if (b == null) continue;
                int idx = (int)b.Tag;
                b.Enabled = false;
                b.BackColor = (idx == q.CorrectIndex)
                    ? Color.FromArgb(35, 134, 54)
                    : (idx == chosen ? Color.FromArgb(139, 0, 0) : InputBg);
            }

            if (correct) quizScore++;
            UpdateQuizScore();

            if (correct)
            {
                lblFeedback.Text = "Correct!\n" + q.Explanation;
                lblFeedback.ForeColor = AccentGreen;
            }
            else
            {
                lblFeedback.Text =
                    "Incorrect. Correct answer: " +
                    ((char)('A' + q.CorrectIndex)) + ") " + q.Options[q.CorrectIndex] +
                    "\n" + q.Explanation;
                lblFeedback.ForeColor = ColWarn;
            }

            lblFeedback.Visible = true;
            btnNextQuestion.Enabled = true;

            ActivityLog.Add("Quiz Q" + (quizIndex + 1) + ": " + (correct ? "correct" : "incorrect"));
            RefreshLogTab();
        }

        private void EndQuiz()
        {
            lblQuestion.Text = "Quiz Complete!  Your score: " + quizScore + " / " + quizQuestions.Count;
            answerPanel.Controls.Clear();
            btnNextQuestion.Enabled = false;

            string rating;
            if (quizScore == quizQuestions.Count) rating = "Perfect score! You're a cybersecurity pro!";
            else if (quizScore >= 8) rating = "Great job! You're a cybersecurity pro!";
            else if (quizScore >= 6) rating = "Good effort! Keep learning to stay safe online!";
            else rating = "Keep learning to stay safe online! Review the Chat tab for tips.";

            lblFeedback.Text = rating;
            lblFeedback.ForeColor = AccentBlue;
            lblFeedback.Visible = true;

            ActivityLog.Add("Quiz ended - score: " + quizScore + "/" + quizQuestions.Count);
            RefreshLogTab();
        }

        private void UpdateQuizScore()
        {
            lblQuizScore.Text = "Score: " + quizScore;
        }

        
        // ACTIVITY LOG TAB
        
        private Panel BuildLogTab(Font uiFont)
        {
            Panel root = new Panel();
            root.Dock = DockStyle.Fill;
            root.BackColor = DarkBg;

            Label header = new Label();
            header.Text = "Activity Log - all chatbot actions are recorded here";
            header.ForeColor = AccentGreen;
            header.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            header.AutoSize = true;
            header.Location = new Point(0, 0);
            root.Controls.Add(header);

            logBox.Location = new Point(0, 28);
            logBox.Size = new Size(root.Width, root.Height - 70);
            logBox.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                                 AnchorStyles.Right | AnchorStyles.Bottom;
            logBox.BackColor = PanelBg;
            logBox.ForeColor = TextMuted;
            logBox.Font = new Font("Consolas", 9f);
            logBox.BorderStyle = BorderStyle.None;
            logBox.ReadOnly = true;
            logBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            root.Controls.Add(logBox);

            Button btnClearLog = MakeButton("Clear Log",
                Color.FromArgb(100, 0, 0), TextMain,
                new Point(0, root.Height - 38), new Size(110, 30), uiFont);
            btnClearLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnClearLog.FlatAppearance.BorderColor = ColWarn;
            btnClearLog.Click += delegate
            {
                DialogResult dr = MessageBox.Show(
                    "Clear the entire activity log?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    ActivityLog.Clear();
                    logBox.Clear();
                    ActivityLog.Add("Log cleared");
                    RefreshLogTab();
                }
            };
            root.Controls.Add(btnClearLog);

            return root;
        }

        private void RefreshLogTab()
        {
            logBox.Clear();
            foreach (LogEntry entry in ActivityLog.Entries)
            {
                logBox.SelectionColor = AccentBlue;
                logBox.AppendText("[" + entry.Time.ToString("HH:mm:ss") + "]  ");
                logBox.SelectionColor = TextMuted;
                logBox.AppendText(entry.Entry + Environment.NewLine);
            }
            logBox.ScrollToCaret();
        }

        
        // SHARED BUTTON HELPER
        
        private static Button MakeButton(string text, Color bg, Color fg,
            Point loc, Size sz, Font font)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.BackColor = bg;
            btn.ForeColor = fg;
            btn.Location = loc;
            btn.Size = sz;
            btn.Font = font;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.FlatAppearance.BorderSize = 1;
            return btn;
        }
    }
}