using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MIS
{
    public class clsNotification
    {
        private static NotifyIcon notifyIcon;
        private static Form targetForm;

        public void Show(string pTitle, string pMessage, Form pTarget = null, ToolTipIcon pIconType = ToolTipIcon.Info, int pTimeout = 5000)
        {
            try
            {
                Form owner = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;
                if (owner != null && owner.InvokeRequired)
                {
                    owner.BeginInvoke((MethodInvoker)(() => Show(pTitle, pMessage, pTarget, pIconType, pTimeout)));
                    return;
                }

                if (notifyIcon == null)
                {
                    notifyIcon = new NotifyIcon();
                    notifyIcon.Icon = GetAppIcon();
                    notifyIcon.Text = "MIS";

                    notifyIcon.BalloonTipClicked += (s, e) => BringTargetToFront();
                    notifyIcon.MouseDoubleClick += (s, e) => BringTargetToFront();

                    Application.ApplicationExit += (s, e) =>
                    {
                        notifyIcon.Visible = false;
                        notifyIcon.Dispose();
                    };
                }

                targetForm = pTarget;

                string sTitle = string.IsNullOrWhiteSpace(pTitle) ? "MIS" : pTitle;
                string sMessage = string.IsNullOrWhiteSpace(pMessage) ? "-" : pMessage;

                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(pTimeout, sTitle, sMessage, pIconType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("clsNotification.Show, error=" + ex.Message);
            }
        }

        private static void BringTargetToFront()
        {
            try
            {
                Form frm = targetForm;
                if (frm == null || frm.IsDisposed) return;

                Form top = frm.MdiParent ?? frm;

                if (top.WindowState == FormWindowState.Minimized)
                    top.WindowState = FormWindowState.Normal;

                if (frm.WindowState == FormWindowState.Minimized)
                    frm.WindowState = FormWindowState.Normal;

                top.Show();
                top.Activate();

                frm.BringToFront();
                frm.Activate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("clsNotification.BringTargetToFront, error=" + ex.Message);
            }
        }

        private static Icon GetAppIcon()
        {
            try
            {
                return Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application;
            }
            catch
            {
                return SystemIcons.Application;
            }
        }
    }
}