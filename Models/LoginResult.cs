namespace Warehouse1.Models
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = null!;
        public User? User { get; set; }
    }
}
