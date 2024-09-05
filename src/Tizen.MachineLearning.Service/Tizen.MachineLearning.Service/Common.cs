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
using System.IO;
using Tizen.System;

namespace Tizen.MachineLearning.Service
{
    internal enum NNServiceError
    {
        None = Tizen.Internals.Errors.ErrorCode.None,
        InvalidParameter = Tizen.Internals.Errors.ErrorCode.InvalidParameter,
        Unknown = Tizen.Internals.Errors.ErrorCode.Unknown,
        NotSupported = Tizen.Internals.Errors.ErrorCode.NotSupported,
        PermissionDenied = Tizen.Internals.Errors.ErrorCode.PermissionDenied,
        InvalidOperation = Tizen.Internals.Errors.ErrorCode.InvalidOperation,
    }

    internal static class NNService
    {
        internal const string TAG = "ML.Service";

        internal const string FeatureKey = "http://tizen.org/feature/machine_learning.service";

        private static int _alreadyChecked = -1;    /* -1: not yet, 0: Not Support, 1: Support */

        internal static void CheckException(NNServiceError error, string msg)
        {
            if (error != NNServiceError.None)
            {
                Log.Error(NNService.TAG, msg + ": " + error.ToString());
                throw NNServiceExceptionFactory.CreateException(error, msg);
            }
        }

        internal static void CheckNNServiceSupport()
        {
            if (_alreadyChecked == 1)
                return;

            string msg = "Machine Learning Service Feature is not supported.";
            if (_alreadyChecked == 0)
            {
                Log.Error(NNService.TAG, msg);
                throw NNServiceExceptionFactory.CreateException(NNServiceError.NotSupported, msg);
            }

            /* Feature Key check */
            bool isSupported = false;
            bool error = Information.TryGetValue<bool>(FeatureKey, out isSupported);
            if (!error || !isSupported)
            {
                _alreadyChecked = 0;

                Log.Error(MLService.TAG, msg);
                throw MLServiceExceptionFactory.CreateException(MLServiceError.NotSupported, msg);
            }

            _alreadyChecked = 1;
        }
    }

    internal class NNServiceExceptionFactory
    {
        internal static Exception CreateException(NNServiceError err, string msg)
        {
            Exception exp;

            switch (err)
            {
                case NNServiceError.InvalidParameter:
                    exp = new ArgumentException(msg);
                    break;

                case NNServiceError.NotSupported:
                    exp = new NotSupportedException(msg);
                    break;

                case NNServiceError.PermissionDenied:
                    exp = new UnauthorizedAccessException(msg);
                    break;

                default:
                    exp = new InvalidOperationException(msg);
                    break;
            }
            return exp;
        }
    }
}
