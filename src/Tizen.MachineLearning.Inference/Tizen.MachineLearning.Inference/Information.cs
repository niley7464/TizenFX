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

        internal MlInformation(IntPtr handle) {
            NNStreamer.CheckNNStreamerSupport();
            _handle = handle;
        }

        ~MlInformation() {
            if (_handle != IntPtr.Zero)
            {
                NNStreamerError ret = Interop.Util.DestroyInformation(_handle);
                NNStreamer.CheckException(ret, "Failed to destroy the information");
            }
        }

        public string GetInformation(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property key is invalid");

            IntPtr value = IntPtr.Zero;

            NNStreamerError ret = Interop.Util.GetValue(_handle, key, out value);
            NNStreamer.CheckException(ret, "Failed to get information value from key");
            return Interop.Util.IntPtrToString(value);
        }
    }
}
