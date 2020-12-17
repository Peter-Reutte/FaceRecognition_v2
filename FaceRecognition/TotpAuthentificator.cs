using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TwoFactorAuthNet;

namespace FaceRecognition
{
    public class TotpAuthentificator : Authentificator<string>
    {
        public static int DefaultQrImageSize { get; set; } = 200;
        public static int DefaultDigitsCount { get; set; } = 6;
        public static int DefualtPeriod { get; set; } = 30;

        public int QrImageSize { get; set; } = DefaultQrImageSize;
        public int DigitsCount { get; set; } = DefaultDigitsCount;
        public int Period { get; set; } = DefualtPeriod;

        private TwoFactorAuth tfa;

        public TotpAuthentificator(UserBase userBase) : base(userBase)
        {
            tfa = new TwoFactorAuth(userBase.BaseName, DigitsCount, Period);
        }

        public override void AddFactorToUser(UserAccount user, string totpSecret = null)
        {
            user.TotopFactor = new TotpFactor(totpSecret);
        }


        public string GetRandomSecret()
        {
            return tfa.CreateSecret();
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        public BitmapImage GetQrCodeForSecret(string secret)
        {
            byte[] imageBytes = null;
            try
            {
                imageBytes = tfa.QrCodeProvider.GetQrCodeImage(tfa.GetQrText(userBase.BaseName, secret), QrImageSize);
                return LoadImage(imageBytes);
            }
            catch
            {
                return null;
            }

        }

        public override void UpdateFactorForUser(UserAccount user, string totpSecret)
        {
            if (user.TotopFactor == null)
                AddFactorToUser(user, totpSecret);
            else
                user.TotopFactor.TotpSecret = totpSecret;
        }

        public override void RemoveFactorForUser(UserAccount user)
        {
            user.TotopFactor = null;
        }

        public override bool Verify(UserAccount user, string totpSecret = null)
        {
            return user.TotopFactor == null || tfa.VerifyCode(user.TotopFactor.TotpSecret, totpSecret);
        }
        
    }
}
