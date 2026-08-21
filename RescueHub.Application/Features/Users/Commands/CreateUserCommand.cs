using FluentValidation;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Application.Contracts.Users;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.AuditLogs;
using System.Text.Json;
using RescueHub.Domain.Interfaces.Roles;

namespace RescueHub.Application.Features.Users.Commands
{
    public record CreateUserCommand(
        string RoleName,
        string? Province,
        string FullName,
        string Email,
        string Phone,
        DateOnly? DateOfBirth,
        Gender? Gender,
        string Password,
        Guid PerformedByUserId
    ) : IRequest<CreateUserDto>;


    public class CreateUserCommandHandler
        : IRequestHandler<CreateUserCommand, CreateUserDto>
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly IRoleRepository _roleRepository;

        public CreateUserCommandHandler(
            IUserService userService,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork,
            IAuditLogService auditLogService,
            IRoleRepository roleRepository)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
            _roleRepository = roleRepository;
        }

        public async Task<CreateUserDto> Handle(
    CreateUserCommand request,
    CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByNameAsync(
                request.RoleName.Trim(),
                cancellationToken);

            if (role == null)
            {
                throw new ArgumentException(
                    $"Role '{request.RoleName}' does not exist.");
            }

            var passwordHash =
                _passwordHasher.Hash(request.Password);

            await _unitOfWork.BeginTransactionAsync(
                cancellationToken);

            try
            {
                var user = await _userService.CreateUserAsync(
                    role.RoleId,
                    request.Province,
                    request.FullName,
                    request.Email,
                    request.Phone,
                    request.DateOfBirth,
                    request.Gender,
                    passwordHash,
                    cancellationToken);

                var newValue = JsonSerializer.Serialize(new
                {
                    roleId = user.RoleId,
                    province = user.Province,
                    fullName = user.FullName,
                    email = user.Email,
                    phone = user.Phone,
                    dateOfBirth = user.DateOfBirth,
                    gender = user.Gender?.ToString(),
                    status = user.Status.ToString(),
                    isVerified = user.IsVerified
                });

                var auditLog = new AuditLog(
                    request.PerformedByUserId,
                    "Create",
                    "User",
                    user.Id,
                    null,
                    newValue);

                await _auditLogService.CreateAsync(
                    auditLog,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                await _unitOfWork.CommitAsync(
                    cancellationToken);

                return new CreateUserDto
                {
                    UserId = user.Id
                };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }
    }


    public class CreateUserCommandValidator
        : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.PerformedByUserId)
                .NotEmpty()
                .WithMessage("PerformedByUserId is required.");

            RuleFor(x => x.RoleName)
                .NotEmpty()
                .WithMessage("Role is required.")
                .MaximumLength(50)
                .WithMessage(
                    "Role name must not exceed 50 characters.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required.")
                .MaximumLength(150)
                .WithMessage(
                    "Full name must not exceed 150 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email format is invalid.")
                .MaximumLength(255)
                .WithMessage(
                    "Email must not exceed 255 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^(0|\+84)[0-9]{9,10}$")
                .WithMessage("Phone number is invalid.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage(
                    "Password must be at least 8 characters long.");

            RuleFor(x => x.Province)
                .MaximumLength(100)
                .WithMessage(
                    "Province must not exceed 100 characters.");

            RuleFor(x => x.Gender)
                .IsInEnum()
                .When(x => x.Gender.HasValue)
                .WithMessage("Gender is invalid.");

            RuleFor(x => x.DateOfBirth)
                .Must(date =>
                    !date.HasValue ||
                    date.Value <
                    DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(
                    "Date of birth must be in the past.");
        }
    }
}