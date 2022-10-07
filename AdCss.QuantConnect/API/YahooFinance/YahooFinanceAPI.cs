using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using MathNet.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YahooFinanceApi;
using QuantConnect.Algorithm;
using QuantConnect.Logging;
using AdCss.QC.Csv;
using AdCss.QC.Enum;

namespace AdCss.QC.API.YahooFinance
{
    public static class YahooFinanceAPI
    {
      
        public static async Task<List<CsvPriceData>> GetHistoPrice(string YahooTicker,  Eperiod periodRange, DateTime? startDate = null)
        {
            var historicalPrice = new List<CsvPriceData>();
            var errorMessage= string.Empty;
            //YahooTicker = YahooTicker.Replace("^", "5E"); // FOr index purpose

            try
            {
                using (var httpClient = new HttpClient()) // go to https://financeapi.net
                {
                    // CHECK THAT YOUR API KEY HAVE NOT BEEN CHANGED;
                    // IT HAPPEN SOMETIMES.
                    Log.Trace($" Exporting {YahooTicker} using Yahoo Finance...");

                    httpClient.BaseAddress = new Uri("https://yfapi.net/");
                    httpClient.DefaultRequestHeaders.Add("X-API-KEY", "kvAlFwfm1waBTMln5KAPv4kDH1UPT2Qr2bzmjpza");

                    string period = periodRange.ToString().Replace('_', ' ').Trim(' ');

                    var responseHistoPrice = await httpClient.GetAsync($"/v8/finance/chart/{YahooTicker}?range={period}&interval=1d");
                   var  responseBody = responseHistoPrice.Content.ReadAsStringAsync();


                    Task.WaitAll(responseBody);

                    var data = (JObject)JsonConvert.DeserializeObject(responseBody.Result);
                    errorMessage = data.ToString();

                    var currency = data.SelectToken("chart.result[0].meta.currency").Value<string>();
                    var exchangeName = data.SelectToken("chart.result[0].meta.exchangeName").Value<string>();
                    var instrumentType = data.SelectToken("chart.result[0].meta.instrumentType").Value<string>();
                    var timeZoneNam = data.SelectToken("chart.result[0].meta.exchangeTimezoneName").Value<string>();
                    var timezone = data.SelectToken("chart.result[0].meta.timezone").Value<string>();


                    var timestamp = data.SelectToken("chart.result[0].timestamp")?.ToObject<long[]>();
                    var volume = data.SelectToken("chart.result[0].indicators.quote[0].volume")?.ToObject<double?[]>();
                    var low = data.SelectToken("chart.result[0].indicators.quote[0].low")?.ToObject<double?[]>();
                    var close = data.SelectToken("chart.result[0].indicators.quote[0].close")?.ToObject<double?[]>();
                    var open = data.SelectToken("chart.result[0].indicators.quote[0].open")?.ToObject<double?[]>();
                    var high = data.SelectToken("chart.result[0].indicators.quote[0].high")?.ToObject<double?[]>();
                    var adjclose = data.SelectToken("chart.result[0].indicators.adjclose[0].adjclose")?.ToObject<double?[]>();


                    var lenght = timestamp.Length;

                    var histoPrice = new List<CsvPriceData>();

                    for (int i = 0; i <= lenght - 1; i++)
                    {
                        var _volume = volume[i];
                        var _low = low[i];
                        var _close = close[i];
                        var _open = open[i];
                        var _high = high[i];
                        var _adjclose = adjclose[i];
                        var _date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp[i]).ToLocalTime();

                        var pricePoint = new CsvPriceData(_low, _close, _adjclose, _high, _open, _volume, _date);
                        pricePoint.Currency = currency;
                        pricePoint.QuoteType = instrumentType;
                        pricePoint.Ticker = YahooTicker;
                        pricePoint.ExchangeName = exchangeName;
                        pricePoint.TimeZone = timezone;
                        pricePoint.TimeZoneName = timeZoneNam;

                        histoPrice.Add(pricePoint);
                    }
                    return histoPrice;
                }
            }
            catch (Exception e)
            {
                Log.Trace($"\n \n Ooops there is an error regarding Yahoo Finance API... -- {YahooTicker} -- :\n  {errorMessage}");
                Log.Trace(e.Message);
                throw;
            }
        }
    }
}

