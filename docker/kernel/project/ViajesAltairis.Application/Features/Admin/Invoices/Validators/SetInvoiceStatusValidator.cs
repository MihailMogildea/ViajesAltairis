using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Invoices.Commands;

namespace ViajesAltairis.Application.Features.Admin.Invoices.Validators;

public class SetInvoiceStatusValidator : AbstractValidator<SetInvoiceStatusCommand>
{
    public SetInvoiceStatusValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.StatusId).GreaterThan(0);
    }
}
