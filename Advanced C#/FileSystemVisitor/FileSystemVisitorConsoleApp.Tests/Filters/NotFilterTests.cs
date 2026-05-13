using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace FileSystemVisitorConsoleApp.Tests.Filters;

public class NotFilterTests : IDisposable
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
    public void Constructor_WithNullFilter_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new NotFilter(null!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void IsMatch_WithFilterReturningTrue_ReturnsFalse()
    {
        // Arrange
        var mock = new Mock<IFileSystemFilter>();
        mock.Setup(f => f.IsMatch(It.IsAny<FileSystemInfo>())).Returns(true);
        mock.Setup(f => f.Description).Returns("Inner Filter");
        
        var filter = new NotFilter(mock.Object);
        var file = FileSystemInfoHelper.CreateFileInfo();
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void IsMatch_WithFilterReturningFalse_ReturnsTrue()
    {
        // Arrange
        var mock = new Mock<IFileSystemFilter>();
        mock.Setup(f => f.IsMatch(It.IsAny<FileSystemInfo>())).Returns(false);
        mock.Setup(f => f.Description).Returns("Inner Filter");
        
        var filter = new NotFilter(mock.Object);
        var file = FileSystemInfoHelper.CreateFileInfo();
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void Description_IncludesInnerFilterDescription()
    {
        // Arrange
        var mock = new Mock<IFileSystemFilter>();
        mock.Setup(f => f.Description).Returns("Inner Filter");
        
        var filter = new NotFilter(mock.Object);
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Be("NOT (Inner Filter)");
    }
}
