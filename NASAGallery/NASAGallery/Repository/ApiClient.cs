using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace NASAGallery.Repository
{
    public class ApiClient
    {
        public static async Task<T> RequestModelAsync<T>(string url) where T: class
        {
            string responseContent = await GetContent(url);

            return string.IsNullOrWhiteSpace(responseContent) ? null : JsonConvert.DeserializeObject<T>(responseContent);
        }

        

        public static async Task<object> RequestDynamicModelAsync(string url)
        {
            string responseContent = await GetContent(url);

            return string.IsNullOrWhiteSpace(responseContent) ? null : JsonConvert.DeserializeObject(responseContent);
        }

        public static async Task<ApodModel> RequestApodAsync(string date = null)
        {
            string requestUrl = $"https://api.nasa.gov/planetary/apod?api_key={Properties.Resources.ApiKey}";

            if (!string.IsNullOrWhiteSpace(date))
                requestUrl += $"&date={date}";

            return await RequestModelAsync<ApodModel>(requestUrl);
        }

        public static async Task<SearchResultModel> SearchAsync(string searchTerms = null, string mediaType = null, string title = null)
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

            var model = await RequestModelAsync<SearchResultModel>(requestUrl);

            if (model?.Collection?.Items != null)
            {
                var videoItems = model.Collection.Items
                    .Where(i => i.Data.Count > 0 && i.Data[0].MediaType == MediaType.Video).ToList();

                foreach (var videoItem in videoItems)
                {
                    var collection = await RequestModelAsync<List<string>>(videoItem.Href);

                    string thumbUrl = GetAssetUrlByPriority(collection, "~mobile_thumb_", "~small_thumb_", "~medium_thumb_", "~preview_thumb_", "~large_thumb_");

                    if (!string.IsNullOrWhiteSpace(thumbUrl))
                    {
                        videoItem.Links.Add(new LinkModel
                        {
                            Href = thumbUrl,
                            Rel = "preview",
                            Render = "image"
                        });
                    }
                }
            }

            return model;
        }

        public static string GetAssetUrlByPriority(List<string> assests, params string[] patterns)
        {
            if (patterns == null || assests == null)
                return string.Empty;

            foreach (var pattern in patterns)
            {
                var value = assests.FirstOrDefault(a => a.Contains(pattern));
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            return string.Empty;
        }

        private static async Task<string> GetContent(string url)
        {
            string responseContent = null;
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }
            }

            return responseContent;
        }
    }
}
