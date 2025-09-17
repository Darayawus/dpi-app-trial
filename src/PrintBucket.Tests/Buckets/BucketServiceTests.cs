using Moq;
using PrintBucket.AWS.Services;
using PrintBucket.Models;
using Xunit;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Serilog;
using System.Text.Json;

namespace PrintBucket.Tests.Services
{
    public class BucketServiceTests
    {
        private readonly Mock<IAmazonDynamoDB> _mockDynamoDb;
        private readonly IBucketService _service;
        private const string TableName = "dpi_bucket";

        public BucketServiceTests()
        {
            _mockDynamoDb = new Mock<IAmazonDynamoDB>();
            //_mockLogger = new Mock<ILogger<BucketService>>();
            _service = new BucketService(_mockDynamoDb.Object);
        }

        [Fact]
        public async Task CreateBucketAsync_WithValidData_ReturnsBucket()
        {
            // Arrange
            var email = "test@example.com";
            var name = "Test Bucket";

            _mockDynamoDb
                .Setup(x => x.PutItemAsync(
                    It.IsAny<PutItemRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PutItemResponse { HttpStatusCode = System.Net.HttpStatusCode.OK });

            // Act
            var result = await _service.CreateBucketAsync(email, name);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal(name, result.Name);
            Assert.NotEmpty(result.Id);
            Assert.NotEmpty(result.AccessCode);
            Assert.Equal("Active", result.Status);
            Assert.True(result.CreatedAt > DateTime.UtcNow.AddMinutes(-1));

            _mockDynamoDb.Verify(x => x.PutItemAsync(
                It.Is<PutItemRequest>(r => 
                    r.TableName == TableName &&
                    r.Item["email"].S == email &&
                    r.Item["name"].S == name),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetBucketByIdAsync_WhenExists_ReturnsBucket()
        {
            // Arrange
            var bucket = new Bucket
            {
                Id = "123",
                Email = "test@example.com",
                Name = "Test Bucket",
                AccessCode = "ABC123",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            var response = new GetItemResponse
            {
                Item = new Dictionary<string, AttributeValue>
                {
                    ["id"] = new AttributeValue { S = bucket.Id },
                    ["email"] = new AttributeValue { S = bucket.Email },
                    ["name"] = new AttributeValue { S = bucket.Name },
                    ["accessCode"] = new AttributeValue { S = bucket.AccessCode },
                    ["status"] = new AttributeValue { S = bucket.Status },
                    ["createdAt"] = new AttributeValue { S = bucket.CreatedAt.ToString("O") }
                },
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };

            _mockDynamoDb
                .Setup(x => x.GetItemAsync(
                    It.Is<GetItemRequest>(r => 
                        r.TableName == TableName && 
                        r.Key["id"].S == bucket.Id),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _service.GetBucketByIdAsync(bucket.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bucket.Id, result.Id);
            Assert.Equal(bucket.Email, result.Email);
            Assert.Equal(bucket.Name, result.Name);
            Assert.Equal(bucket.AccessCode, result.AccessCode);
        }

        [Fact]
        public async Task GetBucketByIdAsync_WhenNotExists_ReturnsNull()
        {
            // Arrange
            var id = "nonexistent";

            _mockDynamoDb
                .Setup(x => x.GetItemAsync(
                    It.IsAny<GetItemRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetItemResponse { HttpStatusCode = System.Net.HttpStatusCode.OK });

            // Act
            var result = await _service.GetBucketByIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetBucketByAccessCodeAsync_WhenExists_ReturnsBucket()
        {
            // Arrange
            var bucket = new Bucket
            {
                Id = "123",
                Email = "test@example.com",
                Name = "Test Bucket",
                AccessCode = "ABC123",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            var queryResponse = new QueryResponse
            {
                Items = new List<Dictionary<string, AttributeValue>>
                {
                    new()
                    {
                        ["id"] = new AttributeValue { S = bucket.Id },
                        ["email"] = new AttributeValue { S = bucket.Email },
                        ["name"] = new AttributeValue { S = bucket.Name },
                        ["accessCode"] = new AttributeValue { S = bucket.AccessCode },
                        ["status"] = new AttributeValue { S = bucket.Status },
                        ["createdAt"] = new AttributeValue { S = bucket.CreatedAt.ToString("O") }
                    }
                },
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };

            _mockDynamoDb
                .Setup(x => x.QueryAsync(
                    It.Is<QueryRequest>(r => 
                        r.TableName == TableName && 
                        r.IndexName == "accessCode-index"),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResponse);

            // Act
            var result = await _service.GetBucketByAccessCodeAsync(bucket.AccessCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bucket.Id, result.Id);
            Assert.Equal(bucket.AccessCode, result.AccessCode);
        }

        [Fact]
        public async Task GetBucketByAccessCodeAsync_WhenNotExists_ReturnsNull()
        {
            // Arrange
            var accessCode = "nonexistent";

            _mockDynamoDb
                .Setup(x => x.QueryAsync(
                    It.IsAny<QueryRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueryResponse 
                { 
                    Items = new List<Dictionary<string, AttributeValue>>(),
                    HttpStatusCode = System.Net.HttpStatusCode.OK 
                });

            // Act
            var result = await _service.GetBucketByAccessCodeAsync(accessCode);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateBucketAsync_WhenDynamoDbFails_ThrowsException()
        {
            // Arrange
            _mockDynamoDb
                .Setup(x => x.PutItemAsync(
                    It.IsAny<PutItemRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AmazonDynamoDBException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _service.CreateBucketAsync("test@example.com", "Test"));
        }
    }
}