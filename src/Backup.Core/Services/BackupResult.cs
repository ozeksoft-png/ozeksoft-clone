namespace Backup.Core.Services;

public sealed record BackupResult(
    string PlanName,
    string DestinationPath,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    long TotalBytes,
    IReadOnlyList<string> CopiedFiles
);
