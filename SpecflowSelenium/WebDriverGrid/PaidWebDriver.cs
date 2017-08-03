using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NUnit.Framework;

namespace Unickq.SeleniumHelper.WebDriverGrid
{
    public abstract class PaidWebDriver : RemoteWebDriver
    {
        protected void Publish(string reqString)
        {
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

        protected static string BuildTransform(string str)
        {
            if (str == null) return null;
            if (str.Equals("@@debug")) str = DateTime.Now.ToString("yyyy/MM/dd hhtt");
            if (str.Equals("@@user")) str = Environment.UserName;
            if (str.Equals("@@machine")) str = Environment.MachineName;
            return str;
        }

        protected static string FixedTestName
        {
            get
            {
                var name = TestContext.CurrentContext.Test.Name;
                if (name.Length > 255)
                {
                    name = name.Substring(0, 252) + "...";
                }
                return name;
            }
        }

        protected PaidWebDriver(string url, string browser, Dictionary<string, string> capabilities) : base(url, browser, capabilities)
        {
        }
    }
}
