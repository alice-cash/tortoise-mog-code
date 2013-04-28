/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 11/24/2010
 * Time: 6:00 PM
 * 
 * Copyright 2012 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 * 
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Matthew Cash OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Matthew Cash. */

using System;
using System.Collections.Generic;
using Tortoise.Shared;
using Tortoise.Shared.Net;
using Tortoise.Shared.IO;

namespace Tortoise.Server.Module
{
    /// <summary>
    /// PlayerData is a central location for data on a connection. It contains fast serialization, to ease movement of data between servers and for storage.
    /// </summary>
    public class ConnectionData
    {
        static Dictionary<Connection, ConnectionData> PlayerArray = new System.Collections.Generic.Dictionary<Connection, ConnectionData>();

        static public ConnectionData GetPlayerData(Connection Connection)
        {
            return PlayerArray.ContainsKey(Connection) ? PlayerArray[Connection] : NewData(Connection);
        }

        static private ConnectionData NewData(Connection Connection)
        {
            ConnectionData pd = new ConnectionData();
            PlayerArray.Add(Connection, pd);
            return pd;
        }

        public ConnectionData()
        {
            ByteValues = new Dictionary<string, byte>();
            ByteArrayValues = new Dictionary<string, byte[]>();
            SByteValues = new Dictionary<string, sbyte>();
            ShortValues = new Dictionary<string, short>();
            UShortValues = new Dictionary<string, ushort>();
            IntValues = new Dictionary<string, int>();
            UIntValues = new Dictionary<string, uint>();
            LongValues = new Dictionary<string, long>();
            ULongValues = new Dictionary<string, ulong>();
            SingleValues = new Dictionary<string, float>();
            DoubleValues = new Dictionary<string, double>();
            StringValues = new Dictionary<string, string>();
        }

        public ExecutionState LoadData(ByteReader br)
        {

            while (br.Avaliable > 0)
            {
                var Name = br.ReadString();
                if (!Name)
                    return ExecutionState.Failed();
                var Type = br.ReadByte();
                if (!Type)
                    return ExecutionState.Failed();
                DataType TypeResult;
                if (!DataTypeHelper.TryParse(Type.Result, out TypeResult))
                    return ExecutionState.Failed();
                switch (TypeResult)
                {
                    case DataType.type_byte:
                        var ValueByte = br.ReadByte();
                        if (!ValueByte)
                            return ExecutionState.Failed();
                        if (ByteValues.ContainsKey(Name.Result))
                            return ExecutionState.Failed();
                        ByteValues.Add(Name.Result, ValueByte.Result);
                        break;

                    case DataType.type_sbyte:
                        var ValueSByte = br.ReadSByte();
                        if (!ValueSByte)
                            return ExecutionState.Failed();
                        if (SByteValues.ContainsKey(Name.Result))
                            return ExecutionState.Failed();
                        SByteValues.Add(Name.Result, ValueSByte.Result);
                        break;

                    case DataType.type_short:
                        var ValueShort = br.ReadShort();
                        if (!ValueShort)
                            return ExecutionState.Failed();
                        if (ShortValues.ContainsKey(Name.Result))
                            return ExecutionState.Failed();
                        ShortValues.Add(Name.Result, ValueShort.Result);
                        break;

                    case DataType.type_ushort:
                        var ValueUShort = br.ReadUShort();
                        if (!ValueUShort)
                            return ExecutionState.Failed();
                        if (UShortValues.ContainsKey(Name.Result))
                            return ExecutionState.Failed();
                        UShortValues.Add(Name.Result, ValueUShort.Result);
                        break;

                    case DataType.type_int:
                        var ValueInt = br.ReadInt();
                        if (!ValueInt)
                            return ExecutionState.Failed();
                        if (IntValues.ContainsKey(Name.Result))
                            return ExecutionState.Failed();
                        IntValues.Add(Name.Result, ValueInt.Result);
                        break;

                    case DataType.type_uint:
                        var ValueUInt = br.ReadUInt();
                        if (!ValueUInt)
                            return ExecutionState.Failed();
                        if (UIntValues.ContainsKey(Name.Result))
                            return ExecutionState.Failed();
                        UIntValues.Add(Name.Result, ValueUInt.Result);
                        break;

                    case DataType.type_long:
                        var ValueLong = br.ReadLong();
                        if (!ValueLong)
                            return ExecutionState.Failed();
                        if (LongValues.ContainsKey(Name.Result))
                            return ExecutionState.Failed();
                        LongValues.Add(Name.Result, ValueLong.Result);
                        break;

                    case DataType.type_ulong:
                        var ValueULong = br.ReadULong();
                        if (!ValueULong)
                            return ExecutionState.Failed();
                        if (ULongValues.ContainsKey(Name.Result))
                            return ExecutionState.Failed();
                        ULongValues.Add(Name.Result, ValueULong.Result);
                        break;

                    case DataType.type_single:
                        var ValueSingle = br.ReadSingle();
                        if (!ValueSingle)
                            return ExecutionState.Failed();
                        if (SingleValues.ContainsKey(Name.Result))
                            return ExecutionState.Failed();
                        SingleValues.Add(Name.Result, ValueSingle.Result);
                        break;

                    case DataType.type_double:
                        var ValueDobule = br.ReadDouble();
                        if (!ValueDobule)
                            return ExecutionState.Failed();
                        if (DoubleValues.ContainsKey(Name.Result))
                            return ExecutionState.Failed();
                        DoubleValues.Add(Name.Result, ValueDobule.Result);
                        break;

                    case DataType.type_string:
                        var ValueString = br.ReadString();
                        if (!ValueString)
                            return ExecutionState.Failed();
                        if (StringValues.ContainsKey(Name.Result))
                            return ExecutionState.Failed();
                        StringValues.Add(Name.Result, ValueString.Result);
                        break;

                }

            }

            return ExecutionState.Succeeded();
        }

