using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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

        public static async Task<SearchResultModel> SearchAsync(string searchTerms = null, string mediaType = null, string title = null)
        {
            string responseContent = null;

            using (HttpClient client = new HttpClient())
            {
                string requestUrl = "https://images-api.nasa.gov/search?";

                List<string> requestParams = new List<string>();

                if (!string.IsNullOrWhiteSpace(searchTerms))
                    requestParams.Add($"q={HttpUtility.UrlEncode(searchTerms)}");

                if (!string.IsNullOrWhiteSpace(mediaType))
                    requestParams.Add($"media_type={HttpUtility.UrlEncode(mediaType).ToLower()}");

                if (!string.IsNullOrWhiteSpace(title))
                    requestParams.Add($"title={HttpUtility.UrlEncode(title)}");

                requestUrl += string.Join("&", requestParams);

                var response = await client.GetAsync(requestUrl);
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }
            }

            return string.IsNullOrWhiteSpace(responseContent) ? null : JsonConvert.DeserializeObject<SearchResultModel>(responseContent);
        }
    }
}
