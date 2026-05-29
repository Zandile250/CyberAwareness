using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;

// This class handles all chatbot logic for the Cyber Awareness project.
// It recognises keywords, gives cybersecurity tips, remembers user preferences,
// and generates responses based on the conversation flow.

namespace CyberAwareness
{
    public class Chatbot
    {
        // Can remember and refer back to them during the conversation
        public string UserName { get; set; } = "User";
        public string LastTopic { get; private set; } = "";

        // This stores the user's favourite topic so we can refer back to it later
        private string favouriteTopic = "";

        // This counts how many messages the user has sent during the conversation
        private int messageCount = 0;

        private readonly Random rand = new Random();

        // Sorted keys cached once at startup instead of re-sorting on every message
        private readonly List<string> sortedKeys;

        public Chatbot()
        {
            sortedKeys = new List<string>(keywords.Keys);
            sortedKeys.Sort((a, b) => b.Length.CompareTo(a.Length));
        }

        // These are the keywords the chatbot recognises and their responses
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

        // Phishing tips are stored in a list and randomly selected each time
        private List<string> phishingTips = new List<string>()
        {
            "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
            "Never click on suspicious links in emails. Go directly to the website by typing the URL yourself.",
            "Check the sender's email address carefully. Scammers often spoof legitimate domains.",
            "Look for spelling mistakes and urgent language in emails. These are classic phishing red flags.",
            "Legitimate banks and companies will never ask for your password via email.",
            "When in doubt call the company directly using a number from their official website."
        };

        // General security tips are also randomly selected from this list
        private List<string> securityTips = new List<string>()
        {
            "Enable automatic updates on all your devices to patch security vulnerabilities quickly.",
            "Use a password manager to generate and store complex unique passwords.",
            "Always log out of accounts when using shared or public computers.",
            "Regularly review which apps have access to your camera microphone and location.",
            "Back up important files regularly using the 3-2-1 rule: 3 copies, 2 media types, 1 offsite.",
            "Be wary of free public Wi-Fi. Use a VPN if you must connect to it."
        };

        // These are used when the user says "tell me more" or "explain more"
        // to give a longer explanation of the last topic
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

        // This dictionary stores sentiment words and their empathetic responses.
        // When the user feels worried, curious or frustrated we respond with encouragement.
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


