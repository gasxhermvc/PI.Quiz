using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PI.Quiz.Bussiness.Models.Requests.UserManagement
{
    public class UserManagementUpdateRequest
    {

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

        [BindProperty(Name = "activated")]
        [Required(ErrorMessage = "The activated is required")]
        public bool Activated { get; set; }

        [BindProperty(Name = "Role")]
        [Required(ErrorMessage = "The role is required")]
        [RegularExpression(@"^(Admin|User)", ErrorMessage = "The role is required for Admin or User only")]
        public string Role { get; set; }
    }
}
