using TheIdleScrolls_Core.Items;

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
