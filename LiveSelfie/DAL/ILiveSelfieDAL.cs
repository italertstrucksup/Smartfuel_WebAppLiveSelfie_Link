using LiveSelfie.Models;

namespace LiveSelfie.DAL
{
    public interface ILiveSelfieDAL
    {
        Task<GetUserDetailModal> CheckLeadExists(string pem);
        Task<CommonResponse<int>> SaveFilekey(string fileKey, long leadId,string MobileNo);
        Task<CommonResponse<int>> CheckSessionStatus(long leadId, string MobileNo);
    }
}
