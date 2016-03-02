using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mapper.Entities;

namespace Mapper.Tests
{
    [TestClass]
    public class SheetInfoTest
    {
        [TestMethod]
        public void ConstructPath_UsualPattern()
        {
            var info = new SheetInfo {Pattern = @"yyyy\\MM\\""some folder""\\dd"};
            var date = new DateTime(2015, 03, 21);
            var root = "c:\\some path\\";
            var actual = info.ConstructPath(root, date);
            var expected = "c:\\some path\\2015\\03\\some folder\\21";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConstructPath_ErroneusPattern()
        {
            var info = new SheetInfo { Pattern = @"\\yyyy\\MM\\""some folder""\\dd" };
            var date = new DateTime(2012, 11, 08);
            var root = "c:\\some path\\";
            var actual = info.ConstructPath(root, date);
            var expected = "\\2012\\11\\some folder\\08";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConstructPath_ThrowsException()
        {
            var info = new SheetInfo { Pattern = @"yyyy\MM\""some folder""\dd" };
            var date = new DateTime(2012, 11, 08);
            var root = "c:\\some path\\";
            info.ConstructPath(root, date);
        }

        [TestMethod]
        public void ConstructPaths_UsualPattern()
        {
            var info = new SheetInfo { Pattern = @"yyyy\\MM\\""some folder""\\dd" };
            var date = new DateTime(2020, 05, 29);
            var root = "c:\\some path\\";
            var actual = info.ConstructPaths(root, date, date.AddDays(4));

            var expected = new Dictionary<DateTime, string>
            {
                {new DateTime(2020,05,29), "c:\\some path\\2020\\05\\some folder\\29" },
                {new DateTime(2020,05,30), "c:\\some path\\2020\\05\\some folder\\30" },
                {new DateTime(2020,05,31), "c:\\some path\\2020\\05\\some folder\\31" },
                {new DateTime(2020,06,01), "c:\\some path\\2020\\06\\some folder\\01" },
                {new DateTime(2020,06,02), "c:\\some path\\2020\\06\\some folder\\02" }
            };

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
