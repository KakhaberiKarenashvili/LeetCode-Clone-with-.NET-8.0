namespace BuildingBlocks.Common.Enums;

public enum Status
{
    TestRunning,
    TestPassed,
    TestFailed,
    CompilationFailed,
    RuntimeFailed,
    TimeLimitExceeded,
    MemoryLimitExceeded,
}