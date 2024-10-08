/*
 * Copyright (c) 2021 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using System.ComponentModel;

namespace Tizen.NUI
{
    /// <summary>
    /// Event arguments that passed via the WebView.ResponsePolicyDecided,
    /// WebView.NavigationPolicyDecided or WebView.NewWindowPolicyDecided.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class WebViewPolicyDecidedEventArgs : EventArgs
    {
        internal WebViewPolicyDecidedEventArgs(WebPolicyDecisionMaker maker)
        {
            PolicyDecisionMaker = maker;
        }

        /// <summary>
        /// Deprecated. The response policy decision maker.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public WebPolicyDecisionMaker ResponsePolicyDecisionMaker
        {
            get
            {
                return PolicyDecisionMaker;
            }
        }

        /// <summary>
        /// The policy decision maker.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public WebPolicyDecisionMaker PolicyDecisionMaker { get; }
    }
}
