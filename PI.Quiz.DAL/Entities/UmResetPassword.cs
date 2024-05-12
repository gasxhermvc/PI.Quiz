using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Quiz.DAL.Entities
{
    public class UmResetPassword
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public bool Revoked { get; set; }

        public long ExpiredIn { get; set; }

        public DateTime ExpiredDate { get; set; }

        public bool Deleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public UmUser User { get; set; }
    }
}
