using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Services.Contracts;
using TechXpress.Services.DTOs.UserDtos;

namespace TechXpress.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Register")]
        public async  Task<IActionResult> Register(UserRegisterDto user)
        {
            return Ok(await _accountService.Register(user));
        }
    }
}
