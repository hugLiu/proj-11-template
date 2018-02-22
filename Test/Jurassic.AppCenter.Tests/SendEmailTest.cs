using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.Com.Tools;
using System.Diagnostics;

namespace Jurassic.OfficialSite.Tests
{
    [TestClass]
    public class SendEmailTest
    {
        [TestMethod]
        public void SendMail()
        {
            SMTPMail mail = new SMTPMail("wangjiaxin@jurassic.com.cn", "Hello myself", "test send mail to my self");

            mail.Send();

            Debug.WriteLine("The Return Message Is: " + mail.ErrorMessage);

            Assert.IsTrue(mail.ErrorMessage.IsEmpty());
        }
    }
}
