﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgileConfig.Server.Service;
using System;
using System.Collections.Generic;
using System.Text;
using AgileConfig.Server.Data.Freesql;
using FreeSql;
using AgileConfig.Server.Data.Entity;
using System.Threading.Tasks;

namespace AgileConfig.Server.Service.Tests.mysql
{
    [TestClass()]
    public class ServerNodeServiceTests
    {
        IFreeSql fsq = null;
        FreeSqlContext freeSqlContext;
        ServerNodeService service = null;

        [TestInitialize]
        public void TestInitialize()
        {
            string conn = "Database=agile_config_test;Data Source=localhost;User Id=root;Password=dev@123;port=3306";
            fsq = new FreeSqlBuilder()
                          .UseConnectionString(FreeSql.DataType.MySql, conn)
                          .UseAutoSyncStructure(true)
                          .Build();
            freeSqlContext = new FreeSqlContext(fsq);

            service = new ServerNodeService(freeSqlContext);
            fsq.Delete<ServerNode>().Where("1=1");

            Console.WriteLine("TestInitialize");
        }



        [TestCleanup]
        public void Clean()
        {
            freeSqlContext.Dispose();
            fsq.Dispose();
        }

        [TestMethod()]
        public async Task AddAsyncTest()
        {
            fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;

            var result = await service.AddAsync(source);
            Assert.IsTrue(result);

            var node = fsq.Select<ServerNode>(new {
                Address = "1"
            }).ToOne();
            Assert.IsNotNull(node);

            Assert.AreEqual(source.Id, node.Id);
            //Assert.AreEqual(source.CreateTime, node.CreateTime);
            //Assert.AreEqual(source.LastEchoTime, node.LastEchoTime);
            Assert.AreEqual(source.Remark, node.Remark);
            Assert.AreEqual(source.Status, node.Status);
        }

        [TestMethod()]
        public async Task DeleteAsyncTest()
        {
            fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;

            var result = await service.AddAsync(source);
            Assert.IsTrue(result);

            var result1 = await service.DeleteAsync(source);
            Assert.IsTrue(result1);

            var node = fsq.Select<ServerNode>(new
            {
                Address = "1"
            }).ToOne();
            Assert.IsNull(node);
        }

        [TestMethod()]
        public async Task DeleteAsyncTest1()
        {
            fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;

            var result = await service.AddAsync(source);
            Assert.IsTrue(result);

            var result1 = await service.DeleteAsync(source.Id);
            Assert.IsTrue(result1);

            var node = fsq.Select<ServerNode>(new
            {
                Address = "1"
            }).ToOne();
            Assert.IsNull(node);
        }

        [TestMethod()]
        public async Task GetAllNodesAsyncTest()
        {
            fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;

            var result = await service.AddAsync(source);
            Assert.IsTrue(result);

            var nodes = await service.GetAllNodesAsync();
            Assert.IsNotNull(nodes);

            Assert.AreEqual(1, nodes.Count);
        }

        [TestMethod()]
        public async Task GetAsyncTest()
        {
            fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;
            var result = await service.AddAsync(source);
            Assert.IsTrue(result);

            var node = await service.GetAsync(source.Id);
            Assert.IsNotNull(node);

            Assert.AreEqual(source.Id, node.Id);
           // Assert.AreEqual(source.CreateTime, node.CreateTime);
            //Assert.AreEqual(source.LastEchoTime, node.LastEchoTime);
            Assert.AreEqual(source.Remark, node.Remark);
            Assert.AreEqual(source.Status, node.Status);
        }

        [TestMethod()]
        public async Task UpdateAsyncTest()
        {
            fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;
            var result = await service.AddAsync(source);
            Assert.IsTrue(result);

            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "3";
            source.Status = NodeStatus.Online;
            var result1 = await service.UpdateAsync(source);
            Assert.IsTrue(result);

            var node = await service.GetAsync(source.Id);
            Assert.IsNotNull(node);

            Assert.AreEqual(source.Id, node.Id);
           // Assert.AreEqual(source.CreateTime, node.CreateTime);
           // Assert.AreEqual(source.LastEchoTime, node.LastEchoTime);
            Assert.AreEqual(source.Remark, node.Remark);
            Assert.AreEqual(source.Status, node.Status);
        }
    }
}