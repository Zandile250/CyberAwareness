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

        Console.ReadLine();
    }
}