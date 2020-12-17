using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    public class UserAccount
    {
        // Поля
        private const int DefaultSaltLength = 10;
        private const int DefaultPbkdf2IterationsCount = 1000;
        private const int DefaultPbkdf2HashSize = 32;

        public static int SaltLength { get; set; } = DefaultSaltLength;
        public static int Pbkdf2IterationsCount { get; set; } = DefaultPbkdf2IterationsCount;
        public static int Pbkdf2HashSize { get; set; } = DefaultPbkdf2HashSize;



        public int UserId { get; set; }

        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public TotpFactor TotopFactor { get; set; }
        public FaceFactor FaceFactor { get; set; }
        public DateTime LastLogin { get; set; }

        public bool IsAdmin { get; set; }

        // Конструкторы
        public UserAccount()
        {

        }

        public UserAccount(string login, string password)
        {
            Login = login;
            PasswordSalt = GetRandomSalt(SaltLength);
            SetPassword(password);
            TotopFactor = null;
            FaceFactor = null;
        }


        // Методы
        private string GetRandomSalt(int bytesCount)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[bytesCount];
            rng.GetBytes(salt);
            return BitConverter.ToString(salt).ToLower().Replace("-", "");
        }

        private uint GetHexDigitValue(char digit)
        {
            if (digit >= '0' && digit <= '9')
                return (uint)(digit - '0');
            return (uint)(digit - 'a' + 10);
        }

        private byte[] SaltStrToBytes(string salt)
        {
            salt = salt.ToLower();
            byte[] saltBytes = new byte[salt.Length / 2];
            for (int i = 0; i < salt.Length; i += 2)
                saltBytes[i >> 1] = (byte)((GetHexDigitValue(salt[i]) << 4) | GetHexDigitValue(salt[i + 1]));
            return saltBytes;
        }

        private string GetSaltedHash(string password, string salt)
        {
            byte[] saltBytes = SaltStrToBytes(salt);
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Pbkdf2IterationsCount);
            byte[] hash = pbkdf2.GetBytes(DefaultPbkdf2HashSize);
            return BitConverter.ToString(hash).ToLower().Replace("-", "");
        }

        public bool CheckPassword(string password)
        {
            return PasswordHash == GetSaltedHash(password, PasswordSalt);
        }

        public void SetPassword(string password)
        {
            PasswordHash = GetSaltedHash(password, PasswordSalt);
        }

        public void DoLogin()
        {
            LastLogin = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Login, PasswordHash, PasswordSalt);
        }
    }
}