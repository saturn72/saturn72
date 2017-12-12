#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

#endregion

namespace Saturn72.Core.ComponentModel
{
    /// <summary>
    ///     Generic Dictionary type converted
    /// </summary>
    /// <typeparam name="TKey">Key type (simple)</typeparam>
    /// <typeparam name="TValue">Value type (simple)</typeparam>
    public class GenericDictionaryTypeConverter<TKey, TValue> : TypeConverter
    {
        protected readonly TypeConverter typeConverterKey;
        protected readonly TypeConverter typeConverterValue;

        /// <summary>
        ///     Ctor
        /// </summary>
        public GenericDictionaryTypeConverter()
        {
            typeConverterKey = TypeDescriptor.GetConverter(typeof (TKey));
            if (typeConverterKey == null)
                throw new InvalidOperationException("No type converter exists for type " + typeof (TKey).FullName);
            typeConverterValue = TypeDescriptor.GetConverter(typeof (TValue));
            if (typeConverterValue == null)
                throw new InvalidOperationException("No type converter exists for type " + typeof (TValue).FullName);
        }

        /// <summary>
        ///     Gets a value indicating whether this converter can
        ///     convert an object in the given source type to the native type of the converter
        ///     using the context.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="sourceType">Source type</param>
        /// <returns>Result</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///     Converts the given object to the converter's native type.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="culture">Culture</param>
        /// <param name="value">Value</param>
        /// <returns>Result</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var input = (string) value;
                var items = string.IsNullOrEmpty(input)
                    ? new string[0]
                    : input.Split(';').Select(x => x.Trim()).ToArray();

                var result = new Dictionary<TKey, TValue>();
                Array.ForEach(items, s =>
                {
                    var keyValueStr = string.IsNullOrEmpty(s)
                        ? new string[0]
                        : s.Split(',').Select(x => x.Trim()).ToArray();
                    if (keyValueStr.Length == 2)
                    {
                        object dictionaryKey = (TKey) typeConverterKey.ConvertFromInvariantString(keyValueStr[0]);
                        var dictionaryValue = typeConverterKey.ConvertFromInvariantString(keyValueStr[1]);
                        if (dictionaryKey != null && dictionaryValue != null)
                        {
                            if (!result.ContainsKey((TKey) dictionaryKey))
                                result.Add((TKey) dictionaryKey, (TValue) dictionaryValue);
                        }
                    }
                });

                return result;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///     Converts the given value object to the specified destination type using the specified context and arguments
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="culture">Culture</param>
        /// <param name="value">Value</param>
        /// <param name="destinationType">Destination type</param>
        /// <returns>Result</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof (string))
            {
                var result = string.Empty;
                if (value != null)
                {
                    //we don't use string.Join() because it doesn't support invariant culture
                    var counter = 0;
                    var dictionary = (IDictionary<TKey, TValue>) value;
                    foreach (var keyValue in dictionary)
                    {
                        result += string.Format("{0}, {1}", Convert.ToString(keyValue.Key, CultureInfo.InvariantCulture),
                            Convert.ToString(keyValue.Value, CultureInfo.InvariantCulture));
                        //don't add ; after the last element
                        if (counter != dictionary.Count - 1)
                            result += ";";
                        counter++;
                    }
                }
                return result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}