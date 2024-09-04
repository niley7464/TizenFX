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
            IntPtr value;

            NNStreamerError ret = Interop.Util.GetValue(_handle, key, out value);
            NNStreamer.CheckException(ret, "Failed to get information value from key");
            return Interop.Util.IntPtrToString(value);
        }
    }
}
