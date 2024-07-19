namespace UsersWebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using UsersWebApi.Helpers;
using UsersWebApi.Models.Users;
using UsersWebApi.Services;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAll(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id}")]
    [UseOptimisticConcurrency]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetById(id, cancellationToken);
        return Ok(user);
    }

    [HttpPost]
    [UseOptimisticConcurrency]
    public async Task<IActionResult> Create(CreateUserRequestViewModel model, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.Create(model, cancellationToken);
            return Ok(new { message = "User created" });
        }
        catch (AppException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{id}")]
    [UseOptimisticConcurrency]
    public async Task<IActionResult> Update(int id, UpdateRequestViewModel model, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.Update(id, model, cancellationToken);
            return Ok(new { message = "User updated" });
        }
        catch (AppException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _userService.Delete(id, cancellationToken);
        return Ok(new { message = "User deleted" });
    }
}