using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SharpPwned.NET.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using SharpPwned.NET.Interfaces;

namespace SharpPwned.NET
{
    public class HaveIBeenPwnedRestClient : IHaveIBeenPwnedRestClient
    {
        private static readonly HttpClient client = new HttpClient();

        private readonly string URL = "https://haveibeenpwned.com/api/v3";
        private readonly string passwordRangeURL = "https://api.pwnedpasswords.com";
        private readonly string userAgent;
        private readonly string hibpApiKey;

        public HaveIBeenPwnedRestClient(string hibpApiKey, string userAgent = null)
        {
            this.userAgent = userAgent;
            this.hibpApiKey = hibpApiKey;
            if (string.IsNullOrWhiteSpace(this.userAgent))
            {
                this.userAgent = "SharpPwned.NET";
            }
        }

        public async Task<List<Paste>> GetPasteAccount(string account)
        {
            string api = "pasteaccount";
            var response = await GETRequestAsync($"{api}/{account}");

            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                var allPastes = JsonConvert.DeserializeObject<List<Paste>>(body);
                return allPastes;
            }

            return null;
        }

        public async Task<Breach> GetBreach(string site)
        {
            string api = "breach";
            var response = await GETRequestAsync($"{api}/{site}");

            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                var breach = JsonConvert.DeserializeObject<Breach>(body);
                return breach;
            }

            return null;

        }

        public async Task<List<Breach>> GetAllBreaches()
        {
            string api = "breaches";
            var response = await GETRequestAsync(api);

            List<Breach> AllBreaches = new List<Breach>();
            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                AllBreaches = JsonConvert.DeserializeObject<List<Breach>>(body);
                return AllBreaches;
            }

            return AllBreaches;

        }

        public async Task<List<Breach>> GetAccountBreaches(string account, bool? includeUnverified = true)
        {
            string api = "breachedaccount";
            string includeUnverifiedQueryString = string.Empty;
            if(includeUnverified.HasValue && !includeUnverified.Value)
            {
                includeUnverifiedQueryString = "?includeUnverified=false";
            }
            var response = await GETRequestAsync($"{api}/{account}{includeUnverifiedQueryString}");

            List<Breach> AllBreaches = new List<Breach>();

            if(response.StatusCode == "OK")
            {
                string body = response.Body;
                AllBreaches = JsonConvert.DeserializeObject<List<Breach>>(body);
                return AllBreaches;
            }

            return AllBreaches;
        }

        public async Task<bool> IsPasswordPwned(string password)
        {
            // Compute the SHA1 hash of the string
            SHA1 sha1 = SHA1.Create();
            byte[] byteString = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha1.ComputeHash(byteString);
            string hashString = "";
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }
            hashString = sb.ToString();

            // Break the SHA1 into two pieces:
            //   1) the first five characters of the hash
            //   2) the rest of the hash
            string hashFirstFive = hashString.Substring(0, 5);
            string hashLeftover = hashString.Substring(5, hashString.Length - 5);

            string api = "range";
            var response = await GETRequestAsync($"{api}/{hashFirstFive}", passwordRangeURL);
            var responseContainsHash = response.Body.Contains(hashLeftover);

            return responseContainsHash;

        }

        private async Task<Response> GETRequestAsync(string parameters)
        {
            Response response = await GETRequestAsync(parameters, URL);
            return response;
        }

        private async Task<Response> GETRequestAsync(string parameters, string overrideURL)
        {
            Response RestResponse = new Response();
            Uri uri = new Uri($"{overrideURL}/{parameters}");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add ("hibp-api-key", hibpApiKey);

            HttpResponseMessage response = null;
            request.Headers.TryAddWithoutValidation("User-Agent", userAgent);

            try
            {
                response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                string statusCode = response.StatusCode.ToString();

                RestResponse.Body = responseBody;
                RestResponse.StatusCode = statusCode;

                return RestResponse;
            }
            catch(HttpRequestException e)
            {
                RestResponse.Body = null;
                if (response != null) RestResponse.StatusCode = response.StatusCode.ToString();
                RestResponse.HttpException = e.Message;
                return RestResponse;
            }
        }
    }
}
