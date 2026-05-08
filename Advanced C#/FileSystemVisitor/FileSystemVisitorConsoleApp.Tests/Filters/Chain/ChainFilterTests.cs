using FileSystemVisitorConsoleApp.Filters.Chain;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;

namespace FileSystemVisitorConsoleApp.Tests.Filters.Chain;

public class ChainFilterTests : IDisposable
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
    public void ExtensionChainFilter_MatchingExtension_ReturnsTrue()
    {
        // Arrange
        var filter = new ExtensionChainFilter(".txt");
        var file = FileSystemInfoHelper.CreateFileInfo(extension: ".txt");
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ExtensionChainFilter_NonMatchingExtension_ReturnsFalse()
    {
        // Arrange
        var filter = new ExtensionChainFilter(".txt");
        var file = FileSystemInfoHelper.CreateFileInfo(extension: ".cs");
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void ExtensionChainFilter_WithoutLeadingDot_Works()
    {
        // Arrange
        var filter = new ExtensionChainFilter("txt");
        var file = FileSystemInfoHelper.CreateFileInfo(extension: ".txt");
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ExtensionChainFilter_WithNull_ThrowsArgumentException()
    {
        // Arrange & Act
        Action act = () => new ExtensionChainFilter(null!);
        
        // Assert
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void SizeChainFilter_FileMeetsMinimumSize_ReturnsTrue()
    {
        // Arrange
        var filter = new SizeChainFilter(minSize: 50, maxSize: null);
        var file = FileSystemInfoHelper.CreateFileInfo(length: 100);
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void SizeChainFilter_FileBelowMinimumSize_ReturnsFalse()
    {
        // Arrange
        var filter = new SizeChainFilter(minSize: 200, maxSize: null);
        var file = FileSystemInfoHelper.CreateFileInfo(length: 100);
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void SizeChainFilter_FileExceedsMaximumSize_ReturnsFalse()
    {
        // Arrange
        var filter = new SizeChainFilter(minSize: null, maxSize: 50);
        var file = FileSystemInfoHelper.CreateFileInfo(length: 100);
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void SizeChainFilter_MinGreaterThanMax_ThrowsArgumentException()
    {
        // Arrange & Act
        Action act = () => new SizeChainFilter(minSize: 100, maxSize: 50);
        
        // Assert
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void ModificationDateChainFilter_FileWithinDateRange_ReturnsTrue()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-7);
        var endDate = DateTime.Now.AddDays(1);
        var filter = new ModificationDateChainFilter(startDate, endDate);
        var file = FileSystemInfoHelper.CreateFileInfo();
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ModificationDateChainFilter_FileBeforeStartDate_ReturnsFalse()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(1);
        var filter = new ModificationDateChainFilter(startDate, null);
        var file = FileSystemInfoHelper.CreateFileInfo();
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void ModificationDateChainFilter_StartDateAfterEndDate_ThrowsArgumentException()
    {
        // Arrange & Act
        var startDate = DateTime.Now.AddDays(7);
        var endDate = DateTime.Now.AddDays(-7);
        Action act = () => new ModificationDateChainFilter(startDate, endDate);
        
        // Assert
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void FileTypeChainFilter_FilesOnly_AcceptsFile()
    {
        // Arrange
        var filter = new FileTypeChainFilter(filesOnly: true);
        var file = FileSystemInfoHelper.CreateFileInfo();
        _tempFiles.Add(file);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void FileTypeChainFilter_FilesOnly_RejectsDirectory()
    {
        // Arrange
        var filter = new FileTypeChainFilter(filesOnly: true);
        var dir = FileSystemInfoHelper.CreateDirectoryInfo();
        _tempFiles.Add(dir);
        
        // Act
        var result = filter.IsMatch(dir);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void FileTypeChainFilter_DirectoriesOnly_AcceptsDirectory()
    {
        // Arrange
        var filter = new FileTypeChainFilter(filesOnly: false);
        var dir = FileSystemInfoHelper.CreateDirectoryInfo();
        _tempFiles.Add(dir);
        
        // Act
        var result = filter.IsMatch(dir);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ChainFilter_TwoFiltersInChain_BothMustPass()
    {
        // Arrange
        var extensionFilter = new ExtensionChainFilter(".txt");
        var sizeFilter = new SizeChainFilter(minSize: 50, maxSize: null);
        extensionFilter.SetNext(sizeFilter);
        
        var file = FileSystemInfoHelper.CreateFileInfo(extension: ".txt", length: 100);
        _tempFiles.Add(file);
        
        // Act
        var result = extensionFilter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ChainFilter_FirstFilterFails_ReturnsFalseWithoutCheckingSecond()
    {
        // Arrange
        var extensionFilter = new ExtensionChainFilter(".txt");
        var sizeFilter = new SizeChainFilter(minSize: 50, maxSize: null);
        extensionFilter.SetNext(sizeFilter);
        
        var file = FileSystemInfoHelper.CreateFileInfo(extension: ".cs", length: 100);
        _tempFiles.Add(file);
        
        // Act
        var result = extensionFilter.IsMatch(file);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void ChainFilter_SecondFilterFails_ReturnsFalse()
    {
        // Arrange
        var extensionFilter = new ExtensionChainFilter(".txt");
        var sizeFilter = new SizeChainFilter(minSize: 200, maxSize: null);
        extensionFilter.SetNext(sizeFilter);
        
        var file = FileSystemInfoHelper.CreateFileInfo(extension: ".txt", length: 100);
        _tempFiles.Add(file);
        
        // Act
        var result = extensionFilter.IsMatch(file);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void ChainFilter_ThreeFiltersInChain_AllMustPass()
    {
        // Arrange
        var typeFilter = new FileTypeChainFilter(filesOnly: true);
        var extensionFilter = new ExtensionChainFilter(".txt");
        var sizeFilter = new SizeChainFilter(minSize: 50, maxSize: 200);
        
        typeFilter.SetNext(extensionFilter);
        extensionFilter.SetNext(sizeFilter);
        
        var file = FileSystemInfoHelper.CreateFileInfo(extension: ".txt", length: 100);
        _tempFiles.Add(file);
        
        // Act
        var result = typeFilter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ChainFilter_Description_ShowsChain()
    {
        // Arrange
        var extensionFilter = new ExtensionChainFilter(".txt");
        var sizeFilter = new SizeChainFilter(minSize: 1024, maxSize: null);
        extensionFilter.SetNext(sizeFilter);
        
        // Act
        var description = extensionFilter.Description;
        
        // Assert
        description.Should().Contain("Extension=.txt");
        description.Should().Contain("?");
        description.Should().Contain("Size:");
    }
}
