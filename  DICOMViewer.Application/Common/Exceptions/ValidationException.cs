using System.Collections.Generic;
using FluentValidation.Results;

namespace TheSSS.DICOMViewer.Application.Common.Exceptions;

public class ValidationException : System.Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException() : base("Validation failures occurred") => Errors = new Dictionary<string, string[]>();

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());
    }
}