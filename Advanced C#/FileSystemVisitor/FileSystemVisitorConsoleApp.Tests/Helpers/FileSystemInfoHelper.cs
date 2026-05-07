using System.Reflection;

namespace FileSystemVisitorConsoleApp.Tests.Helpers;

/// <summary>
/// Helper class for creating FileSystemInfo instances for testing.
/// Since FileInfo and DirectoryInfo are sealed, we use reflection to create test instances.
/// </summary>
public static class FileSystemInfoHelper
{
    public static FileInfo CreateFileInfo(
        string? name = null,
        long length = 1024,
        DateTime? creationTime = null,
        DateTime? lastWriteTime = null,
        string extension = ".txt")
    {
        // Create a unique temporary file name to avoid conflicts
        var fileName = name ?? $"test_{Guid.NewGuid():N}{extension}";
        var tempPath = Path.Combine(Path.GetTempPath(), fileName);
        
        try
        {
            // Always create a new file with unique name
            File.WriteAllBytes(tempPath, new byte[length]);
            
            var fileInfo = new FileInfo(tempPath);
            
            // Set dates if provided
            if (creationTime.HasValue)
            {
                File.SetCreationTime(tempPath, creationTime.Value);
                fileInfo.Refresh();
            }
            
            if (lastWriteTime.HasValue)
            {
                File.SetLastWriteTime(tempPath, lastWriteTime.Value);
                fileInfo.Refresh();
            }
            
            return fileInfo;
        }
        catch
        {
            // Cleanup on error
            if (File.Exists(tempPath))
            {
                try { File.Delete(tempPath); } catch { }
            }
            throw;
        }
    }
    
    public static DirectoryInfo CreateDirectoryInfo(
        string? name = null,
        DateTime? creationTime = null,
        DateTime? lastWriteTime = null)
    {
        // Create a unique temporary directory name to avoid conflicts
        var dirName = name ?? $"TestDir_{Guid.NewGuid():N}";
        var tempPath = Path.Combine(Path.GetTempPath(), dirName);
        
        try
        {
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            
            var dirInfo = new DirectoryInfo(tempPath);
            
            // Set dates if provided
            if (creationTime.HasValue)
            {
                Directory.SetCreationTime(tempPath, creationTime.Value);
                dirInfo.Refresh();
            }
            
            if (lastWriteTime.HasValue)
            {
                Directory.SetLastWriteTime(tempPath, lastWriteTime.Value);
                dirInfo.Refresh();
            }
            
            return dirInfo;
        }
        catch
        {
            // Cleanup on error
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
            throw;
        }
    }
    
    /// <summary>
    /// Cleanup helper to remove temporary test files.
    /// </summary>
    public static void CleanupTempFile(FileSystemInfo info)
    {
        try
        {
            if (info is FileInfo file && file.Exists)
            {
                file.Delete();
            }
            else if (info is DirectoryInfo dir && dir.Exists)
            {
                dir.Delete(true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}
