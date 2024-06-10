using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Windows;
using Newtonsoft.Json;
using RestSharp;

namespace ReportesUnis.API
{
    public class ConsumoAPI
    {

        public dynamic Post(string url, string json, string user, string pass)
        {
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(user + ":" + pass));
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                request.AddHeader("content-type", "application/vnd.oracle.adf.resourceitem+json");
                request.AddHeader("username", user);
                request.AddHeader("password", pass);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);

                dynamic datos = JsonConvert.DeserializeObject(response.Content).ToString();

                if (response.StatusCode.ToString() == "OK" || response.StatusCode.ToString() == "Created")
                    return 0;
                else
                    return 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public string Get(string url, string user, string pass)
        {

            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(user + ":" + pass));

            HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myWebRequest.Headers.Add("Authorization", "Basic " + svcCredentials);
            myWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:23.0) Gecko/20100101 Firefox/23.0";
            myWebRequest.PreAuthenticate = true;
            myWebRequest.Credentials = new NetworkCredential();
            myWebRequest.Proxy = null;
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
            Stream myStream = myHttpWebResponse.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myStream);

            string Datos = HttpUtility.HtmlDecode(myStreamReader.ReadToEnd());

            return Datos;
        }

        public int Patch(string url, string user, string pass, string info, string consulta, string effective)
        {
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(user + ":" + pass));

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.PATCH);
                var range = "RangeMode = UPDATE;RangeStartDate=" + effective + ";RangeSpan=LOGICAL_ROW_END_DATE";
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                request.AddHeader("content-type", "application/vnd.oracle.adf.resourceitem+json");
                request.AddHeader("username", user);
                request.AddHeader("password", pass);
                if (consulta.Equals("legislativeInfo") || consulta.Equals("addresses"))
                {
                    request.AddHeader("effective-of", range);
                    request.AddParameter("application/json", info, ParameterType.RequestBody);
                }
                else if (consulta.Equals("addresses") || consulta.Equals("names"))
                {
                    request.AddHeader("effective-of", range);
                    request.AddParameter("application/vnd.oracle.adf.resourceitem+json", info, ParameterType.RequestBody);

                }
                else
                    request.AddParameter("application/vnd.oracle.adf.resourceitem+json", info, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);

                //dynamic datos = JsonConvert.DeserializeObject(response.Content).ToString();
                if (response.StatusCode.ToString() == "OK")
                    return 0;
                else
                    return 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public int Delete(string url, string user, string pass, string consulta, string effective)
        {
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(user + ":" + pass));

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.DELETE);
                var range = "RangeMode=DELETE_CHANGES;RangeStartDate=" + effective + ";RangeEndDate=4712-12-31";
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                request.AddHeader("content-type", "application/vnd.oracle.adf.resourceitem+json");
                request.AddHeader("username", user);
                request.AddHeader("password", pass);
                if (consulta.Equals("addresses"))
                {
                    request.AddHeader("effective-of", range);
                }

                IRestResponse response = client.Execute(request);

                dynamic datos = JsonConvert.DeserializeObject(response.Content).ToString();
                if (response.StatusCode.ToString() == "NoContent")
                    return 0;
                else
                    return 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public dynamic PostNit(string url, string json)
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/vnd.oracle.adf.resourceitem+json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);

                dynamic datos = JsonConvert.DeserializeObject(response.Content).ToString();

                if (response.StatusCode.ToString() == "OK" || response.StatusCode.ToString() == "Created")
                    return response.Content;
                else if (response.StatusCode.ToString() == "BadRequest")
                    return "BadRequest";
                else
                    return "1";
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public int PatchEnd(string url, string user, string pass, string info, string consulta, string effective, string effectiveEnd)
        {
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(user + ":" + pass));

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.PATCH);
                var range = "RangeMode = UPDATE;RangeStartDate=" + effective + ";RangeSpan=LOGICAL_ROW_END_DATE";
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                request.AddHeader("content-type", "application/vnd.oracle.adf.resourceitem+json");
                request.AddHeader("username", user);
                request.AddHeader("password", pass);
                if (consulta.Equals("legislativeInfo") || consulta.Equals("addresses"))
                {
                    request.AddHeader("effective-of", range);
                    request.AddParameter("application/json", info, ParameterType.RequestBody);
                }
                else if (consulta.Equals("addresses"))
                {
                    request.AddHeader("effective-of", range);
                    request.AddParameter("application/vnd.oracle.adf.resourceitem+json", info, ParameterType.RequestBody);

                }
                else
                    request.AddParameter("application/vnd.oracle.adf.resourceitem+json", info, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);

                dynamic datos = JsonConvert.DeserializeObject(response.Content).ToString();
                if (response.StatusCode.ToString() == "OK")
                    return 0;
                else
                    return 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }


        public int Patch_CRM(string url, string user, string pass, string info)
        {
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(user + ":" + pass));

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.PATCH);
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                request.AddHeader("content-type", "application/vnd.oracle.adf.resourceitem+json");
                request.AddHeader("username", user);
                request.AddHeader("password", pass);
                request.AddParameter("application/vnd.oracle.adf.resourceitem+json", info, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);

                //dynamic datos = JsonConvert.DeserializeObject(response.Content).ToString();
                if (response.StatusCode.ToString() == "OK")
                    return 0;
                else
                    return 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}