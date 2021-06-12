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
        private readonly string URL = "https://haveibeenpwned.com/api/v3";
        private readonly string passwordRangeURL = "https://api.pwnedpasswords.com";

        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly string _userAgent;
        private readonly string _apiKey;

        public HaveIBeenPwnedRestClient(string hibpApiKey, string userAgent = null)
        {
            _userAgent = userAgent;
            _apiKey = hibpApiKey;

            if (string.IsNullOrWhiteSpace(userAgent))
            {
                _userAgent = "SharpPwned.NET";
            }
        }

        public async Task<List<Paste>> GetPasteAccount(string account)
        {
            const string api = "pasteaccount";
            var response = await GETRequestAsync($"{api}/{account}").ConfigureAwait(false);

            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                return JsonConvert.DeserializeObject<List<Paste>>(body);
            }

            return null;
        }

        public async Task<Breach> GetBreach(string site)
        {
            const string api = "breach";
            var response = await GETRequestAsync($"{api}/{site}").ConfigureAwait(false);

            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                return JsonConvert.DeserializeObject<Breach>(body);
            }

            return null;
        }

        public async Task<List<Breach>> GetAllBreaches()
        {
            const string api = "breaches";
            var response = await GETRequestAsync(api).ConfigureAwait(false);

            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                return JsonConvert.DeserializeObject<List<Breach>>(body);
            }

            return new List<Breach>();
        }

        public async Task<List<Breach>> GetAccountBreaches(string account, bool? includeUnverified = true)
        {
            const string api = "breachedaccount";
            string includeUnverifiedQueryString = string.Empty;

            if(includeUnverified == false)
            {
                includeUnverifiedQueryString = "?includeUnverified=false";
            }
            var response = await GETRequestAsync($"{api}/{account}{includeUnverifiedQueryString}").ConfigureAwait(false);

            if(response.StatusCode == "OK")
            {
                string body = response.Body;
                return JsonConvert.DeserializeObject<List<Breach>>(body);
            }

            return new List<Breach>();
        }

        public async Task<bool> IsPasswordPwned(string password)
        {
            // Compute the SHA1 hash of the string
            SHA1 sha1 = SHA1.Create();
            byte[] byteString = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha1.ComputeHash(byteString);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }

            string hashString = sb.ToString();
            // Break the SHA1 into two pieces:
            //   1) the first five characters of the hash
            //   2) the rest of the hash
            string hashFirstFive = hashString.Substring(0, 5);
            string hashLeftover = hashString.Substring(5, hashString.Length - 5);

            const string api = "range";
            var response = await GETRequestAsync($"{api}/{hashFirstFive}", passwordRangeURL).ConfigureAwait(false);

            return response.Body.Contains(hashLeftover);
        }

        private async Task<Response> GETRequestAsync(string parameters)
        {
            return await GETRequestAsync(parameters, URL).ConfigureAwait(false);
        }

        private async Task<Response> GETRequestAsync(string parameters, string overrideURL)
        {
            Response RestResponse = new Response();
            Uri uri = new Uri($"{overrideURL}/{parameters}");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add ("hibp-api-key", _apiKey);

            HttpResponseMessage response = null;
            request.Headers.TryAddWithoutValidation("User-Agent", _userAgent);

            try
            {
                response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
