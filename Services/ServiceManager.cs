using AutoMapper;
using Entities.Models;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
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
        private readonly Lazy<IImageService> imageService;

        public ServiceManager(UserManager<User> manager, IMapper mapper, IConfiguration configManager, IBackgroundJobClient jobClient, IWebHostEnvironment env)
        {
            authService = new Lazy<IAuthService>(new AuthService(manager, mapper, configManager));
            imageService = new Lazy<IImageService>(new ImageService(jobClient, env, manager));
        }

        public IAuthService Auth => authService.Value;

        public IImageService Image => imageService.Value;
    }
}
