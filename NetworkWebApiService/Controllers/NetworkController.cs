using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NetworkService.Services;
using NetworkService.Core.Services;
using System.Threading.Tasks;

namespace NetworkWebApiService.Controllers
{
    [ServiceRequestActionFilter]
    [RoutePrefix("api/Network")]
    public class NetworkController : ApiController
    {
        private NetworkManager _netSrv;

        public NetworkController() {
            _netSrv = new NetworkManager(new WMIManager());
        }



        [HttpGet]
        [Route("GetDeviceAsync")]
        public async Task<IHttpActionResult> GetDeviceAsync(string name)
        {
            try
            {
                var response = await _netSrv.GetDeviceConfigurationAsync(name);
                return Ok(new { error = false, response = response });
            }
            catch (Exception ex)
            {
                return Ok(new { error = true, Error = new { ex } });
            }
        }

        [HttpGet]
        [Route("GetDevice")]
        public async Task<IHttpActionResult> GetDevice(string name)
        {
            try
            {
                var response = await _netSrv.GetDeviceConfigurationAsync(name);
                return Ok(new { error = false, response = response });
            }
            catch (Exception ex)
            {
                return Ok(new { error = true, Error = new { ex } });
            }
        }


        [HttpGet]
        [Route("GetDevicesAsync")]
        public async Task<IHttpActionResult> GetDevicesAsync(bool ipEnabled = true)
        {
            try
            {
                var response = await _netSrv.GetDevicesAsync(ipEnabled);
                return Ok(new { error = false, response = response });
            }
            catch (Exception ex)
            {
                return Ok(new { error = true, Error = new { ex } });
            }
        }

        [HttpGet]
        [Route("GetDevices")]
        public IHttpActionResult GetDevices(bool ipEnabled = true)
        {
            try
            {
                var response = _netSrv.GetDevices(ipEnabled);
                return Ok(new { error = false, response = response });
            }
            catch (Exception ex)
            {
                return Ok(new { error = true, Error = new { ex } });
            }
        }


        [HttpGet]
        [Route("SetDeviceAsync")]
        public async Task<IHttpActionResult> SetDeviceAsync(string name, string IpAddresses)
        {
            try
            {
                var response = await _netSrv.SetDeviceConfigurationAsync(name,IpAddresses,null,null,null);
                return Ok(new { error = false, response = response });
            }
            catch (Exception ex)
            {
                return Ok(new { error = true, Error = new { ex } });
            }
        }

        [HttpGet]
        [Route("SetDevice")]
        public IHttpActionResult SetDevice(string name, string IpAddresses)
        {
            try
            {
                var response = _netSrv.SetDeviceConfiguration(name, IpAddresses, null, null, null);
                return Ok(new { error = false, response = response });
            }
            catch (Exception ex)
            {
                return Ok(new { error = true, Error = new { ex } });
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_netSrv != null)
                {
                    _netSrv.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
    
}