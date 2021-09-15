using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NomiBotDS.Conn
{
    public class tokenGeneration
    {
        string token;

        public void Generate()
        {
            try
            {
                var httpRequest = WebRequest.Create("https://newsenseapi.herokuapp.com/authenticate");
                httpRequest.ContentType = "application/json";
                httpRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    string json = "{\"username\":\"tonya\"," +
                                  "\"password\":\"test\"}";

                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    dynamic convert = JsonConvert.DeserializeObject(result);
                    token = convert.jwtToken;
                }

                saveToken(token);
            }
            catch
            {
                
            }
        }

        public void saveToken(string token)
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + @"\token.txt";
            using (StreamWriter writer = new StreamWriter(filepath))
            {
                writer.Write(token);
            }
        }

        public string readToken()
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + @"\token.txt";
            string token = String.Empty;
            if (File.Exists(filepath))
            {
                return token = File.ReadAllText(filepath);

            }
            else
            {
                return "error";
            }
        }

    }
}
