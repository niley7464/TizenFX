/*
 * Copyright (c) 2024 Samsung Electronics Co., Ltd. All Rights Reserved.
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

namespace Tizen.MachineLearning.Inference
{
    /// <summary>
    /// The MlInformationList class manages list of MlInformation.
    /// </summary>
    /// <since_tizen> 9 </since_tizen>
    public class MlInformationList
    {
        private IntPtr _handle = IntPtr.Zero;

        /// <summary>
        /// Creates a MlInformationList instance.
        /// </summary>
        /// <feature>http://tizen.org/feature/machine_learning.inference</feature>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <since_tizen> 9 </since_tizen>
        internal MlInformationList(IntPtr handle) {
            NNStreamer.CheckNNStreamerSupport();

            if (handle == IntPtr.Zero)
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The information list handle is null");

            _handle = handle;
        }

        /// <summary>
        /// Destroys the MlInformationList resource.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>
        ~MlInformationList() {
            NNStreamerError ret = Interop.Util.DestroyInformationList(_handle);
            NNStreamer.CheckException(ret, "Failed to destroy the information list");
        }

        /// <summary>
        /// Gets a mlInformation value from the mlInformationList instance.
        /// </summary>
        /// <param name="index">The index of mlInformation in mlInformationList.</param>
        /// <returns>The mlInformation of given index.</returns>
        /// <feature>http://tizen.org/feature/machine_learning.inference</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <since_tizen> 9 </since_tizen>
        public MlInformation GetInformation(int index)
        {
            int length = GetLength();

            if (index >= length)
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property index is out of bound");

            IntPtr infoHandle = IntPtr.Zero;
            NNStreamerError ret = Interop.Util.GetInformation(_handle, index, out infoHandle);
            NNStreamer.CheckException(ret, "Failed to get information from list");

            return new MlInformation(infoHandle);
        }

        /// <summary>
        /// Gets the number of mlInformation in the mlInformationList instance.
        /// </summary>
        /// <returns>The number of mlInformation in the mlInformationList instance.</returns>
        /// <feature>http://tizen.org/feature/machine_learning.inference</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <since_tizen> 9 </since_tizen>
        public int GetLength()
        {
            int value = 0;

            NNStreamerError ret = Interop.Util.GetInformationListLength(_handle, out value);
            NNStreamer.CheckException(ret, "Failed to get the length of information list");
            return value;
        }
    }
}
