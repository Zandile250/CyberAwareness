using System;
using System.Collections.Generic;

namespace CyberAwareness
{
    /// <summary>
    /// Cybersecurity chatbot — merges Task 1 console logic into the GUI.
    /// Covers: keyword recognition, random responses, conversation flow.
    /// </summary>
    public class Chatbot
    {
        // ─────────────────────────────────────────────────────────────
        // STATE
        // ─────────────────────────────────────────────────────────────
        public string LastTopic { get; private set; } = "";
        public string UserName { get; set; } = "User";
        private readonly Random rand = new Random();

        // ─────────────────────────────────────────────────────────────
        // KEYWORD → SINGLE RESPONSE  (Req 2 + Task 1 topics)
        // ─────────────────────────────────────────────────────────────
        private readonly Dictionary<string, string> keywords =
            new Dictionary<string, string>
        {
            // ── From Task 1 ──────────────────────────────────────────
            { "password",
              "🔑 Password Safety Tips:\n" +
              "  • Use at least 12 characters\n" +
              "  • Mix uppercase, lowercase, numbers and symbols\n" +
              "  • Never reuse passwords across sites\n" +
              "  • Use a password manager\n" +
              "  • Enable two-factor authentication (2FA)" },

            { "safe browsing",
              "🌐 Safe Browsing Habits:\n" +
              "  • Only visit sites with https://\n" +
              "  • Avoid public Wi-Fi for sensitive tasks\n" +
              "  • Keep your browser and software updated\n" +
              "  • Use a VPN on public networks\n" +
              "  • Don't download files from unknown sources" },

            { "browse safely",
              "🌐 To browse safely: avoid unknown websites, keep your browser updated, " +
              "and never download files from untrusted sources." },

            { "scam",
              "⚠️ Be very cautious of unsolicited messages asking for money, gift cards, or " +
              "personal details. Legitimate organisations will never pressure you urgently. " +
              "Contact the organisation directly through their official channels." },

            { "privacy",
              "🛡️ Protect your privacy by limiting what you share online. Use privacy settings " +
              "on social media, avoid public Wi-Fi for sensitive tasks, and check app " +
              "permissions before installing anything." },

            { "malware",
              "🦠 Malware is malicious software that damages or gains unauthorised access to " +
              "your device. Keep your OS and antivirus updated, avoid untrusted downloads, " +
              "and never open suspicious email attachments." },

            { "firewall",
              "🔥 A firewall monitors incoming and outgoing network traffic and blocks " +
              "unauthorised access. Keep your OS firewall enabled and secure your router." },

            { "2fa",
              "🔐 Two-factor authentication (2FA) adds an extra layer of security. Even if " +
              "someone steals your password, they still need your second factor to log in. " +
              "Enable it on email, banking, and social media." },

            { "two-factor",
              "🔐 Two-factor authentication adds a second verification step. Enable it " +
              "wherever possible to protect your accounts." },

            { "mfa",
              "🔐 Multi-factor authentication uses two or more verification steps. It " +
              "dramatically reduces the risk of unauthorised access." },

            { "vpn",
              "🌍 A VPN (Virtual Private Network) encrypts your internet traffic and hides " +
              "your IP address. Use one when connecting to public Wi-Fi." },

            { "ransomware",
              "💀 Ransomware encrypts your files and demands payment to restore access. " +
              "Back up your data regularly and never open suspicious attachments." },

            { "social engineering",
              "🎭 Social engineering manipulates people into revealing confidential information. " +
              "Always verify the identity of anyone requesting sensitive data." },
        };

