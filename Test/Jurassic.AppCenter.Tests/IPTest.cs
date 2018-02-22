using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.Com.Tools;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Jurassic.OfficialSite.Tests
{
    [TestClass]
    public class IPTest
    {
        [TestMethod]
        public void TestIP()
        {
            IPHelper ipHelper = new IPHelper();
            ipHelper.DataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "qqwry.dat");
            ipHelper.IP = GetInternetIP(); // "221.232.134.250";
            var address = ipHelper.IPLocation();
            Debug.WriteLine("Your Address:" + address);

        }

        /// <summary>
        /// 获取本地IP地址信息
        /// </summary>
        private static string GetIP()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }

        //获取本机的公网IP
        private static string GetInternetIP()
        {
            string tempip = "";
            try
            {
                WebRequest wr = WebRequest.Create("http://www.ip138.com/ip2city.asp");
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                string all = sr.ReadToEnd(); //读取网站的数据

                int start = all.IndexOf("[") + 1;
                int end = all.IndexOf("]", start);
                tempip = all.Substring(start, end - start);
                sr.Close();
                s.Close();
            }
            catch
            {
            }
            return tempip;
        }
    }
}
