using System;
using System.Collections.Generic;
using NUnit.Framework;
using CsgoHoldem.Api.Util;

namespace CsgoHoldem.Tests.Api
{
    public class CheckTest
    {
      
        [Test]
        public void TestCheckNull()
        {
            string test = null;
            Assert.Throws<ArgumentNullException>(() => NullChecker.Ref(nameof(test), test));
        }
        
        [Test]
        public void TestMultipleCheckNull()
        {
            string test = "";
            string test1 = null;
            string test2 = "";
            Assert.Throws<ArgumentNullException>(() => 
                NullChecker.Ref(nameof(test), test).Ref(nameof(test1), test1).Ref(nameof(test2), test2));
        }
        
        [Test]
        public void TestHasNoNulls()
        {
            var items = new List<string>() {"", "", null};
            Assert.Throws<ArgumentException>(() => NullChecker.HasNoNulls(nameof(items), items));
        }
    }
}