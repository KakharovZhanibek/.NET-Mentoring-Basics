using FileSystemVisitorConsoleApp.Factories;
using FileSystemVisitorConsoleApp.Filters;
using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;
using FileSystemVisitorConsoleApp.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace FileSystemVisitorConsoleApp.Tests.Factories;

public class FilterFactoryTests : IDisposable
{
    private readonly FilterFactory _factory;
    private readonly List<FileSystemInfo> _tempFiles = new();
    
    public FilterFactoryTests()
    {
        _factory = new FilterFactory();
    }
    
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
    public void CreateSizeFilter_ReturnsCorrectFilterType()
    {
        // Act
        var filter = _factory.CreateSizeFilter(1024, 2048);
        
        // Assert
        filter.Should().NotBeNull();
        filter.Should().BeOfType<SizeFilter>();
    }
    
    [Fact]
    public void CreateSizeFilter_CreatesWorkingFilter()
    {
        // Arrange
        var filter = _factory.CreateSizeFilter(1024, 2048);
        var file = CreateTestFile(length: 1500);
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void CreateExtensionFilter_ReturnsCorrectFilterType()
    {
        // Act
        var filter = _factory.CreateExtensionFilter(".txt", ".cs");
        
        // Assert
        filter.Should().NotBeNull();
        filter.Should().BeOfType<ExtensionFilter>();
    }
    
    [Fact]
    public void CreateExtensionFilter_CreatesWorkingFilter()
    {
        // Arrange
        var filter = _factory.CreateExtensionFilter(".txt", ".cs");
        var file = CreateTestFile(extension: ".txt");
        
        // Act
        var result = filter.IsMatch(file);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void CreateCreationDateFilter_ReturnsCorrectFilterType()
    {
        // Act
        var filter = _factory.CreateCreationDateFilter(DateTime.Now.AddDays(-7), DateTime.Now);
        
        // Assert
        filter.Should().NotBeNull();
        filter.Should().BeOfType<CreationDateFilter>();
    }
    
    [Fact]
    public void CreateModificationDateFilter_ReturnsCorrectFilterType()
    {
        // Act
        var filter = _factory.CreateModificationDateFilter(DateTime.Now.AddDays(-7), DateTime.Now);
        
        // Assert
        filter.Should().NotBeNull();
        filter.Should().BeOfType<ModificationDateFilter>();
    }
    
    [Fact]
    public void CreateFilesOnlyFilter_ReturnsCorrectFilterType()
    {
        // Act
        var filter = _factory.CreateFilesOnlyFilter();
        
        // Assert
        filter.Should().NotBeNull();
        filter.Should().BeOfType<FilesOnlyFilter>();
    }
    
    [Fact]
    public void CreateDirectoriesOnlyFilter_ReturnsCorrectFilterType()
    {
        // Act
        var filter = _factory.CreateDirectoriesOnlyFilter();
        
        // Assert
        filter.Should().NotBeNull();
        filter.Should().BeOfType<DirectoriesOnlyFilter>();
    }
    
    [Fact]
    public void CreateAndFilter_ReturnsCorrectFilterType()
    {
        // Arrange
        var mock1 = new Mock<IFileSystemFilter>();
        var mock2 = new Mock<IFileSystemFilter>();
        
        // Act
        var filter = _factory.CreateAndFilter(mock1.Object, mock2.Object);
        
        // Assert
        filter.Should().NotBeNull();
        filter.Should().BeOfType<AndFilter>();
    }
    
    [Fact]
    public void CreateAndFilter_CreatesWorkingCompositeFilter()
    {
        // Arrange
        var filter1 = _factory.CreateFilesOnlyFilter();
        var filter2 = _factory.CreateExtensionFilter(".txt");
        var compositeFilter = _factory.CreateAndFilter(filter1, filter2);
        
        var txtFile = CreateTestFile(extension: ".txt");
        var csFile = CreateTestFile(extension: ".cs");
        var directory = CreateTestDirectory();
        
        // Act & Assert
        compositeFilter.IsMatch(txtFile).Should().BeTrue("it's a .txt file");
        compositeFilter.IsMatch(csFile).Should().BeFalse("it's not a .txt file");
        compositeFilter.IsMatch(directory).Should().BeFalse("it's not a file");
    }
    
    [Fact]
    public void CreateOrFilter_ReturnsCorrectFilterType()
    {
        // Arrange
        var mock1 = new Mock<IFileSystemFilter>();
        var mock2 = new Mock<IFileSystemFilter>();
        
        // Act
        var filter = _factory.CreateOrFilter(mock1.Object, mock2.Object);
        
        // Assert
        filter.Should().NotBeNull();
        filter.Should().BeOfType<OrFilter>();
    }
    
    [Fact]
    public void CreateOrFilter_CreatesWorkingCompositeFilter()
    {
        // Arrange
        var filter1 = _factory.CreateExtensionFilter(".txt");
        var filter2 = _factory.CreateExtensionFilter(".cs");
        var compositeFilter = _factory.CreateOrFilter(filter1, filter2);
        
        var txtFile = CreateTestFile(extension: ".txt");
        var csFile = CreateTestFile(extension: ".cs");
        var pdfFile = CreateTestFile(extension: ".pdf");
        
        // Act & Assert
        compositeFilter.IsMatch(txtFile).Should().BeTrue("it's a .txt file");
        compositeFilter.IsMatch(csFile).Should().BeTrue("it's a .cs file");
        compositeFilter.IsMatch(pdfFile).Should().BeFalse("it's neither .txt nor .cs");
    }
    
    [Fact]
    public void CreateNotFilter_ReturnsCorrectFilterType()
    {
        // Arrange
        var mock = new Mock<IFileSystemFilter>();
        
        // Act
        var filter = _factory.CreateNotFilter(mock.Object);
        
        // Assert
        filter.Should().NotBeNull();
        filter.Should().BeOfType<NotFilter>();
    }
    
    [Fact]
    public void CreateNotFilter_CreatesWorkingInverseFilter()
    {
        // Arrange
        var innerFilter = _factory.CreateFilesOnlyFilter();
        var notFilter = _factory.CreateNotFilter(innerFilter);
        
        var file = CreateTestFile();
        var directory = CreateTestDirectory();
        
        // Act & Assert
        notFilter.IsMatch(file).Should().BeFalse("file should not pass (inverse of FilesOnly)");
        notFilter.IsMatch(directory).Should().BeTrue("directory should pass (inverse of FilesOnly)");
    }
    
    [Fact]
    public void ComplexFilterScenario_CombinesMultipleFilters()
    {
        // Arrange
        var sizeFilter = _factory.CreateSizeFilter(1024, 10240); // 1KB - 10KB
        var extensionFilter = _factory.CreateExtensionFilter(".txt", ".cs");
        var dateFilter = _factory.CreateCreationDateFilter(DateTime.Now.AddDays(-30), DateTime.Now);
        
        var combinedFilter = _factory.CreateAndFilter(sizeFilter, extensionFilter, dateFilter);
        
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
}
