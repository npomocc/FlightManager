using System.Threading;
using Flight.Contracts;
using Flight.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Flight.Infrastructure.CQRS.AirFlight;
using MediatR;

namespace FlightManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FlightsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFlightService _flightService;
    private readonly IJwtAuth _jwtAuth;
    public FlightsController(IMediator mediator, IFlightService flightService, IJwtAuth jwtAuth)
    {
        _mediator = mediator;
        _flightService = flightService;
        _jwtAuth = jwtAuth;
    }
    [AllowAnonymous]
    // POST api/<MembersController>
    [HttpPost("authentication")]
    public async Task<IActionResult> Authentication([FromBody] LoginView loginView, CancellationToken cancellationToken)
    {
        var token = await _jwtAuth.Authentication(loginView, cancellationToken);
        return string.IsNullOrWhiteSpace(token) 
            ? Unauthorized() 
            : Ok(token);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var user = User;
        return Ok(await _flightService.List(cancellationToken));
    }

    [Authorize]
    [HttpPost("filter")]
    public async Task<IActionResult> Filter(AirFlightFilterView filterView, CancellationToken cancellationToken)
    {
        var user = User;
        return Ok(await _flightService.Filter(filterView, cancellationToken));
    }

    [Authorize(Roles = "Moderator")]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterFlight(RegisterFlightCommand register, CancellationToken cancellationToken)
    {
        var view = await _mediator.Send(register, cancellationToken);
        return view is not null
            ? Ok(view) 
            : BadRequest("not_added");
    }

    [Authorize(Roles = "Moderator")]
    [HttpPost("edit")]
    public async Task<IActionResult> EditFlight(AirFlightView flightView, CancellationToken cancellationToken)
    {
        var edited = await _flightService.EditFlight(flightView, cancellationToken);
        return edited
            ? Ok("edited")
            : BadRequest("not_edited");
    }
}