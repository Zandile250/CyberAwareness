using System;
using System.Collections.Generic;

// This class handles all chatbot logic for the Cyber Awareness project.
// It recognises keywords, gives cybersecurity tips, remembers user preferences,
// and generates responses based on the conversation flow.
//Added task intent detection so the UI can open the right tab.

namespace CyberAwareness
{
    // Represents what action the chatbot wants the UI to perform after a response
    public enum ChatIntent
    {
        None,
        AddTask,
        ViewTasks,
        StartQuiz
    }

    // Bundles the three values returned by GetResponse so no tuple syntax is needed
    public class ChatResponse
    {
        public string Response { get; set; }
        public ChatIntent Intent { get; set; }
        public string TaskHint { get; set; }

        public ChatResponse(string response, ChatIntent intent, string taskHint)
        {
            Response = response;
            Intent = intent;
            TaskHint = taskHint ?? string.Empty;
        }
    }

    public class Chatbot
    {
        // Can remember and refer back to them during the conversation
        public string UserName { get; set; }
        public string LastTopic { get; private set; }

        // Stores the user's favourite topic so we can refer back to it later
        private string favouriteTopic;

        // Counts how many messages the user has sent during the conversation
        private int messageCount;

        private readonly Random rand = new Random();

        // Sorted keys cached once at startup instead of re-sorting on every message
        private readonly List<string> sortedKeys;

        public Chatbot()
        {
            UserName = "User";
            LastTopic = string.Empty;
            favouriteTopic = string.Empty;
            messageCount = 0;

            sortedKeys = new List<string>(keywords.Keys);
            sortedKeys.Sort(delegate (string a, string b) { return b.Length.CompareTo(a.Length); });
        }

        // Keywords the chatbot recognises and their responses
        private Dictionary<string, string> keywords = new Dictionary<string, string>()
        {
            {
                "password",
                "Make sure to use strong, unique passwords for each account. " +
                "Avoid using personal details in your passwords. " +
                "Use at least 12 characters with uppercase, lowercase, numbers and symbols."
            },
            {
                "scam",
                "Be cautious of messages asking for money or personal details. " +
                "Scammers often pretend to be trusted organisations. " +
                "Never send money or personal info to unverified sources."
            },
            {
                "privacy",
                "Protect your privacy by limiting what you share online. " +
                "Review your social media privacy settings regularly. " +
                "Avoid sharing sensitive information on unsecured platforms."
            },
            {
                "safe browsing",
                "Only visit websites with https://. " +
                "Avoid public Wi-Fi for sensitive tasks. " +
                "Keep your browser and software updated at all times."
            },
            {
                "malware",
                "Malware is malicious software designed to damage your device. " +
                "Keep your antivirus updated and never open suspicious attachments."
            },
            {
                "firewall",
                "A firewall monitors your network traffic and blocks unauthorised access. " +
                "Make sure your firewall is always enabled."
            },
            {
                "vpn",
                "A VPN encrypts your internet traffic and protects your identity online. " +
                "Use one especially when connecting to public Wi-Fi."
            },
            {
                "2fa",
                "Two-factor authentication adds an extra layer of security. " +
                "Even if someone has your password they still cannot log in without your second factor."
            }
        };

        // Phishing tips randomly selected each time
        private List<string> phishingTips = new List<string>()
        {
            "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
            "Never click on suspicious links in emails. Go directly to the website by typing the URL yourself.",
            "Check the sender's email address carefully. Scammers often spoof legitimate domains.",
            "Look for spelling mistakes and urgent language in emails. These are classic phishing red flags.",
            "Legitimate banks and companies will never ask for your password via email.",
            "When in doubt call the company directly using a number from their official website."
        };

        // General security tips randomly selected
        private List<string> securityTips = new List<string>()
        {
            "Enable automatic updates on all your devices to patch security vulnerabilities quickly.",
            "Use a password manager to generate and store complex unique passwords.",
            "Always log out of accounts when using shared or public computers.",
            "Regularly review which apps have access to your camera microphone and location.",
            "Back up important files regularly using the 3-2-1 rule: 3 copies, 2 media types, 1 offsite.",
            "Be wary of free public Wi-Fi. Use a VPN if you must connect to it."
        };

