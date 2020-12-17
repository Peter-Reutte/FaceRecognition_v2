using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    public class FaceAuthentificator : Authentificator<List<int>>
    {
        public FaceAuthentificator(UserBase userBase) : base(userBase)
        {

        }

        public override void AddFactorToUser(UserAccount user, List<int> faceIds = null)
        {
            user.FaceFactor = new FaceFactor();
            if (faceIds != null)
            {
                user.FaceFactor.FaceIds = faceIds.ToList();
            }
        }

        public override void UpdateFactorForUser(UserAccount user, List<int> faceIds)
        {
            if (user.FaceFactor == null)
                AddFactorToUser(user, faceIds);
            else if (faceIds != null)
            {
                user.FaceFactor.FaceIds = user.FaceFactor.FaceIds.Union(faceIds).Distinct().ToList();
            }
        }

        public override void RemoveFactorForUser(UserAccount user)
        {
            user.FaceFactor = null;
        }

        public override bool Verify(UserAccount user, List<int> faceIds = null)
        {
            return user.FaceFactor == null || user.FaceFactor.FaceIds.Exists(f => faceIds.Contains(f));
        }
    }
}
