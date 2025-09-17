using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using PrintBucket.Models;

namespace PrintBucket.AWS.Services
{
    public interface IImageService
    {
        Task<ImageRecord> AddImageAsync(ImageRecord record);
        Task<List<ImageRecord>> GetImagesByBucketAsync(string bucketId, int limit = 100);
    }

    public class ImageService : IImageService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private const string _tableName = "dpi_bucket";

        public ImageService(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<ImageRecord> AddImageAsync(ImageRecord record)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                { "hash_key", new AttributeValue { S = record.BucketId } },
                { "range_key", new AttributeValue { S = record.Id } },
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

        public async Task<List<ImageRecord>> GetImagesByBucketAsync(string bucketId, int limit = 100)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = "hash_key = :hk",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":hk", new AttributeValue { S = bucketId } }
                },
                Limit = limit,
                ScanIndexForward = false // newest first if range_key is chronological
            };

            var response = await _dynamoDb.QueryAsync(request);

            var result = new List<ImageRecord>();
            foreach (var item in response.Items)
            {
                var record = new ImageRecord
                {
                    Id = item.ContainsKey("range_key") ? item["range_key"].S : (item.ContainsKey("id") ? item["id"].S : string.Empty),
                    BucketId = item.ContainsKey("hash_key") ? item["hash_key"].S : string.Empty,
                    OriginalKey = item.ContainsKey("originalKey") ? item["originalKey"].S : string.Empty,
                    LargeKey = item.ContainsKey("largeKey") ? item["largeKey"].S : string.Empty,
                    SmallKey = item.ContainsKey("smallKey") ? item["smallKey"].S : string.Empty,
                    FileName = item.ContainsKey("fileName") ? item["fileName"].S : string.Empty,
                    ContentType = item.ContainsKey("contentType") ? item["contentType"].S : string.Empty,
                    Size = item.ContainsKey("size") && long.TryParse(item["size"].N, out var size) ? size : 0,
                    CreatedAt = item.ContainsKey("createdAt") && DateTime.TryParse(item["createdAt"].S, out var dt) ? dt : DateTime.MinValue
                };

                result.Add(record);
            }

            return result;
        }
    }
}