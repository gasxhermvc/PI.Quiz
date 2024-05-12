using System.ComponentModel.DataAnnotations;

namespace PI.Quiz.Bussiness.Models.Requests.Account
{
    public class AccountResetPasswordRequest
    {
        [Required(ErrorMessage = "The newPassword is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$", ErrorMessage = "The password must be between 8 and 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string newPassword { get; set; }
    }
}
