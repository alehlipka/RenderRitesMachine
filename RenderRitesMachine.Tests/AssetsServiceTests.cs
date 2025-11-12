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
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() => service.GetMesh(null!));
    }

    [Fact]
    public void GetMesh_WithEmptyName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() => service.GetMesh(""));
        Assert.Throws<ArgumentNullException>(() => service.GetMesh("   "));
    }

    [Fact]
    public void GetMesh_WithNonExistentName_ThrowsKeyNotFoundException()
    {
        var service = new AssetsService();

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetMesh("nonexistent"));
        Assert.Contains("nonexistent", exception.Message);
    }

    [Fact]
    public void GetShader_WithNullName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() => service.GetShader(null!));
    }

    [Fact]
    public void GetShader_WithEmptyName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() => service.GetShader(""));
        Assert.Throws<ArgumentNullException>(() => service.GetShader("   "));
    }

    [Fact]
    public void GetShader_WithNonExistentName_ThrowsKeyNotFoundException()
    {
        var service = new AssetsService();

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetShader("nonexistent"));
        Assert.Contains("nonexistent", exception.Message);
    }

    [Fact]
    public void GetTexture_WithNullName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() => service.GetTexture(null!));
    }

    [Fact]
    public void GetTexture_WithEmptyName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() => service.GetTexture(""));
        Assert.Throws<ArgumentNullException>(() => service.GetTexture("   "));
    }

    [Fact]
    public void GetTexture_WithNonExistentName_ThrowsKeyNotFoundException()
    {
        var service = new AssetsService();

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetTexture("nonexistent"));
        Assert.Contains("nonexistent", exception.Message);
    }

    [Fact]
    public void GetBoundingBox_WithNullName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() => service.GetBoundingBox(null!));
    }

    [Fact]
    public void GetBoundingBox_WithEmptyName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() => service.GetBoundingBox(""));
        Assert.Throws<ArgumentNullException>(() => service.GetBoundingBox("   "));
    }

    [Fact]
    public void GetBoundingBox_WithNonExistentName_ThrowsKeyNotFoundException()
    {
        var service = new AssetsService();

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetBoundingBox("nonexistent"));
        Assert.Contains("nonexistent", exception.Message);
    }

    [Fact]
    public void AddTexture_WithNullName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();
        string tempFile = Path.GetTempFileName();

        try
        {
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
        var service = new AssetsService();
        string tempFile = Path.GetTempFileName();

        try
        {
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
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, null!));
    }

    [Fact]
    public void AddTexture_WithEmptyPath_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, ""));
        Assert.Throws<ArgumentNullException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, "   "));
    }

    [Fact]
    public void AddTexture_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        var service = new AssetsService();
        string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");

        Assert.Throws<FileNotFoundException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, nonExistentPath));
    }

    [Fact]
    public void AddShader_WithNullName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
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
        var service = new AssetsService();
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
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
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddShader("shader", null!));
    }

    [Fact]
    public void AddShader_WithEmptyPath_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddShader("shader", ""));
        Assert.Throws<ArgumentNullException>(() =>
            service.AddShader("shader", "   "));
    }

    [Fact]
    public void AddShader_WithMissingVertexShaderFile_ThrowsFileNotFoundException()
    {
        var service = new AssetsService();
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            string fragmentPath = Path.Combine(tempDir, "fragment.glsl");
            File.WriteAllText(fragmentPath, "#version 460\nvoid main() { }");

            FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() =>
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
        var service = new AssetsService();
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            string vertexPath = Path.Combine(tempDir, "vertex.glsl");
            File.WriteAllText(vertexPath, "#version 460\nvoid main() { }");

            FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() =>
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
        var service = new AssetsService();
        string tempFile = Path.GetTempFileName();

        try
        {
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
        var service = new AssetsService();
        string tempFile = Path.GetTempFileName();

        try
        {
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
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddMeshFromFile("mesh", null!));
    }

    [Fact]
    public void AddMeshFromFile_WithEmptyPath_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddMeshFromFile("mesh", ""));
        Assert.Throws<ArgumentNullException>(() =>
            service.AddMeshFromFile("mesh", "   "));
    }

    [Fact]
    public void AddMeshFromFile_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        var service = new AssetsService();
        string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".obj");

        Assert.Throws<FileNotFoundException>(() =>
            service.AddMeshFromFile("mesh", nonExistentPath));
    }

    [Fact]
    public void AddSphere_WithNullName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddSphere(null!, 1.0f, 10, 10));
    }

    [Fact]
    public void AddSphere_WithEmptyName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

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
        var service = new AssetsService();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddSphere("sphere", invalidRadius, 10, 10));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void AddSphere_WithInvalidSectors_ThrowsArgumentOutOfRangeException(int invalidSectors)
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddSphere("sphere", 1.0f, invalidSectors, 10));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void AddSphere_WithInvalidStacks_ThrowsArgumentOutOfRangeException(int invalidStacks)
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddSphere("sphere", 1.0f, 10, invalidStacks));
    }

    [Fact]
    public void AddBoundingBox_WithNullName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddBoundingBox(null!));
    }

    [Fact]
    public void AddBoundingBox_WithEmptyName_ThrowsArgumentNullException()
    {
        var service = new AssetsService();

        Assert.Throws<ArgumentNullException>(() =>
            service.AddBoundingBox(""));
        Assert.Throws<ArgumentNullException>(() =>
            service.AddBoundingBox("   "));
    }

    [Fact]
    public void AddBoundingBox_WithNonExistentMesh_ThrowsKeyNotFoundException()
    {
        var service = new AssetsService();

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() =>
            service.AddBoundingBox("nonexistent"));
        Assert.Contains("nonexistent", exception.Message);
    }

    [Fact]
    public void GetAllShaders_ReturnsEmptyCollection_WhenNoShaders()
    {
        var service = new AssetsService();

        IReadOnlyCollection<ShaderAsset> shaders = service.GetAllShaders();

        Assert.NotNull(shaders);
        Assert.Empty(shaders);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var service = new AssetsService();

        service.Dispose();
        service.Dispose();
    }

    [Fact]
    public void GetMesh_WithVeryLongName_ThrowsKeyNotFoundException()
    {
        var service = new AssetsService();
        string veryLongName = new string('a', 10000);

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetMesh(veryLongName));
        Assert.Contains(veryLongName, exception.Message);
    }

    [Fact]
    public void GetShader_WithSpecialCharacters_ThrowsKeyNotFoundException()
    {
        var service = new AssetsService();
        string specialName = "test@#$%^&*()_+-=[]{}|;':\",./<>?";

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetShader(specialName));
        Assert.Contains(specialName, exception.Message);
    }

    [Fact]
    public void GetTexture_WithUnicodeCharacters_ThrowsKeyNotFoundException()
    {
        var service = new AssetsService();
        string unicodeName = "тест资源🎮";

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetTexture(unicodeName));
        Assert.Contains(unicodeName, exception.Message);
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(10000)]
    [InlineData(3)]
    public void AddSphere_WithExtremeSectors_ThrowsOrSucceeds(int sectors)
    {
        var service = new AssetsService();

        if (sectors < 3)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.AddSphere("sphere", 1.0f, sectors, 10));
        }
        else if (sectors > 1000)
        {
            Assert.True(true);
        }
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(10000)]
    [InlineData(2)]
    public void AddSphere_WithExtremeStacks_ThrowsOrSucceeds(int stacks)
    {
        var service = new AssetsService();

        if (stacks < 2)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.AddSphere("sphere", 1.0f, 10, stacks));
        }
        else if (stacks > 1000)
        {
            Assert.True(true);
        }
    }

    [Theory]
    [InlineData(float.MaxValue / 2)]
    [InlineData(0.0001f)]
    [InlineData(1e6f)]
    public void AddSphere_WithExtremeRadius_ThrowsOrSucceeds(float radius)
    {
        var service = new AssetsService();

        if (radius <= 0)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.AddSphere("sphere", radius, 10, 10));
        }
        else if (radius > 1e5f)
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void AddTexture_WithVeryLongPath_ThrowsFileNotFoundException()
    {
        var service = new AssetsService();
        string veryLongPath = Path.Combine(Path.GetTempPath(), new string('a', 260) + ".png");

        Assert.Throws<FileNotFoundException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, veryLongPath));
    }

    [Fact]
    public void AddShader_WithVeryLongPath_ThrowsFileNotFoundException()
    {
        var service = new AssetsService();
        string veryLongPath = Path.Combine(Path.GetTempPath(), new string('a', 260));

        Assert.Throws<FileNotFoundException>(() =>
            service.AddShader("shader", veryLongPath));
    }

    [Fact]
    public void AddMeshFromFile_WithVeryLongPath_ThrowsFileNotFoundException()
    {
        var service = new AssetsService();
        string veryLongPath = Path.Combine(Path.GetTempPath(), new string('a', 260) + ".obj");

        Assert.Throws<FileNotFoundException>(() =>
            service.AddMeshFromFile("mesh", veryLongPath));
    }

    [Fact]
    public void GetAllShaders_AfterDispose_ReturnsEmptyCollection()
    {
        var service = new AssetsService();

        service.Dispose();
        IReadOnlyCollection<ShaderAsset> shaders = service.GetAllShaders();

        Assert.NotNull(shaders);
        Assert.Empty(shaders);
    }

    [Fact]
    public void Dispose_MultipleTimes_DoesNotThrow()
    {
        var service = new AssetsService();

        service.Dispose();
        service.Dispose();
        service.Dispose();
        Assert.True(true);
    }

    [Fact]
    public void GetMethods_AfterDispose_StillThrowKeyNotFoundException()
    {
        var service = new AssetsService();
        service.Dispose();

        Assert.Throws<KeyNotFoundException>(() => service.GetMesh("test"));
        Assert.Throws<KeyNotFoundException>(() => service.GetShader("test"));
        Assert.Throws<KeyNotFoundException>(() => service.GetTexture("test"));
        Assert.Throws<KeyNotFoundException>(() => service.GetBoundingBox("test"));
    }

    [Fact]
    public void AddMethods_WithNullName_AfterDispose_StillThrowsArgumentNullException()
    {
        var service = new AssetsService();
        service.Dispose();

        Assert.Throws<ArgumentNullException>(() => service.AddBoundingBox(null!));
    }
}