        // MAIN RESPONSE METHOD
        // This is the method that handles all user input and returns a response
        public string GetResponse(string input)
        {
            try
            {
                // Snull or empty input
                if (string.IsNullOrEmpty(input))
                    return $"It looks like you did not type anything, {UserName}. Please enter a question and I will be happy to help!";

                input = input.ToLower().Trim();

                //whitespace-only input
                if (string.IsNullOrWhiteSpace(input))
                    return $"I noticed your message was blank, {UserName}. Try asking about passwords, phishing, scams or privacy!";

                // Count how many messages the user has sent
                messageCount++;

                // Check if the user is expressing a feeling like worried, curious or frustrated.
                // If they are we respond with empathy and automatically share a relevant tip.
                // This means the user does not have to ask again for information.
                string detectedSentiment = "";
                foreach (var sentiment in sentimentResponses.Keys)
                {
                    if (input.Contains(sentiment))
                    {
                        detectedSentiment = sentiment;
                        break;
                    }
                }

                // If we detected a sentiment respond with empathy and then add a relevant tip
                if (!string.IsNullOrEmpty(detectedSentiment))
                {
                    // Get the empathetic response for this sentiment
                    string empathyResponse = sentimentResponses[detectedSentiment];

                    // Check if the user also mentioned a specific topic in the same message
                    // For example "I am worried about scams" - we detect both "worried" and "scam"
                    foreach (var key in keywords.Keys)
                    {
                        if (input.Contains(key))
                        {
                            // Save this as the last topic
                            LastTopic = key;

                            // Return the empathy response plus the tip for that topic automatically
                            return $"{empathyResponse}\n\n{keywords[key]}";
                        }
                    }

                    // If they mentioned phishing check for that too
                    if (input.Contains("phishing"))
                    {
                        LastTopic = "phishing";
                        return $"{empathyResponse}\n\n{GetRandomPhishingTip()}";
                    }

                    // If no specific topic was mentioned just give a general security tip.
                    // This way the user always gets useful information without having to ask again.
                    if (!string.IsNullOrEmpty(LastTopic) && keywords.ContainsKey(LastTopic))
                        return $"{empathyResponse}\n\n{keywords[LastTopic]}";

                    return $"{empathyResponse}\n\n{GetRandomSecurityTip()}";
                }

                //Greeting
                var words = input.Split(new char[] { ' ', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                bool isGreeting = Array.Exists(words, w => w == "hello" || w == "hi" || w == "hey");

                if (isGreeting)
                {
                    LastTopic = "";
                    return $"Hello {UserName}! I am here to help you stay safe online. What would you like to know?";
                }

                // How are you
                if (input.Contains("how are you"))
                {
                    LastTopic = "how are you";
                    return $"I am doing great {UserName}! Ready to help you stay safe online.";
                }

                // Purpose
                if (input.Contains("purpose") || input.Contains("what do you do") || input.Contains("what are you"))
                {
                    LastTopic = "purpose";
                    return $"My purpose is to help you {UserName} understand cybersecurity " +
                           "and protect yourself from online threats.";
                }

                // What can I ask
                if (input.Contains("what can i ask") || input.Contains("help") ||
                    input.Contains("topics") || input.Contains("what can you"))
                {
                    LastTopic = "help";
                    return "You can ask me about:\n" +
                           "  • Password safety\n" +
                           "  • Phishing\n" +
                           "  • Scams\n" +
                           "  • Privacy\n" +
                           "  • Safe browsing\n" +
                           "  • Malware\n" +
                           "  • Firewall\n" +
                           "  • VPN\n" +
                           "  • 2FA\n" +
                           "  • Security tips";
                }

                // Goodbye
                if (input.Contains("bye") || input.Contains("goodbye") || input.Contains("exit"))
                {
                    LastTopic = "";

                    // Remember and refer back to favourite topic on goodbye
                    if (!string.IsNullOrEmpty(favouriteTopic))
                        return $"Goodbye {UserName}! Since you are interested in {favouriteTopic}, " +
                               $"remember to keep up with the latest {favouriteTopic} tips to stay safe online!";

                    return $"Goodbye {UserName}! Stay safe online.";
                }

                // Check if the user is telling us they are interested in a specific topic.
                // We store this so we can refer back to it later in the conversation.
                if (input.Contains("interested in") || input.Contains("i like") || input.Contains("i love") ||
                    input.Contains("my favourite") || input.Contains("i enjoy"))
                {
                    // Check which topic they mentioned
                    foreach (var key in keywords.Keys)
                    {
                        if (input.Contains(key))
                        {
                            favouriteTopic = key;
                            LastTopic = key;

                            return $"Great! I will remember that you are interested in {key}, {UserName}. " +
                                   $"It is a crucial part of staying safe online.\n\n" +
                                   $"Here is something useful about {key}:\n{keywords[key]}";
                        }
                    }

                    // Check if they mentioned phishing specifically
                    if (input.Contains("phishing"))
                    {
                        favouriteTopic = "phishing";
                        LastTopic = "phishing";
                        return $"Great! I will remember that you are interested in phishing awareness, {UserName}. " +
                               $"It is a crucial part of staying safe online.\n\n" +
                               GetRandomPhishingTip();
                    }
                }

                // Every 5 messages remind the user about their favourite topic.
                // This makes the conversation feel more personal and engaging.
                if (messageCount % 5 == 0 && !string.IsNullOrEmpty(favouriteTopic))
                    return $"As someone interested in {favouriteTopic}, {UserName}, " +
                           $"you might want to know: {(keywords.ContainsKey(favouriteTopic) ? keywords[favouriteTopic] : GetRandomPhishingTip())}";

                // If the user wants more details about the last topic
                if ((input.Contains("more") || input.Contains("explain") ||
                     input.Contains("elaborate") || input.Contains("details"))
                    && !string.IsNullOrEmpty(LastTopic))
                {
                    if (topicDetails.ContainsKey(LastTopic))
                        return $"Here is more about {LastTopic}:\n{topicDetails[LastTopic]}";

                    if (LastTopic == "phishing")
                        return GetRandomPhishingTip();

                    if (LastTopic == "tip")
                        return GetRandomSecurityTip();

                    return "I do not have extra details on that yet. " +
                           "Try asking about passwords, phishing, scams or privacy.";
                }

                // If the user asks for another tip on the same topic
                if (input.Contains("another") || input.Contains("one more") ||
                    input.Contains("next tip") || input.Contains("again"))
                {
                    if (LastTopic == "phishing")
                        return GetRandomPhishingTip();

                    if (LastTopic == "tip")
                        return GetRandomSecurityTip();

                    if (!string.IsNullOrEmpty(LastTopic) && keywords.ContainsKey(LastTopic))
                        return keywords[LastTopic];

                    return "Sure! Here is a security tip:\n" + GetRandomSecurityTip();
                }

                // Randomly pick one of several phishing tips
                if (input.Contains("phishing"))
                {
                    LastTopic = "phishing";
                    return GetRandomPhishingTip();
                }

                // Security tips random responses
                if (input.Contains("tip") || input.Contains("advice") || input.Contains("suggest"))
                {
                    LastTopic = "tip";
                    return GetRandomSecurityTip();
                }

                // Uses cached sortedKeys field instead of re-sorting every message
                foreach (var key in sortedKeys)
                {
                    if (input.Contains(key))
                    {
                        LastTopic = key;

                        if (key == favouriteTopic)
                            return $"As someone who is interested in {key}, {UserName}, here is a reminder:\n{keywords[key]}";

                        return keywords[key];
                    }
                }

                //Default response for unrecognised input, keeps app running smoothly
                return $"I am not sure I understand that, {UserName}. Can you try rephrasing? " +
                       "Try asking about passwords, phishing, scams or privacy. " +
                       "Or click one of the quick topic buttons on the left!";
            }
            catch (Exception ex)
            {
                // SECTION 7 - Catches unexpected errors so the chatbot never crashes
                Console.WriteLine($"[Chatbot Error] {ex.Message}");
                return $"Something went wrong on my end, {UserName}. Please try again or rephrase your question.";
            }
        }

        // This method returns a random phishing tip from the list
        private string GetRandomPhishingTip()
        {
            return phishingTips[rand.Next(phishingTips.Count)];
        }

        // This method returns a random security tip from the list
        private string GetRandomSecurityTip()
        {
            return securityTips[rand.Next(securityTips.Count)];
        }

        // This resets the chatbot memory when the user clicks the Clear button
        public void Reset()
        {
            LastTopic = "";
            messageCount = 0;
            //we keep favouriteTopic and UserName so the chatbot still remembers the user
        }
    }
}