        public byte[] GetData()
        {
            ByteWriter bw = new ByteWriter();
            foreach (var item in ByteValues)
            {
                bw.Write(item.Key);
                bw.Write(DataType.type_byte.Value());
                bw.Write(item.Value);
            }

            foreach (var item in SByteValues)
            {
                bw.Write(item.Key);
                bw.Write(DataType.type_sbyte.Value());
                bw.Write(item.Value);
            }

            foreach (var item in ShortValues)
            {
                bw.Write(item.Key);
                bw.Write(DataType.type_short.Value());
                bw.Write(item.Value);
            }

            foreach (var item in UShortValues)
            {
                bw.Write(item.Key);
                bw.Write(DataType.type_short.Value());
                bw.Write(item.Value);
            }

            foreach (var item in IntValues)
            {
                bw.Write(item.Key);
                bw.Write(DataType.type_int.Value());
                bw.Write(item.Value);
            }

            foreach (var item in UIntValues)
            {
                bw.Write(item.Key);
                bw.Write(DataType.type_uint.Value());
                bw.Write(item.Value);
            }

            foreach (var item in LongValues)
            {
                bw.Write(item.Key);
                bw.Write(DataType.type_long.Value());
                bw.Write(item.Value);
            }

            foreach (var item in ULongValues)
            {
                bw.Write(item.Key);
                bw.Write(DataType.type_ulong.Value());
                bw.Write(item.Value);
            }

            foreach (var item in SingleValues)
            {
                bw.Write(item.Key);
                bw.Write(DataType.type_single.Value());
                bw.Write(item.Value);
            }

            foreach (var item in DoubleValues)
            {
                bw.Write(item.Key);
                bw.Write(DataType.type_double.Value());
                bw.Write(item.Value);
            }

            foreach (var item in StringValues)
            {
                bw.Write(item.Key);
                bw.Write(DataType.type_string.Value());
                bw.Write(item.Value);
            }

            return bw.GetArray();
        }


        public Dictionary<String, Byte> ByteValues { get; set; }
        public Dictionary<String, Byte[]> ByteArrayValues { get; set; }
        public Dictionary<String, SByte> SByteValues { get; set; }
        public Dictionary<String, Int16> ShortValues { get; set; }
        public Dictionary<String, UInt16> UShortValues { get; set; }
        public Dictionary<String, Int32> IntValues { get; set; }
        public Dictionary<String, UInt32> UIntValues { get; set; }
        public Dictionary<String, Int64> LongValues { get; set; }
        public Dictionary<String, UInt64> ULongValues { get; set; }
        public Dictionary<String, Single> SingleValues { get; set; }
        public Dictionary<String, Double> DoubleValues { get; set; }
        public Dictionary<String, String> StringValues { get; set; }


    }

    enum DataType
    {
        type_null,
        type_byte,
        type_sbyte,
        type_short,
        type_ushort,
        type_int,
        type_uint,
        type_long,
        type_ulong,
        type_single,
        type_double,
        type_string,
    }

    static class DataTypeHelper
    {
        public static bool TryParse(byte value, out DataType parsed)
        {
            parsed = DataType.type_null;
            if (!Enum.IsDefined(typeof(DataType), value))
                return false;
            parsed = (DataType)value;
            return true;
        }
        public static ushort Value(this DataType mID)
        {
            return (ushort)mID;
        }
    }
}
