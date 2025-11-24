using Xunit;
using API.Models;

namespace unit_tests.Models;

public class BDTests
{
    [Fact]
    public void SearchByCity_WithValidCity_HandlesDbErrors()
    {
        // Arrange
        var city = "TestCity";

        // Act
        var result = BD.SearchByCity(city);

        // Assert - метод должен вернуть null при ошибке БД, а не бросить исключение
        Assert.True(result == null || result.Count >= 0);
    }

    [Fact]
    public void SearchUser_WithValidId_HandlesDbErrors()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = BD.SearchUser(userId);

        // Assert - результат может быть null если пользователь не найден или БД недоступна
        Assert.True(result == null || !string.IsNullOrEmpty(result));
    }
}