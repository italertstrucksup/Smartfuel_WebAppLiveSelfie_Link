using LiveSelfie.Models;

namespace LiveSelfie.BAL
{
    public interface ILiveSelfieBAL
    {
        Task<GetUserDetailModal> CheckLeadExists(string pem);
        Task<CommonResponse<int>> SaveFilekey(string fileKey,long leadId,string MobileNo);
        Task<CommonResponse<int>> CheckSessionStatus(long leadId, string MobileNo);
    }
}
