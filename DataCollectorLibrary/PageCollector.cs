using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;

namespace BookMarket.DataCollectorLibrary
{
    public abstract class PageCollector
    {
        protected readonly HttpClient _httpClient = null!;

        protected PageCollector(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected async Task<AngleSharp.Html.Dom.IHtmlDocument> GetHtmlDoc(string path)
        {
            HttpRequestMessage message = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(path),
            };

            var response = await _httpClient.SendAsync(message);


            //матрешка
            if(!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    response = await _httpClient.GetAsync(path);
                    if (!response.IsSuccessStatusCode) throw new Exception($"{path}\n {response.StatusCode}\n {response?.Content}\n");
                }
            }

            string pageAsString = await response.Content.ReadAsStringAsync();

            var parser = new HtmlParser();

            var doc = await parser.ParseDocumentAsync(pageAsString);

            return doc;
        }
    }
}
