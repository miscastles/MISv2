using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace MIS
{
    public class clsExport
    {        
        public void ExportToExcel(ListView obj, string sPath, string sFileName, string sHeader)
        {
            try
            {
                //lvPDF is nothing but the listview control name
                string[] st = new string[obj.Columns.Count];
                DirectoryInfo di = new DirectoryInfo(sPath);
                if (di.Exists == false)
                    di.Create();
                StreamWriter sw = new StreamWriter(sPath + sFileName, false);
                sw.AutoFlush = true;
                for (int col = 0; col < obj.Columns.Count; col++)
                {
                    sw.Write("\t" + obj.Columns[col].Text.ToString());
                }

                int rowIndex = 1;
                int row = 0;

                // Write Header
                sw.WriteLine(sHeader);

                string st1 = "";
                for (row = 0; row < obj.Items.Count; row++)
                {                           
                    if (rowIndex <= obj.Items.Count)
                        rowIndex++;
                    st1 = "\n";
                    for (int col = 0; col < obj.Columns.Count; col++)
                    {
                        Debug.WriteLine("col=" + col.ToString() + "-" + "Header=" + obj.Items[0].SubItems[col].Text.ToString());

                        switch (col)
                        {
                            case 13:
                            case 14:
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                            case 20:
                            case 21:
                            case 22:
                            case 23:
                            case 24:
                            case 25:
                            case 27:
                                st1 = st1 + "\t" + obj.Items[row].SubItems[col].Text.ToString();
                                break;
                        }

                    }
                    sw.WriteLine(st1);
                }
                sw.Close();
                FileInfo fil = new FileInfo(sPath + sFileName);
                if (fil.Exists == true)
                    MessageBox.Show("Export completed", "Export to Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
