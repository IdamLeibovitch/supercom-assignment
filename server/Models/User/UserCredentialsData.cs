using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class UserCredentialsData
    {
        [Required]
        [StringLength(20)]
        public required string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}