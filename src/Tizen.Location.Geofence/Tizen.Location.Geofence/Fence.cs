﻿/*
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

namespace Tizen.Location.Geofence
{
    /// <summary>
    /// Geo-fence defines a virtual perimeter for a real-world geographic area.
    /// If you create a geofence, you can trigger some activities when a device enters(or exits) the geofences defined by you.
    /// You can create a geofence with the information of Geopoint, Wi-Fi, or BT.
    /// <list >
    /// <item>Geopoint: Geofence is specified by coordinates (Latitude and Longitude) and Radius</item>
    /// <item>WIFI: Geofence is specified by BSSID of Wi-Fi access point</item>
    /// <item>BT: Geofence is specified by Bluetooth address</item>
    /// </list>
    /// Basic service set identification(BSSID) The BSSID is the MAC address of the wireless access point(WAP) generated by combining the 24 bit Organization Unique Identifier(the manufacturer's identity)
    /// and the manufacturer's assigned 24-bit identifier for the radio chipset in the WAP.
    /// </summary>
    public class Fence : IDisposable
    {
        private bool _disposed = false;

        internal IntPtr Handle
        {
            get;
            set;
        }

        internal Fence(IntPtr handle)
        {
            Handle = handle;
        }

        ~Fence()
        {
            Dispose(false);
        }
        /// <summary>
        /// Gets the type of geofence
        /// </summary>
        public FenceType Type
        {
            get
            {
                FenceType val;
                GeofenceError ret = (GeofenceError)Interop.Geofence.FenceType(Handle, out val);
                if (ret != GeofenceError.None)
                {
                    Tizen.Log.Error(GeofenceErrorFactory.LogTag, "Failed to get GeofenceType");
                }

                return val;
            }
        }

        /// <summary>
        /// Gets the id of place.
        /// </summary>
        public int PlaceId
        {
            get
            {
                int result = -1;
                GeofenceError ret = (GeofenceError)Interop.Geofence.FencePlaceID(Handle, out result);
                if (ret != GeofenceError.None)
                {
                    Tizen.Log.Error(GeofenceErrorFactory.LogTag, "Failed to get PlaceId");
                }

                return result;
            }
        }

        /// <summary>
        ///Gets the longitude of geofence.
        /// </summary>
        public double Longitude
        {
            get
            {
                double result = -1;
                GeofenceError ret = (GeofenceError)Interop.Geofence.FenceLongitude(Handle, out result);
                if (ret != GeofenceError.None)
                {
                    Tizen.Log.Error(GeofenceErrorFactory.LogTag, "Failed to get Longitude");
                }

                return result;

            }
        }

        /// <summary>
        ///Gets the latitude of geofence.
        /// </summary>
        public double Latitude
        {
            get
            {
                double result = -1;
                GeofenceError ret = (GeofenceError)Interop.Geofence.FenceLatitude(Handle, out result);
                if (ret != GeofenceError.None)
                {
                    Tizen.Log.Error(GeofenceErrorFactory.LogTag, "Failed to get Latitude");
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the radius of geofence.
        /// </summary>
        public int Radius
        {
            get
            {
                int result = -1;
                GeofenceError ret = (GeofenceError)Interop.Geofence.FenceRadius(Handle, out result);
                if (ret != GeofenceError.None)
                {
                    Tizen.Log.Error(GeofenceErrorFactory.LogTag, "Failed to get Radius");
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the address of geofence.
        /// </summary>
        public string Address
        {
            get
            {
                string result = "";
                GeofenceError ret = (GeofenceError)Interop.Geofence.FenceAddress(Handle, out result);
                if (ret != GeofenceError.None)
                {
                    Tizen.Log.Error(GeofenceErrorFactory.LogTag, "Failed to get Adress");
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the bssid of geofence.
        /// </summary>
        public string Bssid
        {
            get
            {
                string result = "";
                GeofenceError ret = (GeofenceError)Interop.Geofence.FenceBSSID(Handle, out result);
                if (ret != GeofenceError.None)
                {
                    Tizen.Log.Error(GeofenceErrorFactory.LogTag, "Failed to get Bssid");
                }

                return result;
            }
        }

        /// <summary>
        ///Gets the ssid of geofence.
        /// </summary>
        public string Ssid
        {
            get
            {
                string result = "";
                GeofenceError ret = (GeofenceError)Interop.Geofence.FenceSSID(Handle, out result);
                if (ret != GeofenceError.None)
                {
                    Tizen.Log.Error(GeofenceErrorFactory.LogTag, "Failed to get Ssid");
                }

                return result;
            }
        }

        /// <summary>
        /// Creates a geopoint type of new geofence.
        /// </summary>
        /// <param name="placeId">The current place id</param>
        /// <param name="latitude">Specifies the value of latitude of geofence [-90.0 ~ 90.0] (degrees) </param>
        /// <param name="longitude">Specifies the value of longitude of geofence [-180.0 ~ 180.0] (degrees) </param>
        /// <param name="radius">Specifies the value of radius of geofence [100 ~ 500](meter) </param>
        /// <param name="adsress">Specifies the value of address</param>
        /// <returns>Newly created geofence instance </returns>
        /// <exception cref="ArgumentException">Incase of Invalid parameter</exception>
        /// <exception cref="InvalidOperationException">Incase of any System error</exception>
        /// <exception cref="UnauthorizedAccessException">Incase of Pvivileges are not defined</exception>
        /// <exception cref="NotSupportedException">Incase of Geofence is not supported</exception>
        public static Fence CreateGPSFence(int placeId, int latitude, int longitude, int radius, string address)
        {
            IntPtr handle = IntPtr.Zero;
            GeofenceError ret = (GeofenceError)Interop.Geofence.CreateGPSFence(placeId, latitude, longitude, radius,address, out handle);
            if (ret != GeofenceError.None)
            {
                throw GeofenceErrorFactory.CreateException(ret, "Failed to create Geofence from GPS Data for " + placeId);
            }

            return new Fence(handle);
        }

        /// <summary>
        /// Creates a Wi-Fi type of new geofence.
        /// </summary>
        /// <param name="placeId">The current place id </param>
        /// <param name="bssid">Specifies the value of BSSID of Wi-Fi MAC address</param>
        /// <param name="ssid"> Specifies the value of SSID of Wi-Fi Device </param>
        /// <returns>Newly created geofence instance </returns>
        /// <exception cref="ArgumentException">Incase of Invalid parameter</exception>
        /// <exception cref="InvalidOperationException">Incase of any System error</exception>
        /// <exception cref="UnauthorizedAccessException">Incase of Pvivileges are not defined</exception>
        /// <exception cref="NotSupportedException">Incase of Geofence is not supported</exception>
        public static Fence CreateWifiFence(int placeId, string bssid, string ssid)
        {
            IntPtr handle = IntPtr.Zero;
            GeofenceError ret = (GeofenceError)Interop.Geofence.CreateWiFiFence(placeId, bssid, ssid, out handle);
            if (ret != GeofenceError.None)
            {
                throw GeofenceErrorFactory.CreateException(ret, "Failed to create Geofence from Wifi Data for " + placeId);
            }

            return new Fence(handle);
        }

        /// <summary>
        /// Creates a bluetooth type of new geofence.
        /// </summary>
        /// <param name="placeId">The current place id </param>
        /// <param name="bssid">Specifies the value of BSSID of BT MAC address</param>
        /// <param name="ssid"> Specifies the value of SSID of BT Device </param>
        /// <returns>Newly created geofence instance </returns>
        /// <exception cref="ArgumentException">Incase of Invalid parameter</exception>
        /// <exception cref="InvalidOperationException">Incase of any System error</exception>
        /// <exception cref="UnauthorizedAccessException">Incase of Pvivileges are not defined</exception>
        /// <exception cref="NotSupportedException">Incase of Geofence is not supported</exception>
        public static Fence CreateBTFence(int placeId, string bssid, string ssid)
        {
            IntPtr handle = IntPtr.Zero;
            GeofenceError ret = (GeofenceError)Interop.Geofence.CreateBTFence(placeId, bssid, ssid, out handle);
            if (ret != GeofenceError.None)
            {
                throw GeofenceErrorFactory.CreateException(ret, "Failed to create Geofence from Bluetooth Data for " + placeId);
            }

            return new Fence(handle);
        }

        /// <summary>
        /// Overloaded Dispose API for destroying the fence Handle.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (Handle != IntPtr.Zero)
            {
                Interop.Geofence.Destroy(Handle);
                Handle = IntPtr.Zero;
            }

            _disposed = true;
        }
    }
}
