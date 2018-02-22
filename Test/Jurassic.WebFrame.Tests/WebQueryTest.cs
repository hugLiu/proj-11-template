using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.WebQuery;
using Jurassic.WebQuery.Models;
using System.Collections.Generic;
using Jurassic.Com.Tools;
using System.Linq;
using Jurassic.AppCenter;
using Jurassic.CommonModels;

namespace MvcApplication1.Tests
{
    [TestClass]
    public class WebQueryTest : BaseTest
    {
        AdvQueryItem GetQueryItem()
        {
            AdvQueryItem item = new AdvQueryItem
            {
                Name = "test1",
                ModelName = typeof(TestLinqModel).AssemblyQualifiedName,
                Nodes = new List<AdvQueryNode>()
                {
                     new  AdvQueryNode{ Id =1, ParentId = 0,Expression="AND", Type = "Operator" },
                     new AdvQueryNode{Id =2, ParentId =1, Type="Expr", Expression = "Field1", Operator = ">", Value = "100"},
                     new AdvQueryNode {Id =3, ParentId =1, Type="Expr",Expression = "Field2", Operator  = "NOT LIKE", Value="bc" },
                     new AdvQueryNode{Id = 4, ParentId = 1,Expression="OR", Type="Operator"},
                     new AdvQueryNode{Id =5, ParentId =4, Type="Expr", Expression = "Field3", Operator = "=", Value = "2016-03-04"},
                     new AdvQueryNode {Id =6, ParentId =4, Type="Expr",Expression = "Field3", Operator  = "=", Value="2016-03-06" },
                 }
            };
            return item;
        }
        //    AdvQueryManager manager = new AdvQueryManager();
        //    string expr = manager.CalcFinalExpr(item);
        //}

        [TestMethod]
        public void TestExprToLinq()
        {
            //RefHelper.LoadType("");
            List<TestLinqModel> models = new List<TestLinqModel>
            {
                new TestLinqModel{Field1 = 99, Field2 = "aaa",Field3=DateTime.Parse("2016-03-04")},
                new TestLinqModel {Field1 = 101, Field2 = "abc",Field3=DateTime.Parse("2016-03-05")},
                new TestLinqModel {Field1 = 102, Field2 = "abc", Field3=DateTime.Parse("2016-03-06")},
                new TestLinqModel {Field1 = 103, Field2 = "abb", Field3=DateTime.Parse("2016-03-06")},
                new TestLinqModel{Field1= 100, Field2= "aa", Field3=DateTime.Parse("2016-03-07")}
            };
            var result = SiteManager.Get<AdvQueryManager>().Query(models.AsQueryable(), GetQueryItem());

            //var result = models.AsQueryable().Where("Field1>@0 AND !Field2.Contains(@1) AND (Field3 =@2 OR Field3=@3)", 100, "bc", DateTime.Parse("2016-03-04"), DateTime.Parse("2016-03-06"));

            Assert.AreEqual(result.Count(), 1);
        }

        class TestLinqModel
        {
            public decimal Field1 { get; set; }

            public string Field2 { get; set; }

            public DateTime Field3 { get; set; }
        }
    }
}
