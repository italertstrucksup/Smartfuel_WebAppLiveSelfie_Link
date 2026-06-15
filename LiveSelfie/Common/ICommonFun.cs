namespace LiveSelfie.Common
{
    public interface ICommonFun
    {
        Task<string> AWSUploadBase64Media(string base64Image, string filetype, string filename, string foldername);
    }
}
