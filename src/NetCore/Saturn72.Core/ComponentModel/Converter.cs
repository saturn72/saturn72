using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Saturn72.Core.ComponentModel
{
    public class Converter
    {
        private readonly  IDictionary<Type, bool> _canConvertFromData = new Dictionary<Type, bool>();
        private readonly IDictionary<Type, bool> _canConvertToData = new Dictionary<Type, bool>();
        public Converter(TypeConverter typeConverter)
        {
            TypeConverter = typeConverter;
        }

        public TypeConverter TypeConverter { get; }

        public bool CanConvertFrom(Type type)
        {
            if (_canConvertFromData.ContainsKey(type))
                return _canConvertFromData[type];
            var canConvertFrom = TypeConverter.CanConvertFrom(type);
            _canConvertFromData[type] = canConvertFrom;
            return canConvertFrom;
        }

        public bool CanConvertTo(Type type)
        {
            if (_canConvertToData.ContainsKey(type))
                return _canConvertToData[type];
            var canConvertFrom = TypeConverter.CanConvertTo(type);
            _canConvertToData[type] = canConvertFrom;
            return canConvertFrom;
        }
    }
}