        // Longer explanations used when the user says "tell me more"
        private Dictionary<string, string> topicDetails = new Dictionary<string, string>()
        {
            {
                "password",
                "A strong password is at least 12 characters long and combines letters, numbers and symbols. " +
                "Avoid using dictionary words or personal information like birthdays. " +
                "Consider using a passphrase which is three random words joined together. " +
                "Most importantly never reuse a password across different websites."
            },
            {
                "phishing",
                "Phishing attacks come in many forms including email phishing, SMS phishing called smishing " +
                "and voice phishing called vishing. Attackers impersonate trusted organisations to steal " +
                "your credentials or install malware. Always verify the source before clicking any link."
            },
            {
                "scam",
                "Common scams include romance scams, investment fraud, tech support scams and lottery scams. " +
                "They all rely on creating urgency and playing on your emotions. " +
                "Take your time, speak to someone you trust and independently verify any claim before acting."
            },
            {
                "privacy",
                "Digital privacy means controlling what data is collected about you. " +
                "Use privacy-focused browsers, enable ad-blockers and review app permissions regularly. " +
                "Be mindful of what personal information you share publicly on social media."
            },
            {
                "safe browsing",
                "HTTPS encrypts the data between your browser and the website. " +
                "Always look for the padlock icon in the address bar. " +
                "Keep browser extensions minimal as malicious extensions can track everything you type."
            },
            {
                "malware",
                "Types of malware include viruses, trojans, spyware, adware and ransomware. " +
                "Install reputable antivirus software and keep it updated. " +
                "Avoid pirated software as it is one of the most common ways malware is delivered."
            }
        };

        // Sentiment words and empathetic responses
        private Dictionary<string, string> sentimentResponses = new Dictionary<string, string>()
        {
            {
                "worried",
                "It is completely understandable to feel that way. " +
                "The good news is that knowledge is your best defence. " +
                "Let me share a tip to help you stay safe."
            },
            {
                "scared",
                "Do not worry, you are not alone in feeling that way. " +
                "Many people feel the same about online threats. " +
                "Here is something that can help you feel more confident."
            },
            {
                "frustrated",
                "I understand your frustration. Cybersecurity can feel overwhelming at times. " +
                "Let me simplify things with a helpful tip."
            },
            {
                "confused",
                "That is okay, cybersecurity can be confusing at first. " +
                "Let me help clear things up with a simple tip."
            },
            {
                "curious",
                "That is great that you are curious! " +
                "Curiosity is the first step to staying safe online. " +
                "Here is something interesting for you."
            },
            {
                "unsure",
                "It is okay to be unsure. Everyone starts somewhere. " +
                "Let me help you with a useful tip."
            },
            {
                "overwhelmed",
                "Take it one step at a time. " +
                "You do not need to learn everything at once. " +
                "Here is one simple thing you can do right now."
            }
        };

        //Task intent phrases
        private static readonly string[] AddTaskPhrases =
            new string[] { "add task", "create task", "new task", "add a task", "remind me to" };

        private static readonly string[] ViewTasksPhrases =
            new string[] { "view tasks", "show tasks", "list tasks", "my tasks", "see tasks", "show my tasks" };

        private static readonly string[] QuizPhrases =
            new string[] { "quiz", "mini game", "test me", "start game", "play game" };


        // MAIN RESPONSE METHOD
        
        public ChatResponse GetResponse(string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input))
                    return new ChatResponse(
                        "It looks like you did not type anything, " + UserName + ". Please enter a question and I will be happy to help!",
                        ChatIntent.None, string.Empty);

                input = input.ToLower().Trim();

                if (string.IsNullOrWhiteSpace(input))
                    return new ChatResponse(
                        "I noticed your message was blank, " + UserName + ". Try asking about passwords, phishing, scams or privacy!",
                        ChatIntent.None, string.Empty);

                messageCount++;

                // Check task / quiz intents first
                foreach (string phrase in AddTaskPhrases)
                {
                    if (input.Contains(phrase))
                    {
                        string hint = ExtractAfter(input, phrase);
                        LastTopic = "task";
                        string msg = string.IsNullOrWhiteSpace(hint)
                            ? "Sure " + UserName + "! I have opened the Task Assistant for you. Enter your task title and description there."
                            : "Sure " + UserName + "! I have opened the Task Assistant and pre-filled the title with \"" + CapFirst(hint) + "\".";
                        return new ChatResponse(msg, ChatIntent.AddTask, CapFirst(hint));
                    }
                }

                foreach (string phrase in ViewTasksPhrases)
                {
                    if (input.Contains(phrase))
                    {
                        LastTopic = "task";
                        return new ChatResponse(
                            "Opening your task list now, " + UserName + "!",
                            ChatIntent.ViewTasks, string.Empty);
                    }
                }

                foreach (string phrase in QuizPhrases)
                {
                    if (input.Contains(phrase))
                    {
                        LastTopic = "quiz";
                        return new ChatResponse(
                            "Great idea, " + UserName + "! Let's test your cybersecurity knowledge. Opening the quiz now!",
                            ChatIntent.StartQuiz, string.Empty);
                    }
                }

