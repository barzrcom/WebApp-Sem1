using System.ComponentModel.DataAnnotations;

namespace MapApp.Models.AccountViewModels
{
	public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
