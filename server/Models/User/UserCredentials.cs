namespace Backend.Models
{
    public class UserCredentials
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}