/*
* Copyright (c) 2019 Samsung Electronics Co., Ltd. All Rights Reserved.
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
using System.Runtime.InteropServices;
using Tizen.MachineLearning.Service;
using Tizen.MachineLearning.Inference;

internal static partial class Interop
{
    internal static partial class Libraries
    {
        public const string MlService = "libcapi-ml-service.so.1";
    }

    internal static partial class Service
    {
        /* typedef void (*ml_service_event_cb) (ml_service_event_e event, ml_information_h event_data, void *user_data); */
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ServiceEventCallback(Tizen.MachineLearning.Service.Service.EventType event_type, IntPtr event_data, IntPtr user_data);

        /* int ml_service_new (const char *config, ml_service_h *handle); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_new", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError Create(string config, out IntPtr handle);

        /* int ml_service_set_event_cb (ml_service_h handle, ml_service_event_cb cb, void *user_data); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_set_event_cb", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError SetEventCb(IntPtr handle, ServiceEventCallback cb, IntPtr userData);

        /* int ml_service_start (ml_service_h handle); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_start", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError Start(IntPtr handle);

        /* int ml_service_stop (ml_service_h handle); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_stop", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError Stop(IntPtr handle);

        /* int ml_service_get_input_information (ml_service_h handle, const char *name, ml_tensors_info_h *info); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_get_input_information", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError GetInputInformation(IntPtr handle, string name, out IntPtr info);

        /* int ml_service_get_output_information (ml_service_h handle, const char *name, ml_tensors_info_h *info); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_get_output_information", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError GetOutputInformation(IntPtr handle, string name, out IntPtr info);

        /* int ml_service_set_information (ml_service_h handle, const char *name, const char *value); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_set_information", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError SetInformation(IntPtr handle, string name, string value);

        /* int ml_service_get_information (ml_service_h handle, const char *name, char **value); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_get_information", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError GetInformation(IntPtr handle, string name, out IntPtr value);

        /* int ml_service_request (ml_service_h handle, const char *name, const ml_tensors_data_h data); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_request", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError Request(IntPtr handle, string name, IntPtr data);

        /* int ml_service_destroy (ml_service_h handle); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_destroy", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError Destroy(IntPtr handle);

        /* int ml_service_pipeline_set (const char *name, const char *pipeline_desc); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_pipeline_set", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError SetPipeline(string name, string desc);

        /* int ml_service_pipeline_get (const char *name, char **pipeline_desc); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_pipeline_get", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError GetPipeline(string name, out IntPtr desc);

        /* int ml_service_pipeline_delete (const char *name); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_pipeline_delete", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError DeletePipeline(string name);

        /* int ml_service_pipeline_launch (const char *name, ml_service_h *handle); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_pipeline_launch", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError LaunchPipeline(string name, out IntPtr handle);

        /* int ml_service_pipeline_get_state (ml_service_h handle, ml_pipeline_state_e *state); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_pipeline_get_state", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError GetPipelineState(IntPtr handle, out int state);

        /* int ml_service_query_create (ml_option_h option, ml_service_h *handle); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_query_create", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError CreateQuery(IntPtr option, out IntPtr handle);

        /* int ml_service_query_request (ml_service_h handle, const ml_tensors_data_h input, ml_tensors_data_h *output); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_query_request", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError RequestQuery(IntPtr handle, IntPtr input, out IntPtr output);

        /* int ml_service_model_register (const char *name, const char *path, const bool activate, const char *description, unsigned int *version); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_model_register", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError RegisterModel(string name, string path, bool activate, string description, out int version);

        /* int ml_service_model_update_description (const char *name, const unsigned int version, const char *description); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_model_update_description", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError UpdateModelDescription(string name, int version, string description);

        /* int ml_service_model_activate (const char *name, const unsigned int version); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_model_activate", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError ActivateModel(string name, int version);

        /* int ml_service_model_get (const char *name, const unsigned int version, ml_information_h *info); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_model_get", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError GetModel(string name, int version, out IntPtr info);

        /* int ml_service_model_get_activated (const char *name, ml_information_h *info); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_model_get_activated", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError GetActivatedModel(string name, out IntPtr info);

        /* int ml_service_model_get_all (const char *name, ml_information_list_h *info_list); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_model_get_all", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError GetAllModel(string name, out IntPtr info_list);

        /* int ml_service_model_delete (const char *name, const unsigned int version); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_model_delete", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError DeleteModel(string name, int version);

        /* int ml_service_resource_add (const char *name, const char *path, const char *description);*/
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_resource_add", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError AddResource(string name, string path, string description);

        /* int ml_service_resource_delete (const char *name); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_resource_delete", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError DeleteResource(string name);

        /* int ml_service_resource_get (const char *name, ml_information_list_h *res); */
        [DllImport(Libraries.MlService, EntryPoint = "ml_service_resource_get", CallingConvention = CallingConvention.Cdecl)]
        internal static extern MLServiceError GetResource(string name, out IntPtr res);
    }

    internal static partial class Util
    {
        internal static string IntPtrToString(IntPtr val)
        {
            return (val != IntPtr.Zero) ? Marshal.PtrToStringAnsi(val) : string.Empty;
        }
    }
}
