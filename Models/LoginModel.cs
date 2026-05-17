namespace TrustPlus.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginModel
    {

        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
    }
}
