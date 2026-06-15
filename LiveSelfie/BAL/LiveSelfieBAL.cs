
using LiveSelfie.DAL;
using LiveSelfie.Models;

namespace LiveSelfie.BAL
{
    public class LiveSelfieBAL : ILiveSelfieBAL
    {
        private readonly ILiveSelfieDAL _selfieDAL;
        public LiveSelfieBAL(ILiveSelfieDAL selfieDAL)
        {
            _selfieDAL = selfieDAL;
        }
        public async Task<GetUserDetailModal> CheckLeadExists(string pem)
        {
            var data = await _selfieDAL.CheckLeadExists(pem);
            return data;
        }

        public async Task<CommonResponse<int>> SaveFilekey(string fileKey, long leadId, string MobileNo)
        {
            var data = await _selfieDAL.SaveFilekey(fileKey, leadId, MobileNo);
            return data;
        }

        public async Task<CommonResponse<int>> CheckSessionStatus(long leadId, string MobileNo)
        {
                        var data = await _selfieDAL.CheckSessionStatus(leadId, MobileNo);
            return data;
        }
    }
}
