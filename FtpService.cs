using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Datos.Infraestructura
{
   public static class FtpService
   {
       #region Variables
       private static string _ftpServerIP;
       private static string _ftpUserID;
       private static string _ftpPassword;
       private static string _ftpDirectory;
       #endregion
       
       #region Propiedades
       public static string Server { get { return _ftpServerIP; } set { _ftpServerIP = value; } }
       public static string User { get { return _ftpUserID; } set { _ftpUserID = value; } }
       public static string Password { get { return _ftpPassword; } set { _ftpPassword = value; } }
       public static string RemoteDirectory {
           get
           {
               if (string.IsNullOrEmpty(_ftpDirectory))
               {
                   return "";
               }
               else
               {
                   return _ftpDirectory + "/";
               }
           }
           set { _ftpDirectory = value; }
       }
       #endregion
       #region Metodos
       /// <summary>
       /// Method to upload the specified file to the specified FTP Server
       /// </summary>
       /// <param name="filename">file full name to be uploaded</param>
       public static void Upload(string filename)
       {
           FileInfo fileInf = new FileInfo(filename);
           string uri = "ftp://" + Server + "/" + RemoteDirectory + fileInf.Name;
           FtpWebRequest reqFTP;
           // Create FtpWebRequest object from the Uri provided
           reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + Server + "/" + RemoteDirectory + fileInf.Name));
           // Provide the WebPermission Credintials
           reqFTP.Credentials = new NetworkCredential(User, Password);
           // By default KeepAlive is true, where the control connection is not closed
           // after a command is executed.
           reqFTP.KeepAlive = false;
           // Specify the command to be executed.
           reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
           // Specify the data transfer type.
           reqFTP.UseBinary = true;
           // Notify the server about the size of the uploaded file
           reqFTP.ContentLength = fileInf.Length;
           // The buffer size is set to 2kb
           int buffLength = 2048;
           byte[] buff = new byte[buffLength];
           int contentLen;
           // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
           FileStream fs = fileInf.OpenRead();
           // Stream to which the file to be upload is written
           Stream strm = reqFTP.GetRequestStream();
           // Read from the file stream 2kb at a time
           contentLen = fs.Read(buff, 0, buffLength);
           // Till Stream content ends
           while (contentLen != 0)
           {
               // Write Content from the file stream to the FTP Upload Stream
               strm.Write(buff, 0, contentLen);
               contentLen = fs.Read(buff, 0, buffLength);
           }
           // Close the file stream and the Request Stream
           strm.Close();
           fs.Close();
       }
       #endregion
       
    }
}
