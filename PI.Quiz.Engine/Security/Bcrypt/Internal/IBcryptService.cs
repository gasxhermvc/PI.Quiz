namespace PI.Quiz.Engine.Security.Bcrypt
{
    public interface IBcryptService
    {
        string Hash(string input);

        bool Check(string input, string hashing);
    }
}
