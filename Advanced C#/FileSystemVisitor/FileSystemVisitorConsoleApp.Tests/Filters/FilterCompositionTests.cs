using FileSystemVisitorConsoleApp.Filters;
using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace FileSystemVisitorConsoleApp.Tests.Filters;

/// <summary>
/// Tests for filter composition and integration scenarios.
/// Tests that multiple filters can be combined using AndFilter, OrFilter, and NotFilter
/// to create complex filtering logic.
/// </summary>
public class FilterCompositionTests : IDisposable
{
    private readonly List<FileSystemInfo> _tempFiles = new();
    
    public void Dispose()
    {
        foreach (var file in _tempFiles)
        {
            FileSystemInfoHelper.CleanupTempFile(file);
        }
    }
    
    private FileInfo CreateTestFile(long length = 1024, string extension = ".txt", DateTime? creationTime = null)
    {
        var file = FileSystemInfoHelper.CreateFileInfo(
            name: $"test{Guid.NewGuid():N}.{extension.TrimStart('.')}",

length: length,
            extension: extension,
            creationTime: creationTime);
        _tempFiles.Add(file);
        return file;
    }
    
    private DirectoryInfo CreateTestDirectory()
    {
        var dir = FileSystemInfoHelper.CreateDirectoryInfo();
        _tempFiles.Add(dir);
        return dir;
    }
    
    [Fact]
    public void AndFilter_WithTwoSimpleFilters_CombinesCorrectly()
    {
        // Arrange
        var filter1 = new FilesOnlyFilter();
        var filter2 = new ExtensionFilter(".txt");
        var compositeFilter = new AndFilter(filter1, filter2);
        
        var txtFile = CreateTestFile(extension: ".txt");
        var csFile = CreateTestFile(extension: ".cs");
        var directory = CreateTestDirectory();
        
        // Act & Assert
        compositeFilter.IsMatch(txtFile).Should().BeTrue("it's a .txt file");
        compositeFilter.IsMatch(csFile).Should().BeFalse("it's not a .txt file");
        compositeFilter.IsMatch(directory).Should().BeFalse("it's not a file");
    }
    
    [Fact]
    public void OrFilter_WithTwoExtensionFilters_MatchesEither()
    {
        // Arrange
        var filter1 = new ExtensionFilter(".txt");
        var filter2 = new ExtensionFilter(".cs");
        var compositeFilter = new OrFilter(filter1, filter2);
        
        var txtFile = CreateTestFile(extension: ".txt");
        var csFile = CreateTestFile(extension: ".cs");
        var pdfFile = CreateTestFile(extension: ".pdf");
        
        // Act & Assert
        compositeFilter.IsMatch(txtFile).Should().BeTrue("it's a .txt file");
        compositeFilter.IsMatch(csFile).Should().BeTrue("it's a .cs file");
        compositeFilter.IsMatch(pdfFile).Should().BeFalse("it's neither .txt nor .cs");
    }
    
    [Fact]
    public void NotFilter_InvertsFilterLogic()
    {
        // Arrange
        var innerFilter = new FilesOnlyFilter();
        var notFilter = new NotFilter(innerFilter);
        
        var file = CreateTestFile();
        var directory = CreateTestDirectory();
        
        // Act & Assert
        notFilter.IsMatch(file).Should().BeFalse("file should not pass (inverse of FilesOnly)");
        notFilter.IsMatch(directory).Should().BeTrue("directory should pass (inverse of FilesOnly)");
    }
    
    [Fact]
    public void ComplexScenario_CombinesMultipleFilterTypes()
    {
        // Arrange
        var sizeFilter = new SizeFilter(1024, 10240); // 1KB - 10KB
        var extensionFilter = new ExtensionFilter(".txt", ".cs");
        var dateFilter = new CreationDateFilter(DateTime.Now.AddDays(-30), DateTime.Now);
        
        var combinedFilter = new AndFilter(sizeFilter, extensionFilter, dateFilter);
        
        var validFile = CreateTestFile(
            extension: ".txt",
            length: 5120,
            creationTime: DateTime.Now.AddDays(-15));
        
        var invalidFile = CreateTestFile(
            extension: ".pdf",
            length: 5120,
            creationTime: DateTime.Now.AddDays(-15));
        
        // Act & Assert
        combinedFilter.IsMatch(validFile).Should().BeTrue("meets all criteria");
        combinedFilter.IsMatch(invalidFile).Should().BeFalse("wrong extension");
    }
    
