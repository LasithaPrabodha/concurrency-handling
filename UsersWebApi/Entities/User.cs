namespace UsersWebApi.Entities;
 
using System.Text.Json.Serialization;
using UsersWebApi.Services;

public class User : RowVersionBase
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; }
}