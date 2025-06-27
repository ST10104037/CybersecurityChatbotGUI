using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;                   // for Path
using System.Media;                // for SoundPlayer
using System.Windows.Media.Imaging; // for BitmapImage


namespace CybersecurityChatbotGUI
{
    public partial class MainWindow : Window
    {


        private List<string> activityLog = new List<string>();
        private List<CyberTask> tasks = new List<CyberTask>();

        private CyberChatbot chatbot = new CyberChatbot();



        // Quiz state
        private int currentQuestionIndex = 0;
        private int score = 0;

        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>
        {
            new QuizQuestion
            {
                Question = "What should you do if you receive an email asking for your password?",
                Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                CorrectIndex = 2,
                Explanation = "Correct! Reporting phishing emails helps prevent scams."
            },
            new QuizQuestion
            {
                Question = "True or False: Using the same password everywhere is safe.",
                Options = new List<string> { "True", "False" },
                CorrectIndex = 1,
                Explanation = "False! Reusing passwords makes all your accounts vulnerable if one gets breached."
            },
            new QuizQuestion
            {
                Question = "Which is the strongest password?",
                Options = new List<string> { "password123", "123456", "Q#d3!Tz@9Lp", "yourname2023" },
                CorrectIndex = 2,
                Explanation = "Strong passwords use symbols, numbers, and no dictionary words."
            },
            new QuizQuestion
            {
                Question = "What does 2FA stand for?",
                Options = new List<string> { "Two-Factor Authentication", "Too Fast Access", "Two File Alert", "Twice File Access" },
                CorrectIndex = 0,
                Explanation = "2FA adds an extra layer of security by requiring a second step."
            },
            new QuizQuestion
            {
                Question = "Phishing is...",
                Options = new List<string> { "A security tool", "An online scam", "A file extension", "An anti-virus feature" },
                CorrectIndex = 1,
                Explanation = "Phishing is an attempt to trick you into giving up personal info."
            },
            new QuizQuestion
            {
                Question = "True or False: Public Wi-Fi is always secure.",
                Options = new List<string> { "True", "False" },
                CorrectIndex = 1,
                Explanation = "Public Wi-Fi can be a risk if not used with a VPN."
            },
            new QuizQuestion
            {
                Question = "What's the safest way to store passwords?",
                Options = new List<string> { "Write them down", "Use a password manager", "Email them to yourself", "Remember all of them" },
                CorrectIndex = 1,
                Explanation = "Password managers encrypt and store passwords securely."
            },
            new QuizQuestion
            {
                Question = "What is social engineering?",
                Options = new List<string> { "Building software", "Tricking people into revealing info", "Securing servers", "Creating online communities" },
                CorrectIndex = 1,
                Explanation = "Social engineering is manipulating people into giving up confidential info."
            },
            new QuizQuestion
            {
                Question = "Which of the following is a secure site?",
                Options = new List<string> { "http://bank.com", "https://secure-login.com", "www.website.net", "ftp://data.net" },
                CorrectIndex = 1,
                Explanation = "Sites with 'https' use encryption to secure your data."
            },
            new QuizQuestion
            {
                Question = "What is malware?",
                Options = new List<string> { "Software update", "Security patch", "Malicious software", "Cloud app" },
                CorrectIndex = 2,
                Explanation = "Malware stands for 'malicious software' designed to harm your system."
            },
        };
        public MainWindow()
        {
            InitializeComponent();
            LoadLogoImage();
            PlayGreeting();
        }
        private void LoadLogoImage()
        {
            try
            {
                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LOGO.jpg");
                LogoImage.Source = new BitmapImage(new Uri(logoPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading logo image: " + ex.Message);
            }
        }

        private void PlayGreeting()
        {
            try
            {
                string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
                SoundPlayer player = new SoundPlayer(soundPath);
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error playing greeting sound: " + ex.Message);
            }
        }

        private bool HandleGUICommands(string input)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("add task") || lowerInput.Contains("create task") || lowerInput.Contains("new task"))
            {
                ChatHistory.Items.Add("Bot: Sure! Please fill in the task details in the Tasks tab and click 'Add Task'.");
                activityLog.Insert(0, $"User requested to add a task at {DateTime.Now}");
                return true;
            }
            else if (lowerInput.Contains("remind") || lowerInput.Contains("reminder") || lowerInput.Contains("remind me"))
            {
                string taskTitle = ExtractTaskTitleFromReminder(input);
                ChatHistory.Items.Add($"Bot: Reminder noted. You can set it precisely in the Tasks tab.");
                activityLog.Insert(0, $"Reminder request processed at {DateTime.Now} - Task: {taskTitle}");
                return true;
            }
            else if (lowerInput.Contains("show activity") || lowerInput.Contains("activity log") || lowerInput.Contains("what have you done"))
            {
                ChatHistory.Items.Add("Bot: Here's a summary of your recent actions:");
                foreach (var entry in activityLog.Take(10))
                {
                    ChatHistory.Items.Add("Bot Log: " + entry);
                }
                return true;
            }
            else if (lowerInput.Contains("quiz") || lowerInput.Contains("start quiz"))
            {
                ChatHistory.Items.Add("Bot: Go to the 'Quiz' tab and click 'Start Quiz' to begin!");
                activityLog.Insert(0, $"User started quiz instruction at {DateTime.Now}");
                return true;
            }
            else if (lowerInput.Contains("list tasks") || lowerInput.Contains("show tasks"))
            {
                if (tasks.Count == 0)
                {
                    ChatHistory.Items.Add("Bot: You have no tasks currently.");
                }
                else
                {
                    ChatHistory.Items.Add("Bot: Here are your current tasks:");
                    foreach (var task in tasks)
                    {
                        ChatHistory.Items.Add($"- {task.Title} {(task.IsCompleted ? "(Completed)" : "")}");
                    }
                }
                return true;
            }

            return false;
        }

