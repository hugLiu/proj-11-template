using Jurassic.CommonModels.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jurassic.WebWeChat
{
    public class WeChatMessageSender : IMessageSender
    {
        public SendResult Send(JMessage msg)
        {
            return ApiHelper.SendMessage(msg);
        }
    }
}