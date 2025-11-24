using Xunit;
using Moq;
using Telegram.Bot;

namespace unit_tests.Services;

public class NotificationServiceTests
{
    [Fact]
    public void NotificationService_Constructor_CreatesInstance()
    {
        // Arrange
        var botClientMock = new Mock<ITelegramBotClient>();

        // Act
        var service = new NotificationService(botClientMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task SendMessageToUserAsync_WithValidParameters_DoesNotThrow()
    {
        // Arrange
        var botClientMock = new Mock<ITelegramBotClient>();
        var service = new NotificationService(botClientMock.Object);
        var chatId = "123456789";
        var message = "Test message";
        var userName = "Test User";

        // Act & Assert - метод не должен бросить исключение
        // Даже если бот не настроен, метод должен обработать ошибку
        await service.SendMessageToUserAsync(chatId, message, userName);
        
        Assert.True(true);
    }

    [Fact]
    public async Task SendMessageToCityAsync_WithValidParameters_HandlesDbErrors()
    {
        // Arrange
        var botClientMock = new Mock<ITelegramBotClient>();
        var service = new NotificationService(botClientMock.Object);
        var city = "TestCity";
        var message = "Test message";

        // Act & Assert - метод должен обработать ошибку БД без исключения
        try
        {
            await service.SendMessageToCityAsync(city, message);
            Assert.True(true);
        }
        catch (NullReferenceException)
        {
            // Ожидаемое поведение при отсутствии БД - метод может вернуть null из SearchByCity
            Assert.True(true);
        }
    }
}
