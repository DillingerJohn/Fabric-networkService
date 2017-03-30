using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System;

namespace NetworkService.Core.Services
{
    public interface INetworkManager : IDisposable
    {
        Task<object> GetDevicesAsync(bool ipEnabled);
        Task<WMIAdapter> SetDeviceConfigurationAsync(string nicName, string IpAddresses, string SubnetMask, string Gateway, string DnsSearchOrder);
        Task<WMIAdapter> GetDeviceConfigurationAsync(string nicName);
    }
}
