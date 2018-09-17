using System.ComponentModel.DataAnnotations;

namespace MapApp.Models.AccountViewModels
{
	public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
