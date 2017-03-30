using System;
using System.Collections;
using System.Management;
using System.Threading.Tasks;



namespace NetworkService.Core.Services
{
	/// <summary>
	/// Class which provides convenient methods to set/get network configurations
	/// configuration
	/// </summary>
	public class WMIManager
	{
        public WMIManager()
        { }
		#region Public
		/// <summary>
		/// Enable DHCP on the NIC
		/// </summary>
		/// <param name="nicName">Name of the NIC</param>
		public void SetDHCP( string nicName )
		{
            
			ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
			ManagementObjectCollection moc = mc.GetInstances();

			foreach(ManagementObject mo in moc)
			{
				// Make sure this is a IP enabled device. Not something like memory card or VM Ware
				if( (bool)mo["IPEnabled"] )
				{
					if( mo["Caption"].Equals( nicName ) )
					{
						ManagementBaseObject newDNS = mo.GetMethodParameters( "SetDNSServerSearchOrder" );
						newDNS[ "DNSServerSearchOrder" ] = null;
						ManagementBaseObject enableDHCP = mo.InvokeMethod( "EnableDHCP", null, null);
						ManagementBaseObject setDNS = mo.InvokeMethod( "SetDNSServerSearchOrder", newDNS, null);
					}
				}
			}
		}

        /// <summary>
        /// Set IP for the specified network card name
        /// </summary>
        /// <param name="deviceName">Caption of the network card</param>
        /// <param name="IpAddresses">Comma delimited string containing one or more IP</param>
        /// <param name="SubnetMask">Subnet mask</param>
        /// <param name="Gateway">Gateway IP</param>
        /// <param name="Dns">Comma delimited DNS IP</param>
        public async Task<WMIAdapter> SetIPAsync( string deviceName, string IpAddresses, string SubnetMask, string Gateway, string Dns)
		{
            return await Task.Run(async () =>
            {
                using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                {
                    using (ManagementObjectCollection moc = mc.GetInstances())
                    {
                        foreach (ManagementObject mo in moc)
                        {
                            // Make sure this is a IP enabled device. Not something like memory card or VM Ware
                            if ((bool)mo["IPEnabled"])
                            {
                                if (mo["Caption"].Equals(deviceName))
                                {
                                    try
                                    {
                                        //ManagementBaseObject newGate = mo.GetMethodParameters("SetGateways");
                                        //newGate["DefaultIPGateway"] = new string[] { "192.168.0.1" };
                                        //newGate["GatewayCostMetric"] = new int[] { 1 };
                                        //ManagementBaseObject newDNS = mo.GetMethodParameters("SetDNSServerSearchOrder");
                                        //newDNS["DNSServerSearchOrder"] = new string[] { "192.168.0.1" };
                                        //ManagementBaseObject setGateways = mo.InvokeMethod("SetGateways", newGate, null);
                                        //ManagementBaseObject setDNS = mo.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                                        ManagementBaseObject newIP = mo.GetMethodParameters("EnableStatic");
                                        if (NetworkConfigValidator.isValidIP(IpAddresses))
                                        {
                                            newIP["IPAddress"] = IpAddresses.Split(',');
                                            var IpSubnetIps = (string[])mo["IPSubnet"];
                                            newIP["SubnetMask"] = IpSubnetIps[0].Split(',');
                                        }

                                        ManagementBaseObject setIP = mo.InvokeMethod("EnableStatic", newIP, null);
                                        Logging.Logger.Instanse.Log("Info", "Setting device configuration: Device: " + deviceName +
                                            " Ip: " + NetworkConfigValidator.ConvertArrayOfIpsToString(IpAddresses.Split(',')));
                                    }
                                    catch (Exception ex)
                                    {
                                       Logging.Logger.Instanse.Log("Warn", "Unable to Set IP : ", ex);
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }       
                return await GetIPAsync(deviceName);
            });
        }
        public WMIAdapter SetIp(string deviceName, string IpAddresses, string SubnetMask, string Gateway, string Dns)
        {
            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                using (ManagementObjectCollection moc = mc.GetInstances())
                {
                    foreach (ManagementObject mo in moc)
                    {
                        // Make sure this is a IP enabled device. Not something like memory card or VM Ware
                        if ((bool)mo["IPEnabled"])
                        {
                            if (mo["Caption"].Equals(deviceName))
                            {
                                try
                                {
                                    ManagementBaseObject newIP = mo.GetMethodParameters("EnableStatic");
                                    if (NetworkConfigValidator.isValidIP(IpAddresses))
                                    {
                                        newIP["IPAddress"] = IpAddresses.Split(',');
                                        var IpSubnetIps = (string[])mo["IPSubnet"];
                                        newIP["SubnetMask"] = IpSubnetIps[0].Split(',');
                                    }

                                    ManagementBaseObject setIP = mo.InvokeMethod("EnableStatic", newIP, null);
                                    Logging.Logger.Instanse.Log("Info", "Setting device configuration: Device: " + deviceName +
                                        " Ip: " + NetworkConfigValidator.ConvertArrayOfIpsToString(IpAddresses.Split(',')));
                                }
                                catch (Exception ex)
                                {
                                    Logging.Logger.Instanse.Log("Warn", "Unable to Set IP : ", ex);
                                }

                                break;
                            }
                        }
                    }
                }
            }
            return GetIP(deviceName);
        }
        



        /// <summary>
        /// Returns the network device configuration of the specified NIC
        /// </summary>
        /// <param name="deviceName">Name of the Network device</param>
        public async Task<WMIAdapter> GetIPAsync(string deviceName)
		{
            return await Task.Run(() =>
            {
                using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                {
                    using (ManagementObjectCollection moc = mc.GetInstances())
                    {
                        WMIAdapter wmiAdapter = new WMIAdapter() { deviceName = deviceName };
                        foreach (ManagementObject mo in moc)
                        {
                            // Make sure this is a IP enabled device. Not something like memory card or VM Ware
                            if ((bool)mo["ipEnabled"])
                            {
                                if (mo["Caption"].Equals(deviceName))
                                {
                                    wmiAdapter.ipAdresses = (string[])mo["IPAddress"];
                                    wmiAdapter.subnets = (string[])mo["IPSubnet"];
                                    wmiAdapter.gateways = (string[])mo["DefaultIPGateway"];
                                    wmiAdapter.dnses = (string[])mo["DNSServerSearchOrder"];

                                    break;
                                }
                            }
                        }
                        if (wmiAdapter != null)
                        {
                            Logging.Logger.Instanse.Log("Info", "Request for getting IP of " + deviceName +
                                " Response: IPAddress:"+NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.ipAdresses)+ " IPSubnet:"+ NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.subnets)+
                                " DefaultIPGateway:"+ NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.gateways)+" Dns: "+ NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.dnses));
                            return wmiAdapter;
                        }
                        else
                        {
                            Logging.Logger.Instanse.Log("Error", "Request for getting IP of " + deviceName +" Response: null");
                            return wmiAdapter;
                        }
                    }
                }
            });
        }
        public WMIAdapter GetIP(string deviceName)
        {
            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                using (ManagementObjectCollection moc = mc.GetInstances())
                {
                    WMIAdapter wmiAdapter = new WMIAdapter() { deviceName = deviceName };
                    foreach (ManagementObject mo in moc)
                    {
                        // Make sure this is a IP enabled device. Not something like memory card or VM Ware
                        if ((bool)mo["ipEnabled"])
                        {
                            if (mo["Caption"].Equals(deviceName))
                            {
                                wmiAdapter.ipAdresses = (string[])mo["IPAddress"];
                                wmiAdapter.subnets = (string[])mo["IPSubnet"];
                                wmiAdapter.gateways = (string[])mo["DefaultIPGateway"];
                                wmiAdapter.dnses = (string[])mo["DNSServerSearchOrder"];

                                break;
                            }
                        }
                    }
                    if (wmiAdapter != null)
                    {
                        Logging.Logger.Instanse.Log("Info", "Request for getting IP of " + deviceName +
                            " Response: IPAddress:" + NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.ipAdresses) + " IPSubnet:" + NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.subnets) +
                            " DefaultIPGateway:" + NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.gateways) + " Dns: " + NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.dnses));
                        return wmiAdapter;
                    }
                    else
                    {
                        Logging.Logger.Instanse.Log("Error", "Request for getting IP of " + deviceName + " Response: null");
                        return wmiAdapter;
                    }
                }
            }
        }


        /// <summary>
        /// Returns the list of Network Interfaces installed
        /// </summary>
        /// <param name="ipEnabled"> true(default) - returns only ipEnabled(network cards) devices</param>
        /// <returns>Array list of string</returns>
        public object GetDevices(bool ipEnabled = true)
		{
            ArrayList nicNames = new ArrayList { };
            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                using (ManagementObjectCollection moc = mc.GetInstances())
                {
                    var adaps = moc.GetEnumerator();
                    foreach (ManagementObject mo in moc)
                    {
                        if (ipEnabled)
                        {
                            if ((bool)mo["ipEnabled"])
                                nicNames.Add(mo["Caption"]);
                        }
                        else
                            nicNames.Add(mo["Caption"]);
                    }
                    try { Logging.Logger.Instanse.Log("Info", "Request for getting devices"); }
                    catch { }
                    return nicNames;
                }
            }
		}
        /// <summary>
        /// Returns the list of Network Interfaces installed
        /// </summary>
        /// <param name="ipEnabled"> true(default) - returns only ipEnabled(network cards) devices</param>
        /// <returns>Array list of string</returns>
        public async Task<object> GetDevicesAsync(bool ipEnabled = true)
        {
            return await Task.Run(() =>
             {
                 ArrayList nicNames = new ArrayList { };
                 using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                 {
                     using (ManagementObjectCollection moc = mc.GetInstances())
                     {
                         var adaps = moc.GetEnumerator();
                         foreach (ManagementObject mo in moc)
                         {
                             if (ipEnabled)
                             {
                                 if ((bool)mo["ipEnabled"])
                                     nicNames.Add(mo["Caption"]);
                             }
                             else
                                 nicNames.Add(mo["Caption"]);
                         }
                     }
                 }
                 try { Logging.Logger.Instanse.Log("Info", "Request for getting devices"); }
                 catch { }
                 return nicNames;
             });
        }

        #endregion
    }

    public class WMIAdapter
    {
        public string deviceName { get; set; }
        public string[] ipAdresses { get; set; }
        public string[] subnets { get; set; }
        public string[] gateways { get; set; }
        public string[] dnses { get; set; }
    }
}
