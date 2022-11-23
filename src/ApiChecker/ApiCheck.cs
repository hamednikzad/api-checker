using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace ApiChecker
{
    public class ApiCheck
    {
        public ApiCheck(Guid id, string name, string address)
        {
            Id = id;
            Name = name;
            Address = address;
            
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | 
                SecurityProtocolType.Tls11 | 
                SecurityProtocolType.Tls;
            _client = new HttpClient();
        }

        private Guid Id { get; }
        public string Name { get; }
        public string Address { get; private set; }
        public DateTime LastCheck { get; private set; }
        public DateTime LastSuccessTime { get; private set; }
        public bool IsRunning { get; private set; }

        private readonly HttpClient _client;

        public async Task Check()
        {
            try
            {
                var result = await _client.GetAsync(Address);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    IsRunning = true;
                    LastSuccessTime = DateTime.Now;
                }
                else
                {
                    IsRunning = false;
                }
            }
            catch (Exception e)
            {
                IsRunning = false;
            }
            
            LastCheck = DateTime.Now;
        }
    }
}
