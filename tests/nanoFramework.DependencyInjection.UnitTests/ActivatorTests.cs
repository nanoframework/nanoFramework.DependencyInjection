using System;

using nanoFramework.TestFramework;

namespace nanoFramework.DependencyInjection.UnitTests
{
    [TestClass]
    public class ActivatorTests
    {
        [TestMethod]
        public static void CreateInstance()
        {
            Choice1 c = (Choice1)(Activator.CreateInstance(typeof(Choice1)));
            Assert.Equal(1, c.I);

            c = (Choice1)(Activator.CreateInstance(typeof(Choice1).FullName));
            Assert.Equal(1, c.I);

            c = (Choice1)(Activator.CreateInstance(typeof(Choice1), null));
            Assert.Equal(1, c.I);

            c = (Choice1)(Activator.CreateInstance(typeof(Choice1), null, null));
            Assert.Equal(1, c.I);

            c = (Choice1)(Activator.CreateInstance(typeof(Choice1), new object[] { }));
            Assert.Equal(1, c.I);

            c = (Choice1)(Activator.CreateInstance(typeof(Choice1), new object[] { 42 }));
            Assert.Equal(2, c.I);

            c = (Choice1)(Activator.CreateInstance(typeof(Choice1), new object[] { "Hello" }));
            Assert.Equal(3, c.I);

            c = (Choice1)(Activator.CreateInstance(typeof(Choice1), new object[] { 5.1, "Hello" }));
            Assert.Equal(4, c.I);

        }

        public class Choice1 : Attribute
        {
            public Choice1()
            {
                I = 1;
            }

            public Choice1(int i)
            {
                I = 2;
            }

            public Choice1(string s)
            {
                I = 3;
            }

            public Choice1(double d, string optionalS = "Hey")
            {
                I = 4;
            }

            public int I;
        }
    }
}