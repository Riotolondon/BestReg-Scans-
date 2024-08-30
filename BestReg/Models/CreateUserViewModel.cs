using System.ComponentModel.DataAnnotations;

namespace BestReg.Models
{
    // This ViewModel is used to capture data from the user creation form.
    public class CreateUserViewModel
    {
        // The first name of the user to be created.
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        // The last name of the user to be created.
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // The email address of the user to be created. This will also be the username.
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // The password for the new user.
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        // The role to be assigned to the new user.
        [Required]
        public string Role { get; set; }
    }
}
