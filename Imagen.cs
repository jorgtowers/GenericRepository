using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;


namespace Utils
{
    public static class Imagenes
    {
        #region Variables
        static private long _imgLength;
        static private byte[] _imgByte;
        static private string _imgType;
        static SqlParameter param;
        #endregion
        #region Metodos

        public static byte[] Image2Bytes(Image img)
        {
            string sTemp = Path.GetTempFileName();
            FileStream fs = new FileStream(sTemp, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            img.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
            fs.Position = 0;
            //
            int imgLength = Convert.ToInt32(fs.Length);
            byte[] bytes = new byte[imgLength];
            fs.Read(bytes, 0, imgLength);
            fs.Close();
            return bytes;
        }
        public static Image Bytes2Image(byte[] bytes)
        {
            if (bytes == null) return null;
            //
            MemoryStream ms = new MemoryStream(bytes);
            Bitmap bm = null;
            try
            {
                bm = new Bitmap(ms);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return bm;
        }
        public static byte[] CropImageFile(byte[] imageFile, int targetW, int targetH, int targetX, int targetY)
        {
            Image imgPhoto = Image.FromStream(new MemoryStream(imageFile));
            Bitmap bmPhoto = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            grPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, targetW, targetH), targetX, targetY, targetW, targetH, GraphicsUnit.Pixel);
            // Save out to memory and then to a file.  We dispose of all objects to make sure the files don't stay locked.
            MemoryStream mm = new MemoryStream();
            bmPhoto.Save(mm, System.Drawing.Imaging.ImageFormat.Jpeg);
            imgPhoto.Dispose();
            bmPhoto.Dispose();
            grPhoto.Dispose();
            return mm.GetBuffer();
        }
        public static byte[] ResizeImageFile(byte[] imageFile, int targetSize)
        {
            Image original = Image.FromStream(new MemoryStream(imageFile));
            int targetH, targetW;
            if (original.Height > original.Width)
            {
                targetH = targetSize;
                targetW = (int)(original.Width * ((float)targetSize / (float)original.Height));
            }
            else
            {
                targetW = targetSize;
                targetH = (int)(original.Height * ((float)targetSize / (float)original.Width));
            }
            Image imgPhoto = Image.FromStream(new MemoryStream(imageFile));
            // Create a new blank canvas.  The resized image will be drawn on this canvas.
            Bitmap bmPhoto = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            grPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, targetW, targetH), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel);
            // Save out to memory and then to a file.  We dispose of all objects to make sure the files don't stay locked.
            MemoryStream mm = new MemoryStream();
            bmPhoto.Save(mm, System.Drawing.Imaging.ImageFormat.Jpeg);
            original.Dispose();
            imgPhoto.Dispose();
            bmPhoto.Dispose();
            grPhoto.Dispose();
            return mm.GetBuffer();
        }
        public static byte[] ResizeImageWidth(byte[] imageFile, int targetSize)
        {
            Image original = Image.FromStream(new MemoryStream(imageFile));
            int targetH, targetW;

                targetW = targetSize;
                targetH = (int)(original.Height * ((float)targetSize / (float)original.Width));

            Image imgPhoto = Image.FromStream(new MemoryStream(imageFile));
            // Create a new blank canvas.  The resized image will be drawn on this canvas.
            Bitmap bmPhoto = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            grPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, targetW, targetH), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel);
            // Save out to memory and then to a file.  We dispose of all objects to make sure the files don't stay locked.
            MemoryStream mm = new MemoryStream();
            bmPhoto.Save(mm, System.Drawing.Imaging.ImageFormat.Jpeg);
            original.Dispose();
            imgPhoto.Dispose();
            bmPhoto.Dispose();
            grPhoto.Dispose();
            return mm.GetBuffer();
        }
        public static byte[] ResizeImageHeight(byte[] imageFile, int targetSize)
        {
            Image original = Image.FromStream(new MemoryStream(imageFile));
            int targetH, targetW;

                targetH = targetSize;
                targetW = (int)(original.Width * ((float)targetSize / (float)original.Height));
            
            Image imgPhoto = Image.FromStream(new MemoryStream(imageFile));
            // Create a new blank canvas.  The resized image will be drawn on this canvas.
            Bitmap bmPhoto = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            grPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, targetW, targetH), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel);
            // Save out to memory and then to a file.  We dispose of all objects to make sure the files don't stay locked.
            MemoryStream mm = new MemoryStream();
            bmPhoto.Save(mm, System.Drawing.Imaging.ImageFormat.Jpeg);
            original.Dispose();
            imgPhoto.Dispose();
            bmPhoto.Dispose();
            grPhoto.Dispose();
            return mm.GetBuffer();
        }
        /// <summary>
        /// USES ScaleByPercent
        /// string WorkingDirectory = @"C:\Tutorials\ImageResize";
        ///Image imgPhotoVert = Image.FromFile(WorkingDirectory + @"\imageresize_vert.jpg");
        ///Image imgPhotoHoriz = Image.FromFile(WorkingDirectory + @"\imageresize_horiz.jpg");
        ///Image imgPhoto = null;
        /// imgPhoto = ScaleByPercent(imgPhotoVert, 50);
        ///imgPhoto.Save(WorkingDirectory + 
        /// @"\images\imageresize_1.jpg", ImageFormat.Jpeg);
        ///imgPhoto.Dispose();
        /// </summary>
        /// <param name="imgPhoto"></param>
        /// <param name="Percent"></param>
        /// <returns></returns>
        public static Image ScaleByPercent(Image imgPhoto, int Percent)
        {
            float nPercent = ((float)Percent / 100);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight,
                                     PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                    imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
        /// <summary>
        /// Crops an image according to a selection rectangel
        /// </summary>
        /// <param name="image">
        /// the image to be cropped
        /// </param>
        /// <param name="selection">
        /// the selection
        /// </param>
        /// <returns>
        /// cropped image
        /// </returns>
        public static Image Crop(this Image image, Rectangle selection)
        {
            Bitmap bmp = image as Bitmap;

            // Check if it is a bitmap:
            if (bmp == null)
                throw new ArgumentException("Kein gültiges Bild (Bitmap)");

            // Crop the image:
            Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);

            // Release the resources:
            image.Dispose();

            return cropBmp;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fits an image to the size of a picturebox
        /// </summary>
        /// <param name="image">
        /// image to be fit
        /// </param>
        /// <param name="picBox">
        /// picturebox in that the image should fit
        /// </param>
        /// <returns>
        /// fitted image
        /// </returns>
        /// <remarks>
        /// Although the picturebox has the SizeMode-property that offers
        /// the same functionality an OutOfMemory-Exception is thrown
        /// when assigning images to a picturebox several times.
        /// 
        /// AFAIK the SizeMode is designed for assigning an image to
        /// picturebox only once.
        /// </remarks>
        public static Image Fit2PictureBox(this Image image, PictureBox picBox)
        {
            Bitmap bmp = null;
            Graphics g;

            // Scale:
            double scaleY = (double)image.Width / picBox.Width;
            double scaleX = (double)image.Height / picBox.Height;
            double scale = scaleY < scaleX ? scaleX : scaleY;

            // Create new bitmap:
            bmp = new Bitmap(
                (int)((double)image.Width / scale),
                (int)((double)image.Height / scale));

            // Set resolution of the new image:
            bmp.SetResolution(
                image.HorizontalResolution,
                image.VerticalResolution);

            // Create graphics:
            g = Graphics.FromImage(bmp);

            // Set interpolation mode:
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Draw the new image:
            g.DrawImage(
                image,
                new Rectangle(			// Ziel
                    0, 0,
                    bmp.Width, bmp.Height),
                new Rectangle(			// Quelle
                    0, 0,
                    image.Width, image.Height),
                GraphicsUnit.Pixel);

            // Release the resources of the graphics:
            g.Dispose();

            // Release the resources of the origin image:
            image.Dispose();

            return bmp;
        }

        public static void Obtener(Page pageToResponse, byte[] imgData, long imgLength, string imgType , bool isResponse)
        {
            try
            {
                ImgByte = imgData;
                ImgLength = imgLength;
                ImgType = imgType;
                if (isResponse == true)
                {
                    /*Habilita Expiración de Cache para las Imagenes
                    pageToResponse.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
                    pageToResponse.Response.Cache.SetCacheability(HttpCacheability.Public);
                    pageToResponse.Response.Cache.SetValidUntilExpires(true);*/                    

                    //pageToResponse.Response.CacheControl = HttpCacheability.Public.ToString();
                    //pageToResponse.Response.Expires = 60;
                    //pageToResponse.Response.ExpiresAbsolute = DateTime.Now.AddDays(1);                  
                    
                    
                    //pageToResponse.Response.CacheControl = HttpCacheability.Public.ToString();
                    //pageToResponse.Response.Expires = 60;
                    //pageToResponse.Response.ExpiresAbsolute = DateTime.Now.AddDays(1);
                   // pageToResponse.Response.AddHeader("content-disposition", "Notitarde.com");
                   // pageToResponse.Response.AddHeader("Expires", DateTime.Now.AddDays(1).ToLongDateString());
                   // pageToResponse.Response.AddHeader("Cache-Control","private");

                    pageToResponse.Response.AppendHeader("Content-Length", ImgByte.Length.ToString());
                    pageToResponse.Response.ContentType = ImgType;                    
                    pageToResponse.Response.BinaryWrite(ImgByte);
                    pageToResponse.Response.Buffer = true;
                    pageToResponse.Response.Flush();
                }
            }
            catch
            {
                #region Crea una Imagen de 1px x 1px en caso de Error
                byte[] _imgbytes = Convert.FromBase64String("R0lGODlhAQABAIAAANvf7wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==");
                pageToResponse.Response.ContentType = "image/gif";
                pageToResponse.Response.AppendHeader("Content-Length", _imgbytes.Length.ToString());
                pageToResponse.Response.BinaryWrite(_imgbytes);
                pageToResponse.Response.Buffer = true;
                pageToResponse.Response.Flush();
                #endregion
            }
        }

        public static string Info()
        {
            StringBuilder _Info = new StringBuilder();
            _Info.AppendLine("------------------------ INFO -------------------------------");
            _Info.AppendLine("NOTA: La información de la tabla, puede variar,");
            _Info.AppendLine("      tener más columnas según sea la necesidad,");
            _Info.AppendLine("      pero es necesario que cuente con estos campos.");
            _Info.AppendLine("------------------------ TABLA -------------------------------");
            _Info.AppendLine("CREATE TABLE [dbo].[TBIMAGEN](");
            _Info.AppendLine(" [idnImagen] [int] IDENTITY(1,1) NOT NULL,");
            _Info.AppendLine(" [imgData] [image] NULL,");
            _Info.AppendLine(" [imgTitulo] [varchar](8000),");
            _Info.AppendLine(" [imgType] [varchar](8000),");
            _Info.AppendLine(" [imgLength] [bigint] NULL,");
            _Info.AppendLine(" [imgDescrip] [text] NULL,");
            _Info.AppendLine(" [imgStatus] [bit] NULL");
            _Info.AppendLine(" [idnRef] [int] NULL");
            _Info.AppendLine(") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
            _Info.AppendLine("-------------------- STORED PROCEDURE --------------------------");
            _Info.AppendLine("CREATE PROCEDURE spGetImagen");
            _Info.AppendLine(" @idnimagen int");
            _Info.AppendLine("AS");
            _Info.AppendLine("BEGIN");
            _Info.AppendLine(" SELECT * FROM TBIMAGEN WHERE idnimagen=@idnimagen");
            _Info.AppendLine("END");
            return _Info.ToString();
        }
           
#endregion        
        #region Propiedades
        public static byte[] ImgByte { get { return _imgByte; } set { _imgByte = value; } }
        public static string ImgType { get { return _imgType; } set { _imgType = value; } }
        public static long ImgLength { get { return _imgLength; } set { _imgLength = value; } }
        #endregion
    }
}
