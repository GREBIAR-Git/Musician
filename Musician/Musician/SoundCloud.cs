using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Musician
{
    internal class SoundCloud
    {
       /* public SoundCloud()
        {
            Errors = new List<string>();
        }

        private const string baseAddress = "https://api.soundcloud.com/";

        public IList<string> Errors { get; set; }

        public async Task<string> GetNonExpiringTokenAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                var content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("client_id","xxxxxx"),
            new KeyValuePair<string, string>("client_secret","xxxxxx"),
            new KeyValuePair<string, string>("grant_type","password"),
            new KeyValuePair<string, string>("username","xx@xx.com"),
            new KeyValuePair<string, string>("password","xxxxx"),
            new KeyValuePair<string, string>("scope","non-expiring")
        });

                var response = await client.PostAsync("oauth2/token", content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dynamic data = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                    return data.access_token;
                }

                Errors.Add(string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase));

                return null;
            }
        }

        public async void UploadTrackAsync(string filePath)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.ConnectionClose = true;
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MySoundCloudClient", "1.0"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", "MY_AUTH_TOKEN");
                ByteArrayContent titleContent = new ByteArrayContent(Encoding.UTF8.GetBytes("my title"));
                ByteArrayContent sharingContent = new ByteArrayContent(Encoding.UTF8.GetBytes("private"));
                ByteArrayContent byteArrayContent = new ByteArrayContent(Encoding.UTF8.GetBytes("MYFILENAME"));
                byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(titleContent, "track[title]");
                content.Add(sharingContent, "track[sharing]");
                content.Add(byteArrayContent, "track[asset_data]", "all i need");
                HttpResponseMessage message = await httpClient.PostAsync(new Uri("https://api.soundcloud.com/tracks"), content);

                if (message.IsSuccessStatusCode)
                {
                }
            }
        }*/
    }
}
