using FluentValidation;
using TheSSS.DicomViewer.Application.Features.DicomNetwork.CEchoScu.Commands;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CEchoScu.Validators;

public class VerifyPacsConnectionCommandValidator : AbstractValidator<VerifyPacsConnectionCommand>
{
    public VerifyPacsConnectionCommandValidator()
    {
        RuleFor(x => x.PacsNodeId)
            .GreaterThan(0).WithMessage("PACS Node ID must be greater than zero");
    }
}