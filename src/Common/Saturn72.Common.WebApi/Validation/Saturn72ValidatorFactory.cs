#region

using System;
using FluentValidation;
using FluentValidation.Attributes;
using Saturn72.Core.Infrastructure;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.WebApi.Validation
{
    public class Saturn72ValidatorFactory : AttributedValidatorFactory
    {
        public override IValidator GetValidator(Type type)
        {
            if (type.IsNull())
                return null;

            IValidator validatorInstance = null;

            var attribute = (ValidatorAttribute) Attribute.GetCustomAttribute(type, typeof (ValidatorAttribute));

            if ((attribute != null) && (attribute.ValidatorType != null))
            {
                var instance = AppEngine.Current.IocContainerManager.ResolveUnregistered(attribute.ValidatorType);
                validatorInstance =  instance as IValidator;
            }

            return validatorInstance;
        }
    }
}