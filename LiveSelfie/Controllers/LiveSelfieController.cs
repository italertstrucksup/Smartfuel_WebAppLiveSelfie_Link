using LiveSelfie.BAL;
using LiveSelfie.Common;
using Microsoft.AspNetCore.Mvc;

namespace LiveSelfie.Controllers
{
    public class LiveSelfieController : Controller
    {
        private readonly ILiveSelfieBAL _liveSelfieBAL;
        private readonly ICommonFun _common;
        public LiveSelfieController(ILiveSelfieBAL liveSelfieBAL, ICommonFun common)
        {
            _liveSelfieBAL = liveSelfieBAL;
            _common = common;
        }

        public async Task<IActionResult> Index(string pem)
        {
            //if (string.IsNullOrEmpty(leadId) || string.IsNullOrEmpty(MobileNo) || leadId == "0" || MobileNo == "0")
            //{
            //    return RedirectToAction("InvalidLink");
            //}

            //byte[] leadbytes = Convert.FromBase64String(leadId);
            //byte[] mobilebytes = Convert.FromBase64String(MobileNo);
            //var decleadId = System.Text.Encoding.UTF8.GetString(leadbytes);
            //var decMobileNo = System.Text.Encoding.UTF8.GetString(mobilebytes);

            if (string.IsNullOrEmpty(pem))
            {
                return RedirectToAction("SessionExpired");
            }

            var data = await _liveSelfieBAL.CheckLeadExists(pem);

            if (data == null || data.Leadid == 0 || string.IsNullOrEmpty(data.UsermobileNo))
            {
                return RedirectToAction("SessionExpired");
            }

            var sessionRes = await _liveSelfieBAL.CheckSessionStatus(data.Leadid, data.UsermobileNo);

            if (sessionRes == null || sessionRes.StatusCode != 200)
            {
                return RedirectToAction("SessionExpired");
            }

            HttpContext.Session.SetString("LeadId", Convert.ToString(data.Leadid));
            HttpContext.Session.SetString("MobileNo", data.UsermobileNo);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var sessionRes = await _liveSelfieBAL.CheckSessionStatus(Convert.ToInt64(HttpContext.Session.GetString("LeadId")), HttpContext.Session.GetString("MobileNo"));

            if (sessionRes == null || sessionRes.StatusCode != 200)
            {
                return RedirectToAction("SessionExpired");
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var base64Image = Convert.ToBase64String(memoryStream.ToArray());

            var fileKey = await _common.AWSUploadBase64Media(base64Image, file.ContentType, file.FileName, "trucksupImages");
            if (string.IsNullOrEmpty(fileKey))
            {
                return StatusCode(500, "Error uploading file.");
            }

            var result = await _liveSelfieBAL.SaveFilekey(fileKey, Convert.ToInt64(HttpContext.Session.GetString("LeadId")), HttpContext.Session.GetString("MobileNo"));
            return Ok(result);
        }

        public IActionResult SessionExpired()
        {
            return View();
        }

        public IActionResult InvalidLink()
        {
            return View();
        }
    }
}
