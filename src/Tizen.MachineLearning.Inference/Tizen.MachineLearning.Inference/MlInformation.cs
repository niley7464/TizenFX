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
    public class MlInformation
    {
        private IntPtr _handle = IntPtr.Zero;
        private InfoType _type;

        private enum InfoType
        {
            Option = 0,
            Information = 1,
        }

        public MlInformation() {
            NNStreamer.CheckNNStreamerSupport();

            NNStreamerError ret = Interop.Util.CreateOption(out _handle);
            NNStreamer.CheckException(ret, "Failed to create information handle");

            _type = InfoType.Option;
        }

        internal MlInformation(IntPtr handle) {
            NNStreamer.CheckNNStreamerSupport();
            if (handle == IntPtr.Zero)
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The information handle is null");

            _handle = handle;
            _type = InfoType.Information;
        }

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
                        ret = Interop.Util.DestroyInformation(_handle);
                        break;
                }
            }

            NNStreamer.CheckException(ret, "Failed to destroy the information");
        }

        internal IntPtr GetHandle()
        {
            return _handle;
        }

        public void SetInformation(string key, string value)
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

            NNStreamer.CheckException(ret, "Failed to set option value");
        }

        public string GetInformation(string key)
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

            NNStreamer.CheckException(ret, "Failed to get information value from key");
            return Interop.Util.IntPtrToString(value);
        }
    }
}