                // Sentiment detection
                string detectedSentiment = string.Empty;
                foreach (string sentiment in sentimentResponses.Keys)
                {
                    if (input.Contains(sentiment))
                    {
                        detectedSentiment = sentiment;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(detectedSentiment))
                {
                    string empathyResponse = sentimentResponses[detectedSentiment];

                    foreach (string key in keywords.Keys)
                    {
                        if (input.Contains(key))
                        {
                            LastTopic = key;
                            return new ChatResponse(empathyResponse + "\n\n" + keywords[key], ChatIntent.None, string.Empty);
                        }
                    }

                    if (input.Contains("phishing"))
                    {
                        LastTopic = "phishing";
                        return new ChatResponse(empathyResponse + "\n\n" + GetRandomPhishingTip(), ChatIntent.None, string.Empty);
                    }

                    if (!string.IsNullOrEmpty(LastTopic) && keywords.ContainsKey(LastTopic))
                        return new ChatResponse(empathyResponse + "\n\n" + keywords[LastTopic], ChatIntent.None, string.Empty);

                    return new ChatResponse(empathyResponse + "\n\n" + GetRandomSecurityTip(), ChatIntent.None, string.Empty);
                }

                // Greeting
                string[] words = input.Split(new char[] { ' ', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                bool isGreeting = Array.Exists(words, delegate (string w) { return w == "hello" || w == "hi" || w == "hey"; });

                if (isGreeting)
                {
                    LastTopic = string.Empty;
                    return new ChatResponse(
                        "Hello " + UserName + "! I am here to help you stay safe online. What would you like to know?",
                        ChatIntent.None, string.Empty);
                }

                // How are you
                if (input.Contains("how are you"))
                {
                    LastTopic = "how are you";
                    return new ChatResponse(
                        "I am doing great " + UserName + "! Ready to help you stay safe online.",
                        ChatIntent.None, string.Empty);
                }

                // Purpose
                if (input.Contains("purpose") || input.Contains("what do you do") || input.Contains("what are you"))
                {
                    LastTopic = "purpose";
                    return new ChatResponse(
                        "My purpose is to help you " + UserName + " understand cybersecurity and protect yourself from online threats.",
                        ChatIntent.None, string.Empty);
                }

                // Help / topics list
                if (input.Contains("what can i ask") || input.Contains("help") ||
                    input.Contains("topics") || input.Contains("what can you"))
                {
                    LastTopic = "help";
                    return new ChatResponse(
                        "You can ask me about:\n" +
                        "  * Password safety\n" +
                        "  * Phishing\n" +
                        "  * Scams\n" +
                        "  * Privacy\n" +
                        "  * Safe browsing\n" +
                        "  * Malware\n" +
                        "  * Firewall\n" +
                        "  * VPN\n" +
                        "  * 2FA\n" +
                        "  * Security tips\n\n" +
                        "Other features:\n" +
                        "  * Say \"add task\" to save a cybersecurity reminder\n" +
                        "  * Say \"view tasks\" to see your task list\n" +
                        "  * Say \"quiz\" to test your knowledge",
                        ChatIntent.None, string.Empty);
                }

                // Goodbye
                if (input.Contains("bye") || input.Contains("goodbye") || input.Contains("exit"))
                {
                    LastTopic = string.Empty;

                    if (!string.IsNullOrEmpty(favouriteTopic))
                        return new ChatResponse(
                            "Goodbye " + UserName + "! Since you are interested in " + favouriteTopic +
                            ", remember to keep up with the latest " + favouriteTopic + " tips to stay safe online!",
                            ChatIntent.None, string.Empty);

                    return new ChatResponse("Goodbye " + UserName + "! Stay safe online.", ChatIntent.None, string.Empty);
                }

                // Favourite topic memory
                if (input.Contains("interested in") || input.Contains("i like") || input.Contains("i love") ||
                    input.Contains("my favourite") || input.Contains("i enjoy"))
                {
                    foreach (string key in keywords.Keys)
                    {
                        if (input.Contains(key))
                        {
                            favouriteTopic = key;
                            LastTopic = key;
                            return new ChatResponse(
                                "Great! I will remember that you are interested in " + key + ", " + UserName + ". " +
                                "It is a crucial part of staying safe online.\n\n" +
                                "Here is something useful about " + key + ":\n" + keywords[key],
                                ChatIntent.None, string.Empty);
                        }
                    }

                    if (input.Contains("phishing"))
                    {
                        favouriteTopic = "phishing";
                        LastTopic = "phishing";
                        return new ChatResponse(
                            "Great! I will remember that you are interested in phishing awareness, " + UserName + ". " +
                            "It is a crucial part of staying safe online.\n\n" + GetRandomPhishingTip(),
                            ChatIntent.None, string.Empty);
                    }
                }

                // Every 5 messages remind about favourite topic
                if (messageCount % 5 == 0 && !string.IsNullOrEmpty(favouriteTopic))
                {
                    string tip = keywords.ContainsKey(favouriteTopic) ? keywords[favouriteTopic] : GetRandomPhishingTip();
                    return new ChatResponse(
                        "As someone interested in " + favouriteTopic + ", " + UserName + ", you might want to know: " + tip,
                        ChatIntent.None, string.Empty);
                }

                // Tell me more / explain
                if ((input.Contains("more") || input.Contains("explain") ||
                     input.Contains("elaborate") || input.Contains("details"))
                    && !string.IsNullOrEmpty(LastTopic))
                {
                    if (topicDetails.ContainsKey(LastTopic))
                        return new ChatResponse(
                            "Here is more about " + LastTopic + ":\n" + topicDetails[LastTopic],
                            ChatIntent.None, string.Empty);

                    if (LastTopic == "phishing")
                        return new ChatResponse(GetRandomPhishingTip(), ChatIntent.None, string.Empty);

                    if (LastTopic == "tip")
                        return new ChatResponse(GetRandomSecurityTip(), ChatIntent.None, string.Empty);

                    return new ChatResponse(
                        "I do not have extra details on that yet. Try asking about passwords, phishing, scams or privacy.",
                        ChatIntent.None, string.Empty);
                }

                // Another tip
                if (input.Contains("another") || input.Contains("one more") ||
                    input.Contains("next tip") || input.Contains("again"))
                {
                    if (LastTopic == "phishing") return new ChatResponse(GetRandomPhishingTip(), ChatIntent.None, string.Empty);
                    if (LastTopic == "tip") return new ChatResponse(GetRandomSecurityTip(), ChatIntent.None, string.Empty);
                    if (!string.IsNullOrEmpty(LastTopic) && keywords.ContainsKey(LastTopic))
                        return new ChatResponse(keywords[LastTopic], ChatIntent.None, string.Empty);
                    return new ChatResponse("Sure! Here is a security tip:\n" + GetRandomSecurityTip(), ChatIntent.None, string.Empty);
                }

                // Phishing
                if (input.Contains("phishing"))
                {
                    LastTopic = "phishing";
                    return new ChatResponse(GetRandomPhishingTip(), ChatIntent.None, string.Empty);
                }

                // Security tips
                if (input.Contains("tip") || input.Contains("advice") || input.Contains("suggest"))
                {
                    LastTopic = "tip";
                    return new ChatResponse(GetRandomSecurityTip(), ChatIntent.None, string.Empty);
                }

                // Keyword match (longest first so "safe browsing" beats "safe")
                foreach (string key in sortedKeys)
                {
                    if (input.Contains(key))
                    {
                        LastTopic = key;

                        if (key == favouriteTopic)
                            return new ChatResponse(
                                "As someone who is interested in " + key + ", " + UserName + ", here is a reminder:\n" + keywords[key],
                                ChatIntent.None, string.Empty);

                        return new ChatResponse(keywords[key], ChatIntent.None, string.Empty);
                    }
                }

                // Default fallback
                return new ChatResponse(
                    "I am not sure I understand that, " + UserName + ". Can you try rephrasing? " +
                    "Try asking about passwords, phishing, scams or privacy. " +
                    "Or click one of the quick topic buttons on the left!",
                    ChatIntent.None, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Chatbot Error] " + ex.Message);
                return new ChatResponse(
                    "Something went wrong on my end, " + UserName + ". Please try again or rephrase your question.",
                    ChatIntent.None, string.Empty);
            }
        }

        // Returns a random phishing tip from the list
        private string GetRandomPhishingTip()
        {
            return phishingTips[rand.Next(phishingTips.Count)];
        }

        // Returns a random security tip from the list
        private string GetRandomSecurityTip()
        {
            return securityTips[rand.Next(securityTips.Count)];
        }

        // Extracts text that follows a keyword phrase
        private static string ExtractAfter(string input, string phrase)
        {
            int idx = input.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return string.Empty;
            string after = input.Substring(idx + phrase.Length).Trim();
            return after.TrimStart(new char[] { '-', ':', ' ' });
        }

        // Capitalises the first letter of a string
        private static string CapFirst(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        // Resets chatbot memory when the user clicks the Clear button
        public void Reset()
        {
            LastTopic = string.Empty;
            messageCount = 0;
            // Keep favouriteTopic and UserName so the chatbot still remembers the user
        }
    }
}
