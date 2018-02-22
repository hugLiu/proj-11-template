using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.CommonModels.EFProvider;
using Jurassic.CommonModels.EntityBase;
using System.ComponentModel.DataAnnotations.Schema;
using Jurassic.CommonModels.ModelBase;
using System.Collections.Generic;
using System.Linq;
using Jurassic.AppCenter;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;
using Jurassic.CommonModels.Articles;
using Jurassic.CommonModels;
using System.Linq.Expressions;
using System.Data.Entity;
using Jurassic.AppCenter.Resources;

namespace MvcApplication1.Tests
{
    [TestClass]
    public class DataLangTest
    {
        [TestMethod]
        public void TestLangCollectionsRead()
        {
            //SiteManager.Kernel.Bind<IModelEntityConverter<TestLangTableModel, TestLangTable>>().To<LangTableConveter>();
            //Mapper.CreateMap<TestLangTable, TestLangTableModel>().ForMember(d => d.Remark, opt => opt.MapFrom(s => "RemarkTest"));
            var service = new EFAuditDataService<TestLangTable>(new LangContext());
            var query = service.GetQuery().Where(t => t.LangTexts.Count > 0);
            int count = query.Count();
            var list = query.Include("LangTexts").ToList();
            //Assert.IsTrue(list.Count > 0);
            Assert.AreEqual(count, 2);
            var service2 = new EFAuditDataService<TestLangTable2>(new LangContext());
            var query2 = service2.GetQuery().Where(t => t.LangTexts.Count > 0);
            int count2 = query2.Count();
            Assert.AreEqual(count, 2);
            //ModelDataService<TestLangTableModel, TestLangTable> langService2 = new ModelDataService<TestLangTableModel, TestLangTable>(new EFAuditDataService<TestLangTable>(new LangContext()));
            //var query2 = langService2.GetQuery().Where(t => t.Name == "王");
            //int count2 = query2.Count();
            //Assert.IsTrue(count2 == 0);
        }


        [TestMethod]
        public void TestLangCollectionsWrite()
        {
            Sys_DataLanguage lang = new Sys_DataLanguage()
            {
                Language = "en-us",
                BillId = 3,
                Name = "NoName",
                Text = "Hello",
                BillType = "NoTypeName"
            };

            EFAuditDataService<Sys_DataLanguage> langWriter = new EFAuditDataService<Sys_DataLanguage>(new LangContext());

            int i = langWriter.Add(lang);
            Assert.AreEqual(i, 1);
        }

        [TestMethod]
        public void TestLangModelRead()
        {
            //SiteManager.Kernel.Bind<IModelEntityConverter<TestLangTableModel, TestLangTable>>().To<LangTableConveter>();
            //Mapper.CreateMap<TestLangTable, TestLangTableModel>().ForMember(d => d.Remark, opt => opt.MapFrom(s => "RemarkTest"));
            ResHelper.CurrentCultureName = "zh-cn";

            ModelDataService<TestLangTableModel, TestLangTable> langService = new ModelDataService<TestLangTableModel, TestLangTable>(new EFAuditDataService<TestLangTable>(new LangContext()));
            var query = langService.GetQuery().Where(t => t.Name == "张");
            int count = query.Count();
            List<TestLangTableModel> list = query.ToList();
            Assert.IsTrue(count > 0);

            ResHelper.CurrentCultureName = "en-us";
            langService = new ModelDataService<TestLangTableModel, TestLangTable>(new EFAuditDataService<TestLangTable>(new LangContext()));
            query = langService.GetQuery().Where(t => t.Name == "Zhang");
            list = query.ToList();
            count = query.Count();
            Assert.IsTrue(count > 0);
        }

    }

    public class TestLangTable :  MultiLanguage, IId<int>
    {
        public TestLangTable()
        {
            LangTexts = new List<Sys_DataLanguage>();
        }
        public int Id { get; set; }

        //在数据实体中无需定义多语言字段
        //public string Name { get; set; }

        //public string Remark { get; set; }

        public decimal Price { get; set; }
    }

    public class TestLangTable2 : MultiLanguage, IId<int>
    {
        public TestLangTable2()
        {
        }
        public int Id { get; set; }

        public string OtherName { get; set; }

    }

    public class TestLangTable2Model:MultiLanguage, IId<int>
    {
        public int Id { get; set; }


        [CatalogExt(DataType = ExtDataType.MultiLanguage)]
        public string TestName { get; set; }

        [CatalogExt(DataType = ExtDataType.MultiLanguage)]
        public string TestName2 { get; set; }

    }

    public class TestLangTableModel : IIdModel
    {
        public TestLangTableModel()
        {
        }
        public int Id { get; set; }

        [CatalogExt(DataType=ExtDataType.MultiLanguage)]
        public string Name { get; set; }

        [CatalogExt(DataType = ExtDataType.MultiLanguage)]
        public string Remark { get; set; }

        public decimal Price { get; set; }

    }

    public class LangTableConveter : IModelEntityConverter<TestLangTableModel, TestLangTable>
    {

        public System.Linq.Expressions.Expression<Func<TestLangTableModel, TestLangTable>> ModelToEntity
        {
            get
            {
                return a => new TestLangTable
                {
                    Id = a.Id,
                    Price = a.Price
                };
            }

        }

        public System.Linq.Expressions.Expression<Func<TestLangTable, TestLangTableModel>> EntityToModel
        {
            get
            {
                return a => new TestLangTableModel
                {
                    Id = a.Id,
                    Name = null,
                    Price = a.Price
                };
            }
        }
    }

    public class LangContext : ModelContext
    {
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestLangTable>().ToTable("TestLangTable");
            modelBuilder.Entity<TestLangTable2>().ToTable("TestLangTable2");
        }
    }
}
