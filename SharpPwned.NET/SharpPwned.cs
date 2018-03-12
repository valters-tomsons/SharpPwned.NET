using System;
using System.Net.Http;
using System.Threading.Tasks;
using SharpPwned.NET.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SharpPwned.NET
{
    public class HaveIBeenPwnedRestClient
    {
        private static readonly HttpClient client = new HttpClient();

        private readonly string URL = @"https://haveibeenpwned.com/api/v2";

        public HaveIBeenPwnedRestClient()
        {

        }

        public async Task<List<Paste>> GetPasteAccount(string account)
        {
            string api = "pasteaccount";
            var response = await GETRequestAsync($"{api}/{account}");

            List<Paste> allPastes = new List<Paste>();

            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                allPastes = JsonConvert.DeserializeObject<List<Paste>>(body);
                return allPastes;
            }
            else
            {
                return null;
            }
        }

        public async Task<Breach> GetBreach(string site)
        {
            string api = "breach";
            var response = await GETRequestAsync($"{api}/{site}");
            Breach breach = new Breach();

            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                breach = JsonConvert.DeserializeObject<Breach>(body);
                return breach;
            }
            else
            {
                return null;
            }

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
            else
            {
                return AllBreaches;
            }

        }

        public async Task<List<Breach>> GetAccountBreaches(string account)
        {
            string api = "breachedaccount";
            var response = await GETRequestAsync($"{api}/{account}");

            List<Breach> AllBreaches = new List<Breach>();

            if(response.StatusCode == "OK")
            {
                string body = response.Body;
                AllBreaches = JsonConvert.DeserializeObject<List<Breach>>(body);
                return AllBreaches;
            }
            else
            {
                return AllBreaches;
            }
        }

        public async Task<bool> IsPasswordPwned(string password)
        {
            string api = "pwnedpassword";
            var response = await GETRequestAsync($"{api}/{password}");

            if(response.StatusCode == "OK")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private async Task<Response> GETRequestAsync(string parameters)
        {
            Response RestResponse = new Response();
            Uri uri = new Uri($"{URL}/{parameters}");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpResponseMessage response = null;
            request.Headers.TryAddWithoutValidation("User-Agent", "SharpPwned.NET");

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
                RestResponse.StatusCode = response.StatusCode.ToString();
                RestResponse.HttpException = e.Message;
                return RestResponse;
            }
        }
    }
}
