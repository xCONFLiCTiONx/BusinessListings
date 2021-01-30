using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BusinessListings
{
    static class Program
    {
        private static Mutex mutex;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var assembly = typeof(Program).Assembly;
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            var id = attribute.Value;

            mutex = new Mutex(true, id);

            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    startApp();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                exitApp();
            }
        }

        private static void startApp()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        private static void exitApp()
        {
            MessageBox.Show("You can only run one instance of this application at a time. Please close any running instances of Business Listings.exe and try again.", "Business Listings - Single Instance", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Environment.Exit(0);
        }
    }
}
