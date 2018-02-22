using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.CommonModels.EFProvider;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Jurassic.AppCenter;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Diagnostics;
using Jurassic.Com.Tools;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Reflection;

namespace Jurassic.OfficialSite.Tests
{
    [TestClass]
    public class EFTests
    {
        /// <summary>
        /// 测试批量新增或修改，使用.上的TestDb库，当库不存在时会自动创建库和表。
        /// </summary>
        [TestMethod]
        public void EFTestMultiObjectAddOrChange()
        {
            var context = new TestContext();
            Random rand = new Random(100);
            /////////////////////////////添加供应商///////////////////////////////////////
            List<Supplier> suppliers = new List<Supplier>();
            string prefix = DateTime.Now.ToString("yyMMddHHmmss");
            for (int i = 0; i < 100; i++)
            {
                suppliers.Add(new Supplier
                {
                    Name = "Supplier" + prefix + i,
                    Address = "Address" + prefix + i,
                    Tel = "12345" + prefix + i,
                });
            }

            EFAuditDataService<Supplier> supplierProvider = new EFAuditDataService<Supplier>(context);

            var count = supplierProvider.Add(suppliers);
            Assert.AreEqual(count, suppliers.Count);

            /////////////////////////////添加产品///////////////////////////////////////
            List<Product> products = new List<Product>();
            for (int i = 0; i < 1000; i++)
            {
                products.Add(new Product
                {
                    Name = "Product" + prefix + i,
                    Model = "Model" + prefix + i,
                    Price = DateTime.Now.Millisecond,
                    Unit = DateTime.Now.Second,
                });
            }
            EFAuditDataService<Product> productProvider = new EFAuditDataService<Product>(context);

            count = productProvider.Add(products);
            Assert.AreEqual(count, products.Count);

            /////////////////////////////添加服务///////////////////////////////////////
            List<Service> services = new List<Service>();
            for (int i = 0; i < 200; i++)
            {
                services.Add(new Service
                {
                    Name = "Service" + prefix + i,
                    ServiceType = "Type" + prefix + i,
                    Price = DateTime.Now.Millisecond,
                });
            }
            EFAuditDataService<Service> serviceProvider = new EFAuditDataService<Service>(context);

            count = serviceProvider.Add(services);
            Assert.AreEqual(count, services.Count);

            services.ForEach(svc => svc.Price *= 1.2m);

            //测试批量修改
            count = serviceProvider.Change(services);
            Assert.AreEqual(count, services.Count);

            /////////////////////////////添加订单///////////////////////////////////////
            List<Order> orders = new List<Order>();
            for (int i = 0; i < 50; i++)
            {
                var order = new Order
                {
                    Name = "Order" + prefix + i,
                    SupplierId = suppliers[rand.Next(suppliers.Count)].Id,
                };
                order.Details = new List<OrderDetail>();
                for (int j = 0; j < rand.Next(10); j++)
                {
                    order.Details.Add(new OrderDetail
                    {
                        Order = order,
                        Product = products[rand.Next(products.Count)],
                        Quantity = rand.Next(10),
                    });
                }

                order.Amount = order.Details.Sum(detail => detail.Quantity * detail.Product.Price);

                orders.Add(order);
            }

            EFAuditDataService<Order> orderProvider = new EFAuditDataService<Order>(context);

            orderProvider.Add(orders);

            /////////////////////////////添加服务合同///////////////////////////////////////
            List<Contract> contracts = new List<Contract>();
            for (int i = 0; i < 20; i++)
            {
                var service = services[rand.Next(services.Count)];
                contracts.Add(new Contract
                {
                    Service = service,
                    Name = "ServiceContract" + prefix + i,
                    SupplierId = suppliers[rand.Next(suppliers.Count)].Id,
                    Amount = service.Price * 0.9m,
                });
            }
            EFAuditDataService<Contract> contractProvider = new EFAuditDataService<Contract>(context);
            count = contractProvider.Add(contracts);
            Assert.AreEqual(count, contracts.Count);
        }

