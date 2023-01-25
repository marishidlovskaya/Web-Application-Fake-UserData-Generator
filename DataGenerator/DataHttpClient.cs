using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Dataloader
{
    public class DataHttpClient
    {

        HttpClient client = new()
        {
            MaxResponseContentBufferSize = 1_000_000
        };

        public async Task<JObject?> GenerateListOfPersons(int page, string seed, string region)
        {
            int numberOfRecords = 20;
            if (page > 1) numberOfRecords = 10;

            JObject? users = null;

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.BaseAddress = new Uri("https://randomuser.me/");

            HttpResponseMessage response = await client.GetAsync($"api/?page={page}&results={numberOfRecords}&seed={seed}&nat={region}&inc=name,location,login,cell&noinfo");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(result))
                    users = JObject.Parse(result);
            }

            return users;
        }

    }
}
