using System.ComponentModel.DataAnnotations;

namespace UsersWebApi.Services
{
    public class RowVersionBase
    {
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
