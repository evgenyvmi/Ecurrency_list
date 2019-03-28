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
            var a = Request.QueryString;
            try
            {
                //var json = "{\"status\": {\"timestamp\": \"2019-03-19T09:02:07.516Z\",\"error_code\": 0,\"error_message\": null,\"elapsed\": 8,\"credit_count\": 1},\"data\": [{\"id\": 1,\"name\": \"Bitcoin\",\"symbol\": \"BTC\",\"slug\": \"bitcoin\",\"circulating_supply\": 17597387,\"total_supply\": 17597387,\"max_supply\": 21000000,\"date_added\": \"2013-04-28T00:00:00.000Z\",\"num_market_pairs\": 6739,\"tags\": [\"mineable\"],\"platform\": null,\"cmc_rank\": 1,\"last_updated\": \"2019-03-19T09:01:26.000Z\",\"quote\": {\"USD\": {\"price\": 4031.19707691,\"volume_24h\": 8828880001.71942,\"percent_change_1h\": -0.0237571,\"percent_change_24h\": 0.327537,\"percent_change_7d\": 3.41017,\"market_cap\": 70938535035.65404,\"last_updated\": \"2019-03-19T09:01:26.000Z\"}}},{\"id\": 1027,\"name\": \"Ethereum\",\"symbol\": \"ETH\",\"slug\": \"ethereum\",\"circulating_supply\": 105298181.4991,\"total_supply\": 105298181.4991,\"max_supply\": null,\"date_added\": \"2015-08-07T00:00:00.000Z\",\"num_market_pairs\": 4818,\"tags\": [\"mineable\"],\"platform\": null,\"cmc_rank\": 2,\"last_updated\": \"2019-03-19T09:01:19.000Z\",\"quote\": {\"USD\": {\"price\": 139.296523089,\"volume_24h\": 4079494501.34105,\"percent_change_1h\": -0.104906,\"percent_change_24h\": -0.131632,\"percent_change_7d\": 4.37151,\"market_cap\": 14667670570.419096,\"last_updated\": \"2019-03-19T09:01:19.000Z\"}}},]}";
                var json = makeAPICall(sort_by, sort_dir);
                var json_array = JsonConvert.DeserializeObject<JObject>(json).Value<JArray>("data").ToString();
                var ecurrencies = JsonConvert.DeserializeObject<IEnumerable<Ecurrency>> (json_array, new EcurrencyConverter());
                List<int> ids = new List<int>();
                foreach (var item in ecurrencies)
                {
                    ids.Add(item.Id);
                }
                var json_info = gatherLogo(string.Join(",", ids));
                //var json_info = "{\"status\":{\"timestamp\":\"2019-03-28T18:47:00.465Z\",\"error_code\":0,\"error_message\":null,\"elapsed\":5,\"credit_count\":1},\"data\":{\"BTC\":{\"urls\":{\"website\":[\"https:\\bitcoin.org\"],\"twitter\":[],\"reddit\":[\"https:\\reddit.com\r\bitcoin\"],\"message_board\":[\"https:\\bitcointalk.org\"],\"announcement\":[],\"chat\":[],\"explorer\":[\"https:\\blockchain.info\",\"https:\\live.blockcypher.com\btc\",\"https:\\blockchair.com\bitcoin\"],\"source_code\":[\"https:\\github.com\bitcoin\"]},\"logo\":\"https:\\s2.coinmarketcap.com\static\img\coins\64x64\1.png\",\"id\":1,\"name\":\"Bitcoin\",\"symbol\":\"BTC\",\"slug\":\"bitcoin\",\"description\":\"Bitcoin (BTC) is a consensus network that enables a new payment system and a completely digital currency. Powered by its users, it is a peer to peer payment network that requires no central authority to operate. On October 31st, 2008, an individual or group of individuals operating under the pseudonym \\"Satoshi Nakamoto\\" published the Bitcoin Whitepaper and described it as: \\"a purely peer-to-peer version of electronic cash would allow online payments to be sent directly from one party to another without going through a financial institution.\\"\",\"date_added\":\"2013-04-28T00:00:00Z\",\"tags\":[\"mineable\"],\"platform\":null,\"category\":\"coin\"},\"ETH\":{\"urls\":{\"website\":[\"https:\\www.ethereum.org\"],\"twitter\":[\"https:\\twitter.com\ethereum\"],\"reddit\":[\"https:\\reddit.com\r\ethereum\"],\"message_board\":[\"https:\\forum.ethereum.org\"],\"announcement\":[\"https:\\bitcointalk.org\index.php?topic=428589.0\"],\"chat\":[\"https:\\gitter.im\orgs\ethereum\rooms\"],\"explorer\":[\"https:\\etherscan.io\",\"https:\\ethplorer.io\",\"https:\\blockchair.com\ethereum\"],\"source_code\":[\"https:\\github.com\ethereum\"]},\"logo\":\"https:\\s2.coinmarketcap.com\static\img\coins\64x64\1027.png\",\"id\":1027,\"name\":\"Ethereum\",\"symbol\":\"ETH\",\"slug\":\"ethereum\",\"description\":\"Ethereum (ETH) is a smart contract platform that enables developers to build decentralized applications (dapps) conceptualized by Vitalik Buterin in 2013. ETH is the native currency for the Ethereum platform and also works as the transaction fees to miners on the Ethereum network.\\n\\nEthereum is the pioneer for blockchain based smart contracts. When running on the blockchain a smart contract becomes like a self-operating computer program that automatically executes when specific conditions are met. On the blockchain, smart contracts allow for code to be run exactly as programmed without any possibility of downtime, censorship, fraud or third-party interference. It can facilitate the exchange of money, content, property, shares, or anything of value. The Ethereum network went live on July 30th, 2015 with 72 million Ethereum premined.\",\"date_added\":\"2015-08-07T00:00:00Z\",\"tags\":[\"mineable\"],\"platform\":null,\"category\":\"coin\"}}}";
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
                    
                //ecurrency.Capitalization = jo.SelectToken("quote.USD.price").ToString();
                return ecurrency;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}