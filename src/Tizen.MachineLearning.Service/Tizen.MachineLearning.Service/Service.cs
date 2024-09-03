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
    /// <summary>
    /// The Service class provides utility interfaces for AI application developers.
    /// </summary>
    /// <since_tizen> 9 </since_tizen>
    public class Service : IDisposable
    {
        private IntPtr _handle = IntPtr.Zero;
        private bool _disposed = false;

        /// <summary>
        /// Event to be invoked when the service receives a callback.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>

        public event EventHandler<ServiceReceivedEventArgs> EventReceived;
        private Interop.Service.EventCallback _eventCallback;

        public enum EventType
        {
            NewData = 1,
        }

        /// <summary>
        /// Creates a Service instance.
        /// </summary>
        /// <param name="config">The path of configuration file.</param>
        /// <feature>http://tizen.org/feature/machine_learning.service</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <since_tizen> 9 </since_tizen>
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

            _eventCallback = (type, event_data_handle, _) =>
            {
                if (type == EventType.NewData && EventReceived!= null) {
                    MlInformation info = new MlInformation(event_data_handle);
                    EventReceived?.Invoke(this, new ServiceReceivedEventArgs(info));
                }
            };

            MLServiceError ret = Interop.Service.SetEventCb(_handle, _eventCallback, IntPtr.Zero);
            MLService.CheckException(ret, "Failed to register event callback");
        }

        /// <summary>
        /// Destroys the MlInformation resource.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>
        ~Service()
        {
            Dispose(false);
        }

        internal IntPtr GetHandle()
        {
            return _handle;
        }

        /// <summary>
        /// Starts the process of service instance.
        /// </summary>
        /// <feature>http://tizen.org/feature/machine_learning.service</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <exception cref="InvalidOperationException">Thrown when failed to start the service.</exception>
        /// <since_tizen> 9 </since_tizen>
        public void Start()
        {
            MLServiceError ret = Interop.Service.Start(_handle);
            MLService.CheckException(ret, "Failed to start the service because of internal error");
        }

        /// <summary>
        /// Stops the process of service instance.
        /// </summary>
        /// <feature>http://tizen.org/feature/machine_learning.service</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <exception cref="InvalidOperationException">Thrown when failed to stop the service.</exception>
        /// <since_tizen> 9 </since_tizen>
        public void Stop()
        {
            MLServiceError ret = Interop.Service.Stop(_handle);
            MLService.CheckException(ret, "Failed to stop the service because of internal error");
        }

        /// <summary>
        /// Gets the TensorsInfo of the service instance.
        /// </summary>
        /// <param name="IsInput">The boolean value for choosing input or output to get.</param>
        /// <param name="name">The name of node in the pipeline. You can set NULL if serivce is constructed from model configuration.</param>
        /// <feature>http://tizen.org/feature/machine_learning.service</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <since_tizen> 9 </since_tizen>
        public TensorsInfo GetTensorsInformation(bool IsInput, string name = "")
        {
            MLServiceError ret = MLServiceError.InvalidParameter;
            IntPtr nameHandle = Interop.Util.StringToIntPtr(name);
            IntPtr inHandle;

            if (IsInput)
                ret = Interop.Service.GetInputInformation(_handle, nameHandle, out inHandle);
            else
                ret = Interop.Service.GetOutputInformation(_handle, nameHandle, out inHandle);

            MLService.CheckException(ret, "Failed to get TensorsInfo handle");

            return TensorsInfo.ConvertTensorsInfoFromHandle(inHandle);
        }

        /// <summary>
        /// Sets the information of the service instance.
        /// </summary>
        /// <param name="name">The name to be set.</param>
        /// <param name="value">The value to be set.</param>
        /// <feature>http://tizen.org/feature/machine_learning.service</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <since_tizen> 9 </since_tizen>
        public void SetInformation(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

            if (string.IsNullOrEmpty(value))
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property value is invalid");

            IntPtr valuePtr = Interop.Util.StringToIntPtr(value);
            MLServiceError ret = Interop.Service.SetInformation(_handle, name, valuePtr);
            MLService.CheckException(ret, "Failed to set service information");
        }

        /// <summary>
        /// Gets the information of the service instance.
        /// </summary>
        /// <param name="name">The name to get the value.</param>
        /// <returns>The string value of the name.</returns>
        /// <feature>http://tizen.org/feature/machine_learning.service</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <since_tizen> 9 </since_tizen>
        public string GetInformation(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

            IntPtr value = IntPtr.Zero;
            MLServiceError ret = Interop.Service.GetInformation(_handle, name, out value);
            MLService.CheckException(ret, "Failed to get service information");

            return Interop.Util.IntPtrToString(value);
        }

        /// <summary>
        /// Adds an input data to process the model in service instance.
        /// </summary>
        /// <param name="data">The data to be processed.</param>
        /// <param name="name">The name of node in the pipeline. You can set NULL if serivce is constructed from model configuration.</param>
        /// <feature>http://tizen.org/feature/machine_learning.service</feature>
        /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
        /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
        /// <exception cref="InvalidOperationException">Thrown when failed to stop the service.</exception>
        /// <since_tizen> 9 </since_tizen>
        public void Request(TensorsData data, string name = "")
        {
            if (data == null)
                throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "Given data is invalid");

            IntPtr nameHandle = Interop.Util.StringToIntPtr(name);
            MLServiceError ret = Interop.Service.Request(_handle, nameHandle, data.GetHandle());
            MLService.CheckException(ret, "Failed to request service");
        }

        /// <summary>
        /// Releases any unmanaged resources used by this object.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases any unmanaged resources used by this object including opened handle.
        /// </summary>
        /// <param name="disposing">If true, disposes any disposable objects. If false, does not dispose disposable objects.</param>
        /// <since_tizen> 9 </since_tizen>
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

        /// <summary>
        /// The Query class provides service based on MlInformation.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>
        public class Query
        {
            private Service _service;

            /// <summary>
            /// Creates a Query instance.
            /// </summary>
            /// <param name="information">The information used for creating query service.</param>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
            public Query(MlInformation information)
            {
                MLService.CheckMLServiceSupport();

                if (information == null)
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The information is invalid");

                IntPtr handle = IntPtr.Zero;
                MLServiceError ret = Interop.Service.CreateQuery(information.GetHandle(), out handle);
                MLService.CheckException(ret, "Failed to create service query instance");

                _service = new Service(handle);
            }

            /// <summary>
            /// Requests the query service to process the input and produce an output.
            /// </summary>
            /// <param name="input">The input tensors.</param>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
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

        /// <summary>
        /// The Pipeline class provides service based on AI Pipeline.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>
        public class Pipeline
        {
            private Service _service;

            /// <summary>
            /// Creates a Pipeline instance.
            /// </summary>
            /// <param name="name">The service name.</param>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
            public Pipeline(string name)
            {
                MLService.CheckMLServiceSupport();

                IntPtr handle = IntPtr.Zero;
                MLServiceError ret = Interop.Service.LaunchPipeline(name, out handle);
                MLService.CheckException(ret, "Failed to create service pipeline instance");

                _service = new Service(handle);
            }

            /// <summary>
            /// Starts the pipeline instance.
            /// </summary>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <exception cref="InvalidOperationException">Thrown when failed to start the service.</exception>
            /// <since_tizen> 9 </since_tizen>
            public void Start()
            {
                if (_service != null)
                    _service.Start();
            }

            /// <summary>
            /// Stops the pipeline instance.
            /// </summary>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <exception cref="InvalidOperationException">Thrown when failed to start the service.</exception>
            /// <since_tizen> 9 </since_tizen>
            public void Stop()
            {
                if (_service != null)
                    _service.Stop();
            }

            /// <summary>
            /// Sets the pipeline description with a given name.
            /// </summary>
            /// <param name="name">Unique name to retrieve the associated pipeline description.</param>
            /// <param name="desc">The pipeline description to be stored.</param>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
            static public void Set(string name, string desc)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                if (string.IsNullOrEmpty(desc))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property desc is invalid");

                IntPtr descPtr = Interop.Util.StringToIntPtr(desc);
                MLServiceError ret = Interop.Service.SetPipeline(name, descPtr);
                MLService.CheckException(ret, "Failed to create service pipeline");
            }

            /// <summary>
            /// Gets the pipeline description with a given name.
            /// </summary>
            /// <param name="name">The unique name to retrieve.</param>
            /// <returns>The pipeline corresponding with the given name.</returns>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
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

            /// <summary>
            /// Deletes the pipeline description with a given name.
            /// </summary>
            /// <param name="name">The unique name to delete.</param>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
            static public void Delete(string name)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                MLServiceError ret = Interop.Service.DeletePipeline(name);
                MLService.CheckException(ret, "Failed to delete service pipeline");
            }

            /// <summary>
            /// Gets the state of the pipeline instance.
            /// </summary>
            /// <returns>The state of pipeline.</returns>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
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

        /// <summary>
        /// The Model class manages the information of a neural network model.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>
        public class Model
        {
            /// <summary>
            /// Registers new information of a neural network model.
            /// </summary>
            /// <param name="name">The unique name to indicate the model.</param>
            /// <param name="path">The path to neural network model.</param>
            /// <param name="activate">The flag to set the model to be activated. (Default: false)</param>
            /// <param name="description">The description for neural network model.</param>
            /// <returns>The version of registered model.</returns>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
            static public int Register(string name, string path, bool activate = false, string description = "")
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                if (string.IsNullOrEmpty(path))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property path is invalid");

                int version = 0;
                MLServiceError ret = Interop.Service.RegisterModel(name, path, activate, description, out version);
                MLService.CheckException(ret, "Failed to register model");

                return version;
            }

            /// <summary>
            /// Updates the description of neural network model.
            /// </summary>
            /// <param name="name">The unique name to indicate the model.</param>
            /// <param name="version">The version of registered model.</param>
            /// <param name="description">The description for neural network model to update.</param>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
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

            /// <summary>
            /// Activates a neural network model.
            /// </summary>
            /// <param name="name">The unique name to indicate the model.</param>
            /// <param name="version">The version of registered model.</param>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
            static public void Activate(string name, int version)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                MLServiceError ret = Interop.Service.ActivateModel(name, version);
                MLService.CheckException(ret, "Failed to activate the given model");
            }

            /// <summary>
            /// Gets the information of neural network model.
            /// </summary>
            /// <param name="name">The unique name to indicate the model.</param>
            /// <param name="version">The version of registered model.</param>
            /// <returns>The mlInformation of model.</returns>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
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

            /// <summary>
            /// Gets the information of activated neural network model.
            /// </summary>
            /// <param name="name">The unique name to indicate the model.</param>
            /// <returns>The information of activated model.</returns>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
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

            /// <summary>
            /// Gets the list of neural network model.
            /// </summary>
            /// <param name="name">The unique name to indicate the model.</param>
            /// <returns>The list of registered models.</returns>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
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

            /// <summary>
            /// Deletes a model information.
            /// </summary>
            /// <param name="name">The unique name to indicate the model.</param>
            /// <param name="version">The version of registered model.</param>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
            static public void Delete(string name, int version)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                MLServiceError ret = Interop.Service.DeleteModel(name, version);
                MLService.CheckException(ret, "Failed to delete the given model");
            }
        }


        /// <summary>
        /// The Resource class manages AI data files.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>
        public class Resource
        {
            /// <summary>
            /// Adds new information of machine learning resources those contain images, audio samples, binary files, and so on.
            /// </summary>
            /// <param name="name">The unique name to indicate the resources.</param>
            /// <param name="path">The path to machine learning resources..</param>
            /// <param name="description">The vdescription for machine learning resources.</param>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
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

            /// <summary>
            /// Deletes the information of the resources from machine learning service.
            /// </summary>
            /// <param name="name">The unique name to indicate the resources.</param>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
            static public void Delete(string name)
            {
                MLService.CheckMLServiceSupport();

                if (string.IsNullOrEmpty(name))
                    throw MLServiceExceptionFactory.CreateException(MLServiceError.InvalidParameter, "The property name is invalid");

                MLServiceError ret = Interop.Service.DeleteResource(name);
                MLService.CheckException(ret, "Failed to delete resource");
            }

            /// <summary>
            /// Gets the information of the resources.
            /// </summary>
            /// <param name="name">The unique name to indicate the resources.</param>
            /// <returns>The list of the resources with a given name.</returns>
            /// <feature>http://tizen.org/feature/machine_learning.service</feature>
            /// <exception cref="ArgumentException">Thrown when the method failed due to an invalid parameter.</exception>
            /// <exception cref="NotSupportedException">Thrown when the feature is not supported.</exception>
            /// <since_tizen> 9 </since_tizen>
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
