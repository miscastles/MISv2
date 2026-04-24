using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS
{
    public partial class frmToolSelections : Form
    {
        public frmToolSelections()
        {
            InitializeComponent();
        }


        private List<string> selectedFiles = new List<string>();
        private const int MaxFiles = 5;
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB


        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Files";
                openFileDialog.Filter = "All Files|*.*";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileNames.Length + selectedFiles.Count > MaxFiles)
                    {
                        MessageBox.Show($"You can only attach up to {MaxFiles} files.");
                        return;
                    }

                    foreach (string file in openFileDialog.FileNames)
                    {
                        FileInfo fi = new FileInfo(file);

                        if (fi.Length > MaxFileSize)
                        {
                            MessageBox.Show($"{fi.Name} exceeds 10MB. Skipping.");
                            continue;
                        }

                        selectedFiles.Add(file);
                        lstFiles.Items.Add(Path.GetFileName(file));
                    }
                }
            }
        }

        private void SendEmail(string[] p_Attachments)
        {
            var emailData = new
            {
                EmailTo = txtEmailTo.Text,
                Attachments = p_Attachments
            };

            string v_Json = Newtonsoft.Json.JsonConvert.SerializeObject(emailData);

            clsAPI dbAPI = new clsAPI();
            clsFunction dbFunction = new clsFunction();

            dbAPI.ExecuteAPI("POST",
                             "Notify",
                             "Email-Client",
                             v_Json,
                             "Email Notification",
                             "",
                             "EmailNotification");

            dbFunction.SetMessageBox("Send email complete.", "Email", clsFunction.IconType.iInformation);
        }

        private string FormatFileName(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            // Replace spaces with underscores
            fileName = fileName.Replace(" ", "_");

            // Remove invalid characters for file names
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c.ToString(), "");
            }

            // Optionally: add timestamp prefix to avoid duplicates
            string safeName = $"{DateTime.Now:yyyy_MM_dd}_{fileName}";

            return safeName;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("Please select at least one file.");
                return;
            }

            if (selectedFiles.Count > MaxFiles)
            {
                MessageBox.Show($"Maximum of {MaxFiles} files can be uploaded.");
                return;
            }

            try
            {
                string remotePath = "/upload/logs/";
                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);

                List<string> uploadedFilePaths = new List<string>();

                foreach (string localFile in selectedFiles)
                {
                    string fileName = Path.GetFileName(localFile);
                    string newFileName = $"{DateTime.Now:yyyy_MM_dd}_{fileName}";
                    string remoteFilePath = remotePath + newFileName;

                    ftpClient.upload(remoteFilePath, localFile);
                    uploadedFilePaths.Add(remoteFilePath);

                    //MessageBox.Show($"Uploaded: {newFileName}");
                }

                ftpClient.disconnect();
                //SendEmail(uploadedFilePaths.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show("FTP upload failed: " + ex.Message);
            }
        }
    }
}
