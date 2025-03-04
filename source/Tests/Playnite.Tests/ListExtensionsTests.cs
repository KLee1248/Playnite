﻿using NUnit.Framework;
using Playnite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playnite.Tests
{
    [TestFixture]
    public class ListExtensionsTests
    {
        [Test]
        public void HasItemsTest()
        {
            Assert.IsFalse(((IEnumerable<object>)null).HasItems());
            Assert.IsFalse(new List<string>().HasItems());
            Assert.IsTrue(new List<string>() { "1" }.HasItems());
        }

        [Test]
        public void HasNonEmptyItemsTest()
        {
            Assert.IsFalse(new List<string>() { string.Empty }.HasNonEmptyItems());
            Assert.IsFalse(new List<string>() { "" }.HasNonEmptyItems());
            Assert.IsFalse(new List<string>() { "", "" }.HasNonEmptyItems());
            Assert.IsTrue(new List<string>() { "test" }.HasNonEmptyItems());
            Assert.IsTrue(new List<string>() { "", "test" }.HasNonEmptyItems());
        }

        [Test]
        public void IntersectsPartiallyWithTest()
        {
            Assert.IsTrue((new List<string> { "test" }).IntersectsPartiallyWith(new List<string> { "test" }));
            Assert.IsTrue((new List<string> { "Test" }).IntersectsPartiallyWith(new List<string> { "test" }));
            Assert.IsTrue((new List<string> { "Test" }).IntersectsPartiallyWith(new List<string> { "test2" }));
            Assert.IsTrue((new List<string> { "Test2" }).IntersectsPartiallyWith(new List<string> { "test", "test2" }));
            Assert.IsTrue((new List<string> { "test", "test2" }).IntersectsPartiallyWith(new List<string> { "Test2" }));
            Assert.IsFalse((new List<string> { "test", "test2" }).IntersectsPartiallyWith(new List<string> { string.Empty }));
            Assert.IsFalse((new List<string> { "test", "test2" }).IntersectsPartiallyWith(null));
            Assert.IsFalse(((List<string>)null).IntersectsPartiallyWith(new List<string> { "test", "test2" }));
        }

        [Test]
        public void IntersectsExactlyWithTest()
        {
            Assert.IsTrue((new List<string> { "test" }).IntersectsExactlyWith(new List<string> { "test" }));
            Assert.IsTrue((new List<string> { "Test" }).IntersectsExactlyWith(new List<string> { "test" }));
            Assert.IsTrue((new List<string> { "Test2" }).IntersectsExactlyWith(new List<string> { "test", "test2" }));
            Assert.IsTrue((new List<string> { "Test3", "test2" }).IntersectsExactlyWith(new List<string> { "test", "test2" }));
            Assert.IsTrue((new List<string> { "test", "test2" }).IntersectsExactlyWith(new List<string> { "Test2" }));
            Assert.IsFalse((new List<string> { "test", "test2" }).IntersectsExactlyWith(new List<string> { "Test3" }));
            Assert.IsFalse((new List<string> { "test", "test2" }).IntersectsExactlyWith(new List<string> { "Test3", "test5" }));
            Assert.IsFalse((new List<string> { "test", "test2" }).IntersectsExactlyWith(new List<string> { string.Empty }));
            Assert.IsFalse((new List<string> { "test", "test2" }).IntersectsExactlyWith(null));
            Assert.IsFalse(((List<string>)null).IntersectsExactlyWith(new List<string> { "test", "test2" }));
        }

        [Test]
        public void ContainsStringTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ContainsStringPartialTest()
        {
            Assert.Fail();
        }

        [Test]
        public void IsListEqualTest()
        {
            Assert.Fail();
        }

        [Test]
        public void GetCommonItemsTest()
        {
            List<int> list1 = new List<int> { 1, 2, 3, 4, };
            List<int> list2 = new List<int> { 1, 2, 3 };
            List<int> list3 = new List<int> { 1, 2 };
            var common = ListExtensions.GetCommonItems(new List<List<int>> { list1, list2, list3 }).ToArray();
            Assert.AreEqual(2, common.Count());
            Assert.AreEqual(1, common[0]);
            Assert.AreEqual(2, common[1]);
        }

        [Test]
        public void GetDistinctItemsTest()
        {
            List<int> list1 = new List<int> { 1, 2, 3, 4, };
            List<int> list2 = new List<int> { 1, 2 };
            List<int> list3 = new List<int> { 1, 2, 5};
            var distinct = ListExtensions.GetDistinctItems(new List<List<int>> { list1, list2, list3 }).ToArray();
            Assert.AreEqual(3, distinct.Count());
            Assert.AreEqual(3, distinct[0]);
            Assert.AreEqual(4, distinct[1]);
            Assert.AreEqual(5, distinct[2]);
        }
    }
}
