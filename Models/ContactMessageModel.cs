namespace Thesis.courseWebApp.Backend.Models
{
    public class ContactMessageModel
    {
        //[Required(ErrorMessage = "FullName is required")]
        //[FullName(ErrorMessage = "Invalid full name")]
        public string FullName { get; set; }

        public string Email { get; set; }

        public int PhoneNumber { get; set; }

        public string Message { get; set; }
    }
}
