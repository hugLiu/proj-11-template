using Jurassic.Com.Tools;
using Jurassic.CommonModels;
using Jurassic.CommonModels.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jurassic.WebWeChat
{
    /// <summary>
    /// 默认从state转为url的服务
    /// </summary>
    public class DefaultStateUrlService : IStateUrlService
    {
        public string GetUrl(string state)
        {
            return state;
        }
    }
}