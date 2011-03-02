//-----------------------------------------------------------------------
// <copyright file="ProxyClipboard.cs" company="MRLacey.com">
//     Copyright (c) 2011 MRLacey. All rights reserved.
//     This code is part of WP7Clipboard and is distributed under the MIT license.
//     A copy of the license is included in the project.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;
using System.Windows.Media.Imaging;

namespace WP7Clipboard
{
    /// <summary>
    /// Provides an in memory wrapper to the "real" ClipBoard.
    /// Use this for faster access to data and to avoid persisting unnecessary data.
    /// </summary>
    public class ProxyClipboard
    {
        /// <summary>
        /// Internal value used to track changes so we can avoid trying to persist data that hasn't been changed
        /// </summary>
        private bool hasChanged = false;

        /// <summary>
        /// Stores the type of data currently on the Clipboard.
        /// </summary>
        private Type storedType;

        /// <summary>
        /// Stores the current Clipboard data.
        /// </summary>
        private object storedValue;

        ///<summary>
        /// The ProxyClipboard is created empty
        ///</summary>
        public ProxyClipboard()
        {
            this.Clear();
        }

        /// <summary>
        /// Removes all data from the Clipboard.
        /// </summary>
        public void Clear()
        {
            this.hasChanged = true;

            this.storedType = null;
            this.storedValue = null;
        }

        /// <summary>
        /// Indicates whether there is text data on the Clipboard.
        /// </summary>
        /// <returns>true if there is text data on the Clipboard.</returns>
        public bool ContainsText()
        {
            return this.storedType == typeof(string);
        }

        /// <summary>
        /// Retrieves, UTF8Encoded, text data from the Clipboard.
        /// </summary>
        /// <returns>the text on the Clipboard.</returns>
        public string GetText()
        {
            return this.ContainsText() ? (string)this.storedValue : string.Empty;
        }

        /// <summary>
        /// Clears the Clipboard and then adds the, UTF8Encoded, text specified.
        /// </summary>
        /// <param name="text">The text to add to the clipboard.</param>
        public void SetText(string text)
        {
            this.hasChanged = true;

            this.storedValue = text;
            this.storedType = typeof(string);
        }

        /// <summary>
        /// Indicates whether there is data on the Clipboard that is in the Bitmap format.
        /// </summary>
        /// <returns>true if there is image data on the Clipboard; otherwise, false.</returns>
        public bool ContainsImage()
        {
            return this.storedType == typeof(WriteableBitmap);
        }

        /// <summary>
        /// Retrieves an image from the Clipboard.
        /// </summary>
        /// <returns>An Image representing the Clipboard image data or null if the Clipboard does not contain any data that is in the Bitmap format.</returns>
        public WriteableBitmap GetImage()
        {
            return this.ContainsImage() ? (WriteableBitmap)this.storedValue : null;
        }

        /// <summary>
        /// Clears the Clipboard and then adds an Image in the Bitmap format.
        /// </summary>
        /// <param name="image">The BitmapIamge to add to the clipboard.</param>
        public void SetImage(WriteableBitmap image)
        {
            this.hasChanged = true;

            this.storedValue = image;
            this.storedType = typeof(WriteableBitmap);
        }

        /// <summary>
        /// Save the contents of this proxy to the "real" clipboard.
        /// </summary>
        public void Persist()
        {
            if (this.hasChanged)
            {
                if (this.ContainsText())
                {
                    Clipboard.SetText(this.GetText());
                }
                else if (this.ContainsImage())
                {
                    Clipboard.SetImage(this.GetImage());
                }
                else
                {
                    Clipboard.Clear();
                }
            }
        }

        /// <summary>
        /// Reset the hasChanges flag so it doesn't include data set during initialization.
        /// This should only be called from Clipboard.InitializeProxy
        /// </summary>
        internal void HideChangesSetDuringInitialization()
        {
            this.hasChanged = false;
        }
    }
}