using CrystalDecisions.ReportAppServer.DataDefModel;
using MIS.Function;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Tesseract;
using System.Linq;
using static MIS.AppData.ConstData.Api.ReceiptOCR;

namespace MIS
{
    public enum ReceiptPreviewMode
    {
        Original,
        Grayscale,
        Otsu,
        Adaptive
    }
    public class clsReceiptImageProcessor
    {
        private clsFunction dbFunction = new clsFunction();
        private clsFile dbFile = new clsFile();

        // preprocess image increase accuracy
        public Bitmap CreatePreview(string pImagePath, ReceiptPreviewMode previewMode)
        {
            using (Mat sourceImage = Cv2.ImRead(pImagePath, ImreadModes.Color))
            {
                if (!dbFunction.isFileExists(pImagePath))
                {
                    dbFunction.SetMessageBox(
                        $"File is missing check file path: {pImagePath}",
                        "Permission",
                        clsFunction.IconType.iError
                    );

                    throw new Exception("Missing file at filepath");
                }

                if (sourceImage.Empty()) throw new Exception("Could not read the receipt image.");

                using (Mat previewImage = new Mat())
                {
                    switch (previewMode)
                    {
                        case ReceiptPreviewMode.Grayscale:
                            CreateGrayscale(sourceImage, previewImage);
                            break;

                        case ReceiptPreviewMode.Otsu:
                            CreateOtsuBinary(sourceImage, previewImage);
                            break;

                        case ReceiptPreviewMode.Adaptive:
                            CreateAdaptiveBinary(sourceImage, previewImage);
                            break;

                        default:
                            sourceImage.CopyTo(previewImage);
                            break;
                    }

                    return BitmapConverter.ToBitmap(previewImage);
                }
            }
        }

        private void CreateGrayscale(Mat sourceImage, Mat outputImage)
        {
            Cv2.CvtColor(sourceImage, outputImage, ColorConversionCodes.BGR2GRAY);
        }

        private void CreateOtsuBinary(Mat sourceImage, Mat outputImage)
        {
            using (Mat grayImage = new Mat())
            using (Mat blurredImage = new Mat())
            {
                CreateGrayscale(sourceImage, grayImage);

                Cv2.GaussianBlur(grayImage, blurredImage, new OpenCvSharp.Size(3, 3), 0);
                Cv2.Threshold(blurredImage, outputImage, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
            }
        }

        private void CreateAdaptiveBinary(Mat sourceImage, Mat outputImage)
        {
            using (Mat grayImage = new Mat())
            using (Mat blurredImage = new Mat())
            {
                CreateGrayscale(sourceImage, grayImage);

                Cv2.GaussianBlur(grayImage, blurredImage, new OpenCvSharp.Size(3, 3), 0);
                Cv2.AdaptiveThreshold(blurredImage, outputImage, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 31, 15);
            }
        }

        // tesseract ocr

        public string ExtractText(string pImagePath)
        {
            string pOCRDataPath = dbFile.sOCRDataPath;
            string pEnglishDataPath = Path.Combine(
                pOCRDataPath,
                "eng.traineddata"
            );

            if (!Directory.Exists(pOCRDataPath))
            {
                throw new DirectoryNotFoundException(
                    $"Tesseract OCRData folder was not found.\n\n" +
                    $"Path: {pOCRDataPath}"
                );
            }

            if (!File.Exists(pEnglishDataPath))
            {
                throw new FileNotFoundException(
                    $"Tesseract data could not be found.\n\n" +
                    $"File: {pEnglishDataPath}",
                    pEnglishDataPath
                );
            }

            using (Mat sourceImage = Cv2.ImRead(
                pImagePath,
                ImreadModes.Color))
            using (Mat processedImage = new Mat())
            {
                if (sourceImage.Empty())
                {
                    throw new Exception(
                        $"OpenCV could not read the receipt image.\n\n" +
                        $"File: {pImagePath}"
                    );
                }

                CreateProcessedReceipt(
                    sourceImage,
                    processedImage
                );

                byte[] pImageData;

                Cv2.ImEncode(
                    ".png",
                    processedImage,
                    out pImageData
                );

                using (TesseractEngine ocrEngine =
                    new TesseractEngine(
                        pOCRDataPath,
                        "eng",
                        EngineMode.LstmOnly))
                using (Pix receiptImage =
                    Pix.LoadFromMemory(pImageData))
                using (Page ocrPage =
                    ocrEngine.Process(
                        receiptImage,
                        PageSegMode.Auto))
                {
                    return ocrPage.GetText();
                }
            }
        }

        private void CreateProcessedReceipt(Mat sourceImage, Mat outputImage)
        {
            using (Mat grayscaleImage = new Mat())
            using (Mat enlargedImage = new Mat())
            {
                CreateGrayscale(sourceImage, grayscaleImage);

                double dScale = 2000D / grayscaleImage.Height;

                if (dScale < 1D) dScale = 1D;

                Cv2.Resize(
                    grayscaleImage,
                    enlargedImage,
                    new OpenCvSharp.Size(),
                    dScale,
                    dScale,
                    InterpolationFlags.Cubic
                );

                Cv2.Threshold(enlargedImage, outputImage, 0,
                    255,
                    ThresholdTypes.Binary | ThresholdTypes.Otsu
                );

                Debug.WriteLine(
                    "OCR image size: " +
                    outputImage.Width + "x" +
                    outputImage.Height
                );
            }
        }

        public decimal? ExtractTransactionAmount(string pOCRText)
        {
            if (string.IsNullOrWhiteSpace(pOCRText)) return null;

            Match amountMatch = Regex.Match(
                pOCRText,
                @"(?:PHP|PESO(?:S)?|P|₱)\s*[:\-]?\s*" +
                @"([0-9][0-9,\s]*\.\s*[0-9]{2})",
                RegexOptions.IgnoreCase
            );

            if (!amountMatch.Success) return null;

            string pAmount = amountMatch.Groups[1].Value.Replace(" ", "").Replace(",", "");

            decimal dAmount;

            if (decimal.TryParse(pAmount, NumberStyles.Number, CultureInfo.InvariantCulture, out dAmount)) return dAmount;

            return null;
        }

        public string ExtractTransactionDate(string pOCRText)
        {
            string[] pTransactionDateText = GetReceiptOCRText("TransactionDate");

            string[] pOCRLines = pOCRText.Split(
                new string[] { "\r\n", "\n", "\r" },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string pDateText in pTransactionDateText)
            {
                string pNormalizedDateText = NormalizeOCRSearchText(pDateText);

                for (int i = 0; i < pOCRLines.Length; i++)
                {
                    string pNormalizedLine = NormalizeOCRSearchText(pOCRLines[i]);

                    if (!pNormalizedLine.Contains(pNormalizedDateText))
                        continue;

                    string pTextToCheck = pOCRLines[i];

                    if (i + 1 < pOCRLines.Length)
                    {
                        pTextToCheck += " " + pOCRLines[i + 1];
                    }

                    string pTransactionDate = ExtractFirstVisibleDate(pTextToCheck);

                    if (!string.IsNullOrWhiteSpace(pTransactionDate))
                        return pTransactionDate;
                }
            }

            // Fallback: search the complete OCR text
            return ExtractFirstVisibleDate(pOCRText);
        }

        private string[] GetReceiptOCRText(string pPropertyName)
        {
            try
            {
                string pConfigPath =
                    Path.Combine(
                        dbFile.sOCRDataPath,
                        "receiptOCR.json"
                    );

                if (!File.Exists(pConfigPath))
                {
                    Debug.WriteLine(
                        "Receipt OCR configuration was not found: " +
                        pConfigPath
                    );

                    return new string[0];
                }

                JObject pOCRConfig =
                    JObject.Parse(
                        File.ReadAllText(pConfigPath)
                    );

                JArray pSearchText =
                    pOCRConfig[pPropertyName] as JArray;

                if (pSearchText == null)
                    return new string[0];

                return pSearchText
                    .Select(pValue => pValue.ToString())
                    .Where(pValue => !string.IsNullOrWhiteSpace(pValue))
                    .ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Unable to read receipt OCR configuration: " +
                    ex
                );

                return new string[0];
            }
        }

