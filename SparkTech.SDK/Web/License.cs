namespace SparkTech.SDK.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Text;
    using System.Xml.Serialization;

    using EloBuddy.Sandbox;
    using EloBuddy.SDK.Utils;

    using SparkTech.SDK.Web.NetLicensing;

    public class LicenseLink
    {
        private readonly string apiKey;

        private string shop;

        public LicenseLink(string apiKey)
        {
            this.apiKey = apiKey;
        }

        private static readonly string UserName;

        static LicenseLink()
        {
            ServicePointManager.Expect100Continue = false;

            var u = SandboxConfig.Username;

            if (string.IsNullOrEmpty(u) || u == "Guest")
            {
                UserName = null;
            }

            UserName = u;
        }

        public bool IsOwned(string productNumber)
        {
            var parameters = new Dictionary<string, string>
                                 {
                                     ["productNumber"] = productNumber,
                                     ["licenseeName"] = string.Empty
                                 };

            var req = this.ServerCall(parameters, false);

            return req != null && new ValidationResult(req).validations.Values.Any(c => c.properties["valid"].value == "true");
        }

        public string GetShopLink()
        {
            if (UserName == null)
            {
                return null;
            }

            if (this.shop != null)
            {
                return this.shop;
            }

            var parameters = new Dictionary<string, string>
                                 {
                                     ["tokenType"] = "SHOP",
                                     ["licenseeNumber"] = UserName
                                 };

            var req = this.ServerCall(parameters, true);

            if (req == null)
            {
                return null;
            }

            var token = Array.Find(req.items.item[0].property, p => p.name == "number").Value;

            return this.shop = "https://go.netlicensing.io/shop/v2/?shoptoken=" + token;
        }

        private netlicensing ServerCall(Dictionary<string, string> parameters, bool token)
        {
            var requestPayload = string.Empty;
            var first = true;

            foreach (var param in parameters)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    requestPayload += "&";
                }

                requestPayload += WebUtility.UrlEncode(param.Key);
                requestPayload += "=";
                requestPayload += WebUtility.UrlEncode(param.Value);
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
            catch (SecurityException)
            {
                Logger.Warn("SparkTech.SDK: Failed to exchange data with the license server due to locked sandbox environment.");
                return null;
            }

            request.UserAgent = "NetLicensing - Custom client by Spark - C# - " + Environment.Version;
            request.Method = token ? "POST" : "GET";
            request.Credentials = new NetworkCredential("apiKey", this.apiKey);
            request.PreAuthenticate = true;
            request.Accept = "application/xml";
            request.SendChunked = false;

            if (token)
            {
                request.ContentType = "application/x-www-form-urlencoded";
                var bytes = Encoding.UTF8.GetBytes(requestPayload);
                request.ContentLength = bytes.Length;

                var stream = request.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }

            using (var response = request.GetResponse())
            {
                Logger.Info("SparkTech.SDK: Successfully exchanged data with the license server.");
                // ReSharper disable once AssignNullToNotNullAttribute
                return (netlicensing)new XmlSerializer(typeof(netlicensing)).Deserialize(response.GetResponseStream());
            }
        }
    }
}