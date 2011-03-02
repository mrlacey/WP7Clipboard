//-----------------------------------------------------------------------
// <copyright file="Clipboard.cs" company="MRLacey.com">
//     Copyright (c) 2011 MRLacey. All rights reserved.
//     This code is part of WP7Clipboard and is distributed under the MIT license.
//     A copy of the license is included in the project.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;

namespace WP7Clipboard
{
    /// <summary>
    /// Provides methods that simulate placing data on and retrieve data from a system Clipboard.
    /// </summary>
    public static class Clipboard
    {
        /// <summary>
        /// Identifies the format of the data stored in the image/clipboard
        /// </summary>
        private enum DataFormat : byte
        {
            /// <summary>
            /// Indicates that the image/clipboard contains nothing (is empty)
            /// </summary>
            None = 1,

            /// <summary>
            /// Indicates that the image/clipboard stores a string
            /// </summary>
            Text = 2,

            /// <summary>
            /// Indicates that the image/clipboard stores a WriteableBitmap as a byte array
            /// </summary>
            Image = 3
        }

        /// <summary>
        /// This is the name of the image which data is hidden within.
        /// </summary>
        private const string FILENAME = "clipboard.jpg";

        /// <summary>
        /// This must match the length of the image which has the data hidden in it.
        /// </summary>
        private const int RESOURCE_IMAGE_LENGTH = 1196;

        /// <summary>
        /// Removes all data from the Clipboard.
        /// x-ref: http://msdn.microsoft.com/en-us/library/system.windows.forms.clipboard.clear.aspx
        /// </summary>
        public static void Clear()
        {
            try
            {
                SaveData(DataFormat.None, null);
            }
            catch (Exception exc)
            {
                if (exc.GetType() == typeof(ExternalException))
                {
                    throw;
                }
                else
                {
                    throw new ExternalException("The Clipboard could not be cleared.", exc);
                }
            }
        }

        /// <summary>
        /// Indicates whether there is text data on the Clipboard.
        /// x-ref: http://msdn.microsoft.com/en-us/library/a3cyzt72.aspx
        /// </summary>
        /// <returns>true if there is text data on the Clipboard; otherwise, false.</returns>
        public static bool ContainsText()
        {
            try
            {
                var savedData = GetSavedData();

                if (savedData == null)
                {
                    return false;
                }

                return savedData[0] == (byte)DataFormat.Text;
            }
            catch (ObjectDisposedException ode)
            {
                throw new ExternalException("Unable to check if clipboard contains text.", ode);
            }
            catch (InvalidOperationException ioe)
            {
                throw new ExternalException("Unable to check if clipboard contains text.", ioe);
            }
            catch (Exception exc)
            {
                if (exc.GetType() == typeof(ExternalException))
                {
                    throw;
                }
                else
                {
                    throw new ExternalException("Unable to check if clipboard contains text.", exc);
                }
            }
        }

        /// <summary>
        /// Returns the, UTF8Encoded, text data from the Clipboard.
        /// x-ref: http://msdn.microsoft.com/en-us/library/kz40084e.aspx
        /// </summary>
        /// <returns>the text on the Clipboard.</returns>
        public static string GetText()
        {
            try
            {
                var savedData = GetSavedData();

                if (savedData == null)
                {
                    return string.Empty;
                }

                return savedData[0] == (byte)DataFormat.Text ? new UTF8Encoding().GetString(savedData, 1, savedData.Length - 1) : string.Empty;
            }
            catch (ObjectDisposedException ode)
            {
                throw new ExternalException("Unable to get clipboard text.", ode);
            }
            catch (InvalidOperationException ioe)
            {
                throw new ExternalException("Unable to get clipboard text.", ioe);
            }
            catch (Exception exc)
            {
                if (exc.GetType() == typeof(ExternalException))
                {
                    throw;
                }
                else
                {
                    throw new ExternalException("Unable to get clipboard text.", exc);
                }
            }
        }

