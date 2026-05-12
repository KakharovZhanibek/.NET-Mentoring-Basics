using FileSystemVisitorConsoleApp.Filters;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;

namespace FileSystemVisitorConsoleApp.Tests.Filters;

public class FilesOnlyFilterTests : IDisposable
{
    private readonly List<FileSystemInfo> _tempFiles = new();
    
    public void Dispose()
    {
        foreach (var file in _tempFiles)
        {
            FileSystemInfoHelper.CleanupTempFile(file);
        }
    }
    
    [Fact]
    public void IsMatch_WithFileInfo_ReturnsTrue()
    {
        // Arrange
        var filter = new FilesOnlyFilter();
        var file = FileSystemInfoHelper.CreateFileInfo();
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsMatch_WithDirectoryInfo_ReturnsFalse()
    {
        // Arrange
        var filter = new FilesOnlyFilter();
        var directory = FileSystemInfoHelper.CreateDirectoryInfo();
        _tempFiles.Add(directory);
        
        // Act
        var result = filter.IsMatch(directory);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void Description_ReturnsCorrectMessage()
    {
        // Arrange
        var filter = new FilesOnlyFilter();
        
        // Act & Assert
        filter.Description.Should().Be("Type: Files only");
    }
}
