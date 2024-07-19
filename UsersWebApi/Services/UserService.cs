namespace UsersWebApi.Services;

using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using UsersWebApi.Entities;
using UsersWebApi.Helpers;
using UsersWebApi.Models.Users;


public interface IUserService
{
    Task<List<UserResponseViewModel>> GetAll(CancellationToken cancellationToken);
    Task<UserResponseViewModel> GetById(int id, CancellationToken cancellationToken);
    Task<int> Create(CreateUserRequestViewModel model, CancellationToken cancellationToken);
    Task<int> Update(int id, UpdateRequestViewModel model, CancellationToken cancellationToken);
    Task<int> Delete(int id, CancellationToken cancellationToken);
}

public class UserService : IUserService
{
    private readonly ChangeContext _changeContext;
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserService(
        ChangeContext changeContext,
        DataContext context,
        IMapper mapper)
    {
        _changeContext = changeContext;
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<UserResponseViewModel>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _context.Users.ToListAsync(cancellationToken);
        return _mapper.Map<List<UserResponseViewModel>>(users);
    }

    public async Task<UserResponseViewModel> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await GetUser(id, cancellationToken);

        var userVm = _mapper.Map<UserResponseViewModel>(user);

        _changeContext.RowVersion = user.RowVersion;

        return userVm;
    }

    public async Task<int> Create(CreateUserRequestViewModel model, CancellationToken cancellationToken)
    {
        // validate
        if (await _context.Users.AnyAsync(x => x.Email == model.Email, cancellationToken))
            throw new AppException("User with the email '" + model.Email + "' already exists");

        // map model to new user object
        var user = _mapper.Map<User>(model);

        // hash password
        user.PasswordHash = BCrypt.HashPassword(model.Password);

        // save user
        _context.Users.Add(user);

        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> Update(int id, UpdateRequestViewModel model, CancellationToken cancellationToken)
    {
        var user = await GetUser(id, cancellationToken);

        _context.Entry(user).OriginalValues["RowVersion"] = _changeContext.RowVersion;

        // validate
        if (model.Email != user.Email && await _context.Users.AnyAsync(x => x.Email == model.Email, cancellationToken))
            throw new AppException("User with the email '" + model.Email + "' already exists");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.HashPassword(model.Password);

        // copy model to user and save
        _mapper.Map(model, user);

        var result = await _context.SaveChangesAsync(cancellationToken);

        // Get the updated RowVersion
        _changeContext.RowVersion = _context.Entry(user).Property(e => e.RowVersion).CurrentValue;

        return result;
    }

    public async Task<int> Delete(int id, CancellationToken cancellationToken)
    {
        var user = await GetUser(id, cancellationToken);
        _context.Users.Remove(user);
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // helper methods

    private async Task<User> GetUser(int id, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken) ?? throw new KeyNotFoundException("User not found");
        return user;
    }
}