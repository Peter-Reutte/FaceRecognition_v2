using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    public class TotpFactor : AuthenticationFactor
    {
        public string TotpSecret { get; set; }

        public TotpFactor() : this("")
        {

        }

        public TotpFactor(string totppSecret)
        {
            TotpSecret = totppSecret;
        }

        public override string ToString()
        {
            return TotpSecret;
        }
    }
}
