using System;
using System.Windows.Forms;

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
            MainWindow mw;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(mw = new MainWindow());
            // In order to kill some worker threads that may have been launched
            if (mw.t_worker != null && mw.t_worker.ThreadState == System.Threading.ThreadState.Running)
            {
                mw.t_worker.Abort();
            }
            // In order to kill the pattern brute forcing threads that may have been launched
            if (mw.t_GeneratePat != null 
             && (mw.t_GeneratePat.ThreadState == System.Threading.ThreadState.Running
             || mw.t_GeneratePat.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                mw.t_GeneratePat.Abort();
            }
        }
    }
}