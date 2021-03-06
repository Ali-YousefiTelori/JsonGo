﻿using BinaryGo.Json.Deserialize;
using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryGo.Runtime
{
    /// <summary>
    /// Property value caller
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <typeparam name="TPropertyType"></typeparam>
    public class PropertyCallerInfo<TType, TPropertyType>
    {
        /// <summary>
        /// Property value caller
        /// </summary>
        public PropertyCallerInfo(GetPropertyValue<TType, TPropertyType> funcGetValue, Action<TType, TPropertyType> funcSetValue)
        {
            GetValueAction = funcGetValue;
            SetValueAction = funcSetValue;
        }
        /// <summary>
        /// Gets property value
        /// </summary>
        public GetPropertyValue<TType, TPropertyType> GetValueAction { get; set; }
        /// <summary>
        /// Sets property value
        /// </summary>
        public Action<TType, TPropertyType> SetValueAction { get; set; }
    }
}
