using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Quiz.Bussiness.Models.Requests.Authen
{
    public class AuthenTokenRequest
    {
        [BindProperty(Name = "grantType")]
        [JsonProperty("grantType")]
        [RegularExpression("^(password|refresh_token)", ErrorMessage = "The grantType is required for password or refresh_token only")]
        [Required(ErrorMessage = "The grantType is required")]
        public string GrantType { get; set; }

        [BindProperty(Name = "username")]
        [JsonProperty("username")]
        public string? Username { get; set; }

        [BindProperty(Name = "password")]
        [JsonProperty("password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [BindProperty(Name = "clientId")]
        [JsonProperty("clientId")]
        public string? ClientId { get; set; }

        [BindProperty(Name = "refreshToken")]
        [JsonProperty("refreshToken")]
        public string? RefreshToken { get; set; }
    }
}
