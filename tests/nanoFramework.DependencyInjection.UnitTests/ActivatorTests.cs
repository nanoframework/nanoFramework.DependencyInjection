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
            Choice c = (Choice)(Activator.CreateInstance(typeof(Choice)));
            Assert.Equal(1, c.I);

            c = (Choice)(Activator.CreateInstance(typeof(Choice).FullName));
            Assert.Equal(1, c.I);

            c = (Choice)(Activator.CreateInstance(typeof(Choice), null));
            Assert.Equal(1, c.I);

            c = (Choice)(Activator.CreateInstance(typeof(Choice), null, null));
            Assert.Equal(1, c.I);

            c = (Choice)(Activator.CreateInstance(typeof(Choice), new object[] { }));
            Assert.Equal(1, c.I);

            c = (Choice)(Activator.CreateInstance(typeof(Choice), new object[] { 42 }));
            Assert.Equal(2, c.I);

            c = (Choice)(Activator.CreateInstance(typeof(Choice), new object[] { "Hello" }));
            Assert.Equal(3, c.I);

            c = (Choice)(Activator.CreateInstance(typeof(Choice), new object[] { 5.1, "Hello" }));
            Assert.Equal(4, c.I);
        }

        [TestMethod]
        public static void CreateInstanceConstructorWithParamsParameter()
        {
            Choice c = (Choice)(Activator.CreateInstance(typeof(Choice), new object[] { new VarArgs(), new object[] { } }));
            Assert.Equal(5, c.I);

            c = (Choice)(Activator.CreateInstance(typeof(Choice), new object[] { new VarArgs(), new object[] { "P1" } }));
            Assert.Equal(5, c.I);

            c = (Choice)(Activator.CreateInstance(typeof(Choice), new object[] { new VarArgs(), new object[] { "P1", "P2" } }));
            Assert.Equal(5, c.I);

            c = (Choice)(Activator.CreateInstance(typeof(Choice), new object[] { new VarStringArgs(), new string[] { } }));
            Assert.Equal(6, c.I);

            c = (Choice)(Activator.CreateInstance(typeof(Choice), new object[] { new VarStringArgs(), new string[] { "P1" } }));
            Assert.Equal(6, c.I);

            // TODO:? c = (Choice1)(Activator.CreateInstance(typeof(Choice1), new object[] { new VarStringArgs(), "P1", "P2" }));
            c = (Choice)(Activator.CreateInstance(typeof(Choice), new object[] { new VarStringArgs(), new string[] { "P1", "P2" } }));
            Assert.Equal(6, c.I);
        }

        [TestMethod]
        public void CreateInstanceNullTypeThrowsArgumentNullException()
        {
            Assert.Throws(typeof(ArgumentNullException), () => Activator.CreateInstance(null, new object[0]));
        }

        public class Choice
        {
            public Choice()
            {
                I = 1;
            }

            public Choice(int i)
            {
                I = 2;
            }

            public Choice(string s)
            {
                I = 3;
            }

            public Choice(double d, string optionalS = "Hey")
            {
                I = 4;
            }

            public Choice(VarArgs varArgs, params object[] parameters)
            {
                I = 5;
            }

            public Choice(VarStringArgs varArgs, params string[] parameters)
            {
                I = 6;
            }

            public Choice(VarIntArgs varArgs, params int[] parameters)
            {
                I = 7;
            }

            public int I;
        }

        public class VarArgs { }

        public class VarStringArgs { }

        public class VarIntArgs { }
    }
}