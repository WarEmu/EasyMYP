using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EasyMYP
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MainWindow mw;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(mw = new MainWindow());
            if (mw.t_worker != null && mw.t_worker.ThreadState == System.Threading.ThreadState.Running)
            {
                mw.t_worker.Abort();
            }
        }
    }
}