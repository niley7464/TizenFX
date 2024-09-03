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
    /// The MlInformation class manages information based on key-value.
    /// </summary>
    /// <since_tizen> 9 </since_tizen>
    public class MlInformation
    {
        private IntPtr _handle = IntPtr.Zero;
        private InfoType _type;

        private enum InfoType
        {
            Option = 0,
            Information = 1,
        }

        /// <summary>
        /// Creates a MlInformation instance.
        /// </summary>
        /// <feature>http://tizen.org/feature/machine_learning.inference</feature>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <since_tizen> 9 </since_tizen>
        public MlInformation() {
            NNStreamer.CheckNNStreamerSupport();

            NNStreamerError ret = Interop.Util.CreateOption(out _handle);
            NNStreamer.CheckException(ret, "Failed to create information handle");

            _type = InfoType.Option;
        }

        /// <summary>
        /// Creates a MlInformation instance from Native handle.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>
        internal MlInformation(IntPtr handle) {
            NNStreamer.CheckNNStreamerSupport();

            if (handle == IntPtr.Zero)
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The information handle is null");

            _handle = handle;
            _type = InfoType.Information;
        }

        /// <summary>
        /// Destroys the MlInformation resource.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>
        ~MlInformation() {
            NNStreamerError ret = NNStreamerError.InvalidParameter;

            if (_handle != IntPtr.Zero)
            {
                switch(_type)
                {
                    case InfoType.Option:
                        ret = Interop.Util.DestroyOption(_handle);
                        break;
                    case InfoType.Information:
                        /* Information handle is created from native */
                        ret = NNStreamerError.None;
                        break;
                }
            }

            NNStreamer.CheckException(ret, "Failed to destroy the information");
        }

        /// <summary>
        /// Internal method to get the native handle of mlInformation.
        /// </summary>
        /// <returns>The native handle</returns>
        /// <since_tizen> 9 </since_tizen>
        internal IntPtr GetHandle()
        {
            return _handle;
        }

        /// <summary>
        /// Sets a new key-value in mlInformation instance.
        /// </summary>
        /// <param name="key">The key to be set.</param>
        /// <param name="value">The value to be set.</param>
        /// <feature>http://tizen.org/feature/machine_learning.inference</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <since_tizen> 9 </since_tizen>
        public void SetString(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property key is invalid");

            if (string.IsNullOrEmpty(value))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property value is invalid");

            NNStreamerError ret = NNStreamerError.InvalidParameter;

            switch(_type)
            {
                case InfoType.Option:
                    IntPtr valuePtr = Interop.Util.StringToIntPtr(value);
                    ret = Interop.Util.SetOptionValue(_handle, key, valuePtr, null);
                    break;
                case InfoType.Information:
                    ret = NNStreamerError.NotSupported;
                    Log.Error(NNStreamer.TAG, "InfoType Iniformation does not support set value");
                    break;
            }

            NNStreamer.CheckException(ret, "Failed to set string value");
        }

        private IntPtr GetInformationHandle(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property key is invalid");

            IntPtr value = IntPtr.Zero;
            NNStreamerError ret = NNStreamerError.InvalidParameter;

            switch(_type)
            {
                case InfoType.Option:
                    ret = Interop.Util.GetOptionValue(_handle, key, out value);
                    break;
                case InfoType.Information:
                    ret = Interop.Util.GetInformationValue(_handle, key, out value);
                    break;
            }

            NNStreamer.CheckException(ret, "Failed to get information handle from key");

            return value;
        }

        /// <summary>
        /// Gets a string value of key in mlInformation instance.
        /// </summary>
        /// <param name="key">The key to be set.</param>
        /// <returns>The string value of the key.</returns>
        /// <feature>http://tizen.org/feature/machine_learning.inference</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <since_tizen> 9 </since_tizen>
        public string GetString(string key)
        {
            IntPtr value = GetInformationHandle(key);
            return Interop.Util.IntPtrToString(value);
        }

        /// <summary>
        /// Gets a TensorsData value of key in mlInformation instance.
        /// </summary>
        /// <param name="key">The key to be set.</param>
        /// <returns>The TensorsData value of the key.</returns>
        /// <feature>http://tizen.org/feature/machine_learning.inference</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <since_tizen> 9 </since_tizen>
        public TensorsData GetTensorsData(string key, TensorsInfo info)
        {
            IntPtr value = GetInformationHandle(key);
            TensorsData data = TensorsData.CreateFromNativeHandle(value, info.GetTensorsInfoHandle(), true, false);

            return data;
        }
    }
}
