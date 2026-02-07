using System.IO.Compression;
using Backup.Core.Models;

namespace Backup.Core.Services;

public sealed class BackupService
{
    public BackupResult Run(BackupPlan plan)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plan.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(plan.SourceDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(plan.DestinationDirectory);

        var startedAt = DateTimeOffset.Now;
        var sourceDirectory = new DirectoryInfo(plan.SourceDirectory);
        if (!sourceDirectory.Exists)
        {
            throw new DirectoryNotFoundException($"Kaynak klasör bulunamadı: {plan.SourceDirectory}");
        }

        Directory.CreateDirectory(plan.DestinationDirectory);

        var timestamp = startedAt.ToString("yyyyMMdd_HHmmss");
        var backupName = $"{plan.Name}_{timestamp}";
        var outputPath = plan.CompressAsZip
            ? Path.Combine(plan.DestinationDirectory, $"{backupName}.zip")
            : Path.Combine(plan.DestinationDirectory, backupName);

        var copiedFiles = new List<string>();
        long totalBytes = 0;

        if (plan.CompressAsZip)
        {
            using var zipStream = new FileStream(outputPath, FileMode.CreateNew, FileAccess.ReadWrite);
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create);

            foreach (var file in sourceDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(plan.SourceDirectory, file.FullName);
                var entry = archive.CreateEntry(relativePath, CompressionLevel.Optimal);

                using var entryStream = entry.Open();
                using var fileStream = file.OpenRead();
                fileStream.CopyTo(entryStream);

                copiedFiles.Add(relativePath);
                totalBytes += file.Length;
            }
        }
        else
        {
            Directory.CreateDirectory(outputPath);

            foreach (var file in sourceDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(plan.SourceDirectory, file.FullName);
                var targetPath = Path.Combine(outputPath, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                file.CopyTo(targetPath, overwrite: true);

                copiedFiles.Add(relativePath);
                totalBytes += file.Length;
            }
        }

        ApplyRetention(plan, plan.DestinationDirectory);

        var finishedAt = DateTimeOffset.Now;
        return new BackupResult(plan.Name, outputPath, startedAt, finishedAt, totalBytes, copiedFiles);
    }

    private static void ApplyRetention(BackupPlan plan, string destinationDirectory)
    {
        if (plan.RetentionCount <= 0)
        {
            return;
        }

        var directoryInfo = new DirectoryInfo(destinationDirectory);
        if (!directoryInfo.Exists)
        {
            return;
        }

        var backups = directoryInfo
            .GetFileSystemInfos($"{plan.Name}_*")
            .OrderByDescending(info => info.CreationTimeUtc)
            .ToList();

        foreach (var oldBackup in backups.Skip(plan.RetentionCount))
        {
            switch (oldBackup)
            {
                case DirectoryInfo directory:
                    directory.Delete(recursive: true);
                    break;
                case FileInfo file:
                    file.Delete();
                    break;
            }
        }
    }
}
