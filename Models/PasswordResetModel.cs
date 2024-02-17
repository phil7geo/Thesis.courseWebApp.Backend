namespace Thesis.courseWebApp.Backend.Models
{
    public class PasswordResetModel
    {
        // Properties for password reset data
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
