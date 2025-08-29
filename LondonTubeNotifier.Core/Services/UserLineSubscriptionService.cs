using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.Interfaces;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.Exceptions;
using LondonTubeNotifier.Core.ServiceContracts;

namespace LondonTubeNotifier.Core.Services
{
    public class UserLineSubscriptionService : IUserLineSubscriptionService
    {
        private readonly ILineRepository _lineRepository;
        private readonly IUserRepository _userRepository;

        public UserLineSubscriptionService(ILineRepository lineRepository, IUserRepository userRepository )
        {
            _lineRepository = lineRepository;
            _userRepository = userRepository;
        }
        public async Task SubscribeAsync(Guid userId, string lineId)
        {

            IUser user = await _userRepository.GetUserWithSubscriptionsAsync(userId)
                ?? throw new DomainNotFoundException("Usernot found");

            Line line = await _lineRepository.GetLineByLineIdAsync(lineId)
                ?? throw new DomainNotFoundException("Line not found");

            if (user.Subscriptions.Any(s => s.Id == lineId))
                throw new DomainValidationException("Already subscribed");

            await _lineRepository.AddSubscriptionAsync(user, line);
        }

        public async Task UnsubscribeAsync(Guid userId, string lineId)
        {
            IUser user = await _userRepository.GetUserWithSubscriptionsAsync(userId)
                ?? throw new DomainNotFoundException("User not found");

            Line line = await _lineRepository.GetLineByLineIdAsync(lineId)
                ?? throw new DomainNotFoundException("Line not found");

            if (!user.Subscriptions.Any(s => s.Id == lineId))
                throw new DomainValidationException("User not subscribed to this Line");

            await _lineRepository.DeleteSubscriptionAsync(user, line);
        }

        public async Task<IEnumerable<LineDto>> GetUserSubscriptionsAsync(Guid userId)
        {
            IUser user = await _userRepository.GetUserWithSubscriptionsAsync(userId)
                ?? throw new DomainNotFoundException("User not found");

            var linesDto = user.Subscriptions.Select(l => new LineDto
            {
                Id = l.Id,
                Code = l.Code,
                Name = l.Name,
                Color = l.Color,
            });

            return linesDto;
        }

    }
}
