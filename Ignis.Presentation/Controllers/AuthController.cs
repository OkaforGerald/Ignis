using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using SharedAPI;
using SharedAPI.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignis.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IServiceManager manager;

        public AuthController(IServiceManager manager)
        {
            this.manager = manager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request is null) return BadRequest(ResponseDto<string>.Error(400, Error: "Register body can not be null!"));

            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

            var result = await manager.Auth.RegisterUser(request);

            if (!result.Succeeded) return BadRequest(ResponseDto<string>.Error(400, Errors: result.Errors.Select(x => x.Description)));

            return Ok(ResponseDto<string>.Success(200, "Registration successful!"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request is null) return BadRequest(ResponseDto<string>.Error(400, Error: "Login body can not be null!"));

            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

            var result = await manager.Auth.AuthenticateUser(request);

            if(!result) return Unauthorized(ResponseDto<string>.Error(401, Error: "Invalid username/password!"));

            var tokens = await manager.Auth.GenerateToken(true);

            return Ok(ResponseDto<Tokens>.Success(200, tokens));
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] Tokens tokens)
        {
            if (tokens is null) return BadRequest(ResponseDto<string>.Error(400, Error: "Refresh token body can not be null!"));

            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

            var result = await manager.Auth.RefreshToken(tokens);

            return Ok(ResponseDto<Tokens>.Success(200, result));
        }
    }
}
