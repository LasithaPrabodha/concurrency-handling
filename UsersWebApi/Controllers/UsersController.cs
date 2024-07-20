namespace UsersWebApi.Controllers;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using UsersWebApi.Helpers;
using UsersWebApi.ViewModels.Users;
using UsersWebApi.Services;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetByIdWithHistory(int id, [FromQuery] GetUserHistoryRequestViewModel viewModel, CancellationToken cancellationToken)
    {
        var userWithHistory = await _userService.GetByIdWithHistory(id, viewModel, cancellationToken);
        return Ok(userWithHistory);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> RestoreToVersion(int id, [Required] DateTime restorePoint, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.RestoreToVersion(id, restorePoint, cancellationToken);
            return Ok();
        }
        catch (DbUpdateConcurrencyException)
        {
            return new StatusCodeResult(StatusCodes.Status412PreconditionFailed);
        }
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