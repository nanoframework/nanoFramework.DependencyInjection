// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using nanoFramework.TestFramework;

namespace nanoFramework.DependencyInjection.UnitTests
{
    [TestClass]
    public class AggregateExceptionTests
    {
        [TestMethod]
        public static void ConstructorBasic()
        {
            AggregateException ex = new AggregateException();
            Assert.Equal(0, ex.InnerExceptions.Count);
            Assert.True(ex.Message != null, "RunAggregateException_Constructor:  FAILED. Message property is null when the default constructor is used, expected a default message");

            ex = new AggregateException("message");
            Assert.Equal(0, ex.InnerExceptions.Count);
            Assert.True(ex.Message != null, "RunAggregateException_Constructor:  FAILED. Message property is  null when the default constructor(string) is used");

            ex = new AggregateException("message", new Exception());
            Assert.Equal(1, ex.InnerExceptions.Count);
            Assert.True(ex.Message != null, "RunAggregateException_Constructor:  FAILED. Message property is  null when the default constructor(string, Exception) is used");
        }

        [TestMethod]
        public static void ConstructorInvalidArguments()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new AggregateException("message", (Exception)null));
            Assert.Throws(typeof(ArgumentNullException), () => new AggregateException("message", new ArrayList() { null }));
        }

        [TestMethod]
        public static void Message()
        {
            Exception exceptionA = new Exception("A");
            Exception exceptionB = new Exception("B");
            Exception exceptionC = new Exception("C");

            AggregateException aggExceptionBase = new AggregateException("message", exceptionA, exceptionB, exceptionC);
            Assert.Equal("message (A) (B) (C)", aggExceptionBase.Message);
            //Assert.Equal("message (A) (B) (C)\n---> (Inner Exception #0) System.Exception: A <---\n---> (Inner Exception #1) System.Exception: B <---\n---> (Inner Exception #2) System.Exception: C <---\n", aggExceptionBase.ToString());
        }
    }
}