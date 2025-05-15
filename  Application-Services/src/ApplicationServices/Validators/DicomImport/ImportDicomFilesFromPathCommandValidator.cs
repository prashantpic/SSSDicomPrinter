using FluentValidation;
using TheSSS.DicomViewer.Application.Features.DicomImport.Commands;

namespace TheSSS.DicomViewer.Application.Features.DicomImport.Validators;

public class ImportDicomFilesFromPathCommandValidator : AbstractValidator<ImportDicomFilesFromPathCommand>
{
    public ImportDicomFilesFromPathCommandValidator()
    {
        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("Import path cannot be empty")
            .NotNull().WithMessage("Import path is required");
    }
}