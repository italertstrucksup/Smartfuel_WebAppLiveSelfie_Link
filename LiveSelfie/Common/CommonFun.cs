using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace LiveSelfie.Common
{
    public class CommonFun : ICommonFun
    {
        private readonly IConfiguration _configuration;
        private static string AwsAccessKeyId = string.Empty;
        private string AwsSecretKey = string.Empty;
        private static string AwsBucketName = string.Empty;
        public CommonFun(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string AWSUniquePrefixKey(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString() + DateTime.Now.ToString("ddMMyyyyHHmmssfff");
        }

        public async Task<string> AWSUploadBase64Media(string base64Image, string filetype, string filename, string foldername = "trucksupImages")
        {
            // Extract S3 configuration
            AwsAccessKeyId = _configuration.GetValue<string>("S3Settings:AwsAccessKeyId");
            AwsSecretKey = _configuration.GetValue<string>("S3Settings:AwsSecretKey");
            AwsBucketName = _configuration.GetValue<string>("S3Settings:AwsBucketName");

            // Decode Base64 string to byte array
            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(base64Image);
            }
            catch (FormatException)
            {
                return string.Empty; // Return empty if Base64 string is invalid
            }

            // Generate unique file name
            string preFixKey = AWSUniquePrefixKey(5);
            string fileNameParam = $"{preFixKey}.{filename}";

            try
            {
                // Convert byte array to MemoryStream
                using var fileStream = new MemoryStream(imageBytes);

                // Initialize S3 client
                using (IAmazonS3 client = new AmazonS3Client(AwsAccessKeyId, AwsSecretKey, RegionEndpoint.APSouth1))
                {
                    // Prepare the S3 upload request
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = fileStream,
                        Key = $"{foldername}/{fileNameParam}",
                        BucketName = AwsBucketName,
                        CannedACL = S3CannedACL.Private
                    };

                    // Upload file using TransferUtility
                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }

                return fileNameParam; // Return uploaded file name
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                return string.Empty; // Return empty string if upload fails
            }
        }
    }
}
