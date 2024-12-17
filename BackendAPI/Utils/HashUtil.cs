namespace BackendAPI.Utils {
    public class HashUtil {
        public static string Hash(string input) {
            return BCrypt.Net.BCrypt.HashPassword(input);
        }

        public static bool Verify(string s, string hashedString) {
            return BCrypt.Net.BCrypt.Verify(s, hashedString);
        }
    }
}
