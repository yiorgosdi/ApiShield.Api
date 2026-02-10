using ApiShield.Core;
using Moq;

namespace ApiShield.UnitTests;

public class ApiKeyValidatorTests
{
    // #1. Missing key then ArgumentException. 
    [Fact]
    public async Task When_api_key_is_missing_validation_fails()
    {
        // Arrange
        var store = new Mock<IApiKeyStore>();
        var sut = new ApiKeyValidator(store.Object);

        // Act
        Func<Task> act = () => sut.ValidateOrThrowAsync(null, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);

        store.VerifyNoOtherCalls();
    }

    // #2. case: the apikey has the required length >=16 but it does not exist in store. Then UnauthorizedAccessException. ExistsAsync must be called once.  
    [Fact]
    public async Task When_api_key_is_not_found_validation_fails_with_unauthorized()
    {
        var apiKeyStr = "1234567890abcdef1234567890abcabc";//net present key.

        var store = new Mock<IApiKeyStore>();
        store.Setup(s => s.ExistsAsync(apiKeyStr, It.IsAny<CancellationToken>()))
             .ReturnsAsync(false);
        var sut = new ApiKeyValidator(store.Object);

        Func<Task> act = () => sut.ValidateOrThrowAsync(apiKeyStr, CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(act);

        store.Verify(s => s.ExistsAsync(apiKeyStr, It.IsAny<CancellationToken>()), Times.Once);
        store.VerifyNoOtherCalls();
    }
        
    // #3. When a valid API key exists in the store, validation completes successfully.
    [Fact]
    public async Task When_api_key_is_valid_validation_succeeds()
    {
        const string AdminKey = "1234567890abcdef1234567890abcdef";
        
        // Arrange
        var store = new Mock<IApiKeyStore>();
        store.Setup(s => s.ExistsAsync(AdminKey, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var sut = new ApiKeyValidator(store.Object);

        // Act
        Func<Task> act = () => sut.ValidateOrThrowAsync(AdminKey, CancellationToken.None);

        // Assert
        var ex = await Record.ExceptionAsync(act);
        Assert.Null(ex);

        store.Verify(s => s.ExistsAsync(AdminKey, It.IsAny<CancellationToken>()), Times.Once);

        store.VerifyNoOtherCalls();
    }

    // #4. 
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task When_api_key_is_invalid_validation_fails(string? apiKey)
    {
        // Arrange
        var store = new Mock<IApiKeyStore>();
        var sut = new ApiKeyValidator(store.Object);

        // Act
        Func<Task> act = () => sut.ValidateOrThrowAsync(apiKey, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);

        store.VerifyNoOtherCalls();
    }

    // #5. too short. 
    [Fact]
    public async Task When_api_key_is_too_short_validation_fails_with_unauthorized()
    {
        var apiKeyStr = "1234567890abcde";//too short.

        var store = new Mock<IApiKeyStore>();        
        var sut = new ApiKeyValidator(store.Object);

        Func<Task> act = () => sut.ValidateOrThrowAsync(apiKeyStr, CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(act);

        // no need for 
        store.VerifyNoOtherCalls();
    }
}
