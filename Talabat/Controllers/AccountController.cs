using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.IServices;

namespace Talabat.APIs.Controllers
{

    public class AccountController : BaseAPIController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IAuthService authService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _mapper = mapper;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return Unauthorized(new APIsResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new APIsResponse(401));
            return Ok(new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest(new APIsValidationErrorResponse() { Errors = new List<string>() { "This email already in use" } });
            var user = new AppUser()
            {
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                DisplayName = model.DisplayName,
                PhoneNumber = model.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new APIsResponse(400));
            return Ok(new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            return new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            };
        }
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindUserEmailWithAddressAsync(User);
            var address = _mapper.Map<AddressDto>(user.Address);
            return Ok(address);
        }
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto updatedAddress)
        {
            var user = await _userManager.FindUserEmailWithAddressAsync(User);
            var address = _mapper.Map<AddressDto, Address>(updatedAddress);
            address.Id = user.Address.Id;
            user.Address = address;
            var result = await _userManager.UpdateAsync(user);
            if(!result.Succeeded) 
                return BadRequest(new APIsResponse(400));
            return Ok(updatedAddress);
        }
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

    }
}
