namespace UsersWebApi.Models.Users;

using System;
using UsersWebApi.Entities;

public class UserResponseViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Title, FirstName, LastName, Email, Role);
    }
}

