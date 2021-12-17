using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Tracer.API.Helper.AppSetting;

namespace Tracer.API.Helper
{
    public class SaveDocumentsOnServer
    {
        #region SaveDocOnServer
        public async Task<string> SaveDocOnServer(List<IFormFile> files, IFormFile Formfile, string DocPath, string Refid, string FileType)
        {
            string SaveAsFileName = "";
            string Ext = "";
            if (files != null)
            {
                foreach (var file in files)
                {
                    Ext = System.IO.Path.GetExtension(file.FileName);
                }

                if (Ext == ".jpg" || Ext == ".gif" || Ext == ".bmp" || Ext == ".png" || Ext == ".jpeg" || Ext == ".JPG" || Ext == ".JPEG" || Ext == ".GIF" || Ext == ".BMP" || Ext == ".PNG")
                {
                    SaveAsFileName = await GeneratePDFFromImage(files, null, DocPath, Refid, FileType);
                }
                else
                {
                    foreach (var file in files)
                    {
                        SaveAsFileName = CopyFileOnServer(file, DocPath, Refid, FileType);
                    }
                }
            }

            else
            {
                Ext = System.IO.Path.GetExtension(Formfile.FileName);

                if (Ext == ".jpg" || Ext == ".gif" || Ext == ".bmp" || Ext == ".png" || Ext == ".jpeg" || Ext == ".JPG" || Ext == ".JPEG" || Ext == ".GIF" || Ext == ".BMP" || Ext == ".PNG")
                {
                    SaveAsFileName = await GeneratePDFFromImage(null, Formfile, DocPath, Refid, FileType);
                }
                else
                {
                    SaveAsFileName = CopyFileOnServer(Formfile, DocPath, Refid, FileType);
                }
            }
            return SaveAsFileName;

        }
        #endregion

        #region GeneratePDFFromImage
        /// <summary>
        /// Generates the PDF from image.
        /// </summary>
        /// <param name="Files">The files.</param>
        /// <param name="File">The file.</param>
        /// <param name="DocPath">The document path.</param>
        /// <param name="refid">The refid.</param>
        /// <param name="strFileType">Type of the string file.</param>
        /// <returns>GeneratePDFFromImage</returns>
        public async Task<string> GeneratePDFFromImage(List<IFormFile> Files, IFormFile File, string DocPath, string refid, string strFileType)
        {
            string TempFileName = System.DateTime.Now.Ticks.ToString() + "_" + "Temp";
            string Base64Image = "";
            string SaveAsFileName = "";
            if (Files != null && Files.Count > 0)
            {
                foreach (var uploadedFile in Files)
                {
                    //   Base64Image = ConvertPostedFileToBase64(uploadedFile, TempFileName);
                    Base64Image = await ConvertToBase64(uploadedFile, TempFileName);
                }
            }
            else
            {
                if (Files == null)
                {
                    //  Base64Image = ConvertPostedFileToBase64(File, TempFileName);
                    Base64Image = await ConvertToBase64(File, TempFileName);
                }
            }
            // MakeDirectoryIfNotExists(temppath);
            string[] strsplitted = Base64Image.Split('|');
            Document doc = new Document(PageSize.A4, 50, 50, 25, 25);
            try
            {
                MemoryStream memStream = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(doc, memStream);
                doc.Open();
                for (int i = 0; i < strsplitted.Length; i++)
                {
                    if (strsplitted[i] != "")
                    {
                        byte[] imagebyte = Convert.FromBase64String(strsplitted[i]);
                        doc.Add(new Paragraph(""));
                        iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imagebyte);
                        png.ScaleToFit(500f, 842f);
                        doc.Add(png);
                    }
                }
                doc.Close();
                byte[] PDFInBytes = memStream.ToArray();
                SaveAsFileName = CopyByteArrayAsPDFOnServer(PDFInBytes, DocPath, refid, strFileType, true);
            }
            catch (Exception ex)
            {

            }
            return SaveAsFileName;
        }

