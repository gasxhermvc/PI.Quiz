using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Quiz.Engine.Security.Bcrypt
{
    public interface IBcryptService
    {
        string Hash(string input);

        bool Check(string input, string hashing);
    }
}
