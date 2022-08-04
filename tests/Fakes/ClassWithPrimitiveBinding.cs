// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;

namespace nanoFramework.DependencyInjection.UnitTests.Fakes
{
    public class ClassWithPrimitiveBinding
    {
        public string Str { get; set; }
        
        public bool Boolean { get; set; }
        
        public byte Byte { get; set; }

        public sbyte SByte { get; set; }

        public short Short { get; set; }

        public ushort Ushort { get; set; }

        public int Int { get; set; }

        public uint UInt { get; set; }

        public long Long { get; set; }

        public ulong Ulong { get; set; }    

        public double Double { get; set; }
        
        public float Float { get; set; }

        public object Obj { get; set; }

        public DateTime DateTime { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public Guid Guid { get; set; }

        public Array Array { get; set; }

        public ArrayList ArrayList { get; set; }

        public ClassWithPrimitiveBinding(
            string str, 
            bool boolean, 
            byte @byte,
            sbyte sByte,
            short @short,
            ushort @ushort,
            int @int,
            uint uInt,
            long @long,
            ulong @ulong,
            double @double,
            float @float,
            object obj,
            DateTime dateTime,
            TimeSpan timeSpan,
            Guid guid,
            Array array,
            ArrayList arrayList)
        {
            Str = str;
            Boolean = boolean;
            Byte = @byte;
            SByte = sByte;
            Short = @short;
            Ushort = @ushort;
            Int = @int;
            UInt = uInt;
            Long = @long;
            Ulong = @ulong;
            Double = @double;
            Float = @float;
            Obj = obj;
            DateTime = dateTime;
            TimeSpan = timeSpan;
            Guid = guid;
            Array = array;
            ArrayList = arrayList;
        }
    }
}