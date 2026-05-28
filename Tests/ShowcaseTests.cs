using System.Net;

namespace TestProjectIntern_n1.Tests;

/// <summary>
/// Тесты на получение доступных продуктов.
/// </summary>
public class ShowcaseTests : BaseTest
{
    /// <summary>
    /// Получение информации о доступных продуктов.
    /// </summary>
    [Fact]
    public async Task ShowcaseProducts_ReturnsOk()
    {
        // Act
        var showcaseProductsResponse = await ShowcaseRestClient.GetShowcaseProducts();

        // Asserts
        Assert.Equal(HttpStatusCode.OK, showcaseProductsResponse.StatusCode);
    }

    /// <summary>
    /// Получение информации о доступном продукте.
    /// </summary>
    [Fact]
    public async Task ShowcaseProduct_WithProductId_ReturnsOk()
    {
        // Act
        var showcaseProductsResponse = await ShowcaseRestClient.GetShowcaseProduct(1);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, showcaseProductsResponse.StatusCode);
    }

    /// <summary>
    /// Получение информации о доступном продукте c невалидным id.
    /// </summary>
    [Fact]
    public async Task ShowcaseProduct_WithInvaildProductId_ReturnsOk()
    {
        // Act
        var showcaseProductsResponse = await ShowcaseRestClient.GetShowcaseProduct(0);

        // Asserts
        Assert.Equal(HttpStatusCode.BadRequest, showcaseProductsResponse.StatusCode);
    }
}
