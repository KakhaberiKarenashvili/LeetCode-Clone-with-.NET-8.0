using BuildingBlocks.Common.Enums;

namespace BuildingBlocks.Common.Helpers;

public static class EnumParser
{
    public static List<string> ParseCategories(Category category)
    {
        return Enum.GetValues(typeof(Category))
            .Cast<Category>()
            .Where(c => c != Category.None && category.HasFlag(c))
            .Select(c => c.ToString())
            .ToList();
    }
}