namespace UsersWebApi.Services;

using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using UsersWebApi.Entities;
using UsersWebApi.Helpers;
using UsersWebApi.ViewModels.Users;


public interface IUserService
{
    Task<List<UserResponseViewModel>> GetAll(CancellationToken cancellationToken);
    Task<UserResponseViewModel> GetById(int id, CancellationToken cancellationToken);
    Task<List<UserHistoryResponseViewModel>> GetByIdWithHistory(int id, GetUserHistoryRequestViewModel viewModel, CancellationToken cancellationToken);
    Task<int> Create(CreateUserRequestViewModel model, CancellationToken cancellationToken);
    Task<int> Update(int id, UpdateRequestViewModel model, CancellationToken cancellationToken);
    Task<int> Delete(int id, CancellationToken cancellationToken);
    Task<int> RestoreToVersion(int id, DateTime restorePoint, CancellationToken cancellationToken);
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
    public async Task<List<UserHistoryResponseViewModel>> GetByIdWithHistory(int id, GetUserHistoryRequestViewModel viewModel, CancellationToken cancellationToken)
    {
        return await GetUser(id, viewModel, cancellationToken);
    }

    public async Task<int> RestoreToVersion(int id, DateTime restorePoint, CancellationToken cancellationToken)
    {
        var historicalVersion = await GetHistoricalVersion(id, restorePoint, cancellationToken);

        if (historicalVersion != null)
        {

            var currentVersion = await GetUser(id, cancellationToken);

            // Copy all properties except Id and RowVersion
            foreach (var property in typeof(User).GetProperties())
            {
                if (property.Name != nameof(currentVersion.Id) &&
                    property.Name != nameof(currentVersion.RowVersion))
                {
                    property.SetValue(currentVersion, property.GetValue(historicalVersion));
                }
            }

            return await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new KeyNotFoundException("Historical version not found.");
        }
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
        var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        return user;
    }

    private async Task<List<UserHistoryResponseViewModel>> GetUser(int id, GetUserHistoryRequestViewModel viewModel, CancellationToken cancellationToken)
    {
        var user = await _context.Users.TemporalBetween(viewModel.From, viewModel.To ?? DateTime.UtcNow)
                .Where(u => u.Id == id)
                .OrderByDescending(e => EF.Property<DateTime>(e, "ValidFrom"))
                .Select(e => new UserHistoryResponseViewModel
                {
                    Id = e.Id,
                    Email = e.Email,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Role = e.Role,
                    Title = e.Title,
                    ValidFrom = EF.Property<DateTime>(e, "ValidFrom"),
                    ValidTo = EF.Property<DateTime>(e, "ValidTo"),
                })
                .ToListAsync(cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        return user;
    }
    private async Task<User> GetHistoricalVersion(int id, DateTime restorePoint, CancellationToken cancellationToken)
    {
        var user = await _context.Users.TemporalAsOf(restorePoint.AddMilliseconds(1)).SingleAsync(u => u.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("User not found");

        return user;
    }
}