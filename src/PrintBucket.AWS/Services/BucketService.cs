using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using PrintBucket.Models;
using Serilog;
using System.Text.Json;

namespace PrintBucket.AWS.Services
{
    public interface IBucketService
    {
        Task<Bucket> CreateBucketAsync(string email, string name);
        Task<Bucket?> GetBucketByIdAsync(string id);
        Task<Bucket?> GetBucketByAccessCodeAsync(string accessCode);
    }

    public class BucketService : IBucketService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private const string TableName = "dpi_bucket";
        private readonly ILogger _logger;

        public BucketService(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
            _logger = Log.ForContext<BedrockImageAnalyzer>();

        }

        public async Task<Bucket> CreateBucketAsync(string email, string name)
        {
            var space = new Bucket
            {
                Email = email,
                Name = name,
                AccessCode = GenerateAccessCode()
            };

            // Asegurar que la tabla usa clave compuesta: 'hash_key' y 'range_key'
            // Usamos 'hash_key' = space.Id y 'range_key' = Guid para unicidad
            //var rangeKey = Guid.NewGuid().ToString();

            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "hash_key", new AttributeValue { S = $"bucket/{space.AccessCode}" } },
                    { "range_key", new AttributeValue { S = space.Id } },
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
            //TODO: Check if the access code is unique
            return Path.GetRandomFileName().Replace(".", "")[..8].ToUpper();
        }

        public async Task<Bucket?> GetBucketByIdAsync(string id)
        {
            // Para tablas con clave compuesta podemos consultar por la clave de partición (hash_key)
            var request = new QueryRequest
            {
                TableName = TableName,
                KeyConditionExpression = "hash_key = :hk",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":hk", new AttributeValue { S = id } }
                },
                Limit = 1
            };

            var response = await _dynamoDb.QueryAsync(request);
            return response.Items.Count > 0 ? MapToBucket(response.Items[0]) : null;
        }

        public async Task<Bucket?> GetBucketByAccessCodeAsync(string accessCode)
        {
            _logger.Information("GetBucketByAccessCodeAsync called with accessCode: {AccessCode}", accessCode);

            try
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

                _logger.Debug("DynamoDB QueryRequest: {@Request}", request);

                var response = await _dynamoDb.QueryAsync(request);

                _logger.Debug("DynamoDB QueryResponse: {@Response}", response);

                if (response.Items.Count > 0)
                {
                    _logger.Information("Bucket found for accessCode: {AccessCode}", accessCode);
                    return MapToBucket(response.Items[0]);
                }
                else
                {
                    _logger.Warning("No bucket found for accessCode: {AccessCode}", accessCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in GetBucketByAccessCodeAsync for accessCode: {AccessCode}", accessCode);
                return null;
            }
        }

        private Bucket MapToBucket(Dictionary<string, AttributeValue> item)
        {
            // Mapear usando los nombres reales de atributos en la tabla
            return new Bucket
            {
                Id = item.ContainsKey("hash_key") ? item["hash_key"].S : (item.ContainsKey("id") ? item["id"].S : string.Empty),
                Email = item.ContainsKey("email") ? item["email"].S : string.Empty,
                Name = item.ContainsKey("name") ? item["name"].S : string.Empty,
                AccessCode = item.ContainsKey("accessCode") ? item["accessCode"].S : string.Empty,
                CreatedAt = item.ContainsKey("createdAt") ? DateTime.Parse(item["createdAt"].S) : DateTime.MinValue,
                Status = item.ContainsKey("status") ? item["status"].S : string.Empty
            };
        }
    }
}