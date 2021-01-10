using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace CoreJob.Web.Dashboard.Helpers.Common
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
           where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        private readonly IValidationNotificationContext _notificationContext;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators,
            IValidationNotificationContext notificationContext)
        {
            _validators = validators;
            _notificationContext = notificationContext;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);
            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                Notify(failures);
                return next();
            }

            return next();
        }

        private void Notify(IEnumerable<ValidationFailure> failures)
        {
            foreach (var failure in failures)
            {
                _notificationContext.NotifyError(failure);
            }
        }
    }
}
