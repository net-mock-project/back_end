using FluentValidation;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.AuditLogs;
using RescueHub.Domain.Interfaces.Volunteers;

namespace RescueHub.Application.Features.Volunteers.Commands;

public record CancelVolunteerProfileCommand(
    Guid UserId
) : IRequest<bool>;

public class CancelVolunteerProfileCommandHandler
    : IRequestHandler<CancelVolunteerProfileCommand, bool>
{
    private readonly IVolunteerService _volunteerService;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelVolunteerProfileCommandHandler(
        IVolunteerService volunteerService,
        IAuditLogService auditLogService,
        IUnitOfWork unitOfWork)
    {
        _volunteerService = volunteerService;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        CancelVolunteerProfileCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var isSuccess = await _volunteerService.CancelProfileAsync(
                request.UserId,
                cancellationToken);

            if (!isSuccess)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return false;
            }

            // Ghi Audit Log cho hành động hủy đơn
            var auditLog = new AuditLog(
                userId: request.UserId,
                action: "CANCEL_APPLICATION",
                entityName: nameof(Volunteer),
                entityId: request.UserId,
                oldValue: "Pending",
                newValue: "Deleted");

            await _auditLogService.CreateAsync(auditLog, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public class CancelVolunteerProfileCommandValidator
    : AbstractValidator<CancelVolunteerProfileCommand>
    {
        public CancelVolunteerProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");
        }
    }
}