using BuildingBlocks.Common.Enums;

namespace BuildingBlocks.Common.Helpers;

public static class EnumParser
{
    /// <summary>
    /// Extracts individual category flags from a combined Category enum value
    /// </summary>

    public static List<string> ExtractCategoryFlags(Category category)
    {
        return Enum.GetValues(typeof(Category))
            .Cast<Category>()
            .Where(c => c != Category.None && category.HasFlag(c))
            .Select(c => c.ToString())
            .ToList();
    }
    
    /// <summary>
    /// Parses a list of category strings into a combined Category enum value
    /// </summary>

    public static  Category ParseCategoriesFromStrings(List<string> categoryStrings)
    {
        Category categories = Category.None;

        foreach (var categoryStr in categoryStrings)
        {
            if (Enum.TryParse<Category>(categoryStr, out var category))
            {
                categories |= category; // Add to the bitmask
            }
            else
            {
                throw new ArgumentException($"Invalid category: {categoryStr}");
            }
        }

        return categories;
    }
    
    /// <summary>
    /// Parses a difficulty string into a Difficulty enum value
    /// </summary>

    public static Difficulty ParseDifficultyFromString(string difficulty)
    {
        if (Enum.TryParse<Difficulty>(difficulty, out var parsedDifficulty))
        {
            return parsedDifficulty;
        }

        return Difficulty.Easy;
    }
}