        /// <summary>
        /// 测试EF的多表查询
        /// </summary>
        [TestMethod]
        public void EFTestMultiTableQuery()
        {
            //需求：查找所有供应商的产品和服务的总价，同一供应商的产品总价和服务总价作对比。
            // 格式：
            // 供应商名称  订单产品总价  合同服务总价
            // XXXXX        12000         11000
            // ...          ...            ...

            //两个条件：
            //1. 不能遗漏订单或合同统计两者之一不为0的供应商
            //2. 不能留下即没有订单又没有合同的供应商

            // 此示例比较复杂，相对简单一点的示例，请参见msdn:
            // https://msdn.microsoft.com/zh-cn/library/bb311040(v=vs.100).aspx

            var context = new TestContext();

            var supplierProvider = new EFAuditDataService<Supplier>(context);

            //如果没有数据，则调用一下新增数据的方法
            if (supplierProvider.GetQuery().Count() == 0)
            {
                EFTestMultiObjectAddOrChange();
            }

            var orderProvider = new EFAuditDataService<Order>(context);
            var contractProvider = new EFAuditDataService<Contract>(context);

            var result = from s in supplierProvider.GetQuery()
                         join o in orderProvider.GetQuery()
                         .GroupBy(op => op.SupplierId)
               .Select(g => new { SupplierId = g.Key, OrderAmount = g.Sum(order => order.Amount) })
                         on s.Id equals o.SupplierId
                         into orderGroup
                         from item1 in orderGroup.DefaultIfEmpty()
                         join c in contractProvider.GetQuery().GroupBy(op => op.SupplierId)
              .Select(g => new { SupplierId = g.Key, ContractAmount = g.Sum(contract => contract.Amount) })
                         on s.Id equals c.SupplierId
                         into contractGroup
                         from item2 in contractGroup.DefaultIfEmpty()
                         select new { SupplierName = s.Name, OrderAmount = item1 == null ? 0 : item1.OrderAmount, ContractAmount = item2 == null ? 0 : item2.ContractAmount };

            //为了清楚起见，这个查询可以分开写，但最终提交到数据库是单条SQL语句。
            // 注意在查询的变换过程中，中间不要ToList()。
            // 最后的结果如果要分页，也不要ToList()
            result = result.Where(a => a.OrderAmount > 0 || a.ContractAmount > 0);

            //using Jurassic.Com.Tools 后，还可以这样写：
            //result = result.Where("OrderAmount>0 OR ContractAmount>0");

            Debug.WriteLine("SupplierName\t\tOrderAmount\t\tContractAmount");
            foreach (var r in result)
            {
                Debug.WriteLine(r.SupplierName + "\t\t" + r.OrderAmount + "\t\t" + r.ContractAmount);
            }
            Debug.WriteLine("----------------Count=" + result.Count());
        }

        [TestMethod]
        public void TestParseIntArray()
        {
            var list = new List<int> { 1, 2, 3 };

            var pOrder = Expression.Parameter(typeof(Order), "order");
            MemberExpression mOrderId = Expression.Property(pOrder, "Id");

            var constList = Expression.Constant(list, typeof(List<int>));
            MethodInfo containsMethod = typeof(List<int>).GetMethod("Contains");
            var filter = Expression.Call(constList, containsMethod, mOrderId);

            var lambda = Expression.Lambda<Func<Order, bool>>(filter, pOrder);

            var context = new TestContext();
            EFAuditDataService<Order> orderProvider = new EFAuditDataService<Order>(context);
            var query = orderProvider.GetQuery()
                //.Where(order=>list.Contains(order.Id));
                    .Where(lambda);

            int count = query.Count();
            Assert.IsTrue(count > 0);
        }
    }



    #region 测试用实体类和EF的Context
    /// <summary>
    /// 订单, 主要是面向产品，有明细
    /// </summary>
    public class Order
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int SupplierId { get; set; }

        public decimal Amount { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        [ForeignKey("OrderId")]
        public virtual ICollection<OrderDetail> Details { get; set; }
    }

    /// <summary>
    /// 合同，主要是面向服务，没有明细
    /// </summary>
    public class Contract
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int SupplierId { get; set; }

        public decimal Amount { get; set; }

        public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service Service { get; set; }
    }

    /// <summary>
    /// 供应商
    /// </summary>
    public class Supplier
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Tel { get; set; }

        public string Address { get; set; }
    }

    /// <summary>
    /// 产品
    /// </summary>
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public decimal Price { get; set; }

        public int Unit { get; set; }
    }

    /// <summary>
    /// 服务
    /// </summary>
    public class Service
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ServiceType { get; set; }

        public decimal Price { get; set; }
    }

    /// <summary>
    /// 订单明细
    /// </summary>
    public class OrderDetail
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public decimal Quantity { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
    public class TestMigration : DbMigrationsConfiguration<TestContext>
    {
        public TestMigration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }
    }

    public class TestContext : ModelContext
    {
        public TestContext()
        {
            //在ModelContext基类中，不会自动生成表，在此重写一下以便能自动生成表
            System.Data.Entity.Database.SetInitializer<TestContext>(new System.Data.Entity.MigrateDatabaseToLatestVersion<TestContext, TestMigration>());
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>();
            modelBuilder.Entity<OrderDetail>();
            modelBuilder.Entity<Service>();
            modelBuilder.Entity<Product>();
            modelBuilder.Entity<Supplier>();
            modelBuilder.Entity<Contract>();
        }

    }
    #endregion
}
