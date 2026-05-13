using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Filters.Chain;
using FileSystemVisitorConsoleApp.Interfaces;
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
    
    /// <summary>
    /// Builds a filter chain from a list of filters using LINQ.
    /// </summary>
    private static IFileSystemFilter? BuildFilterChain(IEnumerable<ChainFilterBase> filters)
    {
        var filterList = filters.ToList();
        
        if (filterList.Count == 0)
            return null;
        
        // Link each filter to the next one using LINQ
        filterList.Zip(filterList.Skip(1), (current, next) => current.SetNext(next)).ToList();
        
        return filterList.First();
    }
    
    [Fact]
    public void Build_WithNoFilters_ReturnsNull()
    {
        // Arrange
        var filters = new List<ChainFilterBase>();
        
        // Act
        var filter = BuildFilterChain(filters);
        
        // Assert
        filter.Should().BeNull();
    }
    
    [Fact]
    public void Build_WithSingleFilter_ReturnsFilter()
    {
        // Arrange
        var filters = new List<ChainFilterBase>
        {
            new ExtensionChainFilter(".txt")
        };
        
        // Act
        var filter = BuildFilterChain(filters);
        
        // Assert
        filter.Should().NotBeNull();
        filter!.Description.Should().Contain("Extension=.txt");
    }
    
    [Fact]
    public void Build_WithMultipleFilters_CreatesChain()
    {
        // Arrange
        var filters = new List<ChainFilterBase>
        {
            new FileTypeChainFilter(filesOnly: true),
            new ExtensionChainFilter(".txt"),
            new SizeChainFilter(minSize: 100, maxSize: 1000)
        };
        
        // Act
        var filter = BuildFilterChain(filters);
        
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
        var filters = new List<ChainFilterBase>
        {
            new FileTypeChainFilter(filesOnly: true),
            new ExtensionChainFilter(".txt"),
            new SizeChainFilter(minSize: 50, maxSize: 200)
        };
        var filter = BuildFilterChain(filters);
        
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
        var filters = new List<ChainFilterBase>
        {
            new FileTypeChainFilter(filesOnly: true),
            new ExtensionChainFilter(".txt"),
            new SizeChainFilter(minSize: 200, maxSize: null)  // This will fail
        };
        var filter = BuildFilterChain(filters);
        
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
        var filters = new List<ChainFilterBase>
        {
            new ExtensionChainFilter(".txt"),
            new ModificationDateChainFilter(startDate: DateTime.Now.AddDays(-7), endDate: null)
        };
        var filter = BuildFilterChain(filters);
        
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
        var filters = new List<ChainFilterBase>
        {
            new FileTypeChainFilter(filesOnly: true),
            new ExtensionChainFilter(".txt"),
            new SizeChainFilter(minSize: 50, maxSize: null),
            new ModificationDateChainFilter(startDate: DateTime.Now.AddDays(-30), endDate: null)
        };
        var filter = BuildFilterChain(filters);
        
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
        var filters = new List<ChainFilterBase>
        {
            new FileTypeChainFilter(filesOnly: true),
            new ExtensionChainFilter(".txt")
        };
        var filter = BuildFilterChain(filters);
        
        var dir = FileSystemInfoHelper.CreateDirectoryInfo();
        _tempFiles.Add(dir);
        
        // Act
        var result = filter!.IsMatch(dir);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void BuildFilterChain_UsingLinqAggregate_WorksCorrectly()
    {
        // Arrange - Alternative LINQ approach using Aggregate
        var filters = new ChainFilterBase[]
        {
            new ExtensionChainFilter(".txt"),
            new SizeChainFilter(minSize: 50, maxSize: null)
        };
        
        // Build chain using Aggregate - simpler approach
        filters.Aggregate((current, next) =>
        {
            current.SetNext(next);
            return next;
        });
        
        var chainHead = filters.First();
        
        var file = FileSystemInfoHelper.CreateFileInfo("test.txt", length: 100);
        _tempFiles.Add(file);
        
        // Act
        var result = chainHead.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
}