    [Fact]
    public void NestedComposition_AndOfOrFilters_WorksCorrectly()
    {
        // Arrange - (txt OR cs) AND (size between 1KB-10KB)
        var extensionFilter = new OrFilter(
            new ExtensionFilter(".txt"),
            new ExtensionFilter(".cs")
        );
        var sizeFilter = new SizeFilter(1024, 10240);
        var combinedFilter = new AndFilter(extensionFilter, sizeFilter);
        
        var validTxtFile = CreateTestFile(extension: ".txt", length: 5000);
        var validCsFile = CreateTestFile(extension: ".cs", length: 5000);
        var invalidExtFile = CreateTestFile(extension: ".pdf", length: 5000);
        var invalidSizeFile = CreateTestFile(extension: ".txt", length: 500);
        
        // Act & Assert
        combinedFilter.IsMatch(validTxtFile).Should().BeTrue();
        combinedFilter.IsMatch(validCsFile).Should().BeTrue();
        combinedFilter.IsMatch(invalidExtFile).Should().BeFalse("wrong extension");
        combinedFilter.IsMatch(invalidSizeFile).Should().BeFalse("too small");
    }
    
    [Fact]
    public void NestedComposition_NotOfAndFilter_WorksCorrectly()
    {
        // Arrange - NOT (txt files AND large files) = exclude large txt files
        var andFilter = new AndFilter(
            new ExtensionFilter(".txt"),
            new SizeFilter(minSize: 10240, maxSize: null) // > 10KB
        );
        var notFilter = new NotFilter(andFilter);
        
        var largeTxtFile = CreateTestFile(extension: ".txt", length: 20000);
        var smallTxtFile = CreateTestFile(extension: ".txt", length: 5000);
        var largeCsFile = CreateTestFile(extension: ".cs", length: 20000);
        
        // Act & Assert
        notFilter.IsMatch(largeTxtFile).Should().BeFalse("large .txt should be excluded");
        notFilter.IsMatch(smallTxtFile).Should().BeTrue("small .txt is OK");
        notFilter.IsMatch(largeCsFile).Should().BeTrue(".cs is OK even if large");
    }
    
    [Fact]
    public void ThreeWayAnd_AllConditionsMustMatch()
    {
        // Arrange
        var filter = new AndFilter(
            new FilesOnlyFilter(),
            new ExtensionFilter(".txt"),
            new SizeFilter(1024, 10240)
        );
        
        var validFile = CreateTestFile(extension: ".txt", length: 5000);
        var wrongExtFile = CreateTestFile(extension: ".cs", length: 5000);
        var tooSmallFile = CreateTestFile(extension: ".txt", length: 500);
        var directory = CreateTestDirectory();
        
        // Act & Assert
        filter.IsMatch(validFile).Should().BeTrue("meets all conditions");
        filter.IsMatch(wrongExtFile).Should().BeFalse("wrong extension");
        filter.IsMatch(tooSmallFile).Should().BeFalse("too small");
        filter.IsMatch(directory).Should().BeFalse("not a file");
    }
    
    [Fact]
    public void ThreeWayOr_AnyConditionCanMatch()
    {
        // Arrange
        var filter = new OrFilter(
            new ExtensionFilter(".txt"),
            new ExtensionFilter(".cs"),
            new SizeFilter(minSize: 10240, maxSize: null) // > 10KB
        );
        
        var txtFile = CreateTestFile(extension: ".txt", length: 1000);
        var csFile = CreateTestFile(extension: ".cs", length: 1000);
        var largeFile = CreateTestFile(extension: ".pdf", length: 20000);
        var noMatchFile = CreateTestFile(extension: ".pdf", length: 1000);
        
        // Act & Assert
        filter.IsMatch(txtFile).Should().BeTrue(".txt matches");
        filter.IsMatch(csFile).Should().BeTrue(".cs matches");
        filter.IsMatch(largeFile).Should().BeTrue("large file matches");
        filter.IsMatch(noMatchFile).Should().BeFalse("no conditions match");
    }
    
    [Fact]
    public void EmptyAndFilter_MatchesEverything()
    {
        // Arrange
        var filter = new AndFilter();
        var file = CreateTestFile();
        var directory = CreateTestDirectory();
        
        // Act & Assert
        filter.IsMatch(file).Should().BeTrue("empty AND matches everything");
        filter.IsMatch(directory).Should().BeTrue("empty AND matches everything");
    }
    
    [Fact]
    public void RealWorldScenario_RecentSourceCodeFiles()
    {
        // Arrange - Recent C# source files that aren't too large
        var filter = new AndFilter(
            new ExtensionFilter(".cs"),
            new FilesOnlyFilter(),
            new SizeFilter(minSize: null, maxSize: 500 * 1024), // < 500KB
            new ModificationDateFilter(DateTime.Now.AddDays(-7), null) // Last 7 days
        );
        
        var validFile = CreateTestFile(
            extension: ".cs",
            length: 100 * 1024);
        
        var tooLargeFile = CreateTestFile(
            extension: ".cs",
            length: 600 * 1024);
        
        var wrongExtFile = CreateTestFile(
            extension: ".txt",
            length: 100 * 1024);
        
        // Act & Assert
        filter.IsMatch(validFile).Should().BeTrue("meets all criteria");
        filter.IsMatch(tooLargeFile).Should().BeFalse("too large");
        filter.IsMatch(wrongExtFile).Should().BeFalse("wrong extension");
    }
}
