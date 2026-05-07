using FileSystemVisitorConsoleApp.Filters;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;

namespace FileSystemVisitorConsoleApp.Tests.Filters;

public class CreationDateFilterTests : IDisposable
{
    private readonly List<FileSystemInfo> _tempFiles = new();
    
    public void Dispose()
    {
        foreach (var file in _tempFiles)
        {
            FileSystemInfoHelper.CleanupTempFile(file);
        }
    }
    
    private FileInfo CreateTestFile(DateTime? creationTime = null)
    {
        var file = FileSystemInfoHelper.CreateFileInfo(creationTime: creationTime);
        _tempFiles.Add(file);
        return file;
    }
    
    [Fact]
    public void Constructor_WithStartDateAfterEndDate_ThrowsArgumentException()
    {
        // Arrange
        var startDate = new DateTime(2024, 12, 31);
        var endDate = new DateTime(2024, 1, 1);
        
        // Act
        Action act = () => new CreationDateFilter(startDate, endDate);
        
        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Start date cannot be after end date*");
    }
    
    [Theory]
    [InlineData("2024-01-15", null, "2024-02-01", true)]  // Created after startDate
    [InlineData("2024-01-15", null, "2024-01-01", false)] // Created before startDate
    [InlineData(null, "2024-12-31", "2024-06-15", true)]  // Created before endDate
    [InlineData(null, "2024-12-31", "2025-01-01", false)] // Created after endDate
    [InlineData("2024-01-01", "2024-12-31", "2024-06-15", true)]  // In range
    [InlineData("2024-01-01", "2024-12-31", "2023-12-31", false)] // Before range
    [InlineData("2024-01-01", "2024-12-31", "2025-01-01", false)] // After range
    public void IsMatch_WithVariousDates_ReturnsExpectedResult(
        string? startDateStr, string? endDateStr, string creationDateStr, bool expected)
    {
        // Arrange
        var startDate = startDateStr != null ? DateTime.Parse(startDateStr) : (DateTime?)null;
        var endDate = endDateStr != null ? DateTime.Parse(endDateStr) : (DateTime?)null;
        var creationDate = DateTime.Parse(creationDateStr);
        
        var filter = new CreationDateFilter(startDate, endDate);
        var file = CreateTestFile(creationTime: creationDate);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void IsMatch_WithBothDatesNull_AlwaysReturnsTrue()
    {
        // Arrange
        var filter = new CreationDateFilter(null, null);
        var file = CreateTestFile(creationTime: DateTime.Now);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void Description_WithBothDates_ReturnsCorrectFormat()
    {
        // Arrange
        var filter = new CreationDateFilter(
            new DateTime(2024, 1, 1),
            new DateTime(2024, 12, 31));
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Be("Created: 2024-01-01 to 2024-12-31");
    }
    
    [Fact]
    public void Description_WithStartDateOnly_ReturnsAfterFormat()
    {
        // Arrange
        var filter = new CreationDateFilter(new DateTime(2024, 1, 1), null);
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Be("Created: after 2024-01-01");
    }
    
    [Fact]
    public void Description_WithEndDateOnly_ReturnsBeforeFormat()
    {
        // Arrange
        var filter = new CreationDateFilter(null, new DateTime(2024, 12, 31));
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Be("Created: before 2024-12-31");
    }
    
    [Fact]
    public void Description_WithNoDates_ReturnsAnyTimeMessage()
    {
        // Arrange
        var filter = new CreationDateFilter(null, null);
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Be("Created: Any time");
    }
}
