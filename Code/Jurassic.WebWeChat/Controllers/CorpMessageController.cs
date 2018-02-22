using Jurassic.WebFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jurassic.WebWeChat.Controllers
{
    /// <summary>
    /// 接收并处理用户从企业微信发来的消息 
    /// </summary>
    public class CorpMessageController : BaseController
    {
        // GET: CorpMessage
        public ActionResult Index()
        {
            return View();
        }
    }
}