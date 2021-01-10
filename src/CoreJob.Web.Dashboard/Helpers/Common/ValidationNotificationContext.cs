using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace CoreJob.Web.Dashboard.Helpers.Common
{
    public class ValidationNotificationContext : IValidationNotificationContext
    {
        private List<ValidationFailure> failures { get; }

        public ValidationNotificationContext()
        {
            failures = new List<ValidationFailure>();
        }

        public bool HasErrorNotifications { get => failures.Count > 0; }

        public IEnumerable<ValidationFailure> GetErrorNotifications()
        {
            return failures;
        }

        public void NotifyError(ValidationFailure failure)
        {
            this.failures.Add(failure);
        }
    }
}
