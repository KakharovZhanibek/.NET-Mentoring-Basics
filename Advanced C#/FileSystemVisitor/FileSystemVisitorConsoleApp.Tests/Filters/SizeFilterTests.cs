using FileSystemVisitorConsoleApp.Filters;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;

namespace FileSystemVisitorConsoleApp.Tests.Filters;

public class SizeFilterTests : IDisposable
{
    private readonly List<FileSystemInfo> _tempFiles = new();
    
    public void Dispose()
    {
        foreach (var file in _tempFiles)
        {
            FileSystemInfoHelper.CleanupTempFile(file);
        }
    }
    
    private FileInfo CreateTestFile(long length = 1024)
    {
        var file = FileSystemInfoHelper.CreateFileInfo(length: length);
        _tempFiles.Add(file);
        return file;
    }
    
    [Fact]
    public void Constructor_WithMinGreaterThanMax_ThrowsArgumentException()
    {
        // Arrange & Act
        Action act = () => new SizeFilter(2048, 1024);
        
        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Minimum size cannot be greater than maximum size*");
    }
    
    [Fact]
    public void IsMatch_WithDirectoryInfo_ReturnsFalse()
    {
        // Arrange
        var filter = new SizeFilter(1024, null);
        var directory = FileSystemInfoHelper.CreateDirectoryInfo();
        _tempFiles.Add(directory);
        
        // Act
        var result = filter.IsMatch(directory);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(1024L, null, 2048L, true)]  // File 2KB > min 1KB
    [InlineData(1024L, null, 512L, false)]  // File 0.5KB < min 1KB
    [InlineData(null, 2048L, 1024L, true)]  // File 1KB < max 2KB
    [InlineData(null, 2048L, 4096L, false)] // File 4KB > max 2KB
    [InlineData(1024L, 4096L, 2048L, true)]  // File 2KB in range [1KB-4KB]
    [InlineData(1024L, 4096L, 512L, false)]  // File 0.5KB not in range
    [InlineData(1024L, 4096L, 8192L, false)] // File 8KB not in range
    public void IsMatch_WithVariousSizes_ReturnsExpectedResult(
        long? minSize, long? maxSize, long fileSize, bool expected)
    {
        // Arrange
        var filter = new SizeFilter(minSize, maxSize);
        var file = CreateTestFile(fileSize);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void Description_WithMinAndMax_ReturnsCorrectFormat()
    {
        // Arrange
        var filter = new SizeFilter(1024, 2048);
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Be("Size: 1KB - 2KB");
    }
    
    [Fact]
    public void Description_WithMinOnly_ReturnsCorrectFormat()
    {
        // Arrange
        var filter = new SizeFilter(1024, null);
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Be("Size: > 1KB");
    }
    
    [Fact]
    public void Description_WithMaxOnly_ReturnsCorrectFormat()
    {
        // Arrange
        var filter = new SizeFilter(null, 2048);
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Be("Size: < 2KB");
    }
    
    [Fact]
    public void Description_WithNoSizeConstraints_ReturnsAnyMessage()
    {
        // Arrange
        var filter = new SizeFilter(null, null);
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Be("Size: Any");
    }
}
