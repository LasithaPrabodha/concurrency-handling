namespace UsersWebApi.Services;

using AutoMapper;
using BCrypt.Net;
using UsersWebApi.Entities;
using UsersWebApi.Helpers;
using UsersWebApi.Models.Users;


public interface IUserService
{
    IEnumerable<UserViewModel> GetAll();
    Task<UserViewModel> GetById(int id);
    void Create(CreateUserRequest model);
    Task<int> Update(int id, UpdateRequest model);
    void Delete(int id);
}

public class UserService : IUserService
{
    private readonly ChangeContext _changeContext;
    private DataContext _context;
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

    public IEnumerable<UserViewModel> GetAll()
    {
        return _mapper.ProjectTo<UserViewModel>(_context.Users);
    }

    public async Task<UserViewModel> GetById(int id)
    {
        var user = await getUser(id);
        _changeContext.Timestamp = user.RowVersion;

        var userVm = _mapper.Map<UserViewModel>(user);

        return userVm;
    }

    public void Create(CreateUserRequest model)
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

    public async Task<int> Update(int id, UpdateRequest model)
    {
        var user = await getUser(id);

        if (Convert.ToInt64(ByteArrayToHexString(user.RowVersion), 16) > Convert.ToInt64(ByteArrayToHexString(_changeContext.Timestamp), 16))
        {
            _changeContext.Timestamp = user.RowVersion;
            throw new PreconditionFailedException();
        }

        // validate
        if (model.Email != user.Email && _context.Users.Any(x => x.Email == model.Email))
            throw new AppException("User with the email '" + model.Email + "' already exists");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.HashPassword(model.Password);

        // copy model to user and save
        _mapper.Map(model, user);

        var noOfEntries = await _context.SaveChangesAsync();

        _changeContext.Timestamp = user.RowVersion;

        return noOfEntries;
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
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }

    private string ByteArrayToHexString(byte[] b)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("0x");

        foreach (byte val in b)
        {
            sb.Append(val.ToString("X2"));
        }

        return sb.ToString();
    }
}