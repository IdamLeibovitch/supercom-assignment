using System.Text.Json.Serialization;

namespace Backend.Models;

public class UserDetails
{
  public Guid Id { get; set; }
  public string UserName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public IEnumerable<UserPrivilege>? Privileges { get; set; }
}
