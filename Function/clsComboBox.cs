using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS.Function
{
    public static class clsComboBox
    {
        public static void RegisterUpperCase(ComboBox comboBox)
        {
            comboBox.TextChanged += (s, e) =>
            {
                ComboBox cbo = (ComboBox)s;

                int cursorPos = cbo.SelectionStart;
                string upperText = cbo.Text.ToUpper();

                if (cbo.Text != upperText)
                {
                    cbo.Text = upperText;
                    cbo.SelectionStart = cursorPos;
                }
            };
        }
    }
}
