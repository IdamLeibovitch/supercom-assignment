using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data.Models
{
  [Table("AuditLogs")]
  public class AuditLog
  {
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string EntityName { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string Action { get; set; }
    
    [Required]
    public required string Changes { get; set; }
    
    [Required]
    public DateTime Timestamp { get; set; }
  }
}
