using MIS;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using static WinFormAnimation.AnimationFunctions;

namespace MIS
{
    public partial class frmImagePreview : Form
    {
        private clsFunction dbFunction;
        private clsFile dbFile;

        private string sJSONData = "";
        private string pImageURL = "";

        public frmImagePreview(string jsonData, string imageURL)
        {
            InitializeComponent();

            dbFunction = new clsFunction();
            dbFile = new clsFile();

            sJSONData = jsonData;
            pImageURL = imageURL;

            KeyPreview = true;

            InitDataGridViewColumns();
            LoadImageData();
        }

        private void FillImageDetails(JObject imageData)
        {
            dgvImageData.Rows.Clear();

            if (imageData == null) return;

            foreach (JProperty property in imageData.Properties())
            {
                dgvImageData.Rows.Add(property.Name.ToUpper(), Convert.ToString(property.Value));
            }

            dgvImageData.ClearSelection();
        }

        private void InitDataGridViewColumns()
        {
            dgvImageData.AutoGenerateColumns = false;
            dgvImageData.AllowUserToAddRows = false;
            dgvImageData.AllowUserToDeleteRows = false;
            dgvImageData.ReadOnly = true;
            dgvImageData.RowHeadersVisible = false;
            dgvImageData.MultiSelect = false;
            dgvImageData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvImageData.Columns.Clear();

            DataGridViewTextBoxColumn dataColumn = new DataGridViewTextBoxColumn();

            dataColumn.Name = "dgvcol_DetailName";
            dataColumn.HeaderText = "DATA";
            dataColumn.Width = 130;
            dataColumn.ReadOnly = true;

            DataGridViewTextBoxColumn valueColumn = new DataGridViewTextBoxColumn();

            valueColumn.Name = "dgvcol_DetailValue";
            valueColumn.HeaderText = "VALUE";
            valueColumn.ReadOnly = true;
            valueColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvImageData.Columns.Add(dataColumn);
            dgvImageData.Columns.Add(valueColumn);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void LoadImageData()
        {
            if (string.IsNullOrWhiteSpace(sJSONData)) return;
            if (string.IsNullOrWhiteSpace(pImageURL)) return;

            try
            {
                JObject imageData =
                    JObject.Parse(sJSONData);

                FillImageDetails(imageData);

                pBoxPreview.Load(pImageURL);
                pBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox(
                    "Unable to load image information.\n\n" +
                    ex.Message,
                    "Image preview",
                    clsFunction.IconType.iError
                );
            }
        }

        private void frmImagePreview_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnDownloadImage_Click(object sender, EventArgs e)
        {
            ftp ftpClient = null;

            try
            {
                JObject imageData = JObject.Parse(sJSONData);
                
                string pFileName = Convert.ToString(imageData["FileName"]);
                string pFTPFileName = Convert.ToString(imageData["FTPFileName"]);
                string pExpenseFTPHost =
                    $"{clsGlobalVariables.strFTPURL}/" +
                    $"{clsGlobalVariables.strFTPUploadPath}/expenses/" +
                    $"{clsSearch.ClassBankCode}";

                string pLocalFile = Path.Combine(dbFile.sDowloadPath, pFileName);

                Cursor.Current = Cursors.WaitCursor;

                dbFile.CheckFolder(dbFile.sDowloadPath);

                ftpClient = new ftp(pExpenseFTPHost, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);

                ftpClient.download(
                    pFTPFileName,
                    pLocalFile
                );

                dbFunction.SetMessageBox(
                    "Image downloaded.\n\n" +
                    "File: " + pFileName + "\n" +
                    "Location: " + dbFile.sDowloadPath,
                    "Download complete",
                    clsFunction.IconType.iInformation
                );
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox(
                    "Unable to download the selected image.\n\n" +
                    "Error: " + ex.Message,
                    "Download failed",
                    clsFunction.IconType.iError
                );
            }
            finally
            {
                if (ftpClient != null)
                {
                    ftpClient.disconnect();
                }

                Cursor.Current = Cursors.Default;
            }
        }
    }
}
