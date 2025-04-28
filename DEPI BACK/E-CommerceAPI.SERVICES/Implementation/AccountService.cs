using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.AccountDTO;
using E_CommerceAPI.ENTITES.DTOs.UserDTO;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Stripe;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class AccountService : ResponseHandler, IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailService _mailService;
        private readonly ILogger<AccountService> _logger;
        private readonly IFileService _fileService;

        public AccountService(UserManager<ApplicationUser> userManager,
                              IUnitOfWork unitOfWork,
                              IMapper mapper,
                              ITokenService tokenService,
                              IHttpContextAccessor httpContextAccessor,
                              IMailService mailService,
                              ILogger<AccountService> logger,
                              IFileService fileService

                             )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _mailService = mailService;
            _logger = logger;
            _fileService = fileService;

        }

        public async Task<Response<LoginResponse>> LoginAsync(LoginDto dto)
        {
            //Check if user is exist or not
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return BadRequest<LoginResponse>("Invalid Email or Password!");
            }
            if (user.EmailConfirmed == false)
            {
                return BadRequest<LoginResponse>("Please confirm your email before logging in.");
            }
            var (jwtToken, accessToken) = await _tokenService.GenerateJwtToken(user);
            var refreshToken = "";
            DateTime refreshTokenExpiration;

            if (user.RefreshTokens!.Any(t => t.IsActive))
            {
                var activeToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                refreshToken = activeToken.Token;
                refreshTokenExpiration = activeToken.ExpiresOn;
            }
            else
            {
                var RefreshToken = _tokenService.CreateRefreshToken();
                refreshToken = RefreshToken.Token;
                refreshTokenExpiration = RefreshToken.ExpiresOn;
                user.RefreshTokens.Add(RefreshToken);
                await _userManager.UpdateAsync(user);

            }
            return Success(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration,
                Image = user.Image,
                UserName = user.UserName,
                Email = user.Email,

            });

        }
        public async Task<Response<LoginResponse>> GetRefreshToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest<LoginResponse>("Invalid Email!");
            }

            var ActiveToken = user.RefreshTokens!.FirstOrDefault(t => t.IsActive == true);

            if (ActiveToken != null)
            {
                return BadRequest<LoginResponse>("There is a Token still active <3");

            }

            var refreshToken = _tokenService.CreateRefreshToken();
            var (jwtToken, accessToken) = await _tokenService.GenerateJwtToken(user);
            user.RefreshTokens!.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return Success(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn,
                UserName = user.UserName,
                Email = user.Email,
            });

        }
        public async Task<Response<string>> RegisterAsync(RegisterDto register)
        {
            try
            {
                // Check if the user already exists
                if (await _userManager.FindByNameAsync(register.UserName) != null)
                {
                    return BadRequest<string>("A user with this username already exists!");
                }

                if (await _userManager.FindByEmailAsync(register.Email) != null)
                {
                    return BadRequest<string>("A user with this email already exists!");
                }

                // Map DTO to ApplicationUser
                var user = _mapper.Map<ApplicationUser>(register);

                // Create the user
                var result = await _userManager.CreateAsync(user, register.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest<string>(errors);
                }

                // Assign role
                await _userManager.AddToRoleAsync(user, "Customer");

                // Send confirmation email
                var confirmEmailResult = await _mailService.ConfirmEmailAsync(user, "Confirm Your Email");

                if (confirmEmailResult != "Email Confirmed Successfully")
                {
                    return BadRequest<string>("Failed to send the confirmation email. Please try again.");
                }

                return Success("Account created successfully! A confirmation email has been sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user.");
                return BadRequest<string>("An unexpected error occurred during registration.");
            }
        }

        public async Task<Response<string>> DeleteAccountAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return BadRequest<string>("Email or current password is INCORRECT!");

            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest<string>("Failed to delete your account, try again later!");

            }
            return Success("Your Account deleted Successfully,we hope you will back again.");
        }
        public async Task<Response<string>> ChangePassword(PasswordSettingDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.CurrentPassword))
            {
                return BadRequest<string>("Email or current password is INCORRECT!");

            }

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest<string>("Failed to change password, try again!");

            }
            return Success("Your Password changed successfully!");

        }
        public async Task<Response<string>> UpdateProfile(UserDto dto, ApplicationUser user)
        {

            if (user == null)
            {
                return BadRequest<string>("User not found!");
            }

            try
            {
                // Update only the provided fields
                if (!string.IsNullOrEmpty(dto.FirstName)) user.FirstName = dto.FirstName;
                if (!string.IsNullOrEmpty(dto.LastName)) user.LastName = dto.LastName;
                if (!string.IsNullOrEmpty(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
                if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
                if (!string.IsNullOrEmpty(dto.Address)) user.Address = dto.Address;

                // Handle image upload if provided
                if (dto.Image != null)
                {
                    // Define allowed extensions and maximum file size (e.g., 2 MB = 2 * 1024 * 1024 bytes)
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var maxFileSizeInBytes = 2 * 1024 * 1024; // 2 MB

                    // Validate file extension
                    var extension = Path.GetExtension(dto.Image.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        return BadRequest<string>($"Invalid image extension: {extension}. Allowed extensions are: {string.Join(", ", allowedExtensions)}");
                    }

                    // Validate file size
                    if (dto.Image.Length > maxFileSizeInBytes)
                    {
                        return BadRequest<string>($"Image file size exceeds the maximum limit of {maxFileSizeInBytes / (1024 * 1024)} MB.");
                    }

                    // Construct base URL for image storage
                    var httpContext = _httpContextAccessor.HttpContext;
                    if (httpContext?.Request == null)
                    {
                        return BadRequest<string>("HttpContext or Request is not available.");
                    }

                    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

                    // Delete old images from disk
                    if (user.Image != null)
                    {
                        _fileService.DeleteImage(user.Image);
                    }
                    // Upload the image and get the relative path
                    var uploadResult = await _fileService.UploadImage("Accounts", dto.Image);
                    if (uploadResult.StartsWith("Error:"))
                    {
                        return BadRequest<string>(uploadResult);
                    }

                    // Construct the full image URL
                    string imageUrl = $"{baseUrl}{uploadResult}";
                    user.Image = imageUrl;
                }

                // Save changes to the database
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Updated<string>("Your profile updated successfully!");
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest<string>($"Failed to update profile. Errors: {string.Join(", ", errors)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user profile.");
                return BadRequest<string>("Cannot update or edit your profile, try again!");
            }
        }


        public async Task<Response<string>> SendResetPassword(string email)
        {
            var applicationUser = await _userManager.FindByEmailAsync(email);
            if (applicationUser != null)
            {

                var result = await _mailService.SendOtpAsync(applicationUser, "Your OTP for Account Verification");
                if (result == "OTP sent successfully!")
                {
                    _logger.LogInformation("OTP sent successfully to the user.");

                }
                else
                {
                    _logger.LogError($"Failed to send OTP: {result}");

                }
            }

            else
            {
                _logger.LogInformation("No User With This Email Found.");
            }
            return Updated<string>("if you are registered user, please check your email!");
        }


        public async Task<Response<string>> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.email);
            if (user == null)
            {
                return BadRequest<string>("User not found!");
            }
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return BadRequest<string>("New password and confirm password do not match!");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return BadRequest<string>($"Failed to reset password: {errors}");
            }

            return Updated<string>("Your password reset successfully!");
        }
        public async Task<Response<string>> ConfirmResetPassword(string Code, string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
                return BadRequest<string>("There is something wrong");

            var userCode = user.Code;

            if (userCode == Code)
            {
                user.Code = null;
                await _userManager.UpdateAsync(user);
                return Success("OTP Confirmed SuccessFully");
            }
            return BadRequest<string>("OTP Code is Wrong");
        }

        public async Task<Response<string>> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return BadRequest<string>("UserId or Code cannot be null or empty.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound<string>("User not found.");
                }

                var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, code);

                if (!confirmEmailResult.Succeeded)
                {
                    return BadRequest<string>("Failed to confirm your email. Please try again.");
                }

                return Success("Your email has been confirmed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming the user's email.");
                return BadRequest<string>("An unexpected error occurred during email confirmation.");
            }
        }


    }
}
