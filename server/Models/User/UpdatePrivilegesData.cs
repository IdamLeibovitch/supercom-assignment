using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class UpdatePrivilegesRequest
{
    [Required]
    public required IEnumerable<UserPrivilege> Privileges { get; set; }
}
