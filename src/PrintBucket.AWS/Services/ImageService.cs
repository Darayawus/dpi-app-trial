using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using PrintBucket.Models;

namespace PrintBucket.AWS.Services
{
    public interface IImageService
    {
        Task<ImageRecord> AddImageAsync(ImageRecord record);
    }

    public class ImageService : IImageService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private const string _tableName = "dpi_files";

        public ImageService(IAmazonDynamoDB dynamoDb, string  configuration)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<ImageRecord> AddImageAsync(ImageRecord record)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                { "bucketId", new AttributeValue { S = record.BucketId } },
                { "id", new AttributeValue { S = record.Id } },
                { "originalKey", new AttributeValue { S = record.OriginalKey } },
                { "largeKey", new AttributeValue { S = record.LargeKey } },
                { "smallKey", new AttributeValue { S = record.SmallKey } },
                { "fileName", new AttributeValue { S = record.FileName ?? string.Empty } },
                { "contentType", new AttributeValue { S = record.ContentType ?? string.Empty } },
                { "size", new AttributeValue { N = record.Size.ToString() } },
                { "createdAt", new AttributeValue { S = record.CreatedAt.ToString("O") } }
            };

            var req = new PutItemRequest
            {
                TableName = _tableName,
                Item = item
            };

            await _dynamoDb.PutItemAsync(req);
            return record;
        }
    }
}