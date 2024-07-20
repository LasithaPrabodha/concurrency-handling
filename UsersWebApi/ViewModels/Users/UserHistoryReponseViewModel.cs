namespace UsersWebApi.ViewModels.Users;

using System;
using UsersWebApi.Entities;

public class UserHistoryResponseViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}

