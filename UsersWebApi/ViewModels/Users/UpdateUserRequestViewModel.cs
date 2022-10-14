namespace UsersWebApi.Models.Users;

using System;
using System.ComponentModel.DataAnnotations;
using UsersWebApi.Entities;

public class UpdateRequestViewModel
{
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [EnumDataType(typeof(Role))]
    public Role Role { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [MinLength(6)]
    public string? Password { get; set; }

    [Compare("Password")]
    public string? ConfirmPassword { get; set; }
}