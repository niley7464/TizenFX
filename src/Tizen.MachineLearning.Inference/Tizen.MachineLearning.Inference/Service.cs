using System;

namespace Tizen.MachineLearning.Inference
{
    public class Service : IDisposable
    {
        private IntPtr _handle = IntPtr.Zero;
        private bool _disposed = false;

        public enum InitType
        {
            Config = 0,
            Pipeline = 1,
        }

        public Service(string name, InitType type)
        {
            NNStreamer.CheckNNStreamerSupport();
            NNStreamerError ret = NNStreamerError.None;;

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The service configuration path or pipeline name is invalid");

            switch (type) {
                case InitType.Config:
                    ret = Interop.Service.Create(name, out _handle);
                    break;
                case InitType.Pipeline:
                    /* TBU */
                    break;
                default:
                    throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "Invalid service type");
            }

            NNStreamer.CheckException(ret, "Failed to create ml_service instance");
        }

        ~Service()
        {
            Dispose(false);
        }

        public void Start()
        {
            NNStreamerError ret = Interop.Service.Start(_handle);
            NNStreamer.CheckException(ret, "Failed to start the service because of internal error");
        }

        public void Stop()
        {
            NNStreamerError ret = Interop.Service.Stop(_handle);
            NNStreamer.CheckException(ret, "Failed to stop the service because of internal error");
        }

        public TensorsInfo GetTensorsInformation(string name, bool is_input)
        {
            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            IntPtr inHandle;
            NNStreamerError ret;

            if (is_input)
                ret = Interop.Service.GetInputInformation(_handle, name, out inHandle);
            else
                ret = Interop.Service.GetOutputInformation(_handle, name, out inHandle);

            NNStreamer.CheckException(ret, "Failed to get TensorsInfo handle");

            TensorsInfo retInfo = TensorsInfo.ConvertTensorsInfoFromHandle(inHandle);
            return retInfo;
        }

        public void SetInformation(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            if (string.IsNullOrEmpty(value))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property value is invalid");

            NNStreamerError ret = Interop.Service.SetInformation(_handle, name, value);
            NNStreamer.CheckException(ret, "Failed to set service information");
        }

        public string GetInformation(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            string value;
            NNStreamerError ret = Interop.Service.GetInformation(_handle, name, out value);
            NNStreamer.CheckException(ret, "Failed to get service information");

            return value;
        }

        public void Request(string name, TensorsData data)
        {
            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            if (data == null)
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "Given data is invalid");

            NNStreamerError ret = Interop.Service.Request(_handle, name, data.GetHandle());
            NNStreamer.CheckException(ret, "Failed to request service");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // release managed object
            }

            // release unmanaged objects
            if (_handle != IntPtr.Zero)
            {
                NNStreamerError ret = Interop.Service.Destroy(_handle);
                if (ret != NNStreamerError.None)
                    Log.Error(NNStreamer.TAG, "Failed to destroy the service handle");

                _handle = IntPtr.Zero;
            }

            _disposed = true;
        }
    }
}