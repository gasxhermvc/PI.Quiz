namespace PI.Quiz.Engine.Extensions
{
    public static class Base64Encoder
    {
        //=>for encode
        public static string Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        //=>for decode
        public static string Decode(this string plainText)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(plainText);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
