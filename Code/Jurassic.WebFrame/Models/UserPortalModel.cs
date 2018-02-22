
using System.Collections.Generic;
using Jurassic.AppCenter;

namespace Jurassic.WebFrame.Models
{
    /// <summary>
    /// 用户的首页门户配置
    /// </summary>
    public class UserPortalModel
    {
        public IList<WidgetModel> Widgets { get; set; } = new List<WidgetModel>();
    }

    /// <summary>
    /// 用户定制的首页部件数据模型
    /// </summary>
    public class WidgetModel
    {
        /// <summary>
        /// 部件ID，同时也是功能列表的FunctionID
        /// </summary>
        public string Id { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// 是否显示标题
        /// </summary>
        public bool ShowTitle { get; set; } = true;

        /// <summary>
        /// 是否显示该部件
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// 是否显示关闭按钮
        /// </summary>
        public bool ShowCloseButton { get; set; } = true;

        /// <summary>
        /// 部件所占的行数
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// 部件所占的列数
        /// </summary>
        public int Cols { get; set; }

        public int Order { get; set; }
    }

}