using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Jurassic.Com;
using System.IO;

namespace AllForDeploy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Jurassic Frames 程序集列表 框架版本：" + FrameVersion.Version);

            foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    var titleInfo = assembly.GetCustomAttributes(false).FirstOrDefault(attr => attr is AssemblyTitleAttribute) as AssemblyTitleAttribute;
                    if (titleInfo == null) continue;
                    Console.WriteLine(titleInfo.Title + "\t" + assembly.FullName);
                }
                catch
                {
                    continue;
                }
            }
            Console.ReadKey();
        }
    }
}
