using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAuthService> authService;

        public ServiceManager(UserManager<User> manager, IMapper mapper, IConfiguration configManager)
        {
            authService = new Lazy<IAuthService>(new AuthService(manager, mapper, configManager));
        }

        public IAuthService Auth => authService.Value;
    }
}
