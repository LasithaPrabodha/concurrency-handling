namespace UsersWebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using UsersWebApi.Helpers;
using UsersWebApi.Models.Users;
using UsersWebApi.Services;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [UseOptimisticConcurrency]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetById(id);
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Create(CreateUserRequestViewModel model)
    {
        try
        {
            _userService.Create(model);
            return Ok(new { message = "User created" });
        }
        catch (AppException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{id}")]
    [UseOptimisticConcurrency]
    public async Task<IActionResult> Update(int id, UpdateRequestViewModel model)
    {
        try
        {
            await _userService.Update(id, model);
            return Ok(new { message = "User updated" });
        }
        catch (AppException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }

    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _userService.Delete(id);
        return Ok(new { message = "User deleted" });
    }
}