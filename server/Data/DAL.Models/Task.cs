using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data.Models
{
    [Table("Tasks")]
    public class Task
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public required string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public required string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        [EnumDataType(typeof(TaskPriority))]
        public TaskPriority Priority { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Tasks))]
        public required User User { get; set; }
    }

    // [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskPriority
    {
      Low,
      Medium,
      High,
      Critical
    }
}