        // ─────────────────────────────────────────────────────────────
        // KEYWORD → RANDOM RESPONSES  (Req 3)
        // ─────────────────────────────────────────────────────────────
        private readonly Dictionary<string, List<string>> randomResponses =
            new Dictionary<string, List<string>>
        {
            {
                "phishing", new List<string>
                {
                    "🎣 Phishing Awareness:\n" +
                    "  • Never click suspicious links in emails\n" +
                    "  • Check the sender's email address carefully\n" +
                    "  • Legitimate companies never ask for passwords\n" +
                    "  • Look for spelling errors in suspicious emails\n" +
                    "  • When in doubt, go directly to the website",

                    "🎣 Check the sender's email address carefully — scammers often spoof legitimate domains.",
                    "🎣 Never click links in unexpected emails. Type the URL directly into your browser.",
                    "🎣 Look for spelling mistakes and urgent language — classic phishing red flags.",
                    "🎣 Legitimate banks will never ask for your password via email.",
                    "🎣 Hover over links before clicking — the real destination often reveals a phishing attempt."
                }
            },
            {
                "tip", new List<string>
                {
                    "💡 Enable automatic updates on all your devices to patch vulnerabilities quickly.",
                    "💡 Use a password manager to generate and store complex, unique passwords.",
                    "💡 Always log out of accounts when using shared or public computers.",
                    "💡 Regularly review which apps have access to your camera, microphone, and location.",
                    "💡 Back up important files with the 3-2-1 rule: 3 copies, 2 media types, 1 offsite.",
                    "💡 Be wary of free public Wi-Fi. Use a VPN if you must connect."
                }
            },
            {
                "hack", new List<string>
                {
                    "🔓 Use strong, unique passwords and 2FA to significantly reduce hacking risk.",
                    "🔓 Keep all software updated — many hacks exploit known, unpatched vulnerabilities.",
                    "🔓 Be cautious of phishing emails: the most common entry point for hackers.",
                    "🔓 Monitor your accounts for unusual activity and set up login alerts where available."
                }
            }
        };

        // ─────────────────────────────────────────────────────────────
        // EXTENDED DETAILS  (used by "tell me more")
        // ─────────────────────────────────────────────────────────────
        private readonly Dictionary<string, string> topicDetails =
            new Dictionary<string, string>
        {
            { "password",
              "A strong password is at least 12 characters, combining letters, numbers, and " +
              "symbols. Avoid dictionary words or personal info like birthdays. A passphrase — " +
              "three random words joined together — is both memorable and strong. Never reuse " +
              "a password across different sites." },

            { "phishing",
              "Phishing comes in many forms: email phishing, SMS phishing (smishing), and voice " +
              "phishing (vishing). Attackers impersonate trusted entities to steal credentials or " +
              "install malware. Always verify the source before clicking links or providing info." },

            { "scam",
              "Common scams include romance scams, investment fraud, tech support scams, and " +
              "lottery scams. They rely on urgency and emotion. Take your time, talk to someone " +
              "you trust, and independently verify any claim before sending money." },

            { "privacy",
              "Use privacy-focused browsers (e.g. Firefox or Brave), enable ad-blockers, review " +
              "social media privacy settings regularly, and opt out of data collection where " +
              "possible. Be mindful of what you share publicly." },

            { "malware",
              "Types of malware include viruses, trojans, spyware, adware, and ransomware. " +
              "Install reputable antivirus software, keep it updated, and run regular scans. " +
              "Avoid pirated software — one of the most common malware delivery methods." },

            { "safe browsing",
              "HTTPS encrypts data between your browser and the server. Always look for the " +
              "padlock icon. Keep browser extensions minimal — malicious extensions can track " +
              "everything you type." },

            { "hack",
              "Hackers use brute-force attacks, phishing, unpatched software exploits, and social " +
              "engineering. The best defence is layered security: strong passwords, 2FA, updated " +
              "software, and healthy scepticism toward unsolicited contact." },
        };

        // ─────────────────────────────────────────────────────────────
        // CONVERSATION FLOW KEYWORDS  (Req 4)
        // ─────────────────────────────────────────────────────────────
        private static readonly string[] morePhrases =
            { "tell me more", "explain more", "more info", "elaborate", "details", "expand", "more" };

        private static readonly string[] anotherPhrases =
            { "another", "again", "different", "next tip", "one more", "give me another" };

        private static readonly string[] greetings =
            { "hello", "hi", "hey", "greetings", "good morning", "good afternoon", "good evening" };

        private static readonly string[] farewells =
            { "bye", "goodbye", "exit", "quit", "see you", "farewell" };

