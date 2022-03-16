using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParserTWSE
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static List<string> Code = new List<string>();
        public static List<string> TradeVolume = new List<string>();
        public static List<string> Transaction = new List<string>();
        public static List<string> TradeValue = new List<string>();
        public static List<string> OpeningPrice = new List<string>();
        public static List<string> HighestPrice = new List<string>();
        public static List<string> LowestPrice = new List<string>();
        public static List<string> ClosingPrice = new List<string>();
        public static List<string> Dir = new List<string>();
        public static List<string> Change = new List<string>();
        public static List<string> BestBidPrice = new List<string>();
        public static List<string> BestBidVolume = new List<string>();
        public static List<string> BestAskPrice = new List<string>();
        public static List<string> BestAskVolume = new List<string>();
        public static List<string> PriceEarningRatio = new List<string>();
        [STAThread]
        static void Main(string[] args)
        {
            logger.Info("НАЧАЛО РАБОТЫ ПАРСЕРА");
            string url = "https://www.twse.com.tw/en/exchangeReport/MI_INDEX?response=json&date=" + DateTime.Today.ToString("yyyy" + "MM" + "dd") + "&type=ALLBUT0999&";
            WebRequest webRequest;
            string json = "";
            webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Timeout = 40000;
            webRequest.ContentType = "/application/text";
            webRequest.Method = "GET";
            try
            {
                using (var response = webRequest.GetResponse())
                {
                    var result = ((HttpWebResponse)response).StatusCode;
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            json = streamReader.ReadToEnd();
                        }
                    }
                }
                dynamic dataJson = JsonConvert.DeserializeObject(json);
                if (dataJson.data9.Count != 0)
                {
                    int k = 1;
                    foreach (dynamic str in dataJson.data9)
                    {
                        try
                        {
                            Code.Add(" " + str[0].Value.Replace(',', '.') + ",");
                            TradeVolume.Add(" " + str[1].Value.Replace(',', '.') + ",");
                            Transaction.Add(" " + str[2].Value.Replace(',', '.') + ",");
                            TradeValue.Add(" " + str[3].Value.Replace(',', '.') + ",");
                            OpeningPrice.Add(" " + str[4].Value.Replace(',', '.') + ",");
                            HighestPrice.Add(" " + str[5].Value.Replace(',', '.') + ",");
                            LowestPrice.Add(" " + str[6].Value.Replace(',', '.') + ",");
                            ClosingPrice.Add(" " + str[7].Value.Replace(',', '.') + ",");
                            if (str[8].Value.Contains("+"))
                            {
                                Dir.Add(" " + str[8].Value.Substring(str[8].Value.IndexOf('+'), 1) + ",");
                            }
                            else if (str[8].Value.Contains("-"))
                            {
                                Dir.Add(" " + str[8].Value.Substring(str[8].Value.IndexOf('-'), 1) + ",");
                            }
                            else
                            {
                                Dir.Add(" " + str[8].Value.Replace(',', '.') + ",");
                            }
                            Change.Add(" " + str[9].Value.Replace(',', '.') + ",");
                            BestBidPrice.Add(" " + str[10].Value.Replace(',', '.') + ",");
                            BestBidVolume.Add(" " + str[11].Value.Replace(',', '.') + ",");
                            BestAskPrice.Add(" " + str[12].Value.Replace(',', '.') + ",");
                            BestAskVolume.Add(" " + str[13].Value.Replace(',', '.') + ",");
                            PriceEarningRatio.Add(" " + str[14].Value.Replace(',', '.'));
                            logger.Info("Загрузка " + k + " stocks завершена");
                            k++;
                        }
                        catch (Exception ex)
                        {
                            logger.Debug(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
            }        
            try
            {
                using (StreamWriter stream = new StreamWriter(args[0] + "\\" + DateTime.Today.ToString("dd" + "MM" + "yyyy") + ".csv"))
                {
                    stream.WriteLine("sep=,");
                    stream.WriteLine("Trade Volume, Security Code, Closing Price, Transaction, Trade Value, Opening Price, Highest Price, Lowest Price, Dir(+/-), Change, Last Best Bid Price, Last Best Bid Volume, Last Best Ask Price, Last Best Ask Volume, Price-Earning ratio	Note");
                    int k = 0;
                    foreach (var item in Code)
                    {
                        stream.WriteLine(TradeVolume[k] + item + ClosingPrice[k] + Transaction[k] + TradeValue[k] + OpeningPrice[k] + HighestPrice[k] + LowestPrice[k] +  Dir[k] + Change[k] + BestBidPrice[k] + BestBidVolume[k] + BestAskPrice[k] + BestAskVolume[k] + PriceEarningRatio[k]);
                        k++;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
            }
            logger.Info("ОКОНЧАНИЕ РАБОТЫ ПАРСЕРА");
        }
    }
}
