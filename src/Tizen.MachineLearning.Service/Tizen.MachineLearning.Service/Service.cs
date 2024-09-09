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
using Tizen.MachineLearning.Inference;

namespace Tizen.MachineLearning.Service
{
    public class Service : IDisposable
    {
        private IntPtr _handle = IntPtr.Zero;
        private bool _disposed = false;

        private event EventHandler<ServiceReceivedEventArgs> _serviceEventReceived;
        private Interop.Service.ServiceEventCallback _serviceEventCallback;

        public enum EventType
        {
            NewData = 1,
        }

        public static Service Create(string config)
        {
            MLService.CheckMLServiceSupport();

            if (string.IsNullOrEmpty(config))
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The service configuration path is invalid");

            IntPtr handle = IntPtr.Zero;
            MLServiceError ret = Interop.Service.Create(config, out handle);

            MLService.CheckException(ret, "Failed to create service config instance");

            return new Service(handle);
        }

        internal Service(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The service handle is null");

            _handle = handle;

            _serviceEventCallback = (type, event_data_handle, _) =>
            {
                if (type == EventType.NewData && _serviceEventReceived!= null) {
                    MlInformation info = new MlInformation(event_data_handle);
                    _serviceEventReceived?.Invoke(this, new ServiceReceivedEventArgs(info));
                }
            };
        }

        ~Service()
        {
            Dispose(false);
        }

        internal IntPtr GetHandle()
        {
            return _handle;
        }

        public event EventHandler<ServiceReceivedEventArgs> ServiceEventReceived
        {
            add
            {
                if (value == null)
                    return;

                MLServiceError ret = Interop.Service.SetEventCb(_handle, _serviceEventCallback, IntPtr.Zero);
                MLService.CheckException(ret, "Failed to register event callback");
                _serviceEventReceived += value;
            }

            remove
            {
                if (value == null)
                    return;

                _serviceEventReceived -= value;
            }
        }

        public void Start()
        {
            MLServiceError ret = Interop.Service.Start(_handle);
            MLService.CheckException(ret, "Failed to start the service because of internal error");
        }

        public void Stop()
        {
            MLServiceError ret = Interop.Service.Stop(_handle);
            MLService.CheckException(ret, "Failed to stop the service because of internal error");
        }

        public TensorsInfo GetTensorsInformation(string name, bool is_input)
        {
            if (string.IsNullOrEmpty(name))
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

            IntPtr inHandle;
            MLServiceError ret;

            if (is_input)
                ret = Interop.Service.GetInputInformation(_handle, name, out inHandle);
            else
                ret = Interop.Service.GetOutputInformation(_handle, name, out inHandle);

            MLService.CheckException(ret, "Failed to get TensorsInfo handle");

            TensorsInfo retInfo = TensorsInfo.ConvertTensorsInfoFromHandle(inHandle);
            return retInfo;
        }

        public void SetInformation(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

            if (string.IsNullOrEmpty(value))
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property value is invalid");

            MLServiceError ret = Interop.Service.SetInformation(_handle, name, value);
            MLService.CheckException(ret, "Failed to set service information");
        }

        public string GetInformation(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

            IntPtr value = IntPtr.Zero;
            MLServiceError ret = Interop.Service.GetInformation(_handle, name, out value);
            MLService.CheckException(ret, "Failed to get service information");

            return Interop.Util.IntPtrToString(value);
        }

        public void Request(string name, TensorsData data)
        {
            if (string.IsNullOrEmpty(name))
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

            if (data == null)
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "Given data is invalid");

            MLServiceError ret = Interop.Service.Request(_handle, name, data.GetHandle());
            MLService.CheckException(ret, "Failed to request service");
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
                MLServiceError ret = Interop.Service.Destroy(_handle);
                if (ret != MLServiceError.None)
                    Log.Error(MLService.TAG, "Failed to destroy the service handle");

                _handle = IntPtr.Zero;
            }

