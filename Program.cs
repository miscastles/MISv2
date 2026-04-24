using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace MIS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string appName = Assembly.GetExecutingAssembly().GetName().Name;
            bool createdNew;
            
            using (Mutex mutex = new Mutex(true, $"Global\\{appName}Mutex", out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show($"{appName} is already running.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                frmSetting.iType = 0;
                Application.Run(new frmLogin());
            }
        }
    }
}