        // ─────────────────────────────────────────────────────────────
        // PUBLIC API
        // ─────────────────────────────────────────────────────────────
        public string GetResponse(string input)
        {
            input = input.Trim().ToLower();

            // ── Greetings ─────────────────────────────────────────────
            if (ContainsAny(input, greetings))
            {
                LastTopic = "greeting";
                return $"Hello, {UserName}! 👋 I'm here to help you stay safe online. " +
                       "Ask me about passwords, phishing, scams, privacy, malware, and more!";
            }

            // ── Farewells ─────────────────────────────────────────────
            if (ContainsAny(input, farewells))
            {
                LastTopic = "farewell";
                return $"Goodbye, {UserName}! 🔒 Stay safe online.";
            }

            // ── How are you ───────────────────────────────────────────
            if (input.Contains("how are you"))
            {
                LastTopic = "small talk";
                return $"I'm functioning perfectly, {UserName}! 😄 Ready to help you stay safe online.";
            }

            // ── Purpose ───────────────────────────────────────────────
            if (input.Contains("purpose") || input.Contains("what do you do") || input.Contains("what are you"))
            {
                LastTopic = "purpose";
                return $"My purpose is to help you, {UserName}, understand cybersecurity " +
                       "and protect yourself from online threats. 🛡️";
            }

            // ── What can I ask ────────────────────────────────────────
            if (input.Contains("what can i ask") || input.Contains("help") || input.Contains("topics"))
            {
                LastTopic = "help";
                return "You can ask me about:\n" +
                       "  🔑 Password safety\n" +
                       "  🎣 Phishing\n" +
                       "  🌐 Safe browsing\n" +
                       "  ⚠️ Scams\n" +
                       "  🛡️ Privacy\n" +
                       "  🦠 Malware\n" +
                       "  🔥 Firewalls\n" +
                       "  🔐 2FA / MFA\n" +
                       "  🌍 VPN\n" +
                       "  💡 Security tips";
            }

            // ── "Tell me more" → expand last topic ────────────────────
            if (ContainsAny(input, morePhrases) && !string.IsNullOrEmpty(LastTopic))
            {
                if (topicDetails.ContainsKey(LastTopic))
                    return $"📖 More on {LastTopic}:\n{topicDetails[LastTopic]}";

                if (randomResponses.ContainsKey(LastTopic))
                    return GetRandom(LastTopic);

                return "I don't have extra details on that yet. " +
                       "Try asking about phishing, passwords, malware, or scams!";
            }

            // ── "Another tip / give me another" ───────────────────────
            if (ContainsAny(input, anotherPhrases) && !string.IsNullOrEmpty(LastTopic))
            {
                if (randomResponses.ContainsKey(LastTopic))
                    return GetRandom(LastTopic);

                if (keywords.ContainsKey(LastTopic))
                    return keywords[LastTopic];

                return "I don't have more variations for that topic. " +
                       "Try asking about phishing or security tips!";
            }

            // ── Scan: random-response topics first ────────────────────
            foreach (var kvp in randomResponses)
            {
                if (input.Contains(kvp.Key))
                {
                    LastTopic = kvp.Key;
                    return GetRandom(kvp.Key);
                }
            }

            // ── Scan: single-response topics (longest key first) ──────
            var sortedKeys = new List<string>(keywords.Keys);
            sortedKeys.Sort((a, b) => b.Length.CompareTo(a.Length));

            foreach (var key in sortedKeys)
            {
                if (input.Contains(key))
                {
                    LastTopic = key;
                    return keywords[key];
                }
            }

            // ── Fallback ──────────────────────────────────────────────
            return $"I'm not sure about that, {UserName}. Try asking about:\n" +
                   "  Passwords • Phishing • Scams • Privacy\n" +
                   "  Malware • Firewalls • VPN • 2FA • Safe browsing\n" +
                   "Or click a quick-topic button on the left!";
        }

        /// <summary>Resets conversation state (used by the Clear button).</summary>
        public void Reset()
        {
            LastTopic = "";
        }

        // ─────────────────────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────────────────────
        private string GetRandom(string key)
            => randomResponses[key][rand.Next(randomResponses[key].Count)];

        private static bool ContainsAny(string input, string[] phrases)
        {
            foreach (var p in phrases)
                if (input.Contains(p)) return true;
            return false;
        }
    }
}
