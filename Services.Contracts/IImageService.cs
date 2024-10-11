using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IImageService
    {
        Task ProcessImages(string UserId, List<IFormFile> Files);

    }
}
