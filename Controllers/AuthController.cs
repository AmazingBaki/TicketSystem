using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TicketSupportSystem.Data.Entities;
using TicketSupportSystem.DTOs.Requests;
using TicketSupportSystem.DTOs.Responses;
using TicketSupportSystem.Interfaces;

namespace TicketSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenService _tokenService;
        private readonly IMapper _mapper;
        private IValidator<UserLoginDTO> _loginValidator;
        private IValidator<UserRegistrationDTO> _registrationValidator;

        public AuthController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IJwtTokenService tokenService, IValidator<UserRegistrationDTO> registrationValidator, IValidator<UserLoginDTO> loginValidator)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _registrationValidator = registrationValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegistrationDTO userRegDTO)
        {
            userRegDTO.Email = userRegDTO.Email?.Trim() ?? string.Empty;
            userRegDTO.UserName = userRegDTO.UserName?.Trim() ?? string.Empty;
            userRegDTO.Name = userRegDTO.Name?.Trim() ?? string.Empty;
            userRegDTO.Surname = userRegDTO.Surname?.Trim() ?? string.Empty;
            userRegDTO.PhoneNumber = userRegDTO.PhoneNumber?.Trim() ?? string.Empty;

            var validationRes = _registrationValidator.Validate(userRegDTO);
            if (!validationRes.IsValid)
            {
                return BadRequest(new
                {
                    message = "Проверьте введённые данные.",
                    errors = validationRes.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            var user = _mapper.Map<UserRegistrationDTO, User>(userRegDTO);
            var result = await _userManager.CreateAsync(user, userRegDTO.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(user.Id);
            }

            return BadRequest(new
            {
                message = "Не удалось зарегистрироваться. Часто это значит, что такой email или имя пользователя уже заняты.",
                errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
        {
            userLoginDTO.Email = userLoginDTO.Email?.Trim() ?? string.Empty;

            var validationRes = _loginValidator.Validate(userLoginDTO);
            if (!validationRes.IsValid)
            {
                return BadRequest(new
                {
                    message = "Проверьте email и пароль.",
                    errors = validationRes.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            var user = await _userManager.FindByEmailAsync(userLoginDTO.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, userLoginDTO.Password))
            {
                var token = await _tokenService.GenerateJwtToken(user);
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Неверный email или пароль." });
        }

        [Authorize]
        [HttpGet("Me")]
        public async Task<IActionResult> Me()
        {
            var currentUserEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(currentUserEmail))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByEmailAsync(currentUserEmail);
            if (user is null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var currentUser = new CurrentUserDTO
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Roles = roles.ToList()
            };

            return Ok(currentUser);
        }


    }
}
