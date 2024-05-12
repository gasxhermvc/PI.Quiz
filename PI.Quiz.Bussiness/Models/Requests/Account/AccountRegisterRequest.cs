using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PI.Quiz.Bussiness.Models.Requests.Account
{
    public class AccountRegisterRequest
    {
        [BindProperty(Name = "email")]
        [JsonProperty("email")]
        [Required(ErrorMessage = "The email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "The email invalid format")]
        public string Email { get; set; }

        [BindProperty(Name = "password")]
        [JsonProperty("password")]
        [Required(ErrorMessage = "The password is required")]
        [DataType(DataType.Password)]  
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$", ErrorMessage = "The password must be between 8 and 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string Password { get; set; }

        [BindProperty(Name = "firstName")]
        [JsonProperty("firstName")]
        [Required(ErrorMessage = "The firstName is required")]
        public string FirstName { get; set; }

        [BindProperty(Name = "lastName")]
        [JsonProperty("lastName")]
        [Required(ErrorMessage = "The lastName is required")]
        public string LastName { get; set; }

        [BindProperty(Name = "nickName")]
        [JsonProperty("nickName")]
        public string NickName { get; set; }

        [BindProperty(Name = "birthDate")]
        [JsonProperty("birthDate")]
        [Required(ErrorMessage = "The birthDate is required")]
        [DataType(DataType.Date)]
        [RegularExpression(@"\d{4}-\d{2}-\d{2}", ErrorMessage = "The birthDate invalid format date (yyyy-MM-dd)")]
        public string BirthDate { get; set; }

        [BindProperty(Name = "phoneNumber")]
        [JsonProperty("phoneNumber")]
        [Required(ErrorMessage = "The phoneNumber is required")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
