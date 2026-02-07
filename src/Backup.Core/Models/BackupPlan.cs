namespace Backup.Core.Models;

public sealed class BackupPlan
{
    public string Name { get; init; } = string.Empty;
    public string SourceDirectory { get; init; } = string.Empty;
    public string DestinationDirectory { get; init; } = string.Empty;
    public bool CompressAsZip { get; init; } = true;
    public int RetentionCount { get; init; } = 7;
}
