using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.CommonModels.Messages;
using Jurassic.AppCenter;
using Jurassic.CommonModels;
using Jurassic.Com.Tools;

namespace MvcApplication1.Tests
{
    [TestClass]
    public class MessageProcesserTest : BaseTest
    {
        private AppUser GetSendUser()
        {
            var sendUser = AppManager.Instance.UserManager.GetByName("user456def");
            if (sendUser == null)
            {
                sendUser = new AppUser
                {
                    Id = "0",
                    Email = "bwangel@163.com",
                    Name = "user456def",
                    PhoneNumber = "13247145681",
                    Password = "123456"
                };
                AppManager.Instance.UserManager.Add(sendUser);
            }
            return sendUser as AppUser;
        }

        private AppUser GetSendToUser()
        {
            var user = AppManager.Instance.UserManager.GetByName("user123abc");
            if (user == null)
            {
                user = new AppUser
                {
                    Id = "0",
                    Email = "156060158@qq.com",
                    Name = "user123abc",
                    PhoneNumber = "13487071449",
                    Password = "123456"
                };
                AppManager.Instance.UserManager.Add(user);
            }
            return user as AppUser;
        }

        [TestMethod]
        public void TestMessageProcesser()
        {

            JMessage msg = new JMessage
            {
                SenderId = GetSendUser().Id.ToInt(),
                Channel = SendChannel.Email,
                SendToIds = new int[] { GetSendToUser().Id.ToInt() },
                Title = "消息发送测试",
                Content = "<strong>消息发送测试内容</strong>",
            };
            MessageProcesser processer = new MessageProcesser(new MessageRouter());
            processer.Process(msg);
            Assert.IsTrue(msg.Id > 0);
        }

        [TestMethod]
        public void TestMessageManager()
        {
            JMessage msg = new JMessage
            {
                SenderId = GetSendUser().Id.ToInt(),
                Channel = SendChannel.Email,
                SendToIds = new int[] { GetSendToUser().Id.ToInt() },
                Title = "消息发送测试",
                Content = "<strong>消息发送测试内容</strong>",
            };
             SiteManager.Message.Send(msg);
             System.Threading.Thread.Sleep(3000);
             Assert.IsTrue(msg.Id > 0);
        }
    }
}
