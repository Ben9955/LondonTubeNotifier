using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.Interfaces;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Infrastructure.Data;
using LondonTubeNotifier.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LondonTubeNotifier.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IUser?> GetUserWithSubscriptionsAsync(Guid id)
        {
            var user = await _dbContext.Users
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }
    }
}
