using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cricket.Utilities
{
    public class ImageHelper
    {
        public string Upload(HttpPostedFileBase upload)
        {
            try
            {
                string fileName = upload.FileName; // getting File Name
                string fileContentType = upload.ContentType; // getting ContentType
                byte[] tempFileBytes = new byte[upload.ContentLength]; // getting filebytes
                var data = upload.InputStream.Read(tempFileBytes, 0, Convert.ToInt32(upload.ContentLength));
                var types = FileUploadCheck.FileType.Image;  // Setting Image type
                var result = FileUploadCheck.isValidFile(tempFileBytes, types, fileContentType); // Validate Header

                if (result == true)
                {
                    int FileLength = 1024 * 1024 * 6; //FileLength 6 MB 
                    if (upload.ContentLength > FileLength)
                    {
                        return "Error - Maximum allowed size is: " + FileLength + " MB";
                    }
                }
                else
                {
                    return "Error";
                }
            }
            catch
            {
                return "Error";
            }

            return "Success";
        }
    }
}