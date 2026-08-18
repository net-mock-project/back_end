using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models.AuditLogs;
using RescueHub.Application.Common.Exceptions;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Application.Features.AuditLogs.Queries;
using RescueHub.Domain.Common.Querying;

namespace RescueHub.API.Controllers
{
    [ApiController]
    [Route("api/audit-logs")]
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public AuditLogsController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] AuditLogQueryRequest request,
            CancellationToken cancellationToken)
        {
            var queryRequest = _mapper.Map<QueryRequest>(request);
            var criteria = _mapper.Map<QueryCriteria>(queryRequest);

            var result = await _sender.Send(
                new GetAuditLogsQuery(criteria),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAuditLogById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetAuditLogByIdQuery(id),
                cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"Audit log '{id}' not found.");
            }

            return Ok(result);
        }
    }
}
