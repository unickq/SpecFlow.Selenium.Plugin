using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using OpenQA.Selenium.Remote;

namespace Unickq.SeleniumHelper.WebDriverGrid
{
    public abstract class CustomRemoteWebDriver : RemoteWebDriver
    {
        protected void Publish(string reqString)
        {
            Console.WriteLine(reqString);
            var uri = Uri;
            var requestData = Encoding.UTF8.GetBytes(reqString);
            var myWebRequest = WebRequest.Create(uri);
            var myHttpWebRequest = (HttpWebRequest)myWebRequest;
            myWebRequest.ContentType = "application/json";
            myWebRequest.Method = "PUT";
            myWebRequest.ContentLength = requestData.Length;
            using (var st = myWebRequest.GetRequestStream()) st.Write(requestData, 0, requestData.Length);
            var networkCredential = new NetworkCredential(SecretUser, SecretKey);
            var myCredentialCache = new CredentialCache { { uri, "Basic", networkCredential } };
            myHttpWebRequest.PreAuthenticate = true;
            myHttpWebRequest.Credentials = myCredentialCache;
            myWebRequest.GetResponse().Close();
        }

        public abstract void UpdateTestResult();

        protected string SecretUser { get; set; }
        protected string SecretKey { get; set; }
        protected abstract Uri Uri { get; }

        protected CustomRemoteWebDriver(string url, string browser)
            : base(new Uri(url), GetCapabilities(browser))
        {
        }

        protected CustomRemoteWebDriver(string url, string browser, Dictionary<string, string> capabilities)
            : base(new Uri(url), GetCapabilities(browser, capabilities))
        {
        }

        private static DesiredCapabilities GetCapabilities(string browserName,
            Dictionary<string, string> additionalCapabilities = null)
        {
            var capabilityCreationMethod = typeof(DesiredCapabilities).GetMethod(browserName,
                BindingFlags.Public | BindingFlags.Static);
            if (capabilityCreationMethod == null)
                throw new NotSupportedException("Can't find DesiredCapabilities with name " + browserName);

            var capabilities = capabilityCreationMethod.Invoke(null, null) as DesiredCapabilities;
            if (capabilities == null)
                throw new NotSupportedException("Can't find DesiredCapabilities with name " + browserName);

            if (additionalCapabilities == null) return capabilities;
            foreach (var capability in additionalCapabilities)
                capabilities.SetCapability(capability.Key, capability.Value);

            return capabilities;
        }

        protected static string BuildTransform(string str)
        {
            if (str == null) return null;
            if (str.Equals("@@debug")) str = DateTime.Now.ToString("yyyy/MM/dd hhtt");
            if (str.Equals("@@user")) str = Environment.UserName;
            if (str.Equals("@@machine")) str = Environment.MachineName;
            return str;
        }
    }
}
