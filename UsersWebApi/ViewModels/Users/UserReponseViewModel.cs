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
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;

        UserResponseViewModel other = (UserResponseViewModel)obj;
        return Id == other.Id
               && Title == other.Title
               && FirstName == other.FirstName
               && LastName == other.LastName
               && Email == other.Email
               && Role == other.Role;
    }
}

