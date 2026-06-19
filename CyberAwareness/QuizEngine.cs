using System;
using System.Collections.Generic;

// Provides the question bank and shuffle logic for the Mini Game.
// GetShuffledQuestions() returns a randomised subset each time
// so the quiz feels different on every run.

namespace CyberAwareness
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public string[] Options { get; set; }
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; }

        public QuizQuestion()
        {
            Question = string.Empty;
            Options = new string[0];
            Explanation = string.Empty;
        }
    }

    public static class QuizEngine
    {
        private static readonly Random _rng = new Random();

        private static readonly List<QuizQuestion> AllQuestions = new List<QuizQuestion>()
        {
            new QuizQuestion()
            {
                Question     = "What is the minimum recommended length for a strong password?",
                Options      = new string[] { "6 characters", "8 characters", "12 characters", "20 characters" },
                CorrectIndex = 2,
                Explanation  = "Security experts recommend at least 12 characters. Longer passphrases are even better."
            },
            new QuizQuestion()
            {
                Question     = "Which of the following is a classic sign of a phishing email?",
                Options      = new string[] { "Uses your full name", "Sent from the company's official domain", "Creates urgency to click a link immediately", "Contains a verified digital signature" },
                CorrectIndex = 2,
                Explanation  = "Phishing emails create urgency to bypass your critical thinking. Always pause and verify before clicking."
            },
            new QuizQuestion()
            {
                Question     = "What does 2FA stand for?",
                Options      = new string[] { "Two-Factor Authentication", "Two-File Access", "Two-Form Application", "Two-Factor Authorisation Attempt" },
                CorrectIndex = 0,
                Explanation  = "Two-Factor Authentication adds a second verification step on top of your password."
            },
            new QuizQuestion()
            {
                Question     = "What is the 3-2-1 backup rule?",
                Options      = new string[] { "3 devices, 2 cloud services, 1 external drive", "3 copies, 2 different media types, 1 offsite copy", "3 folders, 2 hard drives, 1 password", "3 daily, 2 weekly, 1 monthly" },
                CorrectIndex = 1,
                Explanation  = "Keep 3 copies of data on 2 different storage types, with 1 stored offsite for disaster recovery."
            },
            new QuizQuestion()
            {
                Question     = "Which 2FA method is considered the most secure?",
                Options      = new string[] { "SMS text message", "Email one-time code", "Authenticator app (e.g. Google Authenticator)", "Security question" },
                CorrectIndex = 2,
                Explanation  = "Authenticator apps are safer than SMS, which is vulnerable to SIM-swapping attacks."
            },
            new QuizQuestion()
            {
                Question     = "What does a VPN primarily protect you from on public Wi-Fi?",
                Options      = new string[] { "Viruses and malware on your device", "Phishing websites", "Traffic interception by eavesdroppers", "All cyber threats" },
                CorrectIndex = 2,
                Explanation  = "A VPN encrypts your traffic in transit. It does NOT block malware or phishing on its own."
            },
            new QuizQuestion()
            {
                Question     = "Which file type is commonly used to deliver malware via email?",
                Options      = new string[] { ".txt", ".png", ".docm (macro-enabled Word document)", ".csv" },
                CorrectIndex = 2,
                Explanation  = "Macro-enabled Office files can run code when opened and are a popular malware delivery method."
            },
            new QuizQuestion()
            {
                Question     = "What should you do FIRST if you suspect your account has been hacked?",
                Options      = new string[] { "Delete all your emails", "Change the password immediately", "Post about it on social media", "Wait to see if it resolves itself" },
                CorrectIndex = 1,
                Explanation  = "Change your password immediately to lock out the attacker, then enable 2FA and review recent activity."
            },
            new QuizQuestion()
            {
                Question     = "What does HTTPS indicate about a website?",
                Options      = new string[] { "The site is completely safe and trustworthy", "Your connection to the site is encrypted", "The site contains no malware", "The site is government-approved" },
                CorrectIndex = 1,
                Explanation  = "HTTPS means the connection is encrypted. It does NOT guarantee the site itself is legitimate."
            },
            new QuizQuestion()
            {
                Question     = "Which is NOT a good social media security practice?",
                Options      = new string[] { "Using a strong unique password", "Reviewing app permissions regularly", "Sharing your holiday plans publicly before you leave", "Enabling login notifications" },
                CorrectIndex = 2,
                Explanation  = "Sharing holiday plans publicly tells burglars your home is empty. Post travel photos after you return."
            }
        };

        /// <summary>Returns a shuffled list of 'count' questions (defaults to 5).</summary>
        public static List<QuizQuestion> GetShuffledQuestions(int count)
        {
            List<QuizQuestion> copy = new List<QuizQuestion>(AllQuestions);

            // Fisher-Yates shuffle
            for (int i = copy.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                QuizQuestion temp = copy[i];
                copy[i] = copy[j];
                copy[j] = temp;
            }

            int take = count < copy.Count ? count : copy.Count;
            return copy.GetRange(0, take);
        }
    }
}
