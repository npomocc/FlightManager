using AutoMapper;
using Flight.Contracts;
using Flight.Domain;
using Flight.Infrastructure.CQRS.AirFlight;

namespace Flight.Infrastructure.Profiles
{
    public class FlightProfiles : Profile
    {
        public FlightProfiles()
        {
            CreateMap<AirFlight, AirFlightView>();
            CreateMap<AirFlight, RegisterAirFlight>();
            CreateMap<RegisterAirFlight, AirFlight>();
            CreateMap<RegisterFlightCommand, AirFlight>();

            CreateMap<User, UserView>();
        }
    }
}
