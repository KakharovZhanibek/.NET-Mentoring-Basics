using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace FileSystemVisitorConsoleApp.Tests.Filters;

public class OrFilterTests : IDisposable
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
        Action act = () => new OrFilter((IFileSystemFilter[])null!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void IsMatch_WithAtLeastOneFilterMatching_ReturnsTrue()
    {
        // Arrange
        var mock1 = new Mock<IFileSystemFilter>();
        mock1.Setup(f => f.IsMatch(It.IsAny<FileSystemInfo>())).Returns(false);
        mock1.Setup(f => f.Description).Returns("Filter1");
        
        var mock2 = new Mock<IFileSystemFilter>();
        mock2.Setup(f => f.IsMatch(It.IsAny<FileSystemInfo>())).Returns(true);
        mock2.Setup(f => f.Description).Returns("Filter2");
        
        var filter = new OrFilter(mock1.Object, mock2.Object);
        var file = FileSystemInfoHelper.CreateFileInfo();
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsMatch_WithNoFiltersMatching_ReturnsFalse()
    {
        // Arrange
        var mock1 = new Mock<IFileSystemFilter>();
        mock1.Setup(f => f.IsMatch(It.IsAny<FileSystemInfo>())).Returns(false);
        mock1.Setup(f => f.Description).Returns("Filter1");
        
        var mock2 = new Mock<IFileSystemFilter>();
        mock2.Setup(f => f.IsMatch(It.IsAny<FileSystemInfo>())).Returns(false);
        mock2.Setup(f => f.Description).Returns("Filter2");
        
        var filter = new OrFilter(mock1.Object, mock2.Object);
        var file = FileSystemInfoHelper.CreateFileInfo();
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void Description_CombinesAllFilterDescriptions()
    {
        // Arrange
        var mock1 = new Mock<IFileSystemFilter>();
        mock1.Setup(f => f.Description).Returns("Filter1");
        
        var mock2 = new Mock<IFileSystemFilter>();
        mock2.Setup(f => f.Description).Returns("Filter2");
        
        var filter = new OrFilter(mock1.Object, mock2.Object);
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Be("(Filter1 OR Filter2)");
    }
}
