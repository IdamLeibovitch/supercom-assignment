using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Models
{
    [Table("Users")]
    [Index(nameof(User.UserName), IsUnique = true)]
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(20)]
        public required string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [StringLength(100)]
        public required string FullName { get; set; }

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }

        public string? Privileges { get; set; }

        // Navigation property
        [InverseProperty(nameof(Task.User))]
        public required ICollection<Task> Tasks { get; set; }
    }
}
