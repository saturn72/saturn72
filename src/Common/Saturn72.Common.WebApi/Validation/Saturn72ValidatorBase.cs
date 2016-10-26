#region

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

#endregion

namespace Saturn72.Common.WebApi.Validation
{
    public class Saturn72ValidatorBase<TModel> : AbstractValidator<TModel>
    {
        public virtual bool AllowNullObject { get; protected set; }

        public override ValidationResult Validate(TModel instance)
        {
            return CheckIfNullObjectBeforeValidation(instance, () => base.Validate(instance));
        }

        public override ValidationResult Validate(ValidationContext<TModel> context)
        {
            return CheckIfNullObjectBeforeValidation(context.InstanceToValidate, () => base.Validate(context));
        }

        public override Task<ValidationResult> ValidateAsync(ValidationContext<TModel> context,
            CancellationToken cancellation = new CancellationToken())
        {
            return !AllowNullObject && context.InstanceToValidate == null
                ? Task.Run(
                    () => new ValidationResult(new[] {new ValidationFailure("Instance", "Object cannot be null")}))
                : base.ValidateAsync(context, cancellation);
        }

        #region Utilities

        private ValidationResult CheckIfNullObjectBeforeValidation<T>(T toValidate,
            Func<ValidationResult> validationFunc)
        {
            return !AllowNullObject && toValidate == null
                ? new ValidationResult(new[] {new ValidationFailure("Instance", "Object cannot be null")})
                : validationFunc();
        }

        #endregion
    }
}