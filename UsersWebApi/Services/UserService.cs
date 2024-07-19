namespace UsersWebApi.Services;

using AutoMapper;
using BCrypt.Net;
using UsersWebApi.Entities;
using UsersWebApi.Helpers;
using UsersWebApi.Models.Users;


public interface IUserService
{
    IEnumerable<UserResponseViewModel> GetAll();
    Task<UserResponseViewModel> GetById(int id);
    void Create(CreateUserRequestViewModel model);
    Task<int> Update(int id, UpdateRequestViewModel model);
    void Delete(int id);
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

    public IEnumerable<UserResponseViewModel> GetAll()
    {
        return _mapper.ProjectTo<UserResponseViewModel>(_context.Users);
    }

    public async Task<UserResponseViewModel> GetById(int id)
    {
        var user = await getUser(id);

        var userVm = _mapper.Map<UserResponseViewModel>(user);

        _changeContext.Hash = userVm.GetHashCode();
        return userVm;
    }

    public void Create(CreateUserRequestViewModel model)
    {
        // validate
        if (_context.Users.Any(x => x.Email == model.Email))
            throw new AppException("User with the email '" + model.Email + "' already exists");

        // map model to new user object
        var user = _mapper.Map<User>(model);

        // hash password
        user.PasswordHash = BCrypt.HashPassword(model.Password);

        // save user
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public async Task<int> Update(int id, UpdateRequestViewModel model)
    {
        var user = await getUser(id);

        var userVm = _mapper.Map<UserResponseViewModel>(user);

        userVm.ResolveConcurrency(_changeContext.Hash);

        // validate
        if (model.Email != user.Email && _context.Users.Any(x => x.Email == model.Email))
            throw new AppException("User with the email '" + model.Email + "' already exists");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.HashPassword(model.Password);

        // copy model to user and save
        _mapper.Map(model, user);

        return await _context.SaveChangesAsync();
    }

    public async void Delete(int id)
    {
        var user = await getUser(id);
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    // helper methods

    private async Task<User> getUser(int id)
    {
        var user = await _context.Users.FindAsync(id) ?? throw new KeyNotFoundException("User not found");
        return user;
    }
}