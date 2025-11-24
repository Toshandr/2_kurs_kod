using Xunit;
using API.FileSys;

namespace unit_tests.FileSys;

public class FileSystemTests
{
    [Fact]
    public void LogError_WithMessage_DoesNotThrow()
    {
        // Arrange
        var errorMessage = "Test error message";
        var exception = new Exception("Test exception");

        // Act & Assert
        FileSystem.LogError(errorMessage, exception);
        Assert.True(true);
    }

    [Fact]
    public void LogInfo_WithMessage_DoesNotThrow()
    {
        // Arrange
        var message = "Test info message";

        // Act & Assert
        FileSystem.LogInfo(message);
        Assert.True(true);
    }
}

public class FileUserTests
{
    [Fact]
    public void LogUserMessage_WithValidParameters_DoesNotThrow()
    {
        // Arrange
        long chatId = 123456789;
        string userName = "Test User";
        string message = "Test message";

        // Act & Assert
        FileUser.LogUserMessage(chatId, userName, message);
        Assert.True(true);
    }

    [Fact]
    public void CreateUserFile_WithValidParameters_DoesNotThrow()
    {
        // Arrange
        long chatId = 987654321;
        string userName = "New Test User";
        int age = 25;
        string city = "Test City";

        // Act & Assert
        FileUser.CreateUserFile(chatId, userName, age, city);
        Assert.True(true);
    }

    [Fact]
    public void LogUserMessage_WithSpecialCharactersInName_DoesNotThrow()
    {
        // Arrange
        long chatId = 111222333;
        string userName = "Test/User\\With:Special*Characters";
        string message = "Test message";

        // Act & Assert - имя должно быть очищено от недопустимых символов
        FileUser.LogUserMessage(chatId, userName, message);
        Assert.True(true);
    }
}
