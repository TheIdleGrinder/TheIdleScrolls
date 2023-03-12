using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace Test_TheIdleScrolls_Core
{
    public class Test_ItemIdentifier
    {
        [TestCase("M2-HAR1+4")]
        [TestCase("M2-HAR10+1")]
        [TestCase("L2-LAR10")]
        public void CanParseValidCodes(string code)
        {
            Assert.DoesNotThrow(() => new ItemIdentifier(code));
        }

        [TestCase("M5-LBL1")]
        [TestCase("M1-LLL1")]
        [TestCase("M1-M2-LBL2")]
        public void ParsingInvalidCodesThrows(string code)
        {
            Assert.Throws<ArgumentException>(() => new ItemIdentifier(code));
        }
    }
}
