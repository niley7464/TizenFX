/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InteropFace = Interop.MediaVision.Face;
using InteropInference = Interop.MediaVision.Inference;

namespace Tizen.Multimedia.Vision
{
    /// <summary>
    /// Provides the ability to detect faces on image sources.
    /// </summary>
    /// <remarks>
    /// If you want to use face detection based on inference engine(<see cref="InferenceBackendType"/>),
    /// please use <see cref="DetectAsync(MediaVisionSource, InferenceModelConfiguration)"/>.
    /// </remarks>
    /// <since_tizen> 4 </since_tizen>
    [Obsolete("Deprecated since API12. Will be removed in API15.")]
    public static class FaceDetector
    {
        /// <summary>
        /// Detects faces on the source.<br/>
        /// Each time when DetectAsync is called, a set of the detected faces at the media source are received asynchronously.
        /// </summary>
        /// <param name="source">The source of the media where faces will be detected.</param>
        /// <returns>A task that represents the asynchronous detect operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="NotSupportedException">
        ///     The feature is not supported.<br/>
        ///     -or-<br/>
        ///     The format of <paramref name="source"/> is not supported.
        /// </exception>
        /// <feature>http://tizen.org/feature/vision.face_recognition</feature>
        /// <since_tizen> 4 </since_tizen>
        [Obsolete("Deprecated since API12. Will be removed in API15.")]
        public static async Task<Rectangle[]> DetectAsync(MediaVisionSource source)
        {
            return await DetectAsync(source, (FaceDetectionConfiguration)null);
        }

        /// <summary>
        /// Detects faces on the source.<br/>
        /// Each time when DetectAsync is called, a set of the detected faces at the media source are received asynchronously.
        /// </summary>
        /// <param name="source">The source of the media where faces will be detected.</param>
        /// <param name="config">The configuration of engine will be used for detecting. This value can be null.</param>
        /// <returns>A task that represents the asynchronous detect operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="NotSupportedException">The feature is not supported.</exception>
        /// <feature>http://tizen.org/feature/vision.face_recognition</feature>
        /// <since_tizen> 4 </since_tizen>
        [Obsolete("Deprecated since API12. Will be removed in API15.")]
        public static async Task<Rectangle[]> DetectAsync(MediaVisionSource source,
            FaceDetectionConfiguration config)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            TaskCompletionSource<Rectangle[]> tcs = new TaskCompletionSource<Rectangle[]>();

            using (var cb = ObjectKeeper.Get(GetCallback(tcs)))
            {
                InteropFace.Detect(source.Handle, EngineConfiguration.GetHandle(config), cb.Target).
                    Validate("Failed to perform face detection");

                return await tcs.Task;
            }
        }

        private static InteropFace.DetectedCallback GetCallback(TaskCompletionSource<Rectangle[]> tcs)
        {
            return (IntPtr sourceHandle, IntPtr engineConfig, global::Interop.MediaVision.Rectangle[] facesLocations,
                int numberOfFaces, IntPtr _) =>
            {
                try
                {
                    Log.Info(MediaVisionLog.Tag, $"Faces detected, count : {numberOfFaces}.");
                    Rectangle[] locations = new Rectangle[numberOfFaces];
                    for (int i = 0; i < numberOfFaces; i++)
                    {
                        locations[i] = facesLocations[i].ToApiStruct();
                        Log.Info(MediaVisionLog.Tag, $"Face {0} detected : {locations}.");
                    }

                    if (!tcs.TrySetResult(locations))
                    {
                        Log.Error(MediaVisionLog.Tag, "Failed to set face detection result.");
                    }
                }
                catch (Exception e)
                {
                    MultimediaLog.Info(MediaVisionLog.Tag, "Failed to handle face detection.", e);
                    tcs.TrySetException(e);
                }
            };
        }

        /// <summary>
        /// Detects faces on the source image using inference engine set in <paramref name="config"/>.<br/>
        /// Each time when DetectAsync is called, a set of the detected faces at the media source are received asynchronously.
        /// </summary>
        /// <remarks>
        /// If there's no detected face, empty collection will be returned.
        /// </remarks>
        /// <feature>http://tizen.org/feature/vision.inference</feature>
        /// <feature>http://tizen.org/feature/vision.inference.face</feature>
        /// <param name="source">The source of the media where faces will be detected.</param>
        /// <param name="config">The engine's configuration that will be used for detecting.</param>
        /// <returns>
        /// A task that represents the asynchronous detect operation.<br/>
        /// If there's no detected face, empty collection will be returned.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="config"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Internal error.</exception>
        /// <exception cref="NotSupportedException">The feature is not supported.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller has no required privilege.</exception>
        /// <seealso cref="InferenceModelConfiguration"/>
        /// <since_tizen> 6 </since_tizen>
        [Obsolete("Deprecated since API12. Will be removed in API15.")]
        public static async Task<IEnumerable<FaceDetectionResult>> DetectAsync(MediaVisionSource source,
            InferenceModelConfiguration config)
        {
            // `vision.inference` feature is already checked, when config is created.
            ValidationUtil.ValidateFeatureSupported(VisionFeatures.InferenceFace);

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var tcs = new TaskCompletionSource<IEnumerable<FaceDetectionResult>>();

            using (var cb = ObjectKeeper.Get(GetCallback(tcs)))
            {
                InteropInference.DetectFace(source.Handle, config.GetHandle(), cb.Target).
                    Validate("Failed to detect face.");

                return await tcs.Task;
            }
        }

        private static InteropInference.FaceDetectedCallback GetCallback(TaskCompletionSource<IEnumerable<FaceDetectionResult>> tcs)
        {
            return (IntPtr sourceHandle, int numberOfFaces, float[] confidences,
                global::Interop.MediaVision.Rectangle[] locations, IntPtr _) =>
            {
                try
                {
                    if (!tcs.TrySetResult(GetResults(numberOfFaces, confidences, locations)))
                    {
                        Log.Error(MediaVisionLog.Tag, "Failed to set face detection result.");
                    }
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }
            };
        }

        private static IEnumerable<FaceDetectionResult> GetResults(int number, float[] confidences,
            global::Interop.MediaVision.Rectangle[] locations)
        {
            if (number == 0)
            {
                return Enumerable.Empty<FaceDetectionResult>();
            }

            var results = new List<FaceDetectionResult>();

            for (int i = 0; i < number; i++)
            {
                results.Add(new FaceDetectionResult(confidences[i], locations[i]));
            }

            return results;
        }
    }
}
