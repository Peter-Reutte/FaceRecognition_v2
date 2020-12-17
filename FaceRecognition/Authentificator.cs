using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    public abstract class Authentificator<T>
    {
        protected UserBase userBase;

        public Authentificator(UserBase userBase)
        {
            this.userBase = userBase;
        }

        public abstract void AddFactorToUser(UserAccount user, T parameter = default(T));

        public abstract void UpdateFactorForUser(UserAccount user, T parameter = default(T));

        public abstract void RemoveFactorForUser(UserAccount user);

        public abstract bool Verify(UserAccount user, T parameter = default(T));
    }
}
