using AutoMapper;
using Flight.Contracts;
using Flight.Data;
using Flight.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flight.Infrastructure
{
    public class FlightService : IFlightService
    {
        private readonly IApplicationCache _cache;
        private readonly IFlightRepository _flightRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FlightService> _logger;

        public FlightService(IApplicationCache cache,
            IFlightRepository flightRepository,
            IMapper mapper,
            ILogger<FlightService> logger)
        {
            _cache = cache;
            _flightRepository = flightRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AirFlightView>> Filter(AirFlightFilterView filterView,
            CancellationToken cancellationToken)
        {
            try
            {
                var cachedResult =
                    await _cache.GetFromCache<IEnumerable<AirFlightView>>("flights", cancellationToken) ??
                    await List(cancellationToken);
                var flightViews = cachedResult.ToList();
                if (!flightViews.Any())
                    return Enumerable.Empty<AirFlightView>();
                IEnumerable<AirFlightView>? result;
                if (!string.IsNullOrWhiteSpace(filterView.Origin)
                    && !string.IsNullOrWhiteSpace(filterView.Destination))
                    result = flightViews.Where(p => p.Origin == filterView.Origin);
                else if (string.IsNullOrWhiteSpace(filterView.Origin)
                         && !string.IsNullOrWhiteSpace(filterView.Destination))
                    result = flightViews.Where(p => p.Destination == filterView.Destination);
                else if (string.IsNullOrWhiteSpace(filterView.Destination)
                         && !string.IsNullOrWhiteSpace(filterView.Origin))
                    result = flightViews.Where(p => p.Origin == filterView.Origin);
                else result = Enumerable.Empty<AirFlightView>();
                return _mapper.Map<IEnumerable<AirFlightView>>(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "fail on get flight list");
                return Enumerable.Empty<AirFlightView>();
            }
        }

        public async Task<AirFlightView> RegisterFlight(AirFlight create, CancellationToken cancellationToken)
        {
            try
            {
                await _flightRepository.Insert(_mapper.Map<AirFlight>(create), cancellationToken);
                var view = await UpdateOnCreate(create, cancellationToken);
                return view;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "fail on register flight");
                return null;
            }
        }

        public async Task<bool> EditFlight(AirFlightView view, CancellationToken cancellationToken)
        {
            try
            {
                var flight = await _flightRepository.GetById(view.Id, cancellationToken);
                if (flight is null) return false;
                flight.Origin = view.Origin;
                flight.Departure = view.Departure;
                flight.Destination = view.Destination;
                flight.Arrival = view.Arrival;
                flight.Status = view.Status;
                await _flightRepository.Update(flight, cancellationToken);
                await UpdateOnEdit(view, cancellationToken);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "fail on update flight");
                return false;
            }
        }

        public async Task<IEnumerable<AirFlightView>> List(CancellationToken cancellationToken)
        {
            try
            {
                var cachedResult = await _cache.GetFromCache<IEnumerable<AirFlightView>>("flights", cancellationToken);
                if (cachedResult is not null)
                    return cachedResult;
                var dbResult = await _flightRepository
                    .Get()
                    .ToListAsync(cancellationToken: cancellationToken);
                var result = _mapper.Map<IEnumerable<AirFlightView>>(dbResult);
                await _cache.SetCacheAsync<IEnumerable<AirFlightView>>("flights", result, cancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "fail on flight list");
                return Enumerable.Empty<AirFlightView>();
            }
        }

        private async Task UpdateCacheById(int flightId, CancellationToken cancellationToken)
        {
            var cachedResult = await _cache.GetFromCache<IEnumerable<AirFlightView>>("flights", cancellationToken);
            if (cachedResult != null)
            {
                var flightDb = await _flightRepository.GetById(flightId, cancellationToken);
                var cache = cachedResult.ToList();
                cache.Add(_mapper.Map<AirFlightView>(flightDb));
                await _cache.Remove("flights", cancellationToken);
                _ = await List(cancellationToken);
            }
        }
        private async Task UpdateOnEdit(AirFlightView flightView, CancellationToken cancellationToken)
        {
            var cachedResult = await _cache.GetFromCache<IEnumerable<AirFlightView>>("flights", cancellationToken);
            if (cachedResult != null)
            {
                var flight = await _flightRepository.Get(p => p.Origin == flightView.Origin
                                                              && p.Destination == flightView.Destination && p.Arrival == flightView.Arrival && p.Departure == flightView.Departure
                                                              && p.Status == flightView.Status).FirstOrDefaultAsync(cancellationToken);
                var cache = cachedResult.ToList();
                cache.Add(_mapper.Map<AirFlightView>(flight));
                await _cache.Remove("flights", cancellationToken);
                _ = await List(cancellationToken);
            }
        }
        private async Task<AirFlightView> UpdateOnCreate(AirFlight create, CancellationToken cancellationToken)
        {
            AirFlightView airFlightView = null;
            var cachedResult = await _cache.GetFromCache<IEnumerable<AirFlightView>>("flights", cancellationToken);
            if (cachedResult != null)
            {
                var flight = await _flightRepository.Get(p => p.Origin == create.Origin
                                                              && p.Destination == create.Destination &&
                                                              p.Arrival == create.Arrival &&
                                                              p.Departure == create.Departure
                                                              && p.Status == create.Status)
                    .FirstOrDefaultAsync(cancellationToken);
                List<AirFlightView?> cache = cachedResult.ToList();
                airFlightView = _mapper.Map<AirFlightView>(flight);
                cache.Add(airFlightView);
                await _cache.Remove("flights", cancellationToken);
                _ = await List(cancellationToken);
            }
            return airFlightView;
        }
    }
}
