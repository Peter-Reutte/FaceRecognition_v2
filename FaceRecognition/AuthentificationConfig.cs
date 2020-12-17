using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FaceRecognition
{
    public class AuthentificationConfig
    {
        const int DefaultMinPasswordLegth = 7;
        const int DefualtSaltLength = 10;
        const int DefualtTotpDigitsCount = 6;
        const int DefualtTotpPeriod = 30;
        const int DefualtFaceDetectionThreshold = 100;
        const int DefualtMaxConcurrentFacesCount = 2;
        const int DefualtFaceMaxScanningTime = 15;

        public int MinPasswordLegth { get; set; } = DefaultMinPasswordLegth;
        public int SaltLength { get; set; } = DefualtSaltLength;
        public int TotpDigitsCount { get; set; } = DefualtTotpDigitsCount;
        public int TotpPeriod { get; set; } = DefualtTotpPeriod;
        public int FaceDetectionThreshold { get; set; } = DefualtFaceDetectionThreshold;
        public int MaxConcurrentFacesCount { get; set; } = DefualtMaxConcurrentFacesCount;
        public int FaceMaxScanningTime { get; set; } = DefualtFaceMaxScanningTime;

        private static AuthentificationConfig instance = null;

        public static AuthentificationConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = new AuthentificationConfig();
                return instance;
            }
        }
        public AuthentificationConfig()
        {

        }

        public void Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                string xmlString = File.ReadAllText(fileName);
                DeserializeXmlToAccountList(xmlString);
            }
        }

        public void Save(string fileName)
        {
            File.WriteAllText(fileName, SerializeAccountListToXml());
        }

        private string SerializeAccountListToXml()
        {
            XmlSerializer xmlizer = new XmlSerializer(typeof(AuthentificationConfig));
            StringWriter writer = new StringWriter();
            xmlizer.Serialize(writer, this);

            return writer.ToString();
        }

        private AuthentificationConfig DeserializeXmlToAccountList(string listAsXml)
        {
            XmlSerializer xmlizer = new XmlSerializer(typeof(AuthentificationConfig));
            TextReader textreader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(listAsXml)));

            return (xmlizer.Deserialize(textreader)) as AuthentificationConfig;
        }
    }
}