        private string NormalizeOCRSearchText(
            string pText)
        {
            if (string.IsNullOrWhiteSpace(pText))
                return clsDefines.gNull;

            return Regex.Replace(
                pText.ToUpperInvariant(),
                @"[^A-Z0-9]+",
                " "
            ).Trim();
        }

        private string ExtractFirstVisibleDate(
            string pText)
        {
            if (string.IsNullOrWhiteSpace(pText))
                return clsDefines.gNull;

            string pMonthText =
                @"(?:JAN(?:UARY)?|" +
                @"FEB(?:RUARY)?|" +
                @"MAR(?:CH)?|" +
                @"APR(?:IL)?|" +
                @"MAY|" +
                @"JUN(?:E)?|" +
                @"JUL(?:Y)?|" +
                @"AUG(?:UST)?|" +
                @"SEP(?:TEMBER)?|" +
                @"OCT(?:OBER)?|" +
                @"NOV(?:EMBER)?|" +
                @"DEC(?:EMBER)?)";

            string pDatePattern =
                @"\b(?:" +
                @"(?:19|20)\d{2}[-/.]\d{1,2}[-/.]\d{1,2}|" +
                @"\d{1,2}[-/.]\d{1,2}[-/.](?:\d{2}|\d{4})|" +
                pMonthText +
                @"\s+\d{1,2},?\s+(?:\d{2}|\d{4})|" +
                @"\d{1,2}\s+" +
                pMonthText +
                @"\s+(?:\d{2}|\d{4})" +
                @")\b";

            Match pDateMatch = Regex.Match(pText, pDatePattern, RegexOptions.IgnoreCase);

            return NormalizeTransactionDate(pDateMatch.Value.Trim());
        }

        private string NormalizeTransactionDate(string pReceiptDate)
        {
            if (string.IsNullOrWhiteSpace(pReceiptDate)) return clsDefines.gNull;

            string[] pDateFormats = GetReceiptOCRText("TransactionDateFormats");

            DateTime dReceiptDate;

            bool fDateParsed = DateTime.TryParseExact(
                    pReceiptDate.Trim(),
                    pDateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out dReceiptDate
                );

            return dReceiptDate.ToString(clsFunction.sValueDateFormat);
        }
    }
}
