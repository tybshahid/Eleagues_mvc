using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cricket.Utilities
{
    public static class FileUploadCheck
    {

        #region " Validations for File Types"

        private enum ImageFileExtension
        {
            none = 0,
            jpg = 1,
            jpeg = 2,
            bmp = 3,
            gif = 4,
            png = 5
        }

        private enum VideoFileExtension
        {
            none = 0,
            wmv = 1,
            mpg = 2,
            mpeg = 3,
            mp4 = 4,
            avi = 5,
            flv = 6
        }

        private enum PDFFileExtension
        {
            none = 0,
            PDF = 1
        }

        public enum FileType
        {
            Image = 1,
            Video = 2,
            PDF = 3,
            Text = 4,
            DOC = 5,
            DOCX = 6,
            PPT = 7,
        }

        public static bool XLMimeType(string MimeType, string ext)
        {
            bool isValid = false;

            if (MimeType == "application/x-msexcel" && (ext == ".xlsx" || ext == ".xls"))
            {
                isValid = true;
            }
            else if (MimeType == "application/x-excel" && (ext == ".xlsx" || ext == ".xls"))
            {
                isValid = true;
            }
            else if (MimeType == "application/vnd.ms-excel" && (ext == ".xlsx" || ext == ".xls"))
            {
                isValid = true;
            }
            else if (MimeType == "application/excel" && (ext == ".xlsx" || ext == ".xls"))
            {
                isValid = true;
            }

            return isValid;
        }

        public static bool isValidFile(byte[] bytFile, FileType flType, String FileContentType)
        {
            bool isvalid = false;

            if (flType == FileType.Image)
            {
                isvalid = isValidImageFile(bytFile, FileContentType);
            }
            else if (flType == FileType.Video)
            {
                isvalid = isValidVideoFile(bytFile, FileContentType);
            }
            else if (flType == FileType.PDF)
            {
                isvalid = isValidPDFFile(bytFile, FileContentType);
            }

            return isvalid;


        }

        public static bool isValidImageFile(byte[] bytFile, String FileContentType)
        {
            bool isvalid = false;

            byte[] chkBytejpg = { 255, 216, 255, 224 };
            byte[] chkBytebmp = { 66, 77 };
            byte[] chkBytegif = { 71, 73, 70, 56 };
            byte[] chkBytepng = { 137, 80, 78, 71 };


            ImageFileExtension imgfileExtn = ImageFileExtension.none;

            if (FileContentType.Contains("jpg") | FileContentType.Contains("jpeg"))
            {
                imgfileExtn = ImageFileExtension.jpg;
            }
            else if (FileContentType.Contains("png"))
            {
                imgfileExtn = ImageFileExtension.png;
            }
            else if (FileContentType.Contains("bmp"))
            {
                imgfileExtn = ImageFileExtension.bmp;
            }
            else if (FileContentType.Contains("gif"))
            {
                imgfileExtn = ImageFileExtension.gif;
            }

            if (imgfileExtn == ImageFileExtension.jpg || imgfileExtn == ImageFileExtension.jpeg)
            {
                if (bytFile.Length >= 4)
                {
                    int j = 0;
                    for (Int32 i = 0; i <= 3; i++)
                    {
                        if (bytFile[i] == chkBytejpg[i])
                        {
                            j = j + 1;
                            if (j == 3)
                            {
                                isvalid = true;
                            }
                        }
                    }
                }
            }


            if (imgfileExtn == ImageFileExtension.png)
            {
                if (bytFile.Length >= 4)
                {
                    int j = 0;
                    for (Int32 i = 0; i <= 3; i++)
                    {
                        if (bytFile[i] == chkBytepng[i])
                        {
                            j = j + 1;
                            if (j == 3)
                            {
                                isvalid = true;
                            }
                        }
                    }
                }
            }


            if (imgfileExtn == ImageFileExtension.bmp)
            {
                if (bytFile.Length >= 4)
                {
                    int j = 0;
                    for (Int32 i = 0; i <= 1; i++)
                    {
                        if (bytFile[i] == chkBytebmp[i])
                        {
                            j = j + 1;
                            if (j == 2)
                            {
                                isvalid = true;
                            }
                        }
                    }
                }
            }

            if (imgfileExtn == ImageFileExtension.gif)
            {
                if (bytFile.Length >= 4)
                {
                    int j = 0;
                    for (Int32 i = 0; i <= 1; i++)
                    {
                        if (bytFile[i] == chkBytegif[i])
                        {
                            j = j + 1;
                            if (j == 3)
                            {
                                isvalid = true;
                            }
                        }
                    }
                }
            }

            return isvalid;
        }

        private static bool isValidVideoFile(byte[] bytFile, String FileContentType)
        {
            byte[] chkBytewmv = { 48, 38, 178, 117 };
            byte[] chkByteavi = { 82, 73, 70, 70 };
            byte[] chkByteflv = { 70, 76, 86, 1 };
            byte[] chkBytempg = { 0, 0, 1, 186 };
            byte[] chkBytemp4 = { 0, 0, 0, 20 };
            bool isvalid = false;

            VideoFileExtension vdofileExtn = VideoFileExtension.none;
            if (FileContentType.Contains("wmv"))
            {
                vdofileExtn = VideoFileExtension.wmv;
            }
            else if (FileContentType.Contains("mpg") || FileContentType.Contains("mpeg"))
            {
                vdofileExtn = VideoFileExtension.mpg;
            }
            else if (FileContentType.Contains("mp4"))
            {
                vdofileExtn = VideoFileExtension.mp4;
            }
            else if (FileContentType.Contains("avi"))
            {
                vdofileExtn = VideoFileExtension.avi;
            }
            else if (FileContentType.Contains("flv"))
            {
                vdofileExtn = VideoFileExtension.flv;
            }

            if (vdofileExtn == VideoFileExtension.wmv)
            {
                if (bytFile.Length >= 4)
                {
                    int j = 0;
                    for (Int32 i = 0; i <= 3; i++)
                    {
                        if (bytFile[i] == chkBytewmv[i])
                        {
                            j = j + 1;
                            if (j == 3)
                            {
                                isvalid = true;
                            }
                        }
                    }
                }
            }
            else if ((vdofileExtn == VideoFileExtension.mpg || vdofileExtn == VideoFileExtension.mpeg) & isvalid)
            {
                if (bytFile.Length >= 4)
                {
                    int j = 0;
                    for (Int32 i = 0; i <= 3; i++)
                    {
                        if (bytFile[i] == chkBytempg[i])
                        {
                            j = j + 1;
                            if (j == 3)
                            {
                                isvalid = true;
                            }
                        }
                    }
                }
            }
            else if (vdofileExtn == VideoFileExtension.mp4 & isvalid)
            {
                if (bytFile.Length >= 4)
                {
                    int j = 0;
                    for (Int32 i = 0; i <= 3; i++)
                    {
                        if (bytFile[i] == chkBytemp4[i])
                        {
                            j = j + 1;
                            if (j == 3)
                            {
                                isvalid = true;
                            }
                        }
                    }
                }
            }
            else if (vdofileExtn == VideoFileExtension.avi & isvalid)
            {
                if (bytFile.Length >= 4)
                {
                    int j = 0;
                    for (Int32 i = 0; i <= 3; i++)
                    {
                        if (bytFile[i] == chkByteavi[i])
                        {
                            j = j + 1;
                            if (j == 3)
                            {
                                isvalid = true;
                            }
                        }
                    }
                }
            }
            else if (vdofileExtn == VideoFileExtension.flv & isvalid)
            {
                if (bytFile.Length >= 4)
                {
                    int j = 0;
                    for (Int32 i = 0; i <= 3; i++)
                    {
                        if (bytFile[i] == chkByteflv[i])
                        {
                            j = j + 1;
                            if (j == 3)
                            {
                                isvalid = true;
                            }
                        }
                    }
                }
            }

            return isvalid;

        }

        public static bool isValidPDFFile(byte[] bytFile, String FileContentType)
        {
            byte[] chkBytepdf = { 37, 80, 68, 70 };
            bool isvalid = false;

            PDFFileExtension pdffileExtn = PDFFileExtension.none;
            if (FileContentType.Contains("pdf"))
            {
                pdffileExtn = PDFFileExtension.PDF;
            }

            if (pdffileExtn == PDFFileExtension.PDF)
            {
                if (bytFile.Length >= 4)
                {
                    int j = 0;
                    for (Int32 i = 0; i <= 3; i++)
                    {
                        if (bytFile[i] == chkBytepdf[i])
                        {
                            j = j + 1;
                            if (j == 3)
                            {
                                isvalid = true;
                            }
                        }
                    }
                }
            }

            return isvalid;
        }

        public static bool isValidDimension(byte[] bytFile, int maxRequiredWidth, int maxRequiredHeight)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(bytFile), true, true))
            {
                if (image.Width == maxRequiredWidth && image.Height == maxRequiredHeight)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        #endregion

    }
}