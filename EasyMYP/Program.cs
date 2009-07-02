using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace EasyMYP
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            SetupDirectories();
            MainWindow mw;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(mw = new MainWindow());

            // Kills all the children threads launched.
            // Though we whould program it a bit more nicely
            // But at least, for the moment, it will kill children threads
            Process.GetCurrentProcess().Kill();
            //Process.GetCurrentProcess().Close(); 
            
        }

        static void SetupDirectories()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory()+"/Hash"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory()+"/Hash");
            }
        }

    }
}