            _disposed = true;
        }

        public class Query
        {
            private Service _service;

            public Query(MlInformationList infoList, int index)
            {
                MLService.CheckMLServiceSupport();
                if (infoList == null)
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The information list is invalid");

                IntPtr infoHandle = infoList.GetInformationHandle(index);
                if (infoHandle == null)
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The information handle is invalid");

                IntPtr handle = IntPtr.Zero;
                MLServiceError ret = Interop.Service.CreateQuery(infoHandle, out handle);
                MLService.CheckException(ret, "Failed to create service query instance");

                _service = new Service(handle);
            }

            public TensorsData Request(TensorsData input)
            {
                if (input == null)
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "Given input is invalid");

                IntPtr outputPtr = IntPtr.Zero;
                MLServiceError ret = Interop.Service.RequestQuery(_service.GetHandle(), input.GetHandle(), out outputPtr);
                MLService.CheckException(ret, "Failed to request query");

                return TensorsData.CreateFromNativeHandle(outputPtr, IntPtr.Zero, true);
            }
        }

        public class Pipeline
        {
            private Service _service;

            public Pipeline(string name)
            {
                MLService.CheckMLServiceSupport();
                IntPtr handle = IntPtr.Zero;
                MLServiceError ret = Interop.Service.LaunchPipeline(name, out handle);
                MLService.CheckException(ret, "Failed to create service pipeline instance");

                _service = new Service(handle);
            }

            public void Start()
            {
                if (_service != null)
                    _service.Start();
            }

            public void Stop()
            {
                if (_service != null)
                    _service.Stop();
            }

            static public void Set(string name, string desc)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                if (string.IsNullOrEmpty(desc))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property desc is invalid");

                MLServiceError ret = Interop.Service.SetPipeline(name, desc);
                MLService.CheckException(ret, "Failed to create service pipeline");
            }

            static public string Get(string name)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                IntPtr value = IntPtr.Zero;
                MLServiceError ret = Interop.Service.GetPipeline(name, out value);
                MLService.CheckException(ret, "Failed to get service pipeline");

                return Interop.Util.IntPtrToString(value);
            }

            static public void Delete(string name)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                MLServiceError ret = Interop.Service.DeletePipeline(name);
                MLService.CheckException(ret, "Failed to delete service pipeline");
            }

            public PipelineState GetState()
            {
                MLService.CheckMLServiceSupport();

                int state = 0;
                MLServiceError ret = Interop.Service.GetPipelineState(_service.GetHandle(), out state);
                if (ret == MLServiceError.None && state == 0)
                    ret = MLServiceError.InvalidOperation;

                MLService.CheckException(ret, "Failed to get service pipeline state");

                return (PipelineState) state;
            }
        }

        public class Model
        {
            static public int Register(string name, string path, bool activate = false, string description = "")
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                if (string.IsNullOrEmpty(path))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property path is invalid");

                int version;
                MLServiceError ret = Interop.Service.RegisterModel(name, path, activate, description, out version);
                MLService.CheckException(ret, "Failed to register model");

                return version;
            }

            static public void UpdateDescription(string name, int version, string description)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                if (string.IsNullOrEmpty(description))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property description is invalid");

                MLServiceError ret = Interop.Service.UpdateModelDescription(name, version, description);
                MLService.CheckException(ret, "Failed to update model description");
            }

            static public void Activate(string name, int version, string description)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                MLServiceError ret = Interop.Service.ActivateModel(name, version);
                MLService.CheckException(ret, "Failed to activate the given model");
            }

            static public MlInformation Get(string name, int version)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                IntPtr info = IntPtr.Zero;
                MLServiceError ret = Interop.Service.GetModel(name, version, out info);
                MLService.CheckException(ret, "Failed to get the given model");

                MlInformation result = new MlInformation(info);
                return result;
            }

            static public MlInformation GetActivated(string name)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                IntPtr info = IntPtr.Zero;
                MLServiceError ret = Interop.Service.GetActivatedModel(name, out info);
                MLService.CheckException(ret, "Failed to get the given activated model");

                MlInformation result = new MlInformation(info);
                return result;
            }

            static public MlInformationList GetAll(string name)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                IntPtr info = IntPtr.Zero;
                MLServiceError ret = Interop.Service.GetAllModel(name, out info);
                MLService.CheckException(ret, "Failed to get the given models");

                MlInformationList result = new MlInformationList(info);
                return result;
            }

            static public void Delete(string name, int version)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                MLServiceError ret = Interop.Service.DeleteModel(name, version);
                MLService.CheckException(ret, "Failed to delete the given model");
            }
        }

        public class Resource
        {
            static public void Add(string name, string path, string description="")
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                if (string.IsNullOrEmpty(path))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property path is invalid");

                MLServiceError ret = Interop.Service.AddResource(name, path, description);
                MLService.CheckException(ret, "Failed to add resource");
            }

            static public void Delete(string name)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                MLServiceError ret = Interop.Service.DeleteResource(name);
                MLService.CheckException(ret, "Failed to delete resource");
            }

            static public MlInformationList Get(string name)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                IntPtr infoList = IntPtr.Zero;
                MLServiceError ret = Interop.Service.GetResource(name, out infoList);
                MLService.CheckException(ret, "Failed to get resource");

                return new MlInformationList(infoList);
            }
        }
    }
}
