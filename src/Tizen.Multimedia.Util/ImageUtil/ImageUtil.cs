/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using Tizen.Common;
using NativeUtil = Interop.ImageUtil;

namespace Tizen.Multimedia.Util
{
    /// <summary>
    /// Provides utilities for images.
    /// </summary>
    /// <since_tizen> 4 </since_tizen>
    public static class ImageUtil
    {
        /// <summary>
        /// Retrieves supported colorspaces for a <see cref="ImageFormat"/> that represents formats for <see cref="ImageEncoder"/> and <see cref="ImageDecoder"/>.
        /// </summary>
        /// <returns>An IEnumerable of <see cref="ColorSpace"/> representing the supported color-spaces.</returns>
        /// <param name="format">The <see cref="ImageFormat"/>.</param>
        /// <exception cref="ArgumentException"><paramref name="format"/> is invalid.</exception>
        /// <since_tizen> 4 </since_tizen>
        public static IEnumerable<ColorSpace> GetSupportedColorSpaces(ImageFormat format)
        {
            ValidationUtil.ValidateEnum(typeof(ImageFormat), format, nameof(format));

            var colorspaces = new List<ColorSpace>();

            NativeUtil.ForeachSupportedColorspace(format,
                (colorspace, _) => { colorspaces.Add(colorspace.ToCommonColorSpace()); return true; }).
                ThrowIfFailed("Failed to get supported color-space list from native handle");

            return colorspaces;
        }

        /// <summary>
        /// Extracts representative color from an image buffer.
        /// </summary>
        /// <param name="buffer">Raw image buffer.</param>
        /// <param name="size">Resolution of the image.</param>
        /// <remarks>The image should be <see cref="ColorSpace.Rgb888"/>.</remarks>
        /// <returns>The representative color of the image.</returns>
        /// <see cref="BitmapFrame"/>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="buffer"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     width of <paramref name="size"/> is less than or equal to zero.<br/>
        ///     -or-<br/>
        ///     height of <paramref name="size"/> is less than or equal to zero.
        /// </exception>
        /// <since_tizen> 4 </since_tizen>
        public static Color GetColor(byte[] buffer, Size size)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (buffer.Length == 0)
            {
                throw new ArgumentException("buffer is empty.", nameof(buffer));
            }

            if (size.Width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size.Width,
                    "width can't be less than or equal to zero.");
            }
            if (size.Height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size.Height,
                    "height can't be less than or equal to zero.");
            }

            NativeUtil.ExtractColorFromMemory(buffer, size.Width, size.Height, out var r, out var g, out var b)
                .ThrowIfFailed("Failed to extract color from buffer");

            return Color.FromRgb(r, g, b);
        }
    }
}
