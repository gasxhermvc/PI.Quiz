using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PI.Quiz.Bussiness.Models.Requests.Account
{
    public class AccountForgotPasswordRequest
    {
        [BindProperty(Name = "email")]
        [JsonProperty("email")]
        [Required(ErrorMessage = "The email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "The email invalid format")]
        public string Email { get; set; }
    }
}
