﻿using BinaryGo.Binary.StructureModels;
using BinaryGo.Runtime.Variables;
using BinaryGo.Runtime.Variables.Collections;
using BinaryGo.Runtime.Variables.Enums;
using BinaryGo.Runtime.Variables.Nullables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BinaryGo.Runtime
{
    /// <summary>
    /// base of typeGo
    /// </summary>
    public abstract class BaseTypeGoInfo
    {
        /// <summary>
        /// Data type
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// maximum size of this type taked from memory used for bufferbuilder 
        /// </summary>
        public int Capacity = 2;

        /// <summary>
        /// default value of bytes
        /// </summary>
        public byte[] DefaultBinaryValue;
        /// <summary>
        /// variable of serialize and deserialization
        /// </summary>
        public BaseVariable Variable { get; set; }
        /// <summary>
        /// list of properties
        /// </summary>
        internal abstract Dictionary<string, BaseTypeGoInfo> InternalProperties { get; }
        /// <summary>
        /// remove property from list of type properties
        /// </summary>
        /// <param name="propertyName"></param>
        internal abstract void RemoveProperty(string propertyName);
        /// <summary>
        /// add new property to list of properties
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyGoInfo"></param>
        internal abstract void AddProperty(string propertyName, object propertyGoInfo);
        /// <summary>
        /// regenerate properties of type
        /// </summary>
        internal abstract void ReGenerateProperties(List<MemberBinaryModelInfo> properties);

        /// <summary>
        /// Initializes a variable to a TypeGo
        /// </summary>
        public static TVariable InitializeVariable<TVariable>(BaseTypeGoInfo typeGoInfo, ITypeOptions options)
            where TVariable : BaseVariable, new()
        {
            TVariable variable = new TVariable();
            variable.InitializeBase(typeGoInfo, options);
            return variable;
        }

        static readonly object _lockobj = new object();
        /// <summary>
        /// Initializes a TypeGo for a runtime type
        /// the typeGo makes use of everything faster with easy access
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TypeGoInfo<T> Generate<T>(ITypeOptions options)
        {
            lock (_lockobj)
            {
                Type type = typeof(T);
                if (options.TryGetValueOfTypeGo(type, out object find))
                    return (TypeGoInfo<T>)find;
                var baseType = Nullable.GetUnderlyingType(type);
                bool isNullable = false;
                //when type is not nullable
                if (baseType == null)
                {
                    baseType = type;
                }
                else
                {
                    isNullable = true;
                }

                TypeGoInfo<T> typeGoInfo = new TypeGoInfo<T>
                {
                    Properties = new Dictionary<string, BasePropertyGoInfo<T>>(),
                    Type = type,
                };

                options.AddTypes(type, typeGoInfo);

                if (isNullable)
                {
                    if (baseType == typeof(bool))
                        InitializeVariable<BoolNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(DateTime))
                        InitializeVariable<DateTimeNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(uint))
                        InitializeVariable<UIntNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(long))
                        InitializeVariable<LongNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(short))
                        InitializeVariable<ShortNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(byte))
                        InitializeVariable<ByteNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(double))
                        InitializeVariable<DoubleNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(float))
                        InitializeVariable<FloatNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(decimal))
                        InitializeVariable<DecimalNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(sbyte))
                        InitializeVariable<SByteNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(ulong))
                        InitializeVariable<ULongNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(ushort))
                        InitializeVariable<UShortNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(int))
                        InitializeVariable<IntNullableVariable>(typeGoInfo, options);
                    else if (baseType == typeof(Guid))
                        InitializeVariable<GuidNullableVariable>(typeGoInfo, options);
                    //else if (baseType == typeof(byte[]))
                    //    InitializeVariable<ByteArrayNullableVariable>(typeGoInfo, options);
                    else if (baseType.IsEnum)
                    {
                        BaseVariable variable = (BaseVariable)typeof(EnumNullableVariable<>).MakeGenericType(baseType)
                            .GetMethod("Initialize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                            .Invoke(null, new object[] { });
                        variable.InitializeBase(typeGoInfo, options);
                    }
                }
                else
                {
                    if (baseType == typeof(bool))
                        InitializeVariable<BoolVariable>(typeGoInfo, options);
                    else if (baseType == typeof(DateTime))
                        InitializeVariable<DateTimeVariable>(typeGoInfo, options);
                    else if (baseType == typeof(uint))
                        InitializeVariable<UIntVariable>(typeGoInfo, options);
                    else if (baseType == typeof(long))
                        InitializeVariable<LongVariable>(typeGoInfo, options);
                    else if (baseType == typeof(short))
                        InitializeVariable<ShortVariable>(typeGoInfo, options);
                    else if (baseType == typeof(byte))
                        InitializeVariable<ByteVariable>(typeGoInfo, options);
                    else if (baseType == typeof(double))
                        InitializeVariable<DoubleVariable>(typeGoInfo, options);
                    else if (baseType == typeof(float))
                        InitializeVariable<FloatVariable>(typeGoInfo, options);
                    else if (baseType == typeof(decimal))
                        InitializeVariable<DecimalVariable>(typeGoInfo, options);
                    else if (baseType == typeof(sbyte))
                        InitializeVariable<SByteVariable>(typeGoInfo, options);
                    else if (baseType == typeof(ulong))
                        InitializeVariable<ULongVariable>(typeGoInfo, options);
                    else if (baseType == typeof(ushort))
                        InitializeVariable<UShortVariable>(typeGoInfo, options);
                    else if (baseType == typeof(int))
                        InitializeVariable<IntVariable>(typeGoInfo, options);
                    else if (baseType == typeof(byte[]))
                        InitializeVariable<ByteArrayVariable>(typeGoInfo, options);
                    else if (baseType == typeof(string))
                        InitializeVariable<StringVariable>(typeGoInfo, options);
                    else if (baseType == typeof(Guid))
                        InitializeVariable<GuidVariable>(typeGoInfo, options);
                    else if (baseType.IsEnum)
                    {
                        BaseVariable variable = (BaseVariable)typeof(EnumVariable<>).MakeGenericType(typeof(T))
                            .GetMethod("Initialize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                            .Invoke(null, new object[] { });
                        variable.InitializeBase(typeGoInfo, options);
                    }
                    //array data
                    else if (baseType.IsArray)
                    {
                        var elementType = baseType.GetElementType();
                        var method = typeof(BaseTypeGoInfo)
                            .GetMethod("InitializeVariable", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                            .MakeGenericMethod(typeof(ArrayVariable<>).MakeGenericType(elementType));
                        method.Invoke(null, new object[] { typeGoInfo, options });
                    }
                    //enumrable list data
                    else if (baseType.GetGenericArguments().Length > 0 && baseType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var elementType = baseType.GetGenericArguments()[0];
                        var method = typeof(BaseTypeGoInfo)
                            .GetMethod("InitializeVariable", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                            .MakeGenericMethod(typeof(GenericListVariable<>).MakeGenericType(elementType));
                        method.Invoke(null, new object[] { typeGoInfo, options });
                    }
                    //object daat
                    else
                        InitializeVariable<ObjectVariable<T>>(typeGoInfo, options);
                }

                return typeGoInfo;
            }
        }

        /// <summary>
        /// Generate default variables to an option
        /// </summary>
        /// <param name="options"></param>
        public static void GenerateDefaultVariables(ITypeOptions options)
        {

            foreach (var variableType in ReflectionHelper.VariableTypes)
            {
                var method = typeof(BaseTypeGoInfo).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                     .FirstOrDefault(x => x.Name == "Generate" && x.GetGenericArguments().Length == 1);
                var genericMethod = method.MakeGenericMethod(variableType.Key);
                genericMethod.Invoke(null, new object[] { options });
            }
        }
    }
}
