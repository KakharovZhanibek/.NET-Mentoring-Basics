using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace FileSystemVisitorConsoleApp.Tests.Filters;

public class AndFilterTests : IDisposable
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
    public void Constructor_WithNullFilters_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new AndFilter((IFileSystemFilter[])null!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void IsMatch_WithAllFiltersMatching_ReturnsTrue()
    {
        // Arrange
        var mock1 = new Mock<IFileSystemFilter>();
        mock1.Setup(f => f.IsMatch(It.IsAny<FileSystemInfo>())).Returns(true);
        mock1.Setup(f => f.Description).Returns("Filter1");
        
        var mock2 = new Mock<IFileSystemFilter>();
        mock2.Setup(f => f.IsMatch(It.IsAny<FileSystemInfo>())).Returns(true);
        mock2.Setup(f => f.Description).Returns("Filter2");
        
        var filter = new AndFilter(mock1.Object, mock2.Object);
        var file = FileSystemInfoHelper.CreateFileInfo();
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsMatch_WithOneFilterNotMatching_ReturnsFalse()
    {
        // Arrange
        var mock1 = new Mock<IFileSystemFilter>();
        mock1.Setup(f => f.IsMatch(It.IsAny<FileSystemInfo>())).Returns(true);
        mock1.Setup(f => f.Description).Returns("Filter1");
        
        var mock2 = new Mock<IFileSystemFilter>();
        mock2.Setup(f => f.IsMatch(It.IsAny<FileSystemInfo>())).Returns(false);
        mock2.Setup(f => f.Description).Returns("Filter2");
        
        var filter = new AndFilter(mock1.Object, mock2.Object);
        var file = FileSystemInfoHelper.CreateFileInfo();
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void IsMatch_WithEmptyFilterList_ReturnsTrue()
    {
        // Arrange
        var filter = new AndFilter();
        var file = FileSystemInfoHelper.CreateFileInfo();
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void Description_CombinesAllFilterDescriptions()
    {
        // Arrange
        var mock1 = new Mock<IFileSystemFilter>();
        mock1.Setup(f => f.Description).Returns("Filter1");
        
        var mock2 = new Mock<IFileSystemFilter>();
        mock2.Setup(f => f.Description).Returns("Filter2");
        
        var filter = new AndFilter(mock1.Object, mock2.Object);
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Be("(Filter1 AND Filter2)");
    }
}
