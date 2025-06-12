using AutoFixture.Xunit2;
using BlogModule.Domain;
using Common.Domain;
using FluentAssertions;
namespace BlogModule.UnitTests.Domain
{
  public class CategoryTests
  {
    [Theory]
    [AutoData]
    public void CategoryDbo_WhenPropertiesAreSet_GetReturnsExpectedValues(Category expectedCategoryDbo)
    {
      // arrange and act
      var actualCategoryDbo = new Category
      {
        Id = expectedCategoryDbo.Id,
        CreationDate = expectedCategoryDbo.CreationDate,
        Slug = expectedCategoryDbo.Slug,
        Title = expectedCategoryDbo.Title,
      };

      // assert
      actualCategoryDbo.Should().BeEquivalentTo(expectedCategoryDbo);
    }

    [Fact]
    public void CategoryDbo_ShouldDerivedFromBaseEntity()
    {
      // arrange and act
      var typeCategoryDbo = typeof(Category);

      // assert
      typeCategoryDbo.Should().BeDerivedFrom<BaseEntity>();
    }
  }
}
