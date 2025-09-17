using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using PrintBucket.Models;
using System.Text.Json;

namespace PrintBucket.AWS.Services
{
    public interface IBucketService
    {
        Task<Bucket> CreateSpaceAsync(string email, string name);
        Task<Bucket?> GetSpaceByIdAsync(string id);
        Task<Bucket?> GetSpaceByAccessCodeAsync(string accessCode);
    }

    public class BucketService : IBucketService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private const string TableName = "Spaces";

        public BucketService(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<Bucket> CreateSpaceAsync(string email, string name)
        {
            var space = new Bucket
            {
                Email = email,
                Name = name,
                AccessCode = GenerateAccessCode()
            };

            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "id", new AttributeValue { S = space.Id } },
                    { "email", new AttributeValue { S = space.Email } },
                    { "name", new AttributeValue { S = space.Name } },
                    { "accessCode", new AttributeValue { S = space.AccessCode } },
                    { "createdAt", new AttributeValue { S = space.CreatedAt.ToString("O") } },
                    { "status", new AttributeValue { S = space.Status } }
                }
            };

            await _dynamoDb.PutItemAsync(request);
            return space;
        }

        private string GenerateAccessCode()
        {
            // Genera un código de 8 caracteres alfanuméricos
            return Path.GetRandomFileName().Replace(".", "")[..8].ToUpper();
        }

        public async Task<Bucket?> GetSpaceByIdAsync(string id)
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "id", new AttributeValue { S = id } }
                }
            };

            var response = await _dynamoDb.GetItemAsync(request);
            return response.Item.Count > 0 ? MapToSpace(response.Item) : null;
        }

        public async Task<Bucket?> GetSpaceByAccessCodeAsync(string accessCode)
        {
            var request = new QueryRequest
            {
                TableName = TableName,
                IndexName = "AccessCodeIndex",
                KeyConditionExpression = "accessCode = :accessCode",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":accessCode", new AttributeValue { S = accessCode } }
                }
            };

            var response = await _dynamoDb.QueryAsync(request);
            return response.Items.Count > 0 ? MapToSpace(response.Items[0]) : null;
        }

        private Bucket MapToSpace(Dictionary<string, AttributeValue> item)
        {
            return new Bucket
            {
                Id = item["id"].S,
                Email = item["email"].S,
                Name = item["name"].S,
                AccessCode = item["accessCode"].S,
                CreatedAt = DateTime.Parse(item["createdAt"].S),
                Status = item["status"].S
            };
        }
    }
}