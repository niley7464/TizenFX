/*
 *  Copyright (c) 2024 Samsung Electronics Co., Ltd All Rights Reserved
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License
 */

using System;

namespace Tizen.Security.WebAuthn
{
    /// <summary>
    /// Callback function list used to get assertion with <see cref="Authenticator.GetAssertion"/>.
    /// </summary>
    /// <since_tizen> 12 </since_tizen>
    public class GetAssertionCallbacks
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAssertionCallbacks"/> class.
        /// </summary>
        /// <remarks>
        /// Provided callbacks MUST NOT THROW.
        /// </remarks>
        /// <since_tizen> 12 </since_tizen>
        /// <param name="qrcodeCallback">
        /// Callback function for displaying QR code.
        /// The qr_contents are encoded as you can see in the encodeQRContents() function of the
        /// FIDO specification:
        /// https://fidoalliance.org/specs/fido-v2.2-rd-20230321/fido-client-to-authenticator-protocol-v2.2-rd-20230321.html#hybrid-qr-initiated.
        /// The qr_contents is encoded like "FIDO:/0254318383..........7406596245".
        /// The image to be displayed shall be created from qr_contents
        /// with media vision API (<see cref="Tizen.Multimedia.Vision.BarcodeGenerator"/>).
        /// If the request does not need to display a QR code
        /// then this callback function won't be invoked.
        /// </param>
        /// <param name="responseCallback">
        /// Callback function for getting the final response.
        /// Invoked when the response for the GetAssertion request need to be returned.
        /// The result of the GetAssertion request may be one of the following:
        ///  * <see cref="WauthnError.None"/> if the request is completed well,
        ///  * <see cref="WauthnError.Canceled"/> if the request is cancelled by a Cancel() request.
        ///  * <see cref="WauthnError.InvalidState"/> if the server entered invalid state. Known causes:
        ///    - proxy issues,
        ///    - reached the limit of credentials stored by the authenticator.
        ///  * <see cref="WauthnError.TimedOut"/> if the request times out. Known causes:
        ///    - authenticator does not respond during state assisted transactions due to
        ///      lack of push notifications support (e.g. missing Google Account).
        /// </param>
        /// <param name="linkedDataCallback">
        /// Callback function for getting the updated linked device data. May be called multiple times.
        /// Invoked when the response for the <see cref="Authenticator.GetAssertion"/> request
        /// needs to be returned. The result of this request may be one of the following:
        ///  * <see cref="WauthnError.None"/> if the request is completed well,
        ///  * <see cref="WauthnError.Canceled"/> if the request is cancelled by a Cancel() request.
        ///  * <see cref="WauthnError.InvalidState"/> if the server entered invalid state. Known causes:
        ///    - proxy issues,
        ///    - reached the limit of credentials stored by the authenticator.
        ///  * <see cref="WauthnError.TimedOut"/> if the request times out. Known causes:
        ///    - authenticator does not respond during state assisted transactions due to
        ///      lack of push notifications support (e.g. missing Google Account).
        /// </param>
        /// <param name="userData">User data to be passed to <see cref="QrcodeCallback"/>, <see cref="ResponseCallback"/> and <see cref="LinkedDataCallback"/>.</param>
        public GetAssertionCallbacks(
            Action<string, object> qrcodeCallback,
            Action<PubkeyCredAssertion, WauthnError, object> responseCallback,
            Action<HybridLinkedData, WauthnError, object> linkedDataCallback,
            object userData)
        {
            QrcodeCallback = qrcodeCallback;
            ResponseCallback = responseCallback;
            LinkedDataCallback = linkedDataCallback;
            UserData = userData;
        }

        /// <summary>
        /// Callback function for displaying QR code.
        /// </summary>
        public Action<string, object> QrcodeCallback { get; init; }
        /// <summary>
        /// Callback function for getting the final response.
        /// </summary>
        public Action<PubkeyCredAssertion, WauthnError, object> ResponseCallback { get; init; }
        /// <summary>
        /// Callback function for getting the updated linked device data.
        /// </summary>
        public Action<HybridLinkedData, WauthnError, object> LinkedDataCallback { get; init; }
        /// <summary>
        /// User data to be passed to <see cref="QrcodeCallback"/>, <see cref="ResponseCallback"/> and <see cref="LinkedDataCallback"/>.
        /// </summary>
        public object UserData { get; init; }
    }
}