        // Send button handler (NLP simulation included)
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage)) return;

            ChatHistory.Items.Add("User: " + userMessage);

            // 🔹 NLP logic: Handle task, quiz, reminders, etc.
            ProcessUserInput(userMessage);

            // 🔹 Chatbot logic from CyberChatbot class
            List<string> botReplies = chatbot.ProcessInput(userMessage);
            foreach (var reply in botReplies)
            {
                ChatHistory.Items.Add("Bot: " + reply);
            }

            UserInput.Clear();
        }


        // Basic NLP using string contains and keyword matching
        private void ProcessUserInput(string input)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("add task") || lowerInput.Contains("create task") || lowerInput.Contains("new task"))
            {
                ChatHistory.Items.Add("Bot: Sure! Please fill in the task details in the Tasks tab and click 'Add Task'.");
                activityLog.Insert(0, $"User requested to add a task at {DateTime.Now}");
            }
            else if (lowerInput.Contains("remind") || lowerInput.Contains("reminder") || lowerInput.Contains("remind me"))
            {
                // Try to extract task title from the sentence (simple heuristic)
                string taskTitle = ExtractTaskTitleFromReminder(input);

                ChatHistory.Items.Add($"Bot: Reminder noted. You can set it precisely in the Tasks tab.");
                activityLog.Insert(0, $"Reminder request processed at {DateTime.Now} - Task: {taskTitle}");
            }
            else if (lowerInput.Contains("show activity") || lowerInput.Contains("activity log") || lowerInput.Contains("what have you done"))
            {
                ChatHistory.Items.Add("Bot: Here's a summary of your recent actions:");
                foreach (var entry in activityLog.Take(10))
                {
                    ChatHistory.Items.Add("Bot Log: " + entry);
                }
            }
            else if (lowerInput.Contains("quiz") || lowerInput.Contains("start quiz"))
            {
                ChatHistory.Items.Add("Bot: Go to the 'Quiz' tab and click 'Start Quiz' to begin!");
                activityLog.Insert(0, $"User started quiz instruction at {DateTime.Now}");
            }
            else if (lowerInput.Contains("list tasks") || lowerInput.Contains("show tasks"))
            {
                if (tasks.Count == 0)
                {
                    ChatHistory.Items.Add("Bot: You have no tasks currently.");
                }
                else
                {
                    ChatHistory.Items.Add("Bot: Here are your current tasks:");
                    foreach (var task in tasks)
                    {
                        ChatHistory.Items.Add($"- {task.Title} {(task.IsCompleted ? "(Completed)" : "")}");
                    }
                }
            }
            else
            {
                ChatHistory.Items.Add("Bot: I didn’t quite understand. Try saying things like 'add task', 'set reminder', 'start quiz', or 'show activity'.");
            }
        }

        // Helper to extract a possible task title from reminder input (basic example)
        private string ExtractTaskTitleFromReminder(string input)
        {
            // crude approach: remove "remind me to" or "reminder to" etc.
            string[] triggers = new[] { "remind me to", "remind me", "set reminder to", "reminder to", "reminder for" };
            string lowered = input.ToLower();

            foreach (var trigger in triggers)
            {
                int idx = lowered.IndexOf(trigger);
                if (idx != -1)
                {
                    string after = input.Substring(idx + trigger.Length).Trim(new char[] { '.', ' ', '?' });
                    return after.Length > 50 ? after.Substring(0, 50) + "..." : after;
                }
            }
            return input.Length > 50 ? input.Substring(0, 50) + "..." : input;
        }

        // Add task button
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitle.Text.Trim();
            string description = TaskDescription.Text.Trim();
            DateTime? reminder = ReminderDate.SelectedDate;

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            var task = new CyberTask
            {
                Title = title,
                Description = description,
                Reminder = reminder,
                IsCompleted = false
            };

            tasks.Add(task);
            TaskList.Items.Add(task);
            ChatHistory.Items.Add("Bot: Task added - " + task.Title);

            string reminderText = reminder.HasValue ? $"Reminder set for {reminder.Value.ToShortDateString()}" : "No reminder set";
            activityLog.Insert(0, $"Added task: {task.Title} ({reminderText}) at {DateTime.Now}");

            ClearTaskInputs();
            UpdateActivityLog();
        }

        private void ClearTaskInputs()
        {
            TaskTitle.Clear();
            TaskDescription.Clear();
            ReminderDate.SelectedDate = null;
        }

        // Mark task as completed button
        private void MarkTaskCompleted_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is CyberTask selectedTask)
            {
                selectedTask.IsCompleted = true;
                TaskList.Items.Refresh(); // Update display
                activityLog.Insert(0, $"Task marked as completed: {selectedTask.Title} at {DateTime.Now}");
                UpdateActivityLog();
                ChatHistory.Items.Add($"Bot: Task '{selectedTask.Title}' marked as completed.");
            }
            else
            {
                MessageBox.Show("Please select a task to mark as completed.");
            }
        }

        // Delete task button
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is CyberTask selectedTask)
            {
                tasks.Remove(selectedTask);
                TaskList.Items.Remove(selectedTask);
                activityLog.Insert(0, $"Task deleted: {selectedTask.Title} at {DateTime.Now}");
                UpdateActivityLog();
                ChatHistory.Items.Add($"Bot: Task '{selectedTask.Title}' has been deleted.");
            }
            else
            {
                MessageBox.Show("Please select a task to delete.");
            }
        }

        // Update the activity log ListBox
        private void UpdateActivityLog()
        {
            ActivityLogList.Items.Clear();
            foreach (var entry in activityLog.Take(20))
            {
                ActivityLogList.Items.Add(entry);
            }
        }

        // Quiz - Start Quiz button
        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex = 0;
            score = 0;
            QuizScoreDisplay.Text = "";
            LoadQuizQuestion(currentQuestionIndex);
            activityLog.Insert(0, $"Quiz started at {DateTime.Now}");
            UpdateActivityLog();
            QuizFeedback.Text = "";
        }

        // Load question and options
        private void LoadQuizQuestion(int index)
        {
            if (index < 0 || index >= quizQuestions.Count)
            {
                QuizQuestion.Text = "No more questions.";
                QuizOptions.Items.Clear();
                return;
            }

            QuizQuestion.Text = quizQuestions[index].Question;
            QuizOptions.Items.Clear();

            foreach (var option in quizQuestions[index].Options)
            {
                QuizOptions.Items.Add(option);
            }
        }

        // Submit answer button
        private void SubmitQuizAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestionIndex >= quizQuestions.Count)
            {
                MessageBox.Show("Quiz is over. Please start a new quiz.");
                return;
            }

            int selected = QuizOptions.SelectedIndex;
            if (selected == -1)
            {
                MessageBox.Show("Please select an answer.");
                return;
            }

            var question = quizQuestions[currentQuestionIndex];
            if (selected == question.CorrectIndex)
            {
                score++;
                QuizFeedback.Text = "Correct! " + question.Explanation;
            }
            else
            {
                QuizFeedback.Text = $"Incorrect. {question.Explanation}";
            }

            currentQuestionIndex++;

            if (currentQuestionIndex < quizQuestions.Count)
            {
                LoadQuizQuestion(currentQuestionIndex);
            }
            else
            {
                QuizQuestion.Text = "Quiz complete!";
                QuizOptions.Items.Clear();
                QuizScoreDisplay.Text = $"Your final score is {score} out of {quizQuestions.Count}.";
                activityLog.Insert(0, $"Quiz completed with score {score}/{quizQuestions.Count} at {DateTime.Now}");
                UpdateActivityLog();
            }
        }
    }

    // Task model with ToString override for display
    public class CyberTask
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? Reminder { get; set; }
        public bool IsCompleted { get; set; }

        public override string ToString()
        {
            string completed = IsCompleted ? " (Completed)" : "";
            string reminderText = Reminder.HasValue ? $" [Reminder: {Reminder.Value.ToShortDateString()}]" : "";
            return $"{Title}{completed}{reminderText}";
        }
    }

    public class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }
}