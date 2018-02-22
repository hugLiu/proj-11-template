/* Parital Load 用于脚本动态加载局部视图
(C) Copy right jurassic 2015
在<a href>标签中属性: 
_container_='指定该标签中的链接下载显示的区域' , 默认是使用全局配置defaults中的container
_action_='点击链接时，执行的操作的服务端URL', 表示该链接点击是向服务端提交一个post请求
_level_=级别， 级别高的会覆盖级别低的设置
在<form>标签中的属性 target='任意值' 表示该表单无需处理
options:{
link: 要处理的超链接的选择器 (default is '#accordion .panel-body a',表示bootstrap的手风琴控件中的链接)
container:超链接点击后默认更新的区域选择器 (default is '#razorContainer')
updateNow:是否立即更新container区域中的超链接 (default is true)
level:本次操作的级别 (default is 0)
}
*/
jQuery.extend({
    loadPartial: function (options) {
        var defaults = { link: '#accordion .panel-body a', container: '#razorContainer', updateNow: true, level: 0 };
        defaults = $.extend(defaults, options);
        var razorContainer = $(defaults.container);
        $(defaults.link).each(function () { alink(this) });
        if (defaults.updateNow) alinkInContainer();

        //var razorUrl = "";
        function alink(link, c) {
            var jlink = $(link);
            if (jlink.attr("target") || jlink.attr("onclick")) return true; ////如果是js方法，则不作处理; 如果指明打开新窗口，也不作处理
            if (link.href.indexOf('###') >= 0 || link.href.indexOf('javascript:') >= 0) return true;
            var addr1 = link.href.split('#');
            var addr2 = location.href.split('#');
            if (addr1[0] == addr2[0]) return true; //说明是锚点跳转
            jlink.unbind("click");
            var tc = jlink.attr("_container_");
            var level = parseInt(jlink.attr("_level_"));
            if (!tc || defaults.level > level) {
                if (c) tc = c;
                else tc = defaults.container;
                jlink.attr("_container_", tc)
                jlink.attr("_level_", defaults.level)
            }
            jlink.click(function (e) {
                e.preventDefault();
                if (jlink.attr("_action_")) {
                    doAction(link, jlink.attr("_container_"));
                } else {
                    getRazor(link.href, null, 'get', jlink.attr("_container_"));
                }
            });
        }

        //处理容器中的所有超链接，使它能用ajax方式返回链接的页面
        function alinkInContainer(c) {
            c = c || razorContainer;
            c.find('a').each(function () {
                alink(this, c.selector);
            });
            c.find('form').each(function () {
                var jform = $(this);
                if (this.target) return true; //指定target属性时不处理表单
                if ($.validator && $.validator.unobtrusive) {
                    $.validator.unobtrusive.parse(this);
                }
                // jform.unbind("submit");
                var tc = jform.attr("_container_");
                var level = parseInt(jform.attr("_level_"));
                if (!tc || defaults.level > level) {
                    if (c) tc = c.selector;
                    else tc = defaults.container;
                    jform.attr("_container_", tc)
                    jform.attr("_level_", defaults.level)
                }
                jform.submit(function (e) {
                    e.preventDefault();
                    if (jform.validate) { //判断是否使用了jquery.validate组件
                        var v = jform.validate().form();
                        if (!v) return;
                    }
                    getRazor(this.action, $(this).serialize(), "post", jform.attr("_container_"));
                });
            });
        }

        function getContainer(c) {
            if (!c) return razorContainer;
            var container = $(c);
            if (container.length == 0) return razorContainer;
            return container;
        }

        function getRazor(url, d, m, c) {
            c = getContainer(c);
            $.ajax({
                url: url,
                data: d,
                cache: false,
                type: m,
                success: function (data) {
                    if (data.Type && data.Message) { //从服务器返回了MyException错误
                        showTips(data);
                        return;
                    }
                    data = $("<div>").append($.parseHTML(data, true));
                    var r = data.find('#_partial_'); //查找名为#_partial_的区域
                    if (r.length == 0) r = data; //找不到则用全部内容
                    //razorUrl = url;
                    c.html(r.html());
                    if (m != 'post') {
                        viewHistory.add(url, d, c);
                    }
                    showTips();
                    alinkInContainer(c);
                },
                error: function (response, status, xhr) {
                    c.html(response.responseText);
                }
            });
        }

        //通过超链接中的信息，点击后执行服务端某个动作（比如删除)
        //超链接格式: <a href="执行动作后要跳转到的服务端页面" action="要执行的动作的URL" msg="执行前的提示信息">动作提示</a>
        function doAction(a, c) {
            var self = $(a);
            var confirmmsg = self.attr('_msg_');
            if (confirmmsg && !confirm(confirmmsg)) return;
            $.post(self.attr("_action_"), null, function () {
                getRazor(a.href, null, 'get', c);
            });
        }

        //自制的前进后退功能 
        //todo: 和浏览器的前进后退按钮结合起来
        if (typeof (viewHistory) == 'undefined') viewHistory = (function () {
            var viewLog = [];
            var currentPos = 0;

            function getCurrentPos() {
                return (currentPos >= viewLog.length) ? null : viewLog[currentPos];
            }

            function reload(log) {
                if (log) getRazor(log.url, log.data, "get", log.container);
            }

            return {
                refresh: function () {
                    reload(getCurrentPos());
                },

                add: function (u, d, c) {
                    var last = getCurrentPos();
                    if (last == null || last.url != u || last.data != d) {
                        for (var i = 0; i < viewLog.length - currentPos - 1; i++) viewLog.pop();
                        viewLog.push({
                            url: u,
                            data: d,
                            container: c
                        });
                        currentPos = viewLog.length - 1;
                    }
                },

                backward: function () {
                    if (currentPos > 0) {
                        currentPos--;
                        reload(getCurrentPos());
                    }
                },

                forward: function () {
                    if (currentPos < viewLog.length - 1) {
                        currentPos++
                        reload(getCurrentPos());
                    }
                }
            };
        })();
    }
});
