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

    /// <summary>
    /// Acts like a bridge between the current game and the licensing server
    /// </summary>
    public class LicenseServer
    {
        /// <summary>
        /// The current EloBuddy username
        /// </summary>
        public static readonly string Username;

        /// <summary>
        /// Obtains a list of the product modules owned
        /// </summary>
        /// <param name="productName">The specified product name</param>
        /// <returns>A list of product modules active for the specified product</returns>
        public List<string> GetModulesOwned(string productName)
        {
            return this.ServerCall(productName)?.items.item
                       .Where(item => Array.Exists(item.property, prop => prop.name == "valid" && prop.Value == "true"))
                       .Select(item => Array.Find(item.property, prop => prop.name == "productModuleNumber").Value)
                       .ToList() ?? new List<string>(0);
        }

        /// <summary>
        /// Generates a new shop link
        /// </summary>
        /// <returns>The current shop URL</returns>
        public string GetShopLink()
        {
            if (Username == null)
            {
                return null;
            }

            var req = this.ServerCall();

            if (req == null)
            {
                return null;
            }

            var token = Array.Find(req.items.item[0].property, p => p.name == "number").Value;

            return $"https://go.netlicensing.io/shop/v2/?shoptoken={token}/";
        }

        /// <summary>
        /// The API key to be used in the connection
        /// </summary>
        private readonly string apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseServer"/> class
        /// </summary>
        /// <param name="apiKey">The API key to be used</param>
        public LicenseServer(string apiKey)
        {
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Initializes static members of the <see cref="LicenseServer"/> class
        /// </summary>
        static LicenseServer()
        {
            ServicePointManager.Expect100Continue = false;

            var u = SandboxConfig.Username;

            if (string.IsNullOrEmpty(u) || u == "Guest")
            {
                Username = null;
            }

            Username = u;
        }

        /// <summary>
        /// Transfers the data between the server and the library
        /// </summary>
        /// <param name="productName">The product number to have the license validated</param>
        /// <returns>The netlicensing context</returns>
        private netlicensing ServerCall(string productName = null)
        {
            var token = productName == null;
            var link = "https://go.netlicensing.io/core/v2/rest/";

            if (token)
            {
                link += "token";
            }
            else
            {
                link += $"licensee/{Username}/validate?productNumber={productName}";
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

            Console.Title = "Connecting to NetLicensing...";

            request.UserAgent = $"NetLicensing/Wiciaki/.NET {Environment.Version}";
            request.Method = token ? "POST" : "GET";
            request.Credentials = new NetworkCredential("apiKey", this.apiKey);
            request.PreAuthenticate = true;
            request.Accept = "application/xml";
            request.SendChunked = false;

            if (token)
            {
                request.ContentType = "application/x-www-form-urlencoded";

                var bytes = Encoding.UTF8.GetBytes($"tokenType=SHOP&licenseeNumber={Username}");
                request.ContentLength = bytes.Length;

                var stream = request.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }

            try
            {
                using (var response = request.GetResponse())
                {
                    var stream = response.GetResponseStream();

                    if (stream == null)
                    {
                        throw new WebException("No response obtained!");
                    }

                    Logger.Debug("SparkTech.SDK: Successfully exchanged data with the license server.");
                    return (netlicensing)new XmlSerializer(typeof(netlicensing)).Deserialize(stream);
                }
            }
            catch (WebException)
            {
                Logger.Error("SparkTech.SDK: Connection error. Potential reason: User not registered in database.");

                if (!token)
                {
                    Logger.Error("SparkTech.SDK: Please check whether the provided product number is correct and that \"Auto-create Licensee\" is enabled for the product.");
                }

                return null;
            }
            finally
            {
                Console.Title = "SparkTech.SDK";
            }
        }
    }
}