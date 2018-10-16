using RestSharp;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ExchangeWatcher.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Data;
using MarketObserver.Models;
using MySql.Data.MySqlClient;

namespace ExchangeWatcher
{
    public class DataController
    {

        /**************************************\
        |*         MÉTODOS PRINCIPAIS         *|
        \**************************************/
        // PREENCHIMENTO
        public string AddData()
        {
            string ret = "";

            List<Standard> json = new List<Standard>();

            json = GetTradeOgreData();
            json = GetCrex24Data(json);
            json = GetSatoshiData(json);
            FixDataSize(json);

            //preenche tabela com os dados da api
            bool preencheu = (PreencheTabela(json, null) == "ok");


            return ret;
        }

        public string AddUsdData()
        {
            string retorno;
            RootObject[] usd_data = UnifyUSDValues();

            retorno = PreencheTabela(null, usd_data);

            return retorno;
        }

        // BUSCA
        public string GetCotacoes()
        {
            string qry = "SELECT * FROM api_data ORDER BY DATA_HORA DESC LIMIT 9";
            string ret = "ok";
            List<Standard> jsonLista = new List<Standard>();

            try
            {
                MySqlConnection conn = AbreConexao();

                MySqlCommand cmd = new MySqlCommand(qry, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                
                while (rdr.Read())
                {
                    Standard stnd = new Standard();

                    stnd.mercado = rdr[1].ToString();
                    stnd.exchange = rdr[2].ToString();
                    stnd.high = rdr[3].ToString();
                    stnd.low = rdr[4].ToString();
                    stnd.volume = rdr[5].ToString();
                    stnd.bid = rdr[6].ToString();
                    stnd.ask = rdr[7].ToString();
                    jsonLista.Add(stnd);
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }


            if (ret == "ok")
                ret = JsonConvert.SerializeObject(jsonLista).Replace('\"', '"');

            return ret;
        }

        public string GetCalculos()
        {
            string json = GetCotacoes();
            string jsonp = "";

            decimal ltcCR  = decimal.Parse("0,00200000");
            decimal dogeCR = decimal.Parse("5,00000000");
            decimal xmrCR  = decimal.Parse("0,02000000");
                                                     
            decimal ltcTO  = decimal.Parse("0,00100000");
            decimal dogeTO = decimal.Parse("3,03129372");
            decimal xmrTO  = decimal.Parse("0,00251169");
                                                     
            decimal ltcSA  = decimal.Parse("0,00200000");
            decimal dogeSA = decimal.Parse("3,00000000");
            decimal xmrSA  = decimal.Parse("0,01000000");


            //CRIA DATATABLE
            DataTable dt = jsonStringToTable(json);

            //BUSCA MAIORES BIDS - (VALORES DE VENDA)
            DataRow[] bidLTC = dt.Select("exchange LIKE '%LTC%'", "bid DESC");
            DataRow[] bidDOG = dt.Select("exchange LIKE '%DOGE%'", "bid DESC");
            DataRow[] bidXMR = dt.Select("exchange LIKE '%XMR%' ", "bid DESC");

            //BUSCA MENORES ASKS - (VALORES DE COMPRA)              
            DataRow[] askLTC = dt.Select("exchange LIKE '%LTC%' ", "ask");
            DataRow[] askDOG = dt.Select("exchange LIKE '%DOGE%'", "ask");
            DataRow[] askXMR = dt.Select("exchange LIKE '%XMR%' ", "ask");

            string _bidltc, _biddog, _bidxmr, _askltc, _askdog, _askxmr = "";

            _bidltc = bidLTC[0].ItemArray[5].ToString();
            _biddog = bidDOG[0].ItemArray[5].ToString();
            _bidxmr = bidXMR[0].ItemArray[5].ToString();

            _askltc = askLTC[0].ItemArray[6].ToString();
            _askdog = askDOG[0].ItemArray[6].ToString();
            _askxmr = askXMR[0].ItemArray[6].ToString();

            //CALCULA DIFERENÇA ENTRE VALOR DE VENDA - COMPRA
            decimal Tltc = 0,Tdoge =0, Txmr=0;

            //APLICAÇÃO DE TAXAS

            //LTC
            switch (bidLTC[0].ItemArray[0].ToString())
            {
                case "TRADEOGRE":
                    Tltc = ltcTO;
                    break;
                case "CREX24":
                    Tltc = ltcCR;
                    break;
                case "SATOSHI":
                    Tltc = ltcSA;
                    break;
            }

            //DOGE
            switch (bidDOG[0].ItemArray[0].ToString())
            {
                case "TRADEOGRE":
                    Tdoge = dogeTO;
                    break;
                case "CREX24":
                    Tdoge = dogeCR;
                    break;
                case "SATOSHI":
                    Tdoge = dogeSA;
                    break;
            }

            //XMR
            switch (bidXMR[0].ItemArray[0].ToString())
            {
                case "TRADEOGRE":
                    Txmr = xmrTO;
                    break;
                case "CREX24":
                    Txmr = xmrCR;
                    break;
                case "SATOSHI":
                    Txmr = xmrSA;
                    break;
            }

            string LTC = (decimal.Parse(_bidltc.Replace(".", ",")) - decimal.Parse(_askltc.Replace(".", ",")) - Tltc).ToString();
            string DOG = (decimal.Parse(_biddog.Replace(".", ",")) - decimal.Parse(_askdog.Replace(".", ",")) - Tdoge).ToString();
            string XMR = (decimal.Parse(_bidxmr.Replace(".", ",")) - decimal.Parse(_askxmr.Replace(".", ",")) - Txmr).ToString();

            jsonp  = "{\"maiorBidLTC\":  \"" + _bidltc + "\",";
            jsonp += "\"menorAskLTC\":  \"" + _askltc + "\",";
            jsonp += "\"resultadoLTC\":\"" + LTC + "\",";
            jsonp += "\"maiorBidDOG\":  \"" + _biddog + "\",";
            jsonp += "\"menorAskDOG\":  \"" + _askdog + "\",";
            jsonp += "\"resultadoDOG\": \"" + DOG + "\",";
            jsonp += "\"maiorBidXMR\": \"" + _bidxmr + "\",";
            jsonp += "\"menorAskXMR\":  \"" + _askxmr + "\",";
            jsonp += "\"resultadoXMR\": \"" + XMR + "\"";
            jsonp += ", \"mercadoBidLTC\":\"" + bidLTC[0].ItemArray[0] + "\"";
            jsonp += ", \"mercadoBidDOG\":\"" + bidDOG[0].ItemArray[0] + "\"";
            jsonp += ", \"mercadoBidXMR\":\"" + bidXMR[0].ItemArray[0] + "\"";
            jsonp += ", \"mercadoAskLTC\":\"" + askLTC[0].ItemArray[0] + "\"";
            jsonp += ", \"mercadoAskDOG\":\"" + askDOG[0].ItemArray[0] + "\"";
            jsonp += ", \"mercadoAskXMR\":\"" + askXMR[0].ItemArray[0] + "\"}";

            return jsonp;

        }

        public string GetUsdValue()
        {
            string usd = "{";
            string query = "";

            query += " SELECT ";
            query += "     exchange.moeda AS moeda, ";
            query += " usd_conversion.valor AS valor ";
            query += " FROM ";
            query += " usd_conversion ";
            query += " INNER JOIN ";
            query += " exchange ON usd_conversion.EXCHANGE_ID = exchange.id; ";

            MySqlConnection conn = AbreConexao();

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            RootObject[] array = new RootObject[3];
            int cnt = 0;


            while (rdr.Read())
            {
                usd += "\"" + rdr[0].ToString() + "\":{";
                usd += "    \"valor\": \"" + rdr[1].ToString() + "\"}";
                if (cnt < 2) { usd += ","; }
                cnt++;
            }
            usd += "}";
            rdr.Close();
            conn.Close();

            return usd;
        }

        /*******************************************/


        /*******************************************\
        |*  OPERAÇÕES NOS DADOS RECEBIDOS DAS APIS *|
        \*******************************************/

        //unifica preços das moedas em um array
        private RootObject[] UnifyUSDValues()
        {
            string ltc, xmr, doge = "";
            RootObject[] retorno = new RootObject[3];

            RootObject jLtc = new RootObject();
            RootObject jXmr = new RootObject();
            RootObject jDoge = new RootObject();

            ltc = GetCoinValueUSD("2");
            xmr = GetCoinValueUSD("328");
            doge = GetCoinValueUSD("74");

            jLtc = (RootObject)JsonConvert.DeserializeObject(ltc, typeof(RootObject));
            jXmr = (RootObject)JsonConvert.DeserializeObject(xmr, typeof(RootObject));
            jDoge = (RootObject)JsonConvert.DeserializeObject(doge, typeof(RootObject));

            retorno[0] = jLtc;
            retorno[1] = jXmr;
            retorno[2] = jDoge;

            return retorno;
        }

        //cria string a ser gravada no arquivo de texto em formato json
        private string CreateUSDJsonString(RootObject[] array)
        {
            string json = "";
            json = "{ ";

            for (int i = 0; i < 3; i++)
            {
                json += "\"" + array[i].data.symbol + "\":{";
                json += "    \"valor\": \"" + array[i].data.quotes.USD.price + "\"}";
                if (i < 2) { json += ","; }

            }
            json += "}";

            return json;
        }

        //deserializa json em datatable
        public static DataTable jsonStringToTable(string jsonContent)
        {
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonContent);
            return dt;
        }

        //arruma o tamanho dos valores de bid e ask
        private void FixDataSize(List<Standard> json)
        {
            foreach (var item in json)
            {
                int tam = item.bid.ToString().Length;
                string value = "";

                if (item.bid.ToString().IndexOf("E") == -1)
                {
                    if (tam < 10)
                    {
                        value = item.bid.PadRight(10, '0');
                        item.bid = value;

                    }
                }
                else
                {
                    var converted = String.Format("{0:0.########}", double.Parse(item.bid));
                    value = converted.ToString();
                    item.bid = value;
                }

                if (item.ask.ToString().IndexOf("E") == -1)
                {
                    if (tam < 10)
                    {
                        value = item.ask.PadRight(10, '0');
                        item.ask = value;

                    }
                }
                else
                {
                    var converted = String.Format("{0:0.########}", double.Parse(item.bid));
                    value = converted.ToString();
                    item.bid = value;
                }
            }



        }

        //corrige notação cientifica
        private decimal FixNotation(string x)
        {
            decimal ret = new decimal(0);

            if (x != null)
            {
                ret = decimal.Parse(x, System.Globalization.NumberStyles.Any);
            }

            return ret;
        }

        /*******************************************/



        /*******************************************\
        |*                REQUISIÇÕES              *|
        \*******************************************/
        //realiza requisição
        public string GetData(string url)
        {
            var client = new RestClient(url);

            var response = client.Execute(new RestRequest());

            return response.Content;
        }

        //requests TRADEOGRE
        private List<Standard> GetTradeOgreData()
        {
            string url = "";
            string data = "";

            List<string> exchanges = new List<string>();
            List<Standard> jsonRet = new List<Standard>();

            exchanges.Insert(0, "BTC-LTC");
            exchanges.Insert(1, "BTC-DOGE");
            exchanges.Insert(2, "BTC-XMR");

            foreach (string exchange in exchanges)
            {
                url = "https://tradeogre.com/api/v1/ticker/" + exchange;
                data = GetData(url);
        
                //preenche objeto com o json do retorno do request
                TRADEOGRE obj = (TRADEOGRE)JsonConvert.DeserializeObject(data, typeof(TRADEOGRE));

                Standard stnd = new Standard();

                stnd.mercado = "TRADEOGRE";
                stnd.exchange = exchange;
                stnd.high = obj.high;
                stnd.low = obj.low;
                stnd.volume = obj.volume;

                if(obj.bid.IndexOf("E") == -1)
                {
                    stnd.bid = obj.bid.Replace(",", ".");
                }else
                    stnd.bid = FixNotation(obj.bid).ToString().Replace(",", ".");

                if(obj.ask.IndexOf("E") == -1)
                {
                    stnd.ask = obj.ask.Replace(",", ".");
                }else
                    stnd.ask = FixNotation(obj.ask).ToString().Replace(",", ".");

                jsonRet.Add(stnd);

            }

            return jsonRet;
        }

        //request CREX24
        private List<Standard> GetCrex24Data(List<Standard> jsonRet)
        {
            string url = "";
            string data = "";
            List<string> exchanges = new List<string>();
            List<CREX24> crex24 = new List<CREX24>();

            url = "https://api.crex24.com/v2/public/tickers?instrument=DOGE-BTC,LTC-BTC,XMR-BTC";
            data = GetData(url);

            //preenche objeto com o json do retorno do request
            crex24 = (List<CREX24>)JsonConvert.DeserializeObject(data, typeof(List<CREX24>));


            foreach(var item in crex24)
            {
                Standard stnd = new Standard();

                stnd.mercado = "CREX24";
                stnd.exchange = item.instrument;
                stnd.high = item.high;
                stnd.low = item.low;
                stnd.volume = item.quoteVolume;

                if (item.bid.IndexOf("E") == -1)
                {
                    stnd.bid = item.bid.Replace(",", ".");
                }
                else
                    stnd.bid = FixNotation(item.bid).ToString().Replace(",", ".");

                if (item.bid.IndexOf("E") == -1)
                {
                    stnd.ask = item.ask.Replace(",", ".");
                }
                else
                    stnd.ask = FixNotation(item.ask).ToString().Replace(",", ".");

                jsonRet.Add(stnd);
            }

            return jsonRet;

        }

        //requests TRADESATOSHI
        private List<Standard> GetSatoshiData(List<Standard> jsonRet)
        {
            string url = "";
            string data = "";
            List<string> exchanges = new List<string>();

            exchanges.Insert(0, "LTC_BTC");
            exchanges.Insert(1, "DOGE_BTC");
            exchanges.Insert(2, "XMR_BTC");

            foreach (string exchange in exchanges)
            {
                url = "https://tradesatoshi.com/api/public/getmarketsummary?market=" + exchange;
                data = GetData(url);
                Standard obj = new Standard();

                //preenche objeto com o json do retorno do request
                var d = JObject.Parse(data);
                obj.mercado = "SATOSHI";
                obj.exchange = d["result"]["market"].ToString();
                obj.high = d["result"]["high"].ToString();
                obj.low = d["result"]["low"].ToString();
                obj.volume = d["result"]["volume"].ToString();

                if (d["result"]["bid"].ToString().IndexOf("E") == -1)
                {
                    obj.bid = d["result"]["bid"].ToString().Replace(",", ".");
                }
                else
                    obj.bid = FixNotation(d["result"]["bid"].ToString()).ToString().Replace(",", ".");

                if (d["result"]["ask"].ToString().IndexOf("E") == -1)
                {
                    obj.ask = d["result"]["ask"].ToString().Replace(",", ".");
                }
                else
                    obj.ask = FixNotation(d["result"]["ask"].ToString()).ToString().Replace(",", ".");

                //preenche nome da moeda
                jsonRet.Add(obj);
            }

            return jsonRet;
        }

        //realiza requisição trazendo os valores das moedas em dolar
        private string GetCoinValueUSD(string codigo)
        {
            string url = "https://api.coinmarketcap.com/v2/ticker/"+codigo;
            
            return GetData(url); ;
        }
        
        /*******************************************/
     



         /**************************************\
        |*      OPERAÇÕES BANCO DE DADOS        *|
         \**************************************/

        //PREENCHE TABELA API_DATA NO BD
        private string PreencheTabela(List<Standard> dados, RootObject [] data)
        {
            MySqlConnection conn = AbreConexao();
            string qry = "";
            string error = "ok";
            MySqlCommand cmd;
            bool ret = true;

            if(dados == null)
            {
                qry = MontaInsertUSD(data);
            }else
                qry = MontaInsert(dados);

            try
            {
                cmd = new MySqlCommand(qry, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch(Exception e)
            {
                ret = false;
                error = e.Message;
                conn.Close();
            }


            return error;
        }

        //GERA INSERT
        private string MontaInsert(List<Standard> dados)
        {
            string query = "";

            foreach(var item in dados)
            {
                query += " INSERT INTO api_data ( MERCADO, MOEDA, HIGH, LOW, VOLUME, BID, ASK, DATA_HORA)";
                query += " VALUES ( ";
                query += "\"" + item.mercado + "\", ";
                query += "\"" + item.exchange + "\", ";
                query += "\"" + item.high + "\", ";
                query += "\"" + item.low + "\", ";
                query += "\"" + item.volume + "\", ";
                query += "\"" + item.bid + "\", ";
                query += "\"" + item.ask + "\", ";
                query += "\"" + DateTime.Now.ToString() + "\");";
            }

            return query;
        }

        //ABRE CONEXÃO COM BANCO DE DADOS
        private MySqlConnection AbreConexao()
        {
           MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
            string myConnectionString = "server=127.0.0.1;uid=root;pwd=Pereira@123;database=MarketWatcher";
            string erro;

            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();
            }
            catch (MySqlException ex)
            {
                erro = ex.Message;
            }

            return conn;
        }

        //MONTA INSERT USD
        private string MontaInsertUSD(RootObject [] dados)
        {
            string query = "";

            foreach (var item in dados)
            {
                query += " INSERT INTO usd_conversion ( EXCHANGE_ID, VALOR)";
                query += " VALUES ( ";
                query += "\"" + item.data.id + "\", ";
                query += "\"" + item.data.quotes.USD.price + "\");";
            }

            return query;
        }

        private MySqlDataReader Select(string query)
        {

            MySqlConnection conn = AbreConexao();

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            return rdr;
        }
    }
}
