using LiveSelfie.Helper;
using LiveSelfie.Models;

namespace LiveSelfie.DAL
{
    public class LiveSelfieDAL : ILiveSelfieDAL
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        public LiveSelfieDAL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:TrucksUpDb"];
        }

        public async Task<GetUserDetailModal> CheckLeadExists(string pem)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("@ShortCode", pem);
                var result = await DbHelper.ExecuteQuery<GetUserDetailModal>(_connectionString, "SP_CheckLeadExists", parameters);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<CommonResponse<int>> SaveFilekey(string fileKey, long leadId, string MobileNo)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("@fileKey", fileKey);
                parameters.Add("@leadId", leadId);
                parameters.Add("@MobileNum", MobileNo);
                var result = await DbHelper.ExecuteQuery<CommonResponse<int>>(_connectionString, "SP_SaveFilekey", parameters);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return new CommonResponse<int>
                {
                    Message = "An error occurred while saving the file key",
                    StatusCode = 500,
                    Data = 0
                };
            }
        }

        public async Task<CommonResponse<int>> CheckSessionStatus(long leadId, string MobileNo)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("@leadId", leadId);
                parameters.Add("@MobileNum", MobileNo);
                var result = await DbHelper.ExecuteQuery<CommonResponse<int>>(_connectionString, "SP_CheckSessionExpire", parameters);
                return result.FirstOrDefault();
            }
            catch (Exception ex) 
            {
                return new CommonResponse<int>
                {
                    Message = "An error occurred while checking Session",
                    StatusCode = 500,
                    Data = 0
                };
            }
        }
    }
}
