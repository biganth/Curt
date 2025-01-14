﻿#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using DotNetNuke.Services.FileSystem.Internal;

namespace DotNetNuke.Common.Utilities
{
    public class ImageUtils
    {
        private static int _imgHeight;
        private static int _imgWidth;

        public static Size GetSize(string sPath)
        {
            Image g = Image.FromFile(sPath);
            Size s = g.Size;
            g.Dispose();
            return s;
        }

        /// <summary>
        /// return height of image
        /// </summary>
        /// <param name="sPath">file path of image</param>
        /// <returns></returns>
        public static int GetHeight(string sPath)
        {
            Image g = Image.FromFile(sPath);
            int h = g.Height;
            g.Dispose();
            return h;
        }

        /// <summary>
        /// return width of image
        /// </summary>
        /// <param name="sPath">file path of image</param>
        /// <returns></returns>
        public static int GetWidth(string sPath)
        {
            Image g = Image.FromFile(sPath);
            int w = g.Width;
            g.Dispose();
            return w;
        }

        /// <summary>
        /// return height of image
        /// </summary>
        /// <param name="sFile">Stream of image</param>
        /// <returns></returns>
        public static int GetHeightFromStream(Stream sFile)
        {
            Image g = Image.FromStream(sFile, true);
            return g.Height;
        }

        /// <summary>
        /// width of image
        /// </summary>
        /// <param name="sFile">Steam of image</param>
        /// <returns></returns>
        public static int GetWidthFromStream(Stream sFile)
        {
            Image g = Image.FromStream(sFile, true);
            int w = g.Width;
            g.Dispose();
            return w;
        }

        /// <summary>
        /// create an image
        /// </summary>
        /// <param name="sFile">path of load image file - will be resized according to height and width set</param>
        /// <returns></returns>
        public static string CreateImage(string sFile)
        {
            Image g = Image.FromFile(sFile);
            int h = g.Height;
            int w = g.Width;
            g.Dispose();
            return CreateImage(sFile, h, w);
        }

        /// <summary>
        /// create an image
        /// </summary>
        /// <param name="sFile">path of image file</param>
        /// <param name="intHeight">height</param>
        /// <param name="intWidth">width</param>
        /// <returns></returns>
        public static string CreateImage(string sFile, int intHeight, int intWidth)
        {
            var fi = new FileInfo(sFile);
            string tmp = fi.FullName.Replace(fi.Extension, "_TEMP" + fi.Extension);
            if (FileWrapper.Instance.Exists(tmp))
            {
                FileWrapper.Instance.SetAttributes(tmp, FileAttributes.Normal);
                FileWrapper.Instance.Delete(tmp);
            }

            File.Copy(sFile, tmp);
            var original = new Bitmap(tmp);

            PixelFormat format = original.PixelFormat;
            if (format.ToString().Contains("Indexed"))
            {
                format = PixelFormat.Format24bppRgb;
            }

            int newHeight = intHeight;
            int newWidth = intWidth;
            Size imgSize;
            if (original.Width > newWidth || original.Height > newHeight)
            {
                imgSize = NewImageSize(original.Width, original.Height, newWidth, newHeight);
                _imgHeight = imgSize.Height;
                _imgWidth = imgSize.Width;
            }
            else
            {
                imgSize = new Size(original.Width, original.Height);
                _imgHeight = original.Height;
                _imgWidth = original.Width;
            }

            string sFileExt = fi.Extension;
            string sFileNoExtension = Path.GetFileNameWithoutExtension(sFile);
            string sPath = Path.GetDirectoryName(sFile);
            if (sPath != null)
            {
                sPath = sPath.Replace("/", "\\");
            }
            if (sPath != null && !sPath.EndsWith("\\"))
            {
                sPath += "\\";
            }
            Image img = Image.FromFile(tmp);
            var newImg = new Bitmap(_imgWidth, _imgHeight, format);
            newImg.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            Graphics canvas = Graphics.FromImage(newImg);
            canvas.SmoothingMode = SmoothingMode.None;
            canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
            canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;

            if (sFileExt.ToLowerInvariant() != ".png")
            {
                canvas.Clear(Color.White);
                canvas.FillRectangle(Brushes.White, 0, 0, imgSize.Width, imgSize.Height);
            }
            canvas.DrawImage(img, 0, 0, imgSize.Width, imgSize.Height);
            img.Dispose();
            sFile = sPath;

            sFile += sFileNoExtension + sFileExt;
            if (FileWrapper.Instance.Exists(sFile))
            {
                FileWrapper.Instance.SetAttributes(sFile, FileAttributes.Normal);
                FileWrapper.Instance.Delete(sFile);
            }

            //newImg.Save
            var arrData = new byte[2048];
            Stream content = new MemoryStream();
            ImageFormat imgFormat = ImageFormat.Bmp;
            if (sFileExt.ToLowerInvariant() == ".png")
            {
                imgFormat = ImageFormat.Png;
            }
            else if (sFileExt.ToLowerInvariant() == ".gif")
            {
                imgFormat = ImageFormat.Gif;
            }
            else if (sFileExt.ToLowerInvariant() == ".jpg")
            {
                imgFormat = ImageFormat.Jpeg;
            }
            newImg.Save(content, imgFormat);
            using (Stream outStream = FileWrapper.Instance.Create(sFile))
            {
                long originalPosition = content.Position;
                content.Position = 0;

                try
                {
                    int intLength = content.Read(arrData, 0, arrData.Length);

                    while (intLength > 0)
                    {
                        outStream.Write(arrData, 0, intLength);
                        intLength = content.Read(arrData, 0, arrData.Length);
                    }
                }
                finally
                {
                    content.Position = originalPosition;
                }
            }

            newImg.Dispose();
            original.Dispose();

            canvas.Dispose();
            if (FileWrapper.Instance.Exists(tmp))
            {
                FileWrapper.Instance.SetAttributes(tmp, FileAttributes.Normal);
                FileWrapper.Instance.Delete(tmp);
            }


            return sFile;
        }

