using MediatR;
using Microsoft.AspNetCore.Identity;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Notifications;
using ThomasMathers.Infrastructure.IAM.Responses;

namespace ThomasMathers.Infrastructure.IAM.Services
{
    public interface IUserService
    {
        Task<User> GetUserByUserName(string userName);
        Task<RegisterResponse> Register(User user, string password);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;

        public UserService(UserManager<User> userManager, IMediator mediator)
        {
            _userManager = userManager;
            _mediator = mediator;
        }

        public Task<User> GetUserByUserName(string userName)
        {
            return _userManager.FindByNameAsync(userName);
        }

        public async Task<RegisterResponse> Register(User user, string password)
        {
            var createResult = await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                return new IdentityErrorResponse(createResult.Errors);
            }

            await _mediator.Publish(new UserRegisteredNotification
            {
                User = user
            });

            return new RegisterSuccessResponse(user);
        }
    }
}
