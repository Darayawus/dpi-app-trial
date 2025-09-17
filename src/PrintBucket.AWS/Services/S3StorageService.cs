using Amazon.S3;
using Amazon.S3.Model;

namespace PrintBucket.AWS.Services
{
    public interface IS3StorageService
    {
        Task<List<string>> UploadImageWithVersionsAsync(string bucketName, string bucketId, Stream imageStream, string fileName);
        Task<Stream?> GetImageAsync(string bucketName, string bucketId, string fileName);
        Task DeleteImagesAsync(string bucketName, string bucketId, string fileNameWithoutVersion);
    }

    public class S3StorageService : IS3StorageService
    {
        private readonly IAmazonS3 _s3Client;
        private const string ORIGINAL_SUFFIX = "original";
        private const string LARGE_SUFFIX = "1024";
        private const string SMALL_SUFFIX = "400";

        public S3StorageService(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<List<string>> UploadImageWithVersionsAsync(string bucketName, string bucketId, Stream imageStream, string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var baseKey = $"{bucketId}/o/{Path.GetFileNameWithoutExtension(Guid.NewGuid().ToString())}{extension}";
            var uploadedUrls = new List<string>();

            using (var image = NetVips.Image.NewFromStream(imageStream))
            {
                // Version original
                var originalKey = $"{baseKey}";
                using (var originalStream = new MemoryStream())
                {
                    image.WriteToStream(originalStream, extension);
                    originalStream.Position = 0;
                    await UploadToS3(bucketName, originalStream, originalKey);
                    uploadedUrls.Add(originalKey);
                }

                // Version 1024x1024
                var large = ResizeImage(image, 1024);
                var largeKey = $"{bucketId}/s/{Path.GetFileNameWithoutExtension(Guid.NewGuid().ToString())}{extension}";
                using (var largeStream = new MemoryStream())
                {
                    large.WriteToStream(largeStream, extension);
                    largeStream.Position = 0;
                    await UploadToS3(bucketName, largeStream, largeKey);
                    uploadedUrls.Add(largeKey);
                }

                // Version 400x400
                var small = ResizeImage(image, 400);                
                var smallKey = $"{bucketId}/t/{Path.GetFileNameWithoutExtension(Guid.NewGuid().ToString())}{extension}";

                using (var smallStream = new MemoryStream())
                {
                    small.WriteToStream(smallStream, extension);
                    smallStream.Position = 0;
                    await UploadToS3(bucketName, smallStream, smallKey);
                    uploadedUrls.Add(smallKey);
                }
            }

            return uploadedUrls;
        }

        private NetVips.Image ResizeImage(NetVips.Image image, int targetSize)
        {
            double scale = (double)targetSize / Math.Max(image.Width, image.Height);
            return image.Resize(scale);
        }

        private async Task UploadToS3(string bucketName, Stream stream, string key)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (stream.CanSeek) stream.Position = 0;

            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = "image/jpeg",
                    CannedACL = S3CannedACL.PublicRead
                };

                var response = await _s3Client.PutObjectAsync(putRequest);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK &&
                    response.HttpStatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new InvalidOperationException($"S3 returned unexpected status {response.HttpStatusCode} uploading '{key}' to '{bucketName}'.");
                }
            }
            catch (AmazonS3Exception ex)
            {
                // Proveer contexto útil y relanzar
                throw new InvalidOperationException($"AmazonS3Exception uploading '{key}' to bucket '{bucketName}': {ex.Message} (Code={ex.ErrorCode}, Status={ex.StatusCode})", ex);
            }
            catch (Exception ex)
            {
                // Errores no-S3
                throw new InvalidOperationException($"Unexpected error uploading '{key}' to bucket '{bucketName}': {ex.Message}", ex);
            }
        }

        public async Task<Stream?> GetImageAsync(string bucketName, string bucketId, string fileName)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = $"{bucketId}/{fileName}"
                };

                var response = await _s3Client.GetObjectAsync(request);
                return response.ResponseStream;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task DeleteImagesAsync(string bucketName, string bucketId, string fileNameWithoutVersion)
        {
            var tasks = new[]
            {
                DeleteObject(bucketName, $"{bucketId}/{fileNameWithoutVersion}.{ORIGINAL_SUFFIX}"),
                DeleteObject(bucketName, $"{bucketId}/{fileNameWithoutVersion}.{LARGE_SUFFIX}"),
                DeleteObject(bucketName, $"{bucketId}/{fileNameWithoutVersion}.{SMALL_SUFFIX}")
            };

            await Task.WhenAll(tasks);
        }

        private async Task DeleteObject(string bucketName, string key)
        {
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(deleteRequest);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Ignorar si el archivo no existe
            }
        }
    }
}