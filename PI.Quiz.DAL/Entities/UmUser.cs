namespace PI.Quiz.DAL.Entities
{
    public class UmUser
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? NickName { get; set; }

        public string? BirthDate { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool Activated { get; set; }

        public bool Deleted { get; set; }

        public string Role { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        //public Collection<UmResetPassword> ResetPasswords { get; set; }

        //public Collection<UmToken> Tokens { get; set; }

    }
}
