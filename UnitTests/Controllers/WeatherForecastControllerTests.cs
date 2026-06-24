using DatabaseApi;
using DatabaseApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests.Controllers;

[TestClass]
public class WeatherForecastControllerTests : ControllerTestBase {
    [TestMethod]
    public void Get_ReturnsFiveForecasts()
    {
        // Arrange
        WeatherForecastController controller = new WeatherForecastController();

        // Act
        IEnumerable<WeatherForecast> result = controller.Get();

        // Assert
        Assert.AreEqual(5, result.Count());
    }

    [TestMethod]
    public void Get_EachForecast_HasRequiredProperties()
    {
        // Arrange
        WeatherForecastController controller = new WeatherForecastController();

        // Act
        List<WeatherForecast> result = controller.Get().ToList();

        // Assert
        foreach (WeatherForecast forecast in result)
        {
            Assert.IsTrue(forecast.TemperatureC >= -20 && forecast.TemperatureC <= 55);
            Assert.IsFalse(string.IsNullOrEmpty(forecast.Summary));
        }
    }
}


