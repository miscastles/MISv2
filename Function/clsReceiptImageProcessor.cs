using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Tesseract;
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
            string pOCRDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OCRData");
            string pEnglishDataPath = Path.Combine(pOCRDataPath, "eng.traineddata");

            if (!File.Exists(pEnglishDataPath)) throw new FileNotFoundException("Tesseract data could not be found.", pEnglishDataPath);

            using (Mat sourceImage = Cv2.ImRead(pImagePath, ImreadModes.Color))
            using (Mat processedImage = new Mat())
            {
                if (sourceImage.Empty()) throw new Exception("OpenCV could not read the receipt image.");

                CreateProcessedReceipt(sourceImage, processedImage);

                byte[] pImageData;

                Cv2.ImEncode(".png", processedImage, out pImageData);

                using (TesseractEngine ocrEngine = new TesseractEngine(pOCRDataPath, "eng", EngineMode.LstmOnly))
                using (Pix receiptImage = Pix.LoadFromMemory(pImageData))
                using (Page ocrPage = ocrEngine.Process(receiptImage, PageSegMode.Auto))
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
    }
}
