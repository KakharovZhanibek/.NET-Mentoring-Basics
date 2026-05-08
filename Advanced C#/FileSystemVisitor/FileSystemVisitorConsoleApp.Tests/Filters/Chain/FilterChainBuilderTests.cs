using FileSystemVisitorConsoleApp.Filters.Chain;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;

namespace FileSystemVisitorConsoleApp.Tests.Filters.Chain;

public class FilterChainBuilderTests : IDisposable
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
    public void Build_WithNoFilters_ReturnsNull()
    {
        // Arrange
        var builder = new FilterChainBuilder();
        
        // Act
        var filter = builder.Build();
        
        // Assert
        filter.Should().BeNull();
    }
    
    [Fact]
    public void Build_WithSingleFilter_ReturnsFilter()
    {
        // Arrange
        var builder = new FilterChainBuilder();
        
        // Act
        var filter = builder
            .WithExtension(".txt")
            .Build();
        
        // Assert
        filter.Should().NotBeNull();
        filter!.Description.Should().Contain("Extension=.txt");
    }
    
    [Fact]
    public void Build_WithMultipleFilters_CreatesChain()
    {
        // Arrange
        var builder = new FilterChainBuilder();
        
        // Act
        var filter = builder
            .WithFileType(filesOnly: true)
            .WithExtension(".txt")
            .WithSize(minSize: 100, maxSize: 1000)
            .Build();
        
        // Assert
        filter.Should().NotBeNull();
        var description = filter!.Description;
        description.Should().Contain("Type:File");
        description.Should().Contain("Extension=.txt");
        description.Should().Contain("Size:");
        description.Should().Contain("?");
    }
    
    [Fact]
    public void FluentChain_AllFiltersPass_ReturnsTrue()
    {
        // Arrange
        var filter = new FilterChainBuilder()
            .WithFileType(filesOnly: true)
            .WithExtension(".txt")
            .WithSize(minSize: 50, maxSize: 200)
            .Build();
        
        var file = FileSystemInfoHelper.CreateFileInfo("test.txt", length: 100);
        _tempFiles.Add(file);
        
        // Act
        var result = filter!.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void FluentChain_OneFilterFails_ReturnsFalse()
    {
        // Arrange
        var filter = new FilterChainBuilder()
            .WithFileType(filesOnly: true)
            .WithExtension(".txt")
            .WithSize(minSize: 200, maxSize: null)  // This will fail
            .Build();
        
        var file = FileSystemInfoHelper.CreateFileInfo("test.txt", length: 100);
        _tempFiles.Add(file);
        
        // Act
        var result = filter!.IsMatch(file);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void FluentChain_WithModificationDate_Works()
    {
        // Arrange
        var filter = new FilterChainBuilder()
            .WithExtension(".txt")
            .WithModificationDate(startDate: DateTime.Now.AddDays(-7), endDate: null)
            .Build();
        
        var file = FileSystemInfoHelper.CreateFileInfo("test.txt");
        _tempFiles.Add(file);
        
        // Act
        var result = filter!.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void FluentChain_ComplexScenario_WorksCorrectly()
    {
        // Arrange - Find text files larger than 50 bytes, modified in last 30 days
        var filter = new FilterChainBuilder()
            .WithFileType(filesOnly: true)
            .WithExtension(".txt")
            .WithSize(minSize: 50, maxSize: null)
            .WithModificationDate(startDate: DateTime.Now.AddDays(-30), endDate: null)
            .Build();
        
        var file = FileSystemInfoHelper.CreateFileInfo("document.txt", length: 100);
        _tempFiles.Add(file);
        
        // Act
        var result = filter!.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void FluentChain_RejectsDirectory_WhenFilesOnly()
    {
        // Arrange
        var filter = new FilterChainBuilder()
            .WithFileType(filesOnly: true)
            .WithExtension(".txt")
            .Build();
        
        var dir = FileSystemInfoHelper.CreateDirectoryInfo();
        _tempFiles.Add(dir);
        
        // Act
        var result = filter!.IsMatch(dir);
        
        // Assert
        result.Should().BeFalse();
    }
}
