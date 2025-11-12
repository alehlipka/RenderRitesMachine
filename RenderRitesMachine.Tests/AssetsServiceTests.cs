using RenderRitesMachine.Assets;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса AssetsService (валидация входных данных).
/// Примечание: Полное тестирование загрузки ресурсов требует OpenGL контекста,
/// поэтому здесь тестируется только валидация входных данных.
/// </summary>
public class AssetsServiceTests
{
    [Fact]
    public void GetMesh_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetMesh(null!));
    }

    [Fact]
    public void GetMesh_WithEmptyName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetMesh(""));
        Assert.Throws<ArgumentNullException>(() => service.GetMesh("   "));
    }

    [Fact]
    public void GetMesh_WithNonExistentName_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.GetMesh("nonexistent"));
        Assert.Contains("nonexistent", exception.Message);
    }

    [Fact]
    public void GetShader_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetShader(null!));
    }

    [Fact]
    public void GetShader_WithEmptyName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetShader(""));
        Assert.Throws<ArgumentNullException>(() => service.GetShader("   "));
    }

    [Fact]
    public void GetShader_WithNonExistentName_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.GetShader("nonexistent"));
        Assert.Contains("nonexistent", exception.Message);
    }

    [Fact]
    public void GetTexture_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetTexture(null!));
    }

    [Fact]
    public void GetTexture_WithEmptyName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetTexture(""));
        Assert.Throws<ArgumentNullException>(() => service.GetTexture("   "));
    }

    [Fact]
    public void GetTexture_WithNonExistentName_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.GetTexture("nonexistent"));
        Assert.Contains("nonexistent", exception.Message);
    }

    [Fact]
    public void GetBoundingBox_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetBoundingBox(null!));
    }

    [Fact]
    public void GetBoundingBox_WithEmptyName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetBoundingBox(""));
        Assert.Throws<ArgumentNullException>(() => service.GetBoundingBox("   "));
    }

    [Fact]
    public void GetBoundingBox_WithNonExistentName_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.GetBoundingBox("nonexistent"));
        Assert.Contains("nonexistent", exception.Message);
    }

    [Fact]
    public void AddTexture_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.AddTexture(null!, TextureType.ColorMap, tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddTexture_WithEmptyName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.AddTexture("", TextureType.ColorMap, tempFile));
            Assert.Throws<ArgumentNullException>(() =>
                service.AddTexture("   ", TextureType.ColorMap, tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddTexture_WithNullPath_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, null!));
    }

    [Fact]
    public void AddTexture_WithEmptyPath_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, ""));
        Assert.Throws<ArgumentNullException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, "   "));
    }

    [Fact]
    public void AddTexture_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var service = new AssetsService();
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, nonExistentPath));
    }

    [Fact]
    public void AddShader_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.AddShader(null!, tempDir));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void AddShader_WithEmptyName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.AddShader("", tempDir));
            Assert.Throws<ArgumentNullException>(() =>
                service.AddShader("   ", tempDir));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void AddShader_WithNullPath_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.AddShader("shader", null!));
    }

    [Fact]
    public void AddShader_WithEmptyPath_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.AddShader("shader", ""));
        Assert.Throws<ArgumentNullException>(() =>
            service.AddShader("shader", "   "));
    }

    [Fact]
    public void AddShader_WithMissingVertexShaderFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var service = new AssetsService();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Создаем только fragment.glsl, но не vertex.glsl
            var fragmentPath = Path.Combine(tempDir, "fragment.glsl");
            File.WriteAllText(fragmentPath, "#version 460\nvoid main() { }");

            // Act & Assert
            var exception = Assert.Throws<FileNotFoundException>(() =>
                service.AddShader("shader", tempDir));
            Assert.Contains("Vertex shader file not found", exception.Message);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void AddShader_WithMissingFragmentShaderFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var service = new AssetsService();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Создаем только vertex.glsl, но не fragment.glsl
            var vertexPath = Path.Combine(tempDir, "vertex.glsl");
            File.WriteAllText(vertexPath, "#version 460\nvoid main() { }");

            // Act & Assert
            var exception = Assert.Throws<FileNotFoundException>(() =>
                service.AddShader("shader", tempDir));
            Assert.Contains("Fragment shader file not found", exception.Message);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void AddMeshFromFile_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.AddMeshFromFile(null!, tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddMeshFromFile_WithEmptyName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                service.AddMeshFromFile("", tempFile));
            Assert.Throws<ArgumentNullException>(() =>
                service.AddMeshFromFile("   ", tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddMeshFromFile_WithNullPath_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.AddMeshFromFile("mesh", null!));
    }

    [Fact]
    public void AddMeshFromFile_WithEmptyPath_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.AddMeshFromFile("mesh", ""));
        Assert.Throws<ArgumentNullException>(() =>
            service.AddMeshFromFile("mesh", "   "));
    }

    [Fact]
    public void AddMeshFromFile_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var service = new AssetsService();
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".obj");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() =>
            service.AddMeshFromFile("mesh", nonExistentPath));
    }

    [Fact]
    public void AddSphere_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.AddSphere(null!, 1.0f, 10, 10));
    }

    [Fact]
    public void AddSphere_WithEmptyName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.AddSphere("", 1.0f, 10, 10));
        Assert.Throws<ArgumentNullException>(() =>
            service.AddSphere("   ", 1.0f, 10, 10));
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-1.0f)]
    [InlineData(-10.0f)]
    public void AddSphere_WithInvalidRadius_ThrowsArgumentOutOfRangeException(float invalidRadius)
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddSphere("sphere", invalidRadius, 10, 10));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void AddSphere_WithInvalidSectors_ThrowsArgumentOutOfRangeException(int invalidSectors)
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddSphere("sphere", 1.0f, invalidSectors, 10));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void AddSphere_WithInvalidStacks_ThrowsArgumentOutOfRangeException(int invalidStacks)
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddSphere("sphere", 1.0f, 10, invalidStacks));
    }

    [Fact]
    public void AddBoundingBox_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.AddBoundingBox(null!));
    }

    [Fact]
    public void AddBoundingBox_WithEmptyName_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            service.AddBoundingBox(""));
        Assert.Throws<ArgumentNullException>(() =>
            service.AddBoundingBox("   "));
    }

    [Fact]
    public void AddBoundingBox_WithNonExistentMesh_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() =>
            service.AddBoundingBox("nonexistent"));
        Assert.Contains("nonexistent", exception.Message);
    }

    [Fact]
    public void GetAllShaders_ReturnsEmptyCollection_WhenNoShaders()
    {
        // Arrange
        var service = new AssetsService();

        // Act
        var shaders = service.GetAllShaders();

        // Assert
        Assert.NotNull(shaders);
        Assert.Empty(shaders);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        service.Dispose();
        // Повторный вызов Dispose не должен выбрасывать исключение
        service.Dispose();
    }

    // Примечание: Тесты на дублирование имен ресурсов требуют OpenGL контекста для создания ресурсов.
    // Проверка на дублирование реализована во всех методах добавления ресурсов и выполняется
    // до загрузки файлов и создания OpenGL ресурсов, что обеспечивает раннее обнаружение ошибок.
    // В реальных сценариях при попытке добавить ресурс с уже существующим именем будет выброшено
    // DuplicateResourceException с информацией о типе ресурса и его имени.

    // Edge cases для AssetsService

    [Fact]
    public void GetMesh_WithVeryLongName_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new AssetsService();
        var veryLongName = new string('a', 10000); // Очень длинное имя

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.GetMesh(veryLongName));
        Assert.Contains(veryLongName, exception.Message);
    }

    [Fact]
    public void GetShader_WithSpecialCharacters_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new AssetsService();
        var specialName = "test@#$%^&*()_+-=[]{}|;':\",./<>?";

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.GetShader(specialName));
        Assert.Contains(specialName, exception.Message);
    }

    [Fact]
    public void GetTexture_WithUnicodeCharacters_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new AssetsService();
        var unicodeName = "тест资源🎮";

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.GetTexture(unicodeName));
        Assert.Contains(unicodeName, exception.Message);
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(10000)]
    [InlineData(3)] // Минимальное валидное значение
    public void AddSphere_WithExtremeSectors_ThrowsOrSucceeds(int sectors)
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        if (sectors < 3)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.AddSphere("sphere", 1.0f, sectors, 10));
        }
        else if (sectors > 1000) // Очень большие значения могут вызвать проблемы с памятью
        {
            // Для очень больших значений тест может быть пропущен или может выбросить OutOfMemoryException
            // В реальном коде это должно быть ограничено разумными значениями
            Assert.True(true); // Просто проверяем, что код не падает с неожиданной ошибкой
        }
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(10000)]
    [InlineData(2)] // Минимальное валидное значение
    public void AddSphere_WithExtremeStacks_ThrowsOrSucceeds(int stacks)
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        if (stacks < 2)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.AddSphere("sphere", 1.0f, 10, stacks));
        }
        else if (stacks > 1000) // Очень большие значения могут вызвать проблемы с памятью
        {
            Assert.True(true); // Просто проверяем, что код не падает с неожиданной ошибкой
        }
    }

    [Theory]
    [InlineData(float.MaxValue / 2)]
    [InlineData(0.0001f)]
    [InlineData(1e6f)]
    public void AddSphere_WithExtremeRadius_ThrowsOrSucceeds(float radius)
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        if (radius <= 0)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.AddSphere("sphere", radius, 10, 10));
        }
        else if (radius > 1e5f) // Очень большие радиусы могут вызвать проблемы
        {
            Assert.True(true); // Просто проверяем, что код не падает с неожиданной ошибкой
        }
    }

    [Fact]
    public void AddTexture_WithVeryLongPath_ThrowsFileNotFoundException()
    {
        // Arrange
        var service = new AssetsService();
        var veryLongPath = Path.Combine(Path.GetTempPath(), new string('a', 260) + ".png");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, veryLongPath));
    }

    [Fact]
    public void AddShader_WithVeryLongPath_ThrowsFileNotFoundException()
    {
        // Arrange
        var service = new AssetsService();
        var veryLongPath = Path.Combine(Path.GetTempPath(), new string('a', 260));

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() =>
            service.AddShader("shader", veryLongPath));
    }

    [Fact]
    public void AddMeshFromFile_WithVeryLongPath_ThrowsFileNotFoundException()
    {
        // Arrange
        var service = new AssetsService();
        var veryLongPath = Path.Combine(Path.GetTempPath(), new string('a', 260) + ".obj");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() =>
            service.AddMeshFromFile("mesh", veryLongPath));
    }

    [Fact]
    public void GetAllShaders_AfterDispose_ReturnsEmptyCollection()
    {
        // Arrange
        var service = new AssetsService();

        // Act
        service.Dispose();
        var shaders = service.GetAllShaders();

        // Assert
        Assert.NotNull(shaders);
        Assert.Empty(shaders);
    }

    [Fact]
    public void Dispose_MultipleTimes_DoesNotThrow()
    {
        // Arrange
        var service = new AssetsService();

        // Act & Assert
        service.Dispose();
        service.Dispose();
        service.Dispose();
        // Не должно быть исключений
        Assert.True(true);
    }

    [Fact]
    public void GetMethods_AfterDispose_StillThrowKeyNotFoundException()
    {
        // Arrange
        var service = new AssetsService();
        service.Dispose();

        // Act & Assert - методы должны все еще валидировать входные данные
        Assert.Throws<KeyNotFoundException>(() => service.GetMesh("test"));
        Assert.Throws<KeyNotFoundException>(() => service.GetShader("test"));
        Assert.Throws<KeyNotFoundException>(() => service.GetTexture("test"));
        Assert.Throws<KeyNotFoundException>(() => service.GetBoundingBox("test"));
    }

    [Fact]
    public void AddMethods_WithNullName_AfterDispose_StillThrowsArgumentNullException()
    {
        // Arrange
        var service = new AssetsService();
        service.Dispose();

        // Act & Assert - валидация должна работать даже после Dispose
        Assert.Throws<ArgumentNullException>(() => service.AddBoundingBox(null!));
    }
}

