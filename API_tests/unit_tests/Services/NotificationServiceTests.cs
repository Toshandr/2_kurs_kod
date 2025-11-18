using Xunit;
using Telegram.Bot;
using Telegram.Bot.Types;

public class NotificationServiceTests
{
    private readonly Mock<ITelegramBotClient> _botClientMock;
    private readonly NotificationService _notificationService;

    public NotificationServiceTests()
    {
        _botClientMock = new Mock<ITelegramBotClient>();
        _notificationService = new NotificationService(_botClientMock.Object);
    }
}

public class SendMessageToCityAsyncTests : NotificationServiceTests
{
    [Fact]
    public async Task SendMessageToCityAsync_WithUsersInCity_SendsMessagesToAllUsers()
    {
        // Arrange
        var city = "Moscow";
        var messageText = "Test message";
        var users = new List<User>
        {
            new User { TelegramTeg = "123" },
            new User { TelegramTeg = "456" },
            new User { TelegramTeg = "789" }
        };

        // Мокаем статический метод SearchByCity
        var bdMock = new Mock<BD>();
        BD.SearchByCity = city => users;

        _botClientMock
            .Setup(x => x.SendTextMessageAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _notificationService.SendMessageToCityAsync(city, messageText);

        // Assert
        _botClientMock.Verify(x => x.SendTextMessageAsync(
            "123", messageText, It.IsAny<CancellationToken>()), Times.Once);
        _botClientMock.Verify(x => x.SendTextMessageAsync(
            "456", messageText, It.IsAny<CancellationToken>()), Times.Once);
        _botClientMock.Verify(x => x.SendTextMessageAsync(
            "789", messageText, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageToCityAsync_NoUsersInCity_DoesNotSendAnyMessages()
    {
        // Arrange
        var city = "UnknownCity";
        var messageText = "Test message";

        BD.SearchByCity = city => new List<User>();

        // Act
        await _notificationService.SendMessageToCityAsync(city, messageText);

        // Assert
        _botClientMock.Verify(x => x.SendTextMessageAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SendMessageToCityAsync_WithNullUsersList_HandlesGracefully()
    {
        // Arrange
        var city = "TestCity";
        var messageText = "Test message";

        BD.SearchByCity = city => null;

        // Act & Assert (не должно бросить исключение)
        await _notificationService.SendMessageToCityAsync(city, messageText);
    }

    [Fact]
    public async Task SendMessageToCityAsync_EmptyMessage_SendsEmptyMessage()
    {
        // Arrange
        var city = "Москва";
        var emptyMessage = "";
        var users = new List<User> { new User { TelegramTeg = "123" } };

        BD.SearchByCity = city => users;

        // Act
        await _notificationService.SendMessageToCityAsync(city, emptyMessage);

        // Assert
        _botClientMock.Verify(x => x.SendTextMessageAsync(
            "123", emptyMessage, It.IsAny<CancellationToken>()), Times.Once);
    }
}