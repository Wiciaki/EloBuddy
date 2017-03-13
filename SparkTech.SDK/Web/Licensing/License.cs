namespace SparkTech.SDK.Web.Licensing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Text;
    using System.Xml.Serialization;

    using EloBuddy.Sandbox;

    public static class License
    {
        private static readonly string UserName;

        static License()
        {
            ServicePointManager.Expect100Continue = false;

            var u = SandboxConfig.Username;

            if (string.IsNullOrEmpty(u) || u == "Guest")
            {
                UserName = null;
            }

            UserName = u;
        }

        public static bool Obtain()
        {
            var parameters = new Dictionary<string, string>
                                 {
                                     ["productNumber"] = "SparkTech.SDK",
                                     ["licenseeName"] = ""
                                 };

            var req = Request(parameters, false);

            return req != null && new ValidationResult(req).validations.Values.Any(c => c.properties["valid"].value == "true");
        }

        public static string GenerateToken()
        {
            if (UserName == null)
            {
                return null;
            }

            var parameters = new Dictionary<string, string>
                                 {
                                     ["tokenType"] = "SHOP",
                                     ["licenseeNumber"] = UserName
                                 };

            var property = Request(parameters, true)?.items.item[0].property;

            return property == null ? null : Array.Find(property, p => p.name == "number").Value;
        }

        private static netlicensing Request(Dictionary<string, string> parameters, bool token)
        {
            var requestPayload = new StringBuilder();
            var first = true;

            foreach (var param in parameters)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    requestPayload.Append("&");
                }

                requestPayload.Append(WebUtility.UrlEncode(param.Key));
                requestPayload.Append("=");
                requestPayload.Append(WebUtility.UrlEncode(param.Value));
            }

            var link = "https://go.netlicensing.io/core/v2/rest/";

            if (token)
            {
                link += "token";
            }
            else
            {
                link += "licensee/" + UserName + "/validate?" + requestPayload;
            }

            HttpWebRequest request;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(link);
            }
            catch (SecurityException ex)
            {
                Log.Exception(ex, "SparkTech.SDK can't establish a connection due to sandbox. Skipping the connection...");
                return null;
            }

            request.UserAgent = $"NetLicensing/C# {Environment.Version} (http://netlicensing.io)";
            request.Method = token ? "POST" : "GET";
            request.Credentials = new NetworkCredential("apiKey", "146f7c3c-e5aa-4529-84a7-cf2cf648f69d");
            request.PreAuthenticate = true;
            request.Accept = "application/xml";
            request.SendChunked = false;

            if (token)
            {
                request.ContentType = "application/x-www-form-urlencoded";
                var byteArray = Encoding.UTF8.GetBytes(requestPayload.ToString());
                request.ContentLength = byteArray.Length;

                var dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return null;
                }

                var stream = response.GetResponseStream();

                if (stream == null)
                {
                    return null;
                }

                Console.WriteLine("SparkTech.SDK: Server connection success.");
                return (netlicensing)new XmlSerializer(typeof(netlicensing)).Deserialize(stream);
            }
        }
    }
}
