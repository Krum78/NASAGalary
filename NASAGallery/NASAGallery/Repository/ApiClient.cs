using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NASAGallery.Repository
{
    public class ApiClient
    {
        public static async Task<ApodModel> RequestApodAsync(string date = null)
        {
            string responseContent = null;

            using (HttpClient client = new HttpClient())
            {
                string requestUrl = $"https://api.nasa.gov/planetary/apod?api_key={Properties.Resources.ApiKey}";

                if (!string.IsNullOrWhiteSpace(date))
                    requestUrl += $"&date={date}";

                var response = await client.GetAsync(requestUrl);
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }
            }

            return string.IsNullOrWhiteSpace(responseContent) ? null : JsonConvert.DeserializeObject<ApodModel>(responseContent);
        }
    }
}
