using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    public class FaceFactor : AuthenticationFactor
    {
        public List<int> FaceIds { get; set; }
        public FaceFactor()
        {
            FaceIds = new List<int>();
        }

        public override string ToString()
        {
            return string.Join(" ", FaceIds);
        }
    }
}