        #endregion

        #region ConvertPostedFileToBase64 Old
        /// <summary>
        /// Converts the posted file to base64.
        /// </summary>
        /// <param name="PostedFile">The posted file.</param>
        /// <param name="TempFileName">Name of the temporary file.</param>
        /// <returns></returns>
        public string ConvertPostedFileToBase64(IFormFile PostedFile, string TempFileName)
        {

            try
            {

                string Base64Image = "";
                string dirPath = @"wwwroot\images";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", PostedFile.FileName);
                bool exists = System.IO.Directory.Exists(dirPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(dirPath);

                using (var fs = new FileStream(filePath, FileMode.Create))
                // using (var fs = new FileStream(PostedFile.FileName, FileMode.Create))
                {
                    PostedFile.CopyToAsync(fs);

                    // System.IO.Stream fs = PostedFile.FileName.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    MemoryStream CompressedmemStream = new MemoryStream();
                    TempFileName += Path.GetExtension(PostedFile.FileName);
                    System.Drawing.Image bm_Large = System.Drawing.Image.FromStream(fs);
                    bm_Large = ResizeBitmap((Bitmap)bm_Large, bm_Large.Width, bm_Large.Height);
                    bm_Large.Save(CompressedmemStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    fs.Close();
                    Byte[] bytes = CompressedmemStream.ToArray();
                    Base64Image += Convert.ToBase64String(bytes, 0, bytes.Length) + "|";
                    CompressedmemStream.Dispose();
                    return Base64Image;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region ConvertToBase64
        /// <summary>
        /// Converts to base64.
        /// </summary>
        /// <param name="PostedFile">The posted file.</param>
        /// <param name="TempFileName">Name of the temporary file.</param>
        /// <returns></returns>
        public async Task<string> ConvertToBase64(IFormFile PostedFile, string TempFileName)
        {
            try
            {
                string Base64Image = string.Empty;
                string dirPath = @"wwwroot\images";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", PostedFile.FileName);
                bool exists = System.IO.Directory.Exists(dirPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(dirPath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PostedFile.CopyToAsync(stream);
                }

                Byte[] bytes = File.ReadAllBytes(filePath);
                String file = Convert.ToBase64String(bytes);
                return file;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion

        #region ResizeBitmap
        public static Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            //result.SetResolution(30, 30);
            return result;
        }
        #endregion

        #region CopyByteArrayAsPDFOnServer
        public string CopyByteArrayAsPDFOnServer(Byte[] ByteArray, string strFilePath, string strRefno, string strFileType, bool isTickRequired)
        {
            try
            {
                string strTimeTicks = "";
                String Ext = "";
                //Connect to Azure
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsConfig.AppSetting("CloudDetails:StorageConnectionString"));

                // Create a reference to the file client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                //The next 2 lines create if not exists a container named "democontainer"
                CloudBlobContainer container = blobClient.GetContainerReference(SettingsConfig.AppSetting("CloudDetails:CloudSharedFolderName"));

                strTimeTicks = System.DateTime.Now.Ticks.ToString() + "_";
                string Actualfilename = strTimeTicks + strRefno.Replace("'", "");

                Ext = ".pdf";
                string SaveAsFileName = strTimeTicks + (strRefno.Replace("/", "-")) + " " + strFileType + Ext;

                string month = System.DateTime.Now.Month.ToString();
                string year = System.DateTime.Now.Year.ToString();

                // for differentiating CRO & Main Invoice 
                if (isTickRequired == false)
                {
                    if (strFileType != "E-Invoicing")
                    {
                        Ext = ".pdf";
                        SaveAsFileName = strRefno + Ext;
                    }
                    else
                    {
                        Ext = ".json";
                        SaveAsFileName = strRefno.Replace("/", "-") + Ext;
                    }
                }
                else
                {
                    strFilePath = strFilePath + year + "\\" + month; // + filename;
                }

                CloudBlobDirectory rootDir = container.GetDirectoryReference(strFilePath);

                //// Get a reference to the file share we created previously.
                CloudBlockBlob blockBlob = rootDir.GetBlockBlobReference(SaveAsFileName);

                if (container.Exists())
                {
                    int fileLenght = ByteArray.Length;

                    //if (Ext == ".pdf")
                    //{
                    // Generate a SAS for a file in the share
                    string contentType = "";
                    string fileExt = Ext.Split('.')[1].ToLower();

                    if (fileExt == "pdf")
                    {
                        //Set the appropriate ContentType.
                        contentType = "Application/pdf";
                    }
                    else if (fileExt == "json")
                    {
                        //Set the appropriate ContentType.
                        contentType = "Application/json";
                    }
                    else
                    {
                        contentType = "Image/" + fileExt;
                    }
                    Stream fileStream = new MemoryStream(ByteArray);

                    blockBlob.Properties.ContentType = contentType;
                    blockBlob.UploadFromStream(fileStream);

                    UploadFileToFTP(strFilePath, SaveAsFileName, ByteArray, strFileType);
                    fileStream.Dispose();
                    //}
                }
                return SaveAsFileName;
            }
            catch (Exception ex)
            {
                return "ErrorFile";
            }
        }
        #endregion

        #region UploadFileToFTP
        public void UploadFileToFTP(string strFilePath, string Filename, byte[] FileInBytes, string strFileType)
        {
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = path.Replace("\\bin\\Debug\\net5.0", "").Replace("\\AllBooking", "");
                string FullPhysicalPath = path + "\\" + strFilePath + "\\" + Filename;
                string PhysicalPath = path + "\\" + strFilePath + "\\";

                if (!Directory.Exists(PhysicalPath))
                {
                    //If Directory (Folder) does not exists. Create it.
                    Directory.CreateDirectory(PhysicalPath);
                }

                File.WriteAllBytes(FullPhysicalPath, FileInBytes);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(SettingsConfig.AppSetting("FTP:FTPURL") + strFilePath + "\\" + Filename);
                request.Credentials = new NetworkCredential(SettingsConfig.AppSetting("FTP:FTPURL"), SettingsConfig.AppSetting("FTP:FTPPassword"));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                using (Stream fileStream = File.OpenRead(PhysicalPath + Filename))
                using (Stream ftpStream = request.GetRequestStream())
                {
                    fileStream.CopyTo(ftpStream);
                }
                if (strFileType != "ExcelDepoIn" && strFileType != "ExcelDepoOut" && strFileType != "BLCntrList" && strFileType != "ExcelAddContainer")
                {
                    File.Delete(FullPhysicalPath);
                }
                else if (strFileType == "API")
                {
                    File.Delete(FullPhysicalPath);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "The remote server returned an error: (550) File unavailable (e.g., file not found, no access).")
                {
                    //WebRequest request = WebRequest.Create(FTPURL + FolderPath.Replace("\\","//"));
                    //request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    //request.Credentials = new NetworkCredential(FTPUserName, FTPPassword);

                    bool exito = true;
                    string[] lstfolders = strFilePath.Replace("\\", "//").Split('/');
                    string DirectoryPath = SettingsConfig.AppSetting("FTP:FTPURL");
                    foreach (string fol in lstfolders)
                    {
                        if (fol != "")
                        {
                            try
                            {
                                DirectoryPath += "/" + fol;
                                //create the directory
                                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(DirectoryPath));
                                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                                requestDir.Credentials = new NetworkCredential(SettingsConfig.AppSetting("FTP:FTPUserName"), SettingsConfig.AppSetting("FTP:FTPPassword"));
                                requestDir.UsePassive = true;
                                requestDir.UseBinary = true;
                                requestDir.KeepAlive = false;
                                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                                Stream ftpStream = response.GetResponseStream();

                                ftpStream.Close();
                                response.Close();
                            }
                            catch (WebException exp)
                            {
                                FtpWebResponse response = (FtpWebResponse)exp.Response;
                                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                                {
                                    response.Close();
                                    exito = true;
                                }
                                else
                                {
                                    response.Close();
                                    exito = false;
                                }
                            }
                        }
                    }
                    if (exito == true)
                    {
                        UploadFileToFTP(strFilePath, Filename, FileInBytes, strFileType);
                    }
                }
                else
                {
                }
            }
        }
        #endregion

        #region CopyFileOnServer
        public string CopyFileOnServer(IFormFile file, string strFilePath, string strRefno, string strFileType)
        {
            try
            {
                string strTimeTicks = "";
                String Ext = "";
                //Connect to Azure
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsConfig.AppSetting("CloudDetails:StorageConnectionString"));

                // Create a reference to the file client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                //The next 2 lines create if not exists a container named "democontainer"
                CloudBlobContainer container = blobClient.GetContainerReference(SettingsConfig.AppSetting("CloudDetails:CloudSharedFolderName"));

                strTimeTicks = System.DateTime.Now.Ticks.ToString() + "_";
                string Actualfilename = strTimeTicks + System.IO.Path.GetFileName(file.FileName).Replace("'", "");

                Ext = System.IO.Path.GetExtension(file.FileName);
                string SaveAsFileName = strTimeTicks + (strRefno.Replace("/", "-")) + " " + strFileType + Ext;

                string month = System.DateTime.Now.Month.ToString();
                string year = System.DateTime.Now.Year.ToString();
                strFilePath = strFilePath + year + "\\" + month; // + filename;            

                CloudBlobDirectory rootDir = container.GetDirectoryReference(strFilePath);

                //// Get a reference to the file share we created previously.
                CloudBlockBlob blockBlob = rootDir.GetBlockBlobReference(SaveAsFileName);

                if (container.Exists())
                {
                    long fileLenght = file.Length;

                    if (Ext == ".jpg" || Ext == ".gif" || Ext == ".bmp" || Ext == ".png" || Ext == ".jpeg" || Ext == ".JPG" || Ext == ".JPEG" || Ext == ".GIF" || Ext == ".BMP" || Ext == ".PNG" || Ext == ".pdf" || Ext == ".PDF" || Ext == ".xls" || Ext == ".XLS" || Ext == ".xlsx" || Ext == ".XLSX" || Ext == ".DOC" || Ext == ".DOCX" || Ext == ".docx")
                    {

                        // Generate a SAS for a file in the share
                        string contentType = "";
                        string fileExt = Ext.Split('.')[1].ToLower();

                        if (fileExt == "pdf")
                        {
                            //Set the appropriate ContentType.
                            contentType = "Application/pdf";
                        }
                        else if (fileExt == "xls")
                        {
                            contentType = "application/vnd.ms-excel";
                        }
                        else if (fileExt == "xlsx")
                        {
                            contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        }
                        else if (fileExt == "doc")
                        {
                            contentType = "application/msword";
                        }
                        else if (fileExt == "docx")
                        {
                            contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        }
                        else
                        {
                            contentType = "Image/" + fileExt;
                        }


                        //var filePath = Path.Combine(Directory.GetCurrentDirectory(), strFilePath, file.FileName);
                        //bool exists = System.IO.Directory.Exists(strFilePath);
                        //if (!exists)
                        //    System.IO.Directory.CreateDirectory(strFilePath);

                        try
                        {

                            Stream fileStream = file.OpenReadStream();
                            blockBlob.Properties.ContentType = contentType;
                            blockBlob.UploadFromStream(fileStream);

                            fileStream.Position = 0;
                            fileStream = file.OpenReadStream();
                            BinaryReader br = new BinaryReader(fileStream);
                            byte[] FileInBytes = br.ReadBytes((Int32)fileStream.Length);
                            UploadFileToFTP(strFilePath, SaveAsFileName, FileInBytes, strFileType);

                            fileStream.Dispose();
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }
                }
                return SaveAsFileName;
            }
            catch (Exception ex)
            {
                //  ExceptionLogger.LogError(ex, strFileType, "CopyFileOnServer");
                return "ErrorFile";
            }
        }
        #endregion

        #region CopyFileOnServer
        public string CopyFileOnServer(MemoryStream stream, string FileName, string strFilePath, string strRefno, string strFileType)
        {
            try
            {
                string strTimeTicks = "";
                String Ext = "";
                string SaveAsFileName = "";
                //Connect to Azure
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsConfig.AppSetting("CloudDetails:StorageConnectionString"));

                // Create a reference to the file client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                //The next 2 lines create if not exists a container named "democontainer"
                CloudBlobContainer container = blobClient.GetContainerReference(SettingsConfig.AppSetting("CloudDetails:CloudSharedFolderName"));

                strTimeTicks = System.DateTime.Now.Ticks.ToString() + "_";
                string Actualfilename = strTimeTicks + System.IO.Path.GetFileName(FileName).Replace("'", "");

                Ext = System.IO.Path.GetExtension(FileName);

                if (strFilePath == @"EconWebsite\" || strFilePath == @"EmployeeDetails\")
                {
                    SaveAsFileName = strTimeTicks + strRefno + "" + strFileType + "_" + FileName;
                }
                else
                {
                    SaveAsFileName = strTimeTicks + strRefno + " " + strFileType + Ext;
                }

                string month = System.DateTime.Now.Month.ToString();
                string year = System.DateTime.Now.Year.ToString();
                strFilePath = strFilePath + year + "\\" + month; // + filename;            

                CloudBlobDirectory rootDir = container.GetDirectoryReference(strFilePath);

                //// Get a reference to the file share we created previously.
                CloudBlockBlob blockBlob = rootDir.GetBlockBlobReference(SaveAsFileName);

                if (container.Exists())
                {
                    //int fileLenght = FileUploadControl.Length;

                    //if (Ext == ".jpg" || Ext == ".gif" || Ext == ".bmp" || Ext == ".png" || Ext == ".jpeg" || Ext == ".JPG" || Ext == ".JPEG" || Ext == ".GIF" || Ext == ".BMP" || Ext == ".PNG" || Ext == ".pdf" || Ext == ".PDF")
                    if (Ext == ".jpg" || Ext == ".gif" || Ext == ".bmp" || Ext == ".png" || Ext == ".jpeg" || Ext == ".JPG" || Ext == ".JPEG" || Ext == ".GIF" || Ext == ".BMP" || Ext == ".PNG" || Ext == ".pdf" || Ext == ".PDF" || Ext == ".xls" || Ext == ".XLS" || Ext == ".xlsx" || Ext == ".XLSX" || Ext == ".doc" || Ext == ".DOC" || Ext == ".docx" || Ext == ".DOCX")
                    {

                        // Generate a SAS for a file in the share
                        string contentType = "";
                        string fileExt = Ext.Split('.')[1].ToLower();

                        if (fileExt == "pdf")
                        {
                            //Set the appropriate ContentType.
                            contentType = "Application/pdf";
                        }
                        else if (fileExt == "xls")
                        {
                            contentType = "application/vnd.ms-excel";
                        }
                        else if (fileExt == "xlsx")
                        {
                            contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        }
                        else if (fileExt == "doc")
                        {
                            contentType = "application/msword";
                        }
                        else if (fileExt == "docx")
                        {
                            contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        }
                        else
                        {
                            contentType = "Image/" + fileExt;
                        }
                        //Stream fileStream = FileUploadControl.PostedFile.InputStream;
                        Stream fileStream = stream;

                        blockBlob.Properties.ContentType = contentType;
                        blockBlob.UploadFromStream(fileStream);

                        byte[] FileInBytes = stream.ToArray();
                        UploadFileToFTP(strFilePath, SaveAsFileName, FileInBytes, "API");

                        fileStream.Dispose();
                    }
                }
                return SaveAsFileName;
            }
            catch (Exception ex)
            {
                return "ErrorFile";
            }
        }
        #endregion

    }
}
