using FileSystemVisitorConsoleApp.Filters;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;

namespace FileSystemVisitorConsoleApp.Tests.Filters;

public class ModificationDateFilterTests : IDisposable
{
    private readonly List<FileSystemInfo> _tempFiles = new();
    
    public void Dispose()
    {
        foreach (var file in _tempFiles)
        {
            FileSystemInfoHelper.CleanupTempFile(file);
        }
    }
    
    private FileInfo CreateTestFile(DateTime? lastWriteTime = null)
    {
        var file = FileSystemInfoHelper.CreateFileInfo(lastWriteTime: lastWriteTime);
        _tempFiles.Add(file);
        return file;
    }
    
    [Fact]
    public void Constructor_WithStartDateAfterEndDate_ThrowsArgumentException()
    {
        // Arrange & Act
        Action act = () => new ModificationDateFilter(
            new DateTime(2024, 12, 31),
            new DateTime(2024, 1, 1));
        
        // Assert
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void IsMatch_WithFileModifiedInRange_ReturnsTrue()
    {
        // Arrange
        var filter = new ModificationDateFilter(
            new DateTime(2024, 1, 1),
            new DateTime(2024, 12, 31));
        var file = CreateTestFile(lastWriteTime: new DateTime(2024, 6, 15));
        
        // Act & Assert
        filter.IsMatch(file).Should().BeTrue();
    }
    
    [Fact]
    public void IsMatch_WithFileModifiedOutsideRange_ReturnsFalse()
    {
        // Arrange
        var filter = new ModificationDateFilter(
            new DateTime(2024, 1, 1),
            new DateTime(2024, 12, 31));
        var file = CreateTestFile(lastWriteTime: new DateTime(2025, 1, 1));
        
        // Act & Assert
        filter.IsMatch(file).Should().BeFalse();
    }
    
    [Fact]
    public void Description_WithBothDates_ReturnsCorrectFormat()
    {
        // Arrange
        var filter = new ModificationDateFilter(
            new DateTime(2024, 1, 1),
            new DateTime(2024, 12, 31));
        
        // Act & Assert
        filter.Description.Should().Be("Modified: 2024-01-01 to 2024-12-31");
    }
}