        /// <summary>
        /// create a JPG image
        /// </summary>
        /// <param name="sFile">name of image</param>
        /// <param name="img">bitmap of image</param>
        /// <param name="compressionLevel">image quality</param>
        /// <returns></returns>
        public static string CreateJPG(string sFile, Bitmap img, int compressionLevel)
        {
            Graphics bmpOutput = Graphics.FromImage(img);
            bmpOutput.InterpolationMode = InterpolationMode.HighQualityBicubic;
            bmpOutput.SmoothingMode = SmoothingMode.HighQuality;
            var compressionRectange = new Rectangle(0, 0, _imgWidth, _imgHeight);
            bmpOutput.DrawImage(img, compressionRectange);

            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            Encoder myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, compressionLevel);
            myEncoderParameters.Param[0] = myEncoderParameter;
            if (File.Exists(sFile))
            {
                File.Delete(sFile);
            }
            try
            {
                img.Save(sFile, myImageCodecInfo, myEncoderParameters);
            }
            catch (Exception)
            {
                //suppress unexpected exceptions
            }


            img.Dispose();
            bmpOutput.Dispose();
            return sFile;
        }

        /// <summary>
        /// create an image based on a stream (read from a database)
        /// </summary>
        /// <param name="sFile">image name</param>
        /// <param name="intHeight">height</param>
        /// <param name="intWidth">width</param>
        /// <returns>steam</returns>
        public static MemoryStream CreateImageForDB(Stream sFile, int intHeight, int intWidth)
        {
            var newStream = new MemoryStream();
            Image g = Image.FromStream(sFile);
            //Dim thisFormat = g.RawFormat
            if (intHeight > 0 & intWidth > 0)
            {
                int newHeight = intHeight;
                int newWidth = intWidth;
                if (g.Width > newWidth | g.Height > newHeight)
                {
                    Size imgSize = NewImageSize(g.Width, g.Height, newWidth, newHeight);
                    _imgHeight = imgSize.Height;
                    _imgWidth = imgSize.Width;
                }
                else
                {
                    _imgHeight = g.Height;
                    _imgWidth = g.Width;
                }
            }
            else
            {
                _imgWidth = g.Width;
                _imgHeight = g.Height;
            }

            var imgOutput1 = new Bitmap(g, _imgWidth, _imgHeight);
            Graphics bmpOutput = Graphics.FromImage(imgOutput1);
            bmpOutput.InterpolationMode = InterpolationMode.HighQualityBicubic;
            bmpOutput.SmoothingMode = SmoothingMode.HighQuality;
            var compressionRectange = new Rectangle(0, 0, _imgWidth, _imgHeight);
            bmpOutput.DrawImage(g, compressionRectange);
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            Encoder myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, 90);
            myEncoderParameters.Param[0] = myEncoderParameter;
            imgOutput1.Save(newStream, myImageCodecInfo, myEncoderParameters);
            g.Dispose();
            imgOutput1.Dispose();
            bmpOutput.Dispose();
            return newStream;
        }

        /// <summary>
        /// return the approriate encoded for the mime-type of the image being created
        /// </summary>
        /// <param name="myMimeType">mime type (e.g jpg/png)</param>
        /// <returns></returns>
        public static ImageCodecInfo GetEncoderInfo(string myMimeType)
        {
            try
            {
                int i;
                ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
                for (i = 0; i <= (encoders.Length - 1); i++)
                {
                    if ((encoders[i].MimeType == myMimeType))
                    {
                        return encoders[i];
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// scale an image based on existing dimensions and updated requirement
        /// </summary>
        /// <param name="currentWidth">current width</param>
        /// <param name="currentHeight">current height</param>
        /// <param name="newWidth">new width</param>
        /// <param name="newHeight">new height</param>
        /// <returns>updated calculated height/width minesions</returns>
        public static Size NewImageSize(int currentWidth, int currentHeight, int newWidth, int newHeight)
        {
            decimal decScale = ((decimal)currentWidth / (decimal)newWidth) > ((decimal)currentHeight / (decimal)newHeight) ? Convert.ToDecimal((decimal)currentWidth / (decimal)newWidth) : Convert.ToDecimal((decimal)currentHeight / (decimal)newHeight);
            newWidth = Convert.ToInt32(Math.Floor((decimal)currentWidth / decScale));
            newHeight = Convert.ToInt32(Math.Floor((decimal)currentHeight / decScale));

            var newSize = new Size(newWidth, newHeight);

            return newSize;
        }
    }
}