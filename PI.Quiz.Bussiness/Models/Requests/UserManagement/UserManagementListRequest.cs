using Microsoft.AspNetCore.Mvc;

namespace PI.Quiz.Bussiness.Models.Requests.UserManagement
{
    public class UserManagementListRequest
    {
        [BindProperty(Name = "keyword")]
        public string? Keyword { get; set; }
    }
}
