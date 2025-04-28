using System.Net;
using E_Commerce_API.Base;
using E_CommerceAPI.ENTITES.DTOs.AccountDTO;
using E_CommerceAPI.ENTITES.DTOs.UserDTO;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Accounts")]

    public class AccountController : AppControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMailService _mailService;

        public AccountController(IAccountService accountService,
                                 IMailService mailService,
                                 ICurrentUserService currentUserService)
        {
            _accountService = accountService;
            _mailService = mailService;
            _currentUserService = currentUserService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _accountService.RegisterAsync(dto);

            return NewResult(account);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _accountService.LoginAsync(dto);
            return NewResult(response);
        }


        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordSettingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.ChangePassword(dto);
            return NewResult(response);
        }

        [HttpPost("ConfirmResetPassword")]
        public async Task<IActionResult> ConfirmResetPassword([FromQuery] string code, [FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(email))
                return BadRequest("Code and Email are required.");

            var response = await _accountService.ConfirmResetPassword(code, email);
            return NewResult(response);
        }
        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.ResetPassword(dto);
            return NewResult(response);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailQuery query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.ConfirmEmail(query.UserId, query.Code);
            return NewResult(response);
        }

        [HttpPost("SendResetPasswordEmail")]
        public async Task<IActionResult> SendResetPasswordEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var response = await _accountService.SendResetPassword(email);
            return NewResult(response);
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> NewRefreshToken([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var token = await _accountService.GetRefreshToken(email);
            return NewResult(token);
        }

        [HttpPut("EditProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUser = await _currentUserService.GetUserAsync();
            if (currentUser.Data == null)
                return NewResult(new Response<string>
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated.",
                });

            var account = await _accountService.UpdateProfile(dto, currentUser.Data);
            return NewResult(account);
        }

        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.DeleteAccountAsync(dto);
            return NewResult(response);
        }
    }
}
