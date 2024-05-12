using System.Data.Entity;

namespace PI.Quiz.DAL.Entities
{
    public class UmToken
    {
        public int Id { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public bool Revoked { get; set; }

        public long ExpiredIn { get; set; }

        public DateTime ExpiredDate { get; set; }

        public bool Deleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public UmUser User { get; set; }
    }
}
