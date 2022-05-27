using AutoMapper;
using Flight.Contracts;
using Flight.Infrastructure;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Flight.Infrastructure.CQRS.AirFlight;

namespace FlightManager.CQRS.AirFlight
{
    public class RegisterFlightCommandHandler : IRequestHandler<RegisterFlightCommand, AirFlightView>
    {
        private readonly IMapper _mapper;
        private readonly IFlightService _service;
        public RegisterFlightCommandHandler(
            IFlightService service, 
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        public async Task<AirFlightView> Handle(RegisterFlightCommand command, CancellationToken cancellationToken)
        {
            return await _service.RegisterFlight(_mapper.Map<Flight.Domain.AirFlight>(command), cancellationToken);
        }
    }

}
