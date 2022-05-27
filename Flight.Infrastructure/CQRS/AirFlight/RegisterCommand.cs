using AutoMapper;
using Flight.Contracts;
using Flight.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flight.Infrastructure.CQRS.AirFlight
{
    public class RegisterFlightCommand : IRequest<AirFlightView>
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTimeOffset Departure { get; set; }
        public DateTimeOffset Arrival { get; set; }
        public Status Status { get; set; }
    }
    public class RegisterFlightCommandHandler : IRequestHandler<RegisterFlightCommand, AirFlightView>
    {
        private readonly IMapper _mapper;
        private readonly IFlightService _service;
        private readonly ILogger<RegisterFlightCommandHandler> _logger;
        public RegisterFlightCommandHandler(ILogger<RegisterFlightCommandHandler> logger,
            IFlightService service, IMapper mapper)
        {
            _logger = logger;
            _service = service;
            _mapper = mapper;
        }
        public async Task<AirFlightView> Handle(RegisterFlightCommand command, CancellationToken cancellationToken)
        {
            return await _service.RegisterFlight(_mapper.Map<Domain.AirFlight>(command), cancellationToken) ?? null;
        }
    }

}
