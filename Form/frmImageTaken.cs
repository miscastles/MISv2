using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace MIS
{
    public partial class frmImageTaken : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsFile dbFile;
        private clsINI dbINI;

        private string pFileName = clsFunction.sNull;
        private string poutFileName = clsFunction.sNull;
        private string poutFullPathFileName = clsFunction.sNull;
        private int pIndex = 0;

        int increment = 10; // Change size by 20 pixels
        private bool isDragging = false;
        private Point dragStartPoint;

        private float scaleFactor = 1.1f; // Scaling factor for zooming

        // Add these fields at the class level (outside the method)
        private Size previewDefaultSize = new Size(778, 511);   // your default size
        private Point previewDefaultLocation = new Point(5, 298);

        private float zoomFactor = 1.1f;   // zoom step
        private float currentZoom = 1.0f;  // current zoom
        private Size originalPicSize;      // default size of picPreview

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmImageTaken()
        {
            InitializeComponent();

            // Enable MouseWheel event (requires focus on the form)
            this.MouseWheel += new MouseEventHandler(Form_MouseWheel);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void frmImageTaken_Load(object sender, EventArgs e)
        {
            string pTitle = "IMAGE TAKEN";

            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbFile = new clsFile();
            dbINI = new clsINI();

            dbINI.InitFTPSetting();

            InitAttachment();

            txtMerchantName.Text = clsSearch.ClassParticularName;
            txtIRTID.Text = clsSearch.ClassTID;
            txtIRMID.Text = clsSearch.ClassMID;
            txtServiceNo.Text = clsSearch.ClassServiceNo.ToString();

            // merchant representative
            txtRepMerchName.Text = clsParticular.ClassParticularName;
            txtRepMerchPosition.Text = clsParticular.ClassPosition;
            txtRepMerchContactNo.Text = clsParticular.ClassContactNumber;
            txtRepMercEmail.Text = clsParticular.ClassEmail;

            // vendor representative
            txtRepVendorName.Text = clsVendor.ClassParticularName;
            txtRepVendorPosition.Text = clsVendor.ClassPosition;
            txtRepMerchContactNo.Text = clsVendor.ClassContactNumber;
            txtRepVendorEmail.Text = clsVendor.ClassEmail;

            txtTerminalSN.Text = clsSearch.ClassTerminalSN;
            txtSIMSN.Text = clsSearch.ClassSIMSerialNo;

            string pRemotePath = $"{clsGlobalVariables.strFTPRemoteImagesPath}{clsSearch.ClassBankCode}/";
            bool isSSL = dbAPI.isSSLEnable();
            string pURL = dbAPI.getAPISSLEnable() + clsGlobalVariables.strAPIServerIPAddress;
            string pServiceNo = $"{clsSearch.ClassServiceNo}";
            string pPath = $"{clsGlobalVariables.strAPIFolder}{pRemotePath}";

            // ------------------------------------------------------
            // load sign/images     
            // ------------------------------------------------------
            Cursor.Current = Cursors.WaitCursor;
            lblHeader.Text = $"{pTitle}" + " - " + "Loading images, please wait...";

            try
            {
                foreach (var task in new[]
            {
                    new Func<Task>(() => clsFunction.LoadImageFromURLAsync(pURL, pPath, picAttachment1, 1, pServiceNo, false)),
                    new Func<Task>(() => clsFunction.LoadImageFromURLAsync(pURL, pPath, picAttachment2, 2, pServiceNo, false)),
                    new Func<Task>(() => clsFunction.LoadImageFromURLAsync(pURL, pPath, picAttachment3, 3, pServiceNo, false)),
                    new Func<Task>(() => clsFunction.LoadImageFromURLAsync(pURL, pPath, picAttachment4, 4, pServiceNo, false)),
                    new Func<Task>(() => clsFunction.LoadImageFromURLAsync(pURL, pPath, picSignature1, 5, pServiceNo, true)),
                    new Func<Task>(() => clsFunction.LoadImageFromURLAsync(pURL, pPath, picSignature2, 6, pServiceNo, true))
                })
                {
                    await LoadImageWithRetry(task, maxRetries: 5, delayMs: 300);
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;                
            }

            // ------------------------------------------------------
            // load sign/images     
            // ------------------------------------------------------

            initPath();

            lblHeader.Text = $"{pTitle}" + " - " + 
                            dbFunction.AddBracketStartEnd(clsSearch.ClassJobTypeDescription) + " " + 
                            dbFunction.AddBracketStartEnd($"TID-{clsSearch.ClassTID}") + " " + 
                            dbFunction.AddBracketStartEnd($"MID-{clsSearch.ClassMID}") + " " + 
                            dbFunction.AddBracketStartEnd($"Service No.-{clsSearch.ClassServiceNo}");

            tabInfo.SelectedIndex = 1;

            // store original size once when form loads
            originalPicSize = picPreview.Size;

            // make sure panel allows scrolling if image gets bigger
            pnlPreviewImage.AutoScroll = true;

            Cursor.Current = Cursors.Default;
        }

        private void frmImageTaken_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:

                    if (pnlPreviewImage.Visible)
                    {
                        pnlPreviewImage.Visible = false;
                        if (picPreview.Image != null)
                            picPreview.Image.Dispose();
                    }
                        
                    else
                        this.Close();
                    break;               
            }
        }

        private void InitAttachment()
        {
            picSignature1.Image = picSignature2.Image = picAttachment1.Image = picAttachment2.Image = picAttachment3.Image = picAttachment4.Image = null;

            clsAttachment.ClassSignature1 = clsAttachment.ClassSignature2 =
            clsAttachment.ClassAttachment1 = clsAttachment.ClassAttachment2 =
            clsAttachment.ClassAttachment3 = clsAttachment.ClassAttachment4 = clsFunction.sNull;
        }

        private void frmImageTaken_Activated(object sender, EventArgs e)
        {
            btnExit.Focus();
        }

        private void picAttachment1_Click(object sender, EventArgs e)
        {
            previewImage(picAttachment1);
        }

        private void picAttachment2_Click(object sender, EventArgs e)
        {
            previewImage(picAttachment2);
        }

        private void picAttachment3_Click(object sender, EventArgs e)
        {
            previewImage(picAttachment3);
        }

        private void picAttachment4_Click(object sender, EventArgs e)
        {
            previewImage(picAttachment4);
        }

        private void previewImage(PictureBox obj)
        {
            if (obj.Image != null)
            {
                pnlPreviewImage.Visible = true;
                pnlPreviewImage.Location = previewDefaultLocation;

                // reset zoom state whenever a new image is previewed
                currentZoom = 1.0f;
                picPreview.Size = originalPicSize;

                // reset image
                picPreview.Image = null;
                picPreview.SizeMode = PictureBoxSizeMode.Zoom;
                picPreview.Image = new Bitmap(obj.Image);

                // center it
                CenterPictureBox();
            }
            else
            {
                dbFunction.SetMessageBox(
                    "No image to be previewed.",
                    clsDefines.FIELD_CHECK_MSG,
                    clsFunction.IconType.iError
                );
            }
        }

        private void rotateImage()
        {
            Image img = picPreview.Image;
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            picPreview.Image = img;
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            rotateImage();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            pnlPreviewImage.Visible = false;

            if (picPreview.Image != null)
                picPreview.Image.Dispose();
        }

        private void picAttachment1_MouseHover(object sender, EventArgs e)
        {
            dbFunction.setToolTip(picAttachment1, "Click to enlarge picture");
        }

        private void picAttachment2_MouseHover(object sender, EventArgs e)
        {
            dbFunction.setToolTip(picAttachment2, "Click to enlarge picture");
        }

        private void picAttachment3_MouseHover(object sender, EventArgs e)
        {
            dbFunction.setToolTip(picAttachment3, "Click to enlarge picture");
        }

        private void picAttachment4_MouseHover(object sender, EventArgs e)
        {
            dbFunction.setToolTip(picAttachment4, "Click to enlarge picture");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            InitAttachment();
        }

        private void btnAttachMerchSign_Click(object sender, EventArgs e)
        {
            if (!dbFile.isValidPath(txtImportPath.Text)) return;

            pIndex = 5;
            pFileName = txtServiceNo.Text + "_" + dbFunction.padLeftChar(pIndex.ToString(), "0", 2) + clsDefines.FILE_EXT_PNG;
            dbFile.attachImage(ref poutFileName, ref poutFullPathFileName, picSignature1, txtImportPath.Text);            
            dbFile.saveImage(picSignature1, pFileName, clsDefines.FILE_EXT_PNG); // merchant
            clsAttachment.ClassSignature1 = pFileName;
        }

        private void btnAttachVendorSign_Click(object sender, EventArgs e)
        {
            if (!dbFile.isValidPath(@dbFile.sVendorSignaturePath)) return;

            pIndex = 6;
            pFileName = txtServiceNo.Text + "_" + dbFunction.padLeftChar(pIndex.ToString(), "0", 2) + clsDefines.FILE_EXT_PNG;
            dbFile.attachImage(ref poutFileName, ref poutFullPathFileName, picSignature2, @dbFile.sVendorSignaturePath);          
            dbFile.saveImage(picSignature2, pFileName, clsDefines.FILE_EXT_PNG); // vendor
            clsAttachment.ClassSignature2 = pFileName;
        }

        private void btnAttachImage1_Click(object sender, EventArgs e)
        {
            if (!dbFile.isValidPath(txtImportPath.Text)) return;

            pIndex = 1;
            pFileName = txtServiceNo.Text + "_" + dbFunction.padLeftChar(pIndex.ToString(), "0", 2) + clsDefines.FILE_EXT_JPG;
            dbFile.attachImage(ref poutFileName, ref poutFullPathFileName, picAttachment1, txtImportPath.Text);           
            dbFile.saveImage(picAttachment1, pFileName, clsDefines.FILE_EXT_JPG);
            clsAttachment.ClassAttachment1 = pFileName;
        }

        private void btnAttachImage2_Click(object sender, EventArgs e)
        {
            if (!dbFile.isValidPath(txtImportPath.Text)) return;

            pIndex = 2;
            pFileName = txtServiceNo.Text + "_" + dbFunction.padLeftChar(pIndex.ToString(), "0", 2) + clsDefines.FILE_EXT_JPG;
            dbFile.attachImage(ref poutFileName, ref poutFullPathFileName, picAttachment2, txtImportPath.Text);           
            dbFile.saveImage(picAttachment2, pFileName, clsDefines.FILE_EXT_JPG);
            clsAttachment.ClassAttachment2 = pFileName;
        }

        private void btnAttachImage3_Click(object sender, EventArgs e)
        {
            if (!dbFile.isValidPath(txtImportPath.Text)) return;

            pIndex = 3;
            pFileName = txtServiceNo.Text + "_" + dbFunction.padLeftChar(pIndex.ToString(), "0", 2) + clsDefines.FILE_EXT_JPG;
            dbFile.attachImage(ref poutFileName, ref poutFullPathFileName, picAttachment3, txtImportPath.Text);            
            dbFile.saveImage(picAttachment3, pFileName, clsDefines.FILE_EXT_JPG);
            clsAttachment.ClassAttachment3 = pFileName;
        }

        private void btnAttachImage4_Click(object sender, EventArgs e)
        {
            if (!dbFile.isValidPath(txtImportPath.Text)) return;

            pIndex = 4;
            pFileName = txtServiceNo.Text + "_" + dbFunction.padLeftChar(pIndex.ToString(), "0", 2) + clsDefines.FILE_EXT_JPG;
            dbFile.attachImage(ref poutFileName, ref poutFullPathFileName, picAttachment4, txtImportPath.Text);            
            dbFile.saveImage(picAttachment4, pFileName, clsDefines.FILE_EXT_JPG);
            clsAttachment.ClassAttachment4 = pFileName;
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            string sFile = "";
            string sRemotePath = $"{clsGlobalVariables.strFTPRemoteImagesPath}{clsSearch.ClassBankCode}/";
            string sLocalPath = dbFile.sImportPath;

            Debug.WriteLine("--btnUpload_Click--");
            Debug.WriteLine("sRemotePath="+ sRemotePath);
            Debug.WriteLine("sLocalPath="+ sLocalPath);

            if (!dbFunction.fPromptConfirmation("Are you sure to upload attached images?")) return;

            Cursor.Current = Cursors.WaitCursor;

            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);

            try
            {
                // signature - merchant           
                if (clsAttachment.ClassSignature1.Length > 0)
                {
                    sFile = clsAttachment.ClassSignature1;
                    ftpClient.delete(sRemotePath + sFile);
                    ftpClient.upload(sRemotePath + sFile, sLocalPath + sFile);
                }

                // signature - verndor          
                if (clsAttachment.ClassSignature2.Length > 0)
                {
                    sFile = clsAttachment.ClassSignature2;
                    ftpClient.delete(sRemotePath + sFile);
                    ftpClient.upload(sRemotePath + sFile, sLocalPath + sFile);
                }

                // attachemnt 1           
                if (clsAttachment.ClassAttachment1.Length > 0)
                {
                    sFile = clsAttachment.ClassAttachment1;
                    ftpClient.delete(sRemotePath + sFile);
                    ftpClient.upload(sRemotePath + sFile, sLocalPath + sFile);
                }

                // attachemnt 2            
                if (clsAttachment.ClassAttachment2.Length > 0)
                {
                    sFile = clsAttachment.ClassAttachment2;
                    ftpClient.delete(sRemotePath + sFile);
                    ftpClient.upload(sRemotePath + sFile, sLocalPath + sFile);

                }

                // attachemnt 3          
                if (clsAttachment.ClassAttachment3.Length > 0)
                {
                    sFile = clsAttachment.ClassAttachment3;
                    ftpClient.delete(sRemotePath + sFile);
                    ftpClient.upload(sRemotePath + sFile, sLocalPath + sFile);
                }

                // attachemnt 4            
                if (clsAttachment.ClassAttachment4.Length > 0)
                {
                    sFile = clsAttachment.ClassAttachment4;
                    ftpClient.delete(sRemotePath + sFile);
                    ftpClient.upload(sRemotePath + sFile, sLocalPath + sFile);
                }

                ftpClient.disconnect(); // ftp disconnect

                dbFunction.SetMessageBox("Upload images complete.", "Image taken", clsFunction.IconType.iInformation);
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Exceptional error " + ex.Message, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
            
            Cursor.Current = Cursors.Default;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string pFileName = "";
            
            if (!dbFunction.fPromptConfirmation("Are you sure to delete uploaded images?")) return;

            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // Signature
                RemoveUploadedImage(picSignature1, 5, clsDefines.FILE_EXT_PNG);
                RemoveUploadedImage(picSignature2, 6, clsDefines.FILE_EXT_PNG);

                // Attachment
                RemoveUploadedImage(picAttachment1, 1, clsDefines.FILE_EXT_JPG);
                RemoveUploadedImage(picAttachment2, 2, clsDefines.FILE_EXT_JPG);
                RemoveUploadedImage(picAttachment3, 3, clsDefines.FILE_EXT_JPG);
                RemoveUploadedImage(picAttachment4, 4, clsDefines.FILE_EXT_JPG);

                dbFunction.SetMessageBox("Delete images complete.", "Image taken", clsFunction.IconType.iInformation);
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Exceptional error " + ex.Message, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
            
            Cursor.Current = Cursors.Default;
        }

        private void btnRemoveMerchSign_Click(object sender, EventArgs e)
        {
            RemoveUploadedImage(picSignature1, 5, clsDefines.FILE_EXT_PNG);
        }

        private void btnRemoveVendorSign_Click(object sender, EventArgs e)
        {
            RemoveUploadedImage(picSignature2, 6, clsDefines.FILE_EXT_PNG);
        }

        private void btnRemoveImage1_Click(object sender, EventArgs e)
        {
            RemoveUploadedImage(picAttachment1, 1, clsDefines.FILE_EXT_JPG);
        }

        private void btnRemoveImage2_Click(object sender, EventArgs e)
        {
            RemoveUploadedImage(picAttachment2, 2, clsDefines.FILE_EXT_JPG);
        }

        private void btnRemoveImage3_Click(object sender, EventArgs e)
        {
            RemoveUploadedImage(picAttachment3, 3, clsDefines.FILE_EXT_JPG);
        }

        private void btnRemoveImage4_Click(object sender, EventArgs e)
        {
            RemoveUploadedImage(picAttachment4, 4, clsDefines.FILE_EXT_JPG);
        }

        private void RemoveUploadedImage(PictureBox obj, int pIndex, string pFileExtension)
        {
            string pFileName = "";
            string sRemotePath = $"{clsGlobalVariables.strFTPRemoteImagesPath}{clsSearch.ClassBankCode}/";

            ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (obj.Image != null)
                {
                    pFileName = txtServiceNo.Text + "_" + dbFunction.padLeftChar(pIndex.ToString(), "0", 2) + pFileExtension;
                    ftpClient.delete(sRemotePath + pFileName);

                    obj.Image = null;
                }

                //dbFunction.SetMessageBox("Delete images complete.", "Image taken", clsFunction.IconType.iInformation);

            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Exceptional error " + ex.Message, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
            
            Cursor.Current = Cursors.Default;
        }

        private void downloadImage(PictureBox obj, int index, bool isSignature)
        {
            string sFile = txtServiceNo.Text + "_" + dbFunction.padLeftChar(index.ToString(), "0", 2) + (isSignature ? clsDefines.FILE_EXT_PNG : clsDefines.FILE_EXT_JPG);
            string sRemotePath = $"{clsGlobalVariables.strFTPRemoteImagesPath}{clsSearch.ClassBankCode}/";
            string sLocaPath = dbFile.sDowloadPath;

            try
            {
                if (obj.Image != null)
                {
                    if (!dbFunction.fPromptConfirmation("Are you sure to download " + (isSignature ? "attachment#" : "signature#") + index + "?")) return;

                    Cursor.Current = Cursors.WaitCursor;

                    ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                    ftpClient.download(sRemotePath + sFile, sLocaPath + sFile);
                    ftpClient.disconnect();

                    Cursor.Current = Cursors.Default;

                    if (isSignature)
                        dbFunction.SetMessageBox("Image download complete." +
                        "\n\n" +
                        "Download path " + dbFunction.AddBracketStartEnd(sLocaPath + sFile), clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
                    else
                        dbFunction.SetMessageBox("Signature download complete." +
                        "\n\n" +
                        "Download path " + dbFunction.AddBracketStartEnd(sLocaPath + sFile), clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

                }
                else
                {
                    if (isSignature)
                        dbFunction.SetMessageBox("No signature to be download.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                    else
                        dbFunction.SetMessageBox("No image to be download.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
                }
            }
            catch (Exception ex)
            {
                dbFunction.SetMessageBox("Exceptional error " + ex.Message, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
            
        }

        private void btnDownloadImage1_Click(object sender, EventArgs e)
        {
            downloadImage(picAttachment1, clsDefines.ATTACHMENT_1_INDEX, false);
        }

        private void btnDownloadImage2_Click(object sender, EventArgs e)
        {
            downloadImage(picAttachment2, clsDefines.ATTACHMENT_2_INDEX, false);
        }

        private void btnDownloadImage3_Click(object sender, EventArgs e)
        {
            downloadImage(picAttachment3, clsDefines.ATTACHMENT_3_INDEX, false);
        }

        private void btnDownloadImage4_Click(object sender, EventArgs e)
        {
            downloadImage(picAttachment4, clsDefines.ATTACHMENT_4_INDEX, false);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            frmImageTaken_Load(this, e);
        }

        private void btnDownloadMerchantSign_Click(object sender, EventArgs e)
        {
            downloadImage(picSignature1, clsDefines.MERCHANT_SIGNATURE_INDEX, true);
        }

        private void btnDownloadVendorSign_Click(object sender, EventArgs e)
        {
            downloadImage(picSignature2, clsDefines.VENDOR_SIGNATURE_INDEX, true);
        }
        
        private void btnImagePath_Click(object sender, EventArgs e)
        {
            txtImportPath.Text = dbFile.openFolderDialog();
        }

        private void initPath()
        {
            txtImportPath.Text = clsGlobalVariables.strLocalSignPath;
        }
        
        private void btnSaveImagePath_Click(object sender, EventArgs e)
        {
            savePath();
        }

        private void savePath()
        {
            if (dbFunction.isValidDescription(txtImportPath.Text) && dbFunction.isValidDescription(txtImportPath.Text))
            {
                if (!dbFile.isValidPath(txtImportPath.Text)) return;

                if (!dbFile.isValidPath(txtImportPath.Text)) return;
                
                clsGlobalVariables.strLocalSignPath = txtImportPath.Text;
                
                dbINI.WriteFTPSetting();

                dbFunction.SetMessageBox("Path save successfully.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);

            }
            else
            {
                dbFunction.SetMessageBox("Sign and image path must not be blank.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

        }

        private void btnPlusSize_Click(object sender, EventArgs e)
        {
            picPreview.Size = new Size(picPreview.Width + increment, picPreview.Height + increment);
        }

        private void btnMinusSize_Click(object sender, EventArgs e)
        {
            picPreview.Size = new Size(picPreview.Width - increment, picPreview.Height - increment);
        }
        
        private void picPreview_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartPoint = e.Location;
            }
        }

        private void picPreview_MouseUp(object sender, MouseEventArgs e)
        {   
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        private void picPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Calculate the new position
                Point newLocation = picPreview.Location;
                newLocation.Offset(e.Location.X - dragStartPoint.X, e.Location.Y - dragStartPoint.Y);
                picPreview.Location = newLocation;
            }
        }

        private void Form_MouseWheel(object sender, MouseEventArgs e)
        {
            if (picPreview.Image == null) return;

            if (e.Delta > 0) // zoom in
            {
                currentZoom *= zoomFactor;
            }
            else if (e.Delta < 0) // zoom out
            {
                currentZoom /= zoomFactor;
                if (currentZoom < 0.1f) currentZoom = 0.1f;
            }

            // scale relative to the original size
            picPreview.Width = (int)(originalPicSize.Width * currentZoom);
            picPreview.Height = (int)(originalPicSize.Height * currentZoom);

            // recenter after zoom
            CenterPictureBox();
        }

        private void picAttachment1_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the dragged item is a file
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void picAttachment1_DragDrop(object sender, DragEventArgs e)
        {
            dragDropImage(picAttachment1, sender, e);
        }

        private void picAttachment2_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the dragged item is a file
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void picAttachment2_DragDrop(object sender, DragEventArgs e)
        {
            dragDropImage(picAttachment2, sender, e);
        }

        private void picAttachment3_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the dragged item is a file
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void picAttachment3_DragDrop(object sender, DragEventArgs e)
        {
            dragDropImage(picAttachment3, sender, e);
        }

        private void picAttachment4_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the dragged item is a file
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void picAttachment4_DragDrop(object sender, DragEventArgs e)
        {
            dragDropImage(picAttachment4, sender, e);
        }

        private void picSignature1_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the dragged item is a file
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void picSignature1_DragDrop(object sender, DragEventArgs e)
        {
            dragDropImage(picSignature1, sender, e);
        }

        private void picSignature2_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the dragged item is a file
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void picSignature2_DragDrop(object sender, DragEventArgs e)
        {
            dragDropImage(picSignature2, sender, e);
        }

        private void dragDropImage(PictureBox obj, object sender, DragEventArgs e)
        {
            // Get the dropped file paths
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Load the first image file into the PictureBox
            if (files.Length > 0)
            {
                try
                {
                    // Load the image and set it to PictureBox
                    Image img = Image.FromFile(files[0]);
                    obj.Image = img;

                    // Ensure the image resizes properly inside the PictureBox
                    obj.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid file format. Please drop an image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Helper method to check if mouse is over the PictureBox
        private bool IsMouseOverControl(Control control)
        {
            Point clientPos = control.PointToClient(Cursor.Position);
            return clientPos.X >= 0 && clientPos.X < control.Width && clientPos.Y >= 0 && clientPos.Y < control.Height;
        }

        private void frmImageTaken_DragEnter(object sender, DragEventArgs e)
        {
            // Allow only files to be dropped
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // Change cursor to indicate drop is allowed
            }
        }

        private void frmImageTaken_DragDrop(object sender, DragEventArgs e)
        {
            // Get the dropped file paths
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // If there are files, and the mouse is over the PictureBox, load the first one
            if (files.Length > 0)
            {
                if (IsMouseOverControl(picSignature1))
                    dragDropImage(picSignature1, sender, e);

                else if (IsMouseOverControl(picSignature2))
                    dragDropImage(picSignature2, sender, e);

                else if (IsMouseOverControl(picAttachment1))
                    dragDropImage(picAttachment1, sender, e);

                else if (IsMouseOverControl(picAttachment2))
                    dragDropImage(picAttachment2, sender, e);

                else if (IsMouseOverControl(picAttachment3))
                    dragDropImage(picAttachment3, sender, e);

                else if (IsMouseOverControl(picAttachment4))
                    dragDropImage(picAttachment4, sender, e);

            }
        }

        private void btnEmptyImport_Click(object sender, EventArgs e)
        {   
            if (!dbFunction.fPromptConfirmation($"Are you sure you want to permanently delete all contents of:\n\n{txtImportPath.Text}?")) return;

            dbFile.emptyFolder(txtImportPath.Text);
        }

        private void CenterPictureBox()
        {
            if (picPreview.Parent != null)
            {
                int x = Math.Max((pnlPreviewImage.ClientSize.Width - picPreview.Width) / 2, 0);
                int y = Math.Max((pnlPreviewImage.ClientSize.Height - picPreview.Height) / 2, 0);

                picPreview.Location = new Point(x, y);
            }
        }

        private async Task LoadImageWithRetry(Func<Task> loadTask, int maxRetries = 3, int delayMs = 500, Control control = null)
        {
            int attempt = 0;

            // wait until control handle is created
            if (control != null)
            {
                while (!control.IsHandleCreated)
                    await Task.Delay(50);
            }

            while (true)
            {
                try
                {
                    await loadTask();
                    break;
                }
                catch (Exception ex)
                {
                    attempt++;
                    if (attempt > maxRetries)
                    {
                        Console.WriteLine($"Image load failed after {attempt} attempts: {ex.Message}");
                        break;
                    }

                    await Task.Delay(delayMs);
                }
            }
        }

    }
}
