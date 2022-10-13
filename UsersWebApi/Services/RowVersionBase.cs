using System.ComponentModel.DataAnnotations;

namespace UsersWebApi.Services
{
    public class RowVersionBase
    {
        public byte[] RowVersion { get; set; } = null!;
    }
}
