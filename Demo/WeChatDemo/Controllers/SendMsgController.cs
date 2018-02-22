using Jurassic.CommonModels;
using Jurassic.CommonModels.Messages;
using Jurassic.WebFrame;
using System.Collections.Generic;
using System.Web.Mvc;
using Jurassic.Com.Tools;
using Jurassic.WebWeChat;

namespace WeChatDemo.Controllers
{
    public class SendMsgController : BaseController
    {
        static Dictionary<string, string> msgDict = new Dictionary<string, string>();
        // GET: SendMsg
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string title, string msg)
        {
            title = CommOp.CutStr(title, 20);

            ApiHelper.BroadCastMessage(new JMessage
            {
                Channel = SendChannel.Custom, //Custom代表发微信
                Content = CommOp.CutStr(msg, 100),
                Title = title,
                SenderId = CurrentUserId.ToInt(),
                //SendToIds = new int[] { CurrentUserId.ToInt() },
                Url = Url.Action("Detail", new { title = title })
            });
            msgDict[title] = msg;
            return JsonTips("success", "发送成功！");
        }

        public ActionResult Detail(string title)
        {
            ViewBag.Title = title;
            if (msgDict.ContainsKey(title))
            {
                ViewBag.Msg = msgDict[title];
            }
            return View();
        }
    }
}