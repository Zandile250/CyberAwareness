using SoundPlaying.CyberAwareness;

class Program
{
    
    static void Main(string[] args)
    {
        // This clears anything on the screen before we start
        Console.Clear();

        // This changes the text color to cyan (light blue)
        Console.ForegroundColor = ConsoleColor.Cyan;

        // This shows our cool logo when the program starts
        Console.WriteLine(@"
        ██████╗ ██████╗ ████████╗
       ██╔════╝██╔═══██╗╚══██╔══╝
       ██║     ██║   ██║   ██║   
       ██║     ██║   ██║   ██║   
       ╚██████╗╚██████╔╝   ██║   
        ╚═════╝ ╚═════╝    ╚═╝   

    +---------------------------------+
    |    /\        /\        /\       |
    |   /  \  /\  /  \  /\ /  \      |
    |  / /\ \/__\/ /\ \/__/ /\ \     |
    |  \/  \/    \/  \/   \/  \/     |
    |                                 |
    |  [ CYBERSECURITY AWARENESS BOT ]|
    |  [ PROTECTING YOU ONLINE      ] |
    |  [ STAY SAFE. STAY AWARE.     ] |
    +---------------------------------+

         🔒 THINK SAFE  🛡️ ACT SMART  ⚠️ STAY ALERT
        ");
        Console.ResetColor();


        // we then prints our welcome message to the screen
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  Hello! Welcome to the Cybersecurity Awareness Bot.");
        Console.WriteLine("  I'm here to help you stay safe online.");
        Console.ResetColor();

        // This creates a new Sound object so we can use it to play sounds
        Sound sound = new Sound();

        // The location of our sound file on the computer
        string path = @"C:\Users\zandi\source\repos\CyberAwareness\CyberAwareness\Sound.wav";

        // It then checks if the sound file actually exists before trying to play it
        if (File.Exists(path))
        {
            sound.Sound_wav(path);
        }
        else
        {
            Console.WriteLine("Sound file not found!");
        }

        // Prompting the user for their name
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\n  Please enter your name: ");
        Console.ResetColor();

        // Store the name the user types in
        string name = Console.ReadLine();

        // Display a personalised welcome message using their name
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n  ╔══════════════════════════════════════════════╗");
        Console.WriteLine($"  ║  Hello, {name.PadRight(37)}║");
        Console.WriteLine($"  ║  Welcome to the Cybersecurity Awareness Bot. ║");
        Console.WriteLine($"  ║  I am here to help you stay safe online,     ║");
        Console.WriteLine($"  ║  {name.PadRight(45)}║");
        Console.WriteLine("  ╚══════════════════════════════════════════════╝");
        Console.ResetColor();

        // Display a menu header using decorative borders so the user knows what to do next
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n  ╔══════════════════════════════════════════════╗");
        Console.WriteLine("  ║         What would you like to know?         ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════╝");

        // List all the available options the user can choose from
        Console.WriteLine("\n  You can ask me about:");
        Console.WriteLine("  [1] How are you?");
        Console.WriteLine("  [2] What's your purpose?");
        Console.WriteLine("  [3] What can I ask you about?");
        Console.WriteLine("  [4] Password safety");
        Console.WriteLine("  [5] Phishing");
        Console.WriteLine("  [6] Safe browsing");
        Console.WriteLine("  [0] Exit");
        Console.ResetColor();

        // This boolean controls the loop - when it becomes false the chat ends
        bool chatting = true;

        // Keep looping so the user can ask multiple questions without restarting
        while (chatting)
        {
            // Prompt the user to enter their choice, using their name to personalise it
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\n  {name}, enter your choice: ");
            Console.ResetColor();

            // Read what the user typed, remove extra spaces and convert to lowercase
            // so that "Password Safety" and "password safety" both work the same way
            string input = Console.ReadLine()?.Trim().ToLower();

            Console.ForegroundColor = ConsoleColor.Green;

            // Check the user's input and display the matching response
            switch (input)
            {
                // If the user types 1 or the actual question, respond to how are you
                case "1":
                case "how are you":
                    Console.WriteLine($"\n  I'm doing great, {name}! Ready to help you stay safe online.");
                    break;

                // Explains what the bot was built to do
                case "2":
                case "what's your purpose":
                    Console.WriteLine($"\n  My purpose is to help you, {name}, understand cybersecurity");
                    Console.WriteLine("  and protect yourself from online threats.");
                    break;

                // Lists the topics the user can explore
                case "3":
                case "what can i ask you about":
                    Console.WriteLine("\n  You can ask me about:");
                    Console.WriteLine("  - Password safety");
                    Console.WriteLine("  - Phishing scams");
                    Console.WriteLine("  - Safe browsing habits");
                    break;

                // Password safety tips - covers the most important rules for strong passwords
                case "4":
                case "password safety":
                    Console.WriteLine("\n  ── Password Safety Tips ──────────────────────");
                    Console.WriteLine("  • Use at least 12 characters");
                    Console.WriteLine("  • Mix uppercase, lowercase, numbers and symbols");
                    Console.WriteLine("  • Never reuse passwords across sites");
                    Console.WriteLine("  • Use a password manager");
                    Console.WriteLine("  • Enable two-factor authentication (2FA)");
                    break;

                // Phishing awareness - teaches the user how to spot and avoid phishing scams
                case "5":
                case "phishing":
                    Console.WriteLine("\n  ── Phishing Awareness ────────────────────────");
                    Console.WriteLine("  • Never click suspicious links in emails");
                    Console.WriteLine("  • Check the sender's email address carefully");
                    Console.WriteLine("  • Legitimate companies never ask for passwords");
                    Console.WriteLine("  • Look for spelling errors in emails");
                    Console.WriteLine("  • When in doubt, go directly to the website");
                    break;

                // Safe browsing tips - helps the user stay safe while using the internet
                case "6":
                case "safe browsing":
                    Console.WriteLine("\n  ── Safe Browsing Habits ──────────────────────");
                    Console.WriteLine("  • Only visit sites with https://");
                    Console.WriteLine("  • Avoid public Wi-Fi for sensitive tasks");
                    Console.WriteLine("  • Keep your browser and software updated");
                    Console.WriteLine("  • Use a VPN on public networks");
                    Console.WriteLine("  • Don't download files from unknown sources");
                    break;

                // If the user chooses 0 or types exit, set chatting to false to end the loop
                case "0":
                case "exit":
                    Console.WriteLine($"\n  Goodbye, {name}! Stay safe online.");
                    chatting = false; // This stops the while loop from running again
                    break;

                // If the input does not match any case, warn the user and ask them to try again
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Sorry, I didn't understand that. Please choose a number from the menu.");
                    break;
            }

            // Reset the colour back to default after every response
            Console.ResetColor();
        }

        Console.ReadLine();
    }
}