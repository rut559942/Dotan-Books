using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

    namespace DotanBooks.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class UsersController : ControllerBase
        {
            private readonly IUserService _userService;

            public UsersController(IUserService userService)
            {
                _userService = userService;
            }

            [HttpPost("Register")]
            public async Task<ActionResult<int>> Register([FromBody] NewUserDto newUserDto)
            {
                
                    var userId = await _userService.Register(newUserDto);
                    return CreatedAtAction(nameof(Register), new { id = userId }, userId);
                
            }

            [HttpPost("login")]
            [AllowAnonymous]
            [EnableRateLimiting("LoginPolicy")]
            public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginDto loginDto)
            {
                    var result = await _userService.Login(loginDto);

            var cookieOptions = new CookieOptions
            {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(60),
            Path = "/"
            };

            Response.Cookies.Append("auth", result.Token, cookieOptions);

            return Ok(result);
            }

            [HttpPut("{id}")]
             public async Task<ActionResult> Update(int id, [FromBody] UpdateUserDto updateUserDto)
             {

                    await _userService.Update(id, updateUserDto);
                    return NoContent(); // ב-PUT נהוג להחזיר 204 NoContent אם העדכון הצליח
             }

    }
    }

