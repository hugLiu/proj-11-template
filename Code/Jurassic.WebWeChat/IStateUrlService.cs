using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.WebWeChat
{
    /// <summary>
    /// 从state转为Url的服务接口
    /// </summary>
    public interface IStateUrlService
    {
        /// <summary>
        /// 从state转为Url
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        string GetUrl(string state);
    }
}
