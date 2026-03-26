using ApiShield.Core;

using Moq;

namespace ApiShield.UnitTests;

public class ApiKeyValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ValidateOrThrowAsync_WhenApiKeyIsNullOrWhitespace_ThrowsArgumentException(string? apiKey)
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

    // below limit, 15-chars. 
    [Fact]
    public async Task ValidateOrThrowAsync_WhenApiKeyIsTooShort_ThrowsArgumentException()
    {
        var shortApiKeyStr = "sd6s8fg6sdgf123";

        // Arrange 
        var store = new Mock<IApiKeyStore>();
        var sut = new ApiKeyValidator(store.Object);

        // Act 
        Func<Task> act  = () => sut.ValidateOrThrowAsync(shortApiKeyStr, CancellationToken.None);

        // Assert 
        await Assert.ThrowsAsync<ArgumentException>(act); // ThrowsAsync instead of ThrowsAnyAsync (in purpose).
    }

    // classic boundary-value testing, exactly 16-char. 
    [Fact]
    public async Task ValidateOrThrowAsync_WhenApiKeyLengthIsExactly16_DoesNotThrow()
    {
        const string apiKey = "1234567890123456"; // 16 chars

        var store = new Mock<IApiKeyStore>();
        store.Setup(s => s.ExistsAsync(apiKey, It.IsAny<CancellationToken>()))
             .ReturnsAsync(true);

        var sut = new ApiKeyValidator(store.Object);

        Func<Task> act = () => sut.ValidateOrThrowAsync(apiKey, CancellationToken.None);

        var ex = await Record.ExceptionAsync(act);

        Assert.Null(ex);

        store.Verify(s => s.ExistsAsync(apiKey, It.IsAny<CancellationToken>()), Times.Once);
        store.VerifyNoOtherCalls();
    }

    // case: the apikey has the required length >16 but it does not exist in store. Then UnauthorizedAccessException. ExistsAsync must be called once. 
    [Fact]
    public async Task ValidateOrThrowAsync_WhenApiKeyDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        const string nonExistingApiKey = "456wsx854frtg964dsaw";

        // Arrange
        var store = new Mock<IApiKeyStore>();
        store.Setup(s => s.ExistsAsync(nonExistingApiKey, It.IsAny<CancellationToken>()))
             .ReturnsAsync(false);

        var sut = new ApiKeyValidator(store.Object);

        // Act
        Func<Task> act = () => sut.ValidateOrThrowAsync(nonExistingApiKey, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(act);

        store.Verify(s => s.ExistsAsync(nonExistingApiKey, It.IsAny<CancellationToken>()), Times.Once);
        store.VerifyNoOtherCalls();
    }

    // When a valid API key exists in the store, validation completes successfully.
    [Fact]
    public async Task ValidateOrThrowAsync_WhenApiKeyIsValid_DoesNotThrow()
    {
        const string validApiKey = "valid-api-key-1234567890abcdef";

        // Arrange
        var store = new Mock<IApiKeyStore>();
        store.Setup(s => s.ExistsAsync(validApiKey, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var sut = new ApiKeyValidator(store.Object);

        // Act
        Func<Task> act = () => sut.ValidateOrThrowAsync(validApiKey, CancellationToken.None);

        // Assert
        var ex = await Record.ExceptionAsync(act);
        Assert.Null(ex);

        store.Verify(s => s.ExistsAsync(validApiKey, It.IsAny<CancellationToken>()), Times.Once);

        store.VerifyNoOtherCalls();
    }    
}
