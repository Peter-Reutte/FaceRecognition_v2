using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FaceRecognition
{
    public class UserBase
    {
        private const string DefaultBaseName = "AuthDemo";
        private const string DefaultFileName = "accounts.db";

        public string BaseName { get; private set; }
        public string FileName { get; private set; }

        public ObservableCollection<UserAccount> UserAccounts { get; private set; }

        public FaceAuthentificator FaceAuthentificator { get; private set; }
        public TotpAuthentificator TotpAuthentificator { get; private set; }

        public UserBase(string baseName = DefaultBaseName, string fileName = DefaultFileName)
        {
            BaseName = baseName;
            FileName = fileName;
            FaceAuthentificator = new FaceAuthentificator(this);
            TotpAuthentificator = new TotpAuthentificator(this);
            UserAccounts = new ObservableCollection<UserAccount>();
            LoadBase(FileName);
        }

        private void LoadBase(string fileName)
        {
            if (File.Exists(fileName))
            {
                string xmlString = File.ReadAllText(fileName);
                DeserializeXmlToAccountList(xmlString);
            }
        }

        private void SaveBase(string fileName)
        {
            File.WriteAllText(fileName, SerializeAccountListToXml());
        }

        private string SerializeAccountListToXml()
        {
            XmlSerializer xmlizer = new XmlSerializer(typeof(ObservableCollection<UserAccount>));
            StringWriter writer = new StringWriter();
            xmlizer.Serialize(writer, UserAccounts);

            return writer.ToString();
        }

        private ObservableCollection<UserAccount> DeserializeXmlToAccountList(string listAsXml)
        {
            XmlSerializer xmlizer = new XmlSerializer(typeof(ObservableCollection<UserAccount>));
            TextReader textreader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(listAsXml)));

            return UserAccounts = (xmlizer.Deserialize(textreader)) as ObservableCollection<UserAccount>;
        }

        public void AddUser(string login, string password, string totpSecret = "", List<int> faceIds = null, bool isAdmin = false)
        {
            var possibleUsers = UserAccounts.Where(u => u.Login == login);
            if (possibleUsers.Count() == 0)
            {
                UserAccount user = new UserAccount(login, password);
                if (totpSecret != "")
                    TotpAuthentificator.AddFactorToUser(user, totpSecret);
                if (faceIds != null)
                    FaceAuthentificator.AddFactorToUser(user, faceIds);
                user.IsAdmin = isAdmin;
                UserAccounts.Add(user);
                SaveBase(FileName);
            }
            else
            {
                throw new Exception(string.Format("User {0} already exists!", login));
            }
        }

        public void UpdateUser(string login, string password = null, string totpSecret = "", List<int> faceIds = null, bool isAdmin = false)
        {
            var possibleUsers = UserAccounts.Where(u => u.Login == login);
            if (possibleUsers.Count() == 0)
                throw new Exception(string.Format("User {0} doesn't exist!", login));
            if (possibleUsers.Count() > 1)
                throw new Exception(string.Format("There are more then one user with login {0}!", login));
            var user = possibleUsers.ElementAt(0);
            if (password != null)
                user.SetPassword(password);
            if (totpSecret == "")
                TotpAuthentificator.RemoveFactorForUser(user);
            else
                TotpAuthentificator.UpdateFactorForUser(user, totpSecret);
            if (faceIds == null)
                FaceAuthentificator.RemoveFactorForUser(user);
            else
                FaceAuthentificator.UpdateFactorForUser(user, faceIds);
            user.IsAdmin = isAdmin;
            SaveBase(FileName);
        }

        public void DeleteUser(string login)
        {
            var possibleUsers = UserAccounts.Where(u => u.Login == login);
            if (possibleUsers.Count() == 0)
                throw new Exception(string.Format("User {0} doesn't exist!", login));
            if (possibleUsers.Count() > 0)
                throw new Exception(string.Format("There are more then one user with login {0}!", login));
            DeleteUser(possibleUsers.ElementAt(0));
        }

        public void DeleteUser(UserAccount user)
        {
            UserAccounts.Remove(user);
            SaveBase(FileName);
        }

        public void ClearBase()
        {
            UserAccounts.Clear();
            SaveBase(FileName);
        }

        public UserAccount GetUserByLogin(string login)
        {
            var possibleUsers = UserAccounts.Where(u => u.Login == login);
            if (possibleUsers.Count() == 0)
                return null;
            if (possibleUsers.Count() > 1)
                throw new Exception(string.Format("There are many users!"));
            return possibleUsers.ElementAt(0);
        }

        public UserAccount GetUserByFaceId(int faceId)
        {
            for (int i = 0; i < UserAccounts.Count; i++)
                if (UserAccounts[i].FaceFactor != null && UserAccounts[i].FaceFactor.FaceIds.Contains(faceId))
                    return UserAccounts[i];
            return null;
        }

        public bool CheckUserFaceIdUnique()
        {
            List<int> faces = UserAccounts.Where(u => u.FaceFactor != null).SelectMany(u => u.FaceFactor.FaceIds).ToList();
            return faces.Count() == faces.Distinct().Count();
        }

        public bool VerifyUser(string login, string password, string totpCode = "", List<int> faceIds = null)
        {
            var possibleUsers = UserAccounts.Where(u => u.Login == login && u.CheckPassword(password));
            if (possibleUsers.Count() == 0)
                return false;
            if (possibleUsers.Count() > 1)
                throw new Exception(string.Format("There are many users!"));
            var user = possibleUsers.ElementAt(0);
            if (totpCode != "" && !TotpAuthentificator.Verify(user, totpCode))
                return false;
            if (faceIds != null && !FaceAuthentificator.Verify(user, faceIds))
                return false;
            return true;
        }

        public bool IsUserExists(string login)
        {
            return UserAccounts.Count(u => u.Login == login) > 0;
        }

        public void DoUserLogin(UserAccount user)
        {
            user.DoLogin();
            SaveBase(FileName);
        }
    }
}
