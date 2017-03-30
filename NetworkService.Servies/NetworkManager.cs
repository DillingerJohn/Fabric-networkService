using NetworkService.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Services
{
    public class NetworkManager : INetworkManager
    {

        private WMIManager _wmiManager;
        public NetworkManager(WMIManager wmiManager)
        {
            _wmiManager = wmiManager;
        }

        /// <summary>
        /// Returns the list of Network Interfaces installed
        /// </summary>
        /// <param name="ipEnabled"> true(default) - returns only ipEnabled(network cards) devices</param>
        public object GetDevices(bool ipEnabled = true)
        {
            return _wmiManager.GetDevices(ipEnabled);
            //return WMIManager.GetDevices(ipEnabled);
        }

        /// <summary>
        /// Returns the list of Network Interfaces installed
        /// </summary>
        /// <param name="ipEnabled"> true(default) - returns only ipEnabled(network cards) devices</param>
        public async Task<object> GetDevicesAsync(bool ipEnabled = true)
        {
            return await _wmiManager.GetDevicesAsync(ipEnabled);
        }

        /// <summary>
        /// Loads current tcp network configuration for the specified NIC
        /// </summary>
        /// <param name="deviceName"></param>
        public async Task<WMIAdapter> GetDeviceConfigurationAsync(string deviceName)
        {
            return await _wmiManager.GetIPAsync(deviceName);
        }
        public WMIAdapter GetDeviceConfiguration(string deviceName)
        {
            return _wmiManager.GetIP(deviceName);
        }

        /// <summary>
        /// Set tcp network configuration for the specified NIC
        /// </summary>
        /// <param name="deviceName, IpAddresses, SubnetMask, Gateway, Dns"></param>
        public async Task<WMIAdapter> SetDeviceConfigurationAsync(string deviceName, string IpAddresses, string SubnetMask, string Gateway, string Dns)
        {
            return await _wmiManager.SetIPAsync(deviceName, IpAddresses, SubnetMask, Gateway, Dns);
        }
        public WMIAdapter SetDeviceConfiguration(string deviceName, string IpAddresses, string SubnetMask, string Gateway, string Dns)
        {
            return  _wmiManager.SetIp(deviceName, IpAddresses, SubnetMask, Gateway, Dns);
        }



        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _wmiManager = null;
                    //_wmiManager.Dispose();
                    //this.Dispose(disposing);
                    // TODO: dispose managed state (managed objects).
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NetworkService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}