        /// <summary>
        /// Single method acting as a central point for saving data to the library
        /// </summary>
        /// <param name="dataFormat">The type of data to save.</param>
        /// <param name="data">The data to save.</param>
        private static void SaveData(DataFormat dataFormat, byte[] data)
        {
            var sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("WP7Clipboard.clipboard.jpg");

            if (sr == null)
            {
                throw new ExternalException("Unable to set Clipboard data");
            }

            var imgBytes = new byte[RESOURCE_IMAGE_LENGTH];

            sr.Read(imgBytes, 0, imgBytes.Length);

            var toSave = new byte[imgBytes.Length + 1 + data.Length];

            Buffer.BlockCopy(imgBytes, 0, toSave, 0, imgBytes.Length);
            Buffer.SetByte(toSave, imgBytes.Length, (byte)dataFormat);
            Buffer.BlockCopy(data, 0, toSave, imgBytes.Length + 1, data.Length);

            using (var ml = new MediaLibrary())
            {
                ml.SavePicture(FILENAME, toSave);
            }
        }

        /// <summary>
        /// Single method acting as a central point of access to the data saved in the library.
        /// </summary>
        /// <returns>the data stored inside the image saved in the library</returns>
        private static byte[] GetSavedData()
        {
            using (var ml = new MediaLibrary())
            {
                using (var pics = ml.SavedPictures)
                {
                    using (var img = pics.LastOrDefault(pic => pic.Name == FILENAME))
                    {
                        if (img == null)
                        {
                            return null;
                        }

                        using (var strm = img.GetImage())
                        {
                            // There may be a file with the same name but created from a different source
                            if (strm.Length < RESOURCE_IMAGE_LENGTH)
                            {
                                return null;
                            }

                            var buffer = new byte[strm.Length - RESOURCE_IMAGE_LENGTH];

                            strm.Read(buffer, RESOURCE_IMAGE_LENGTH, Convert.ToInt32(strm.Length));

                            return buffer;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clears the Clipboard and then adds the, UTF8Encoded, text specified.
        /// x-ref: http://msdn.microsoft.com/en-us/library/ydby206k.aspx
        /// </summary>
        /// <param name="text">The text to add to the clipboard.</param>
        public static void SetText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            try
            {
                SaveData(DataFormat.Text, new UTF8Encoding().GetBytes(text));
            }
            catch (Exception exc)
            {
                if (exc.GetType() == typeof(ExternalException))
                {
                    throw;
                }
                else
                {
                    throw new ExternalException("Unable to set clipboard text.", exc);
                }
            }
        }

        /// <summary>
        /// Indicates whether there is data on the Clipboard that is in the Bitmap format.
        /// </summary>
        /// <returns>true if there is image data on the Clipboard; otherwise, false.</returns>
        public static bool ContainsImage()
        {
            try
            {
                var savedData = GetSavedData();

                if (savedData == null)
                {
                    return false;
                }

                return savedData[0] == (byte)DataFormat.Image;
            }
            catch (ObjectDisposedException ode)
            {
                throw new ExternalException("Unable to check if clipboard contains an image.", ode);
            }
            catch (InvalidOperationException ioe)
            {
                throw new ExternalException("Unable to check if clipboard contains an image.", ioe);
            }
            catch (Exception exc)
            {
                if (exc.GetType() == typeof(ExternalException))
                {
                    throw;
                }
                else
                {
                    throw new ExternalException("Unable to check if clipboard contains an image.", exc);
                }
            }
        }

        /// <summary>
        /// Retrieves an image from the Clipboard.
        /// </summary>
        /// <returns>An Image representing the Clipboard image data or null if the Clipboard does not contain any data that is in the Bitmap format.</returns>
        public static WriteableBitmap GetImage()
        {
            try
            {
                var savedData = GetSavedData();

                if (savedData == null)
                {
                    return null;
                }

                if (savedData[0] == (byte)DataFormat.Image)
                {
                    var width = BitConverter.ToInt32(savedData, 1);
                    var height = BitConverter.ToInt32(savedData, 5);

                    var result = new WriteableBitmap(width, height);

                    using (var ms = new MemoryStream())
                    {
                        ms.Write(savedData, 9, savedData.Length - 9);

                        result.SetSource(ms);
                    }

                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (ObjectDisposedException ode)
            {
                throw new ExternalException("Unable to get clipboard image.", ode);
            }
            catch (InvalidOperationException ioe)
            {
                throw new ExternalException("Unable to get clipboard image.", ioe);
            }
            catch (Exception exc)
            {
                if (exc.GetType() == typeof(ExternalException))
                {
                    throw;
                }
                else
                {
                    throw new ExternalException("Unable to get clipboard image.", exc);
                }
            }
        }

        /// <summary>
        /// Clears the Clipboard and then adds an Image in the Bitmap format.
        /// </summary>
        /// <param name="image">The WriteableBitmap to add to the clipboard.</param>
        public static void SetImage(WriteableBitmap image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            try
            {
                int[] p = image.Pixels;
                int len = (p.Length * 4) + 8;
                var buffer = new byte[len];

                Buffer.BlockCopy(BitConverter.GetBytes(image.PixelWidth), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(image.PixelHeight), 0, buffer, 4, 4);
                Buffer.BlockCopy(p, 0, buffer, 8, len - 8);

                SaveData(DataFormat.Image, buffer);
            }
            catch (Exception exc)
            {
                if (exc.GetType() == typeof(ExternalException))
                {
                    throw;
                }
                else
                {
                    throw new ExternalException("Unable to set clipboard image.", exc);
                }
            }
        }

        /// <summary>
        /// Generates a ProxyClipboard for use by an application
        /// to minimize unnecessary file generation
        /// </summary>
        /// <returns cref="ProxyClipboard">A ProxyClipboard object matching the current clipboard state</returns>
        public static ProxyClipboard GenerateProxy()
        {
            var result = new ProxyClipboard();

            InitializeProxy(result);

            return result;
        }

        /// <summary>
        /// Set the proxy to match the real data in the clipboard.
        /// This avoid making multiple uses of the PictureLibrary - which isn't permitted.
        /// </summary>
        /// <param name="proxy">The ProxyClipboard to initialize.</param>
        private static void InitializeProxy(ProxyClipboard proxy)
        {
            try
            {
                using (var ml = new MediaLibrary())
                {
                    using (var pics = ml.SavedPictures)
                    {
                        using (var img = pics.LastOrDefault(pic => pic.Name == FILENAME))
                        {
                            if (img == null)
                            {
                                return;
                            }

                            using (var strm = img.GetImage())
                            {
                                // There may be a file with the same name but created from a different source
                                if (strm.Length < RESOURCE_IMAGE_LENGTH)
                                {
                                    return;
                                }

                                var buffer = new byte[strm.Length - RESOURCE_IMAGE_LENGTH];

                                Buffer.BlockCopy(ReadFully(strm), RESOURCE_IMAGE_LENGTH, buffer, 0, Convert.ToInt32(strm.Length) - RESOURCE_IMAGE_LENGTH);

                                if (buffer[0] == (byte)DataFormat.Text)
                                {
                                    proxy.SetText(new UTF8Encoding().GetString(buffer, 1, buffer.Length - 1));
                                }
                                else if (buffer[0] == (byte)DataFormat.Image)
                                {
                                    var width = BitConverter.ToInt32(buffer, 1);
                                    var height = BitConverter.ToInt32(buffer, 5);

                                    var wb = new WriteableBitmap(width, height);

                                    using (var ms = new MemoryStream())
                                    {
                                        ms.Write(buffer, 9, buffer.Length - 9);

                                        wb.SetSource(ms);
                                    }

                                    proxy.SetImage(wb);
                                }
                            }
                        }
                    }
                }

                proxy.HideChangesSetDuringInitialization();
            }
            catch (ObjectDisposedException ode)
            {
                throw new ExternalException("Unable to get initialize proxy.", ode);
            }
            catch (InvalidOperationException ioe)
            {
                throw new ExternalException("Unable to get initialize proxy.", ioe);
            }
            catch (Exception exc)
            {
                if (exc.GetType() == typeof(ExternalException))
                {
                    throw;
                }
                else
                {
                    throw new ExternalException("Unable to get initialize proxy.", exc);
                }
            }
        }

        /// <summary>
        /// Read a stream and return it as a Byte Array
        /// </summary>
        /// <param name="input">The stream to read/reformatted.</param>
        /// <returns>the contents of the stream</returns>
        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];

            using (MemoryStream ms = new MemoryStream())
            {
                int read;

                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }
    }
}
