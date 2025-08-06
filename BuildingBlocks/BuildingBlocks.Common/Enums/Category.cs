namespace BuildingBlocks.Common.Enums;

[Flags]
public enum Category
{
    None = 0,
    IfElse = 1 << 0,         // 1
    Arrays = 1 << 1,         // 2
    Lists = 1 << 2,          // 4
    Maps = 1 << 3,           // 8
    Recursion = 1 << 4,      // 16
    DynamicProgramming = 1 << 5,
    Graphs = 1 << 6,
    Trees = 1 << 7,
    Strings = 1 << 8,
    Math = 1 << 9,
    Greedy = 1 << 10,
    Sorting = 1 << 11,
    Searching = 1 << 12,
    BitManipulation = 1 << 13,
    Backtracking = 1 << 14,
}