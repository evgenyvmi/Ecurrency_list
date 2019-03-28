using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private static string API_KEY = "6ce318b5-c25e-4968-857e-c9f022d53b57";

        public ActionResult Index(string sort_by = "market_cap", string sort_dir = "desc")
        {
            try
            {
                var json = makeAPICall(sort_by, sort_dir);
                var json_array = JsonConvert.DeserializeObject<JObject>(json).Value<JArray>("data").ToString();
                var ecurrencies = JsonConvert.DeserializeObject<IEnumerable<Ecurrency>> (json_array, new EcurrencyConverter());
                List<int> ids = new List<int>();
                foreach (var item in ecurrencies)
                {
                    ids.Add(item.Id);
                }
                var json_info = gatherLogo(string.Join(",", ids));
                foreach (var item in ecurrencies)
                {
                    item.LogoUrl = JsonConvert.DeserializeObject<JObject>(json_info).Value<JToken>("data")[item.Id.ToString()]["logo"].Value<string>();
                }

                    return View(ecurrencies);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }
            return View();
        }

        static string makeAPICall(string sort, string sort_dir)
        {
            var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["start"] = "1";
            queryString["limit"] = "30";
            queryString["convert"] = "USD";
            queryString["sort"] = sort;
            queryString["sort_dir"] = sort_dir;

            URL.Query = queryString.ToString();

            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
            client.Headers.Add("Accepts", "application/json");
            var response = client.DownloadString(URL.ToString());
            return response;
        }

        static string gatherLogo(string ids)
        {
            var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/info");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["id"] = ids;

            URL.Query = queryString.ToString();

            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
            client.Headers.Add("Accepts", "application/json");
            var response = client.DownloadString(URL.ToString());
            return response;
        }

        public class EcurrencyConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(Ecurrency));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                
                Ecurrency ecurrency = new Ecurrency();
                ecurrency.Id = jo.SelectToken("id").Value<int>();
                ecurrency.Name = jo.SelectToken("name").Value<string>();
                ecurrency.Symbol = jo.SelectToken("symbol").Value<string>();
                ecurrency.Price = jo.SelectToken("quote.USD.market_cap").Value<float>();
                ecurrency.Capitalization = jo.SelectToken("quote.USD.price").Value<float>();
                ecurrency.Percent_change_1h = jo.SelectToken("quote.USD.percent_change_1h").Value<float>();
                ecurrency.Percent_change_24h = jo.SelectToken("quote.USD.percent_change_24h").Value<float>();
                ecurrency.UpdateTime = jo.SelectToken("quote.USD.last_updated").Value<DateTime>();
                    
                return ecurrency;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}