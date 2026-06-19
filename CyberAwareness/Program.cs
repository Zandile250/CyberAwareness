using System;
using System.Windows.Forms;
namespace CyberAwareness
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            try
            {
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Startup error:\n\n" + ex.ToString(),
                    "CyberAwareness - Crash",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}