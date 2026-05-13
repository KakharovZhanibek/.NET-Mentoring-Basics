using FileSystemVisitorConsoleApp.Filters;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;

namespace FileSystemVisitorConsoleApp.Tests.Filters;

public class ExtensionFilterTests : IDisposable
{
    private readonly List<FileSystemInfo> _tempFiles = new();
    
    public void Dispose()
    {
        foreach (var file in _tempFiles)
        {
            FileSystemInfoHelper.CleanupTempFile(file);
        }
    }
    
    private FileInfo CreateTestFile(string extension = ".txt")
    {
        var file = FileSystemInfoHelper.CreateFileInfo(
            name: $"test{Guid.NewGuid():N}.{extension.TrimStart('.')}",

extension: extension);
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
    public void Constructor_WithNullExtensions_ThrowsArgumentException()
    {
        // Arrange & Act
        Action act = () => new ExtensionFilter(null!);
        
        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("extensions");
    }
    
    [Fact]
    public void Constructor_WithEmptyExtensions_ThrowsArgumentException()
    {
        // Arrange & Act
        Action act = () => new ExtensionFilter();
        
        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("extensions");
    }
    
    [Theory]
    [InlineData(".txt", true)]
    [InlineData(".cs", false)]
    [InlineData(".pdf", false)]
    public void IsMatch_WithSingleExtension_ReturnsCorrectResult(string fileExtension, bool expected)
    {
        // Arrange
        var filter = new ExtensionFilter(".txt");
        var file = CreateTestFile(fileExtension);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void IsMatch_WithMultipleExtensions_MatchesAnyExtension()
    {
        // Arrange
        var filter = new ExtensionFilter(".txt", ".cs", ".pdf");
        
        // Act & Assert
        var txtFile = CreateTestFile(".txt");
        filter.IsMatch(txtFile).Should().BeTrue();
        
        var csFile = CreateTestFile(".cs");
        filter.IsMatch(csFile).Should().BeTrue();
        
        var xmlFile = CreateTestFile(".xml");
        filter.IsMatch(xmlFile).Should().BeFalse();
    }
    
    [Fact]
    public void IsMatch_WithExtensionsWithoutDot_NormalizesProperly()
    {
        // Arrange
        var filter = new ExtensionFilter("txt", "cs");
        var file = CreateTestFile(".txt");
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsMatch_IsCaseInsensitive()
    {
        // Arrange
        var filter = new ExtensionFilter(".TXT");
        var file = CreateTestFile(".txt");
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsMatch_WithDirectoryInfo_ReturnsFalse()
    {
        // Arrange
        var filter = new ExtensionFilter(".txt");
        var directory = CreateTestDirectory();
        
        // Act
        var result = filter.IsMatch(directory);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void Description_IncludesAllExtensions()
    {
        // Arrange
        var filter = new ExtensionFilter(".txt", ".cs", ".pdf");
        
        // Act
        var description = filter.Description;
        
        // Assert
        description.Should().Contain(".txt");
        description.Should().Contain(".cs");
        description.Should().Contain(".pdf");
        description.Should().StartWith("Extension:");
    }
}
