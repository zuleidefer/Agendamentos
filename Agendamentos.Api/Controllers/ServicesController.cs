using Agendamentos.Application.Common;
using Agendamentos.Application.Features.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agendamentos.Api.Controllers;

[ApiController]
[Route("api/services")]
public class ServicesController(IServiceAppService appService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ServiceResponse>> Create(CreateServiceRequest request, CancellationToken ct)
    {
        var created = await appService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ServiceResponse>>> List(
        int page = 1, int pageSize = 10, CancellationToken ct = default) =>
        Ok(await appService.ListAsync(page, pageSize, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServiceResponse>> GetById(Guid id, CancellationToken ct) =>
        Ok(await appService.GetByIdAsync(id, ct));
}