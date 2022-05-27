using FluentValidation;
using MediatR;

namespace Flight.Infrastructure.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!_validators.Any())
                return await next();
            var failures = _validators
                .Select(x => x.Validate(new ValidationContext<TRequest>(request)))
                .SelectMany(x => x.Errors)
                .Where(x => x != null).ToList();
            if (failures.Any())
                throw new ValidationException(failures);
            return await next();
        }
    }
}
