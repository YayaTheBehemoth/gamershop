using gamershop.Server.Services;
using gamershop.Shared.DTOs;
using gamershop.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace gamershop.Server.Controllers;

[ApiController]
[Route("[Controller]")]
public class UserController : ControllerBase
{
    private UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Route("createuser")]
    public async Task CreateUser([FromBody] UserDTO requestbody)
    {
        HashedPassword hashedPassword = await _userService.EncryptPassword(requestbody.Password);

        await _userService.StoreUser(requestbody.Username, hashedPassword);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] UserDTO requestbody)
    {
        User storedUser = await _userService.GetUser(requestbody.Username);

        bool passwordCheck = await _userService.CheckPassword(storedUser.StoredPassword, requestbody.Password);
        if (passwordCheck == true)
        {
            var token = _userService.GenerateJWT(requestbody.Username);
            return Ok(new { token });
        }
        
        return Unauthorized();
    }
}