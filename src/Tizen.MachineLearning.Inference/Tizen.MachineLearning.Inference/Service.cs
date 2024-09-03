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
            NNStreamerError ret = NNStreamerError.None;

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The service configuration path or pipeline name is invalid");

            switch (type) {
                case InitType.Config:
                    ret = Interop.Service.Create(name, out _handle);
                    break;
                case InitType.Pipeline:
                    ret = Interop.Service.LaunchPipeline(name, out _handle);
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

        static public void SetPipeline(string name, string desc)
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            if (string.IsNullOrEmpty(desc))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property desc is invalid");

            NNStreamerError ret = Interop.Service.SetPipeline(name, desc);
            NNStreamer.CheckException(ret, "Failed to create service pipeline");
        }

        static public string GetPipeline(string name)
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            string value;
            NNStreamerError ret = Interop.Service.GetPipeline(name, out value);
            NNStreamer.CheckException(ret, "Failed to get service pipeline");

            return value;
        }

        static public void DeletePipeline(string name)
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            NNStreamerError ret = Interop.Service.DeletePipeline(name);
            NNStreamer.CheckException(ret, "Failed to delete service pipeline");
        }

        public PipelineState GetPipelineState()
        {
            NNStreamer.CheckNNStreamerSupport();

            int state = 0;
            NNStreamerError ret = Interop.Service.GetPipelineState(_handle, out state);
            if (ret == NNStreamerError.None && state == 0)
                ret = NNStreamerError.InvalidOperation;

            NNStreamer.CheckException(ret, "Failed to get service pipeline state");

            return (PipelineState) state;
        }

        static public int RegisterModel(string name, string path, bool activate = false, string description = "")
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            if (string.IsNullOrEmpty(path))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property path is invalid");

            int version;
            NNStreamerError ret = Interop.Service.RegisterModel(name, path, activate, description, out version);
            NNStreamer.CheckException(ret, "Failed to register model");

            return version;
        }

        static public void UpdateModelDescription(string name, int version, string description)
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            if (string.IsNullOrEmpty(description))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property description is invalid");

            NNStreamerError ret = Interop.Service.UpdateModelDescription(name, version, description);
            NNStreamer.CheckException(ret, "Failed to update model description");
        }

        static public void ActivateModel(string name, int version, string description)
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            NNStreamerError ret = Interop.Service.ActivateModel(name, version);
            NNStreamer.CheckException(ret, "Failed to activate the given model");
        }

        static public MlInformation GetModel(string name, int version)
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            IntPtr info = IntPtr.Zero;
            NNStreamerError ret = Interop.Service.GetModel(name, version, out info);
            NNStreamer.CheckException(ret, "Failed to get the given model");

            MlInformation result = new MlInformation(info);
            return result;
        }

        static public MlInformation GetActivatedModel(string name)
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            IntPtr info = IntPtr.Zero;
            NNStreamerError ret = Interop.Service.GetActivatedModel(name, out info);
            NNStreamer.CheckException(ret, "Failed to get the given activated model");

            MlInformation result = new MlInformation(info);
            return result;
        }

        static public MlInformationList GetModels(string name)
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            IntPtr info = IntPtr.Zero;
            NNStreamerError ret = Interop.Service.GetAllModel(name, out info);
            NNStreamer.CheckException(ret, "Failed to get the given models");

            MlInformationList result = new MlInformationList(info);
            return result;
        }

        static public void DeleteModel(string name, int version)
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            NNStreamerError ret = Interop.Service.DeleteModel(name, version);
            NNStreamer.CheckException(ret, "Failed to delete the given model");
        }

        static public void AddResource(string name, string path, string description="")
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            if (string.IsNullOrEmpty(path))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property path is invalid");

            NNStreamerError ret = Interop.Service.AddResource(name, path, description);
            NNStreamer.CheckException(ret, "Failed to add resource");
        }

        static public void DeleteResource(string name)
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            NNStreamerError ret = Interop.Service.DeleteResource(name);
            NNStreamer.CheckException(ret, "Failed to delete resource");
        }

        static public MlInformationList GetResource(string name)
        {
            NNStreamer.CheckNNStreamerSupport();

            if (string.IsNullOrEmpty(name))
                throw NNStreamerExceptionFactory.CreateException(NNStreamerError.InvalidParameter, "The property name is invalid");

            IntPtr infoList = IntPtr.Zero;
            NNStreamerError ret = Interop.Service.GetResource(name, out infoList);
            NNStreamer.CheckException(ret, "Failed to get resource");

            return new MlInformationList(infoList);
        }
    }

    public class MlInformation {
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

    public class MlInformationList {
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
