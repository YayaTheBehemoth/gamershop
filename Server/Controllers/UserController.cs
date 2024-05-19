using gamershop.Server.Database;
using gamershop.Server.Services;
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
    [Route("CreateUser")]
    public async Task CreateUser([FromBody]Requestbody requestbody)
    {
        HashedPassword hashedPassword = await _userService.EncryptPassword(requestbody.Password);

        await _userService.CreateUser(requestbody.Username, hashedPassword);
    }

    public class Requestbody
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}