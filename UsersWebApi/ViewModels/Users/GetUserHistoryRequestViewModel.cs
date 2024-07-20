namespace UsersWebApi.ViewModels.Users;

using System;
using System.ComponentModel.DataAnnotations;

public class GetUserHistoryRequestViewModel
{
    [Required]
    public DateTime From { get; set; }
    public DateTime? To { get; set; }

}