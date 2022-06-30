// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    public class ClassWithPrimitiveBinding
    {
        public object Obj { get; set; }
        public string Str { get; set; }
        public int Integer { get; set; }
        public bool Boolean { get; set; }
        //public string[] Characters { get; set; }
        public Guid Guid { get; set; }
        public DateTime Time { get; set; }  
        
        public ClassWithPrimitiveBinding(
            object obj,
            string str,
            int integer,
            bool boolean,
            //string[] characters,
            Guid guid,
            DateTime time)
        {
            Obj = obj;
            Str = str;
            Integer = integer;
            Boolean = boolean;
            //Characters = characters;
            Guid = guid;
            Time = time;
        }
    }
}