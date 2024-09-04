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
