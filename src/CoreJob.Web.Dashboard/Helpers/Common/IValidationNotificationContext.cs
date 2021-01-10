using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace CoreJob.Web.Dashboard.Helpers.Common
{
    public interface IValidationNotificationContext
    {
        bool HasErrorNotifications { get; }

        IEnumerable<ValidationFailure> GetErrorNotifications();

        void NotifyError(ValidationFailure failure);
    }
}
