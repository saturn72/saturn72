using System;
using System.Collections.Generic;

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public class IocRegistrationRecord
    {
        public Type ImplementedType { get; set; }

        public IEnumerable<Type> ServiceTypes { get; set; }
        public IDictionary<string, object> Metadata { get; set; }
        public string RegistrationId { get; set; }
        public ActivatorType ActivatorType { get; set; }
        public IEnumerable<object> Keys { get; set; }
    }
}