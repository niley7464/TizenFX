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
    public class MlInformationList
    {
        private IntPtr _handle = IntPtr.Zero;

        internal MlInformationList(IntPtr handle) {
            NNStreamer.CheckNNStreamerSupport();
            _handle = handle;
        }

        ~MlInformationList() {
            NNStreamerError ret = Interop.Util.DestroyInformationList(_handle);
            NNStreamer.CheckException(ret, "Failed to destroy the information list");
        }

        public MlInformation GetInformation(int index)
        {
            IntPtr value;

            NNStreamerError ret = Interop.Util.GetInformation(_handle, index, out value);
            NNStreamer.CheckException(ret, "Failed to get information from list");
            return new MlInformation(value);
        }

        public int GetLength()
        {
            int value = 0;

            NNStreamerError ret = Interop.Util.GetInformationListLength(_handle, out value);
            NNStreamer.CheckException(ret, "Failed to get the length of information list");
            return value;
        }
    }
}
