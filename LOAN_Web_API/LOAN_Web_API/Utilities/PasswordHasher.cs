namespace LOAN_Web_API.Utilities
{
    public class PasswordHasher
    {
        // პაროლის დაჰეშვა
        public static string HashPassword(string password)
        {
            // იყენებს BCrypt.Net-ის სტანდარტულ მარილს (salt) და ჰეშავს
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // პაროლის შემოწმება ჰეშთან
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // თუ ჰეში არასწორ ფორმატშია (ძალიან იშვიათი), ჩათვალე ვერ მოხერხდა
                return false;
            }
        }
    }
}
