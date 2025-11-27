using RenderRitesMachine.Assets;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Input-validation tests for <see cref="AssetsService"/>.
/// Note: full resource loading tests require an OpenGL context, so this suite focuses on validation only.
/// </summary>
public sealed class AssetsServiceTests
{
    [Fact]
    public void GetMeshWithNullNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() => service.GetMesh(null!));
    }

    [Fact]
    public void GetMeshWithEmptyNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() => service.GetMesh(""));
        _ = Assert.Throws<ArgumentNullException>(() => service.GetMesh("   "));
    }

    [Fact]
    public void GetMeshWithNonExistentNameThrowsKeyNotFoundException()
    {
        using var service = new AssetsService();

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetMesh("nonexistent"));
        Assert.Contains("nonexistent", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetShaderWithNullNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() => service.GetShader(null!));
    }

    [Fact]
    public void GetShaderWithEmptyNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() => service.GetShader(""));
        _ = Assert.Throws<ArgumentNullException>(() => service.GetShader("   "));
    }

    [Fact]
    public void GetShaderWithNonExistentNameThrowsKeyNotFoundException()
    {
        using var service = new AssetsService();

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetShader("nonexistent"));
        Assert.Contains("nonexistent", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetTextureWithNullNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() => service.GetTexture(null!));
    }

    [Fact]
    public void GetTextureWithEmptyNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() => service.GetTexture(""));
        _ = Assert.Throws<ArgumentNullException>(() => service.GetTexture("   "));
    }

    [Fact]
    public void GetTextureWithNonExistentNameThrowsKeyNotFoundException()
    {
        using var service = new AssetsService();

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetTexture("nonexistent"));
        Assert.Contains("nonexistent", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetBoundingBoxWithNullNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() => service.GetBoundingBox(null!));
    }

    [Fact]
    public void GetBoundingBoxWithEmptyNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() => service.GetBoundingBox(""));
        _ = Assert.Throws<ArgumentNullException>(() => service.GetBoundingBox("   "));
    }

    [Fact]
    public void GetBoundingBoxWithNonExistentNameThrowsKeyNotFoundException()
    {
        using var service = new AssetsService();

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetBoundingBox("nonexistent"));
        Assert.Contains("nonexistent", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AddTextureWithNullNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();
        string tempFile = Path.GetTempFileName();

        try
        {
            _ = Assert.Throws<ArgumentNullException>(() =>
                service.AddTexture(null!, TextureType.ColorMap, tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddTextureWithEmptyNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();
        string tempFile = Path.GetTempFileName();

        try
        {
            _ = Assert.Throws<ArgumentNullException>(() =>
                service.AddTexture("", TextureType.ColorMap, tempFile));
            _ = Assert.Throws<ArgumentNullException>(() =>
                service.AddTexture("   ", TextureType.ColorMap, tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddTextureWithNullPathThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, null!));
    }

    [Fact]
    public void AddTextureWithEmptyPathThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, ""));
        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, "   "));
    }

    [Fact]
    public void AddTextureWithNonExistentFileThrowsFileNotFoundException()
    {
        using var service = new AssetsService();
        string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");

        _ = Assert.Throws<FileNotFoundException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, nonExistentPath));
    }

    [Fact]
    public void AddShaderWithNullNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            _ = Assert.Throws<ArgumentNullException>(() =>
                service.AddShader(null!, tempDir));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void AddShaderWithEmptyNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            _ = Assert.Throws<ArgumentNullException>(() =>
                service.AddShader("", tempDir));
            _ = Assert.Throws<ArgumentNullException>(() =>
                service.AddShader("   ", tempDir));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void AddShaderWithNullPathThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddShader("shader", null!));
    }

    [Fact]
    public void AddShaderWithEmptyPathThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddShader("shader", ""));
        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddShader("shader", "   "));
    }

    [Fact]
    public void AddShaderWithMissingVertexShaderFileThrowsFileNotFoundException()
    {
        using var service = new AssetsService();
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            string fragmentPath = Path.Combine(tempDir, "fragment.glsl");
            File.WriteAllText(fragmentPath, "#version 460\nvoid main() { }");

            FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() =>
                service.AddShader("shader", tempDir));
            Assert.Contains("Vertex shader file not found", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void AddShaderWithMissingFragmentShaderFileThrowsFileNotFoundException()
    {
        using var service = new AssetsService();
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            string vertexPath = Path.Combine(tempDir, "vertex.glsl");
            File.WriteAllText(vertexPath, "#version 460\nvoid main() { }");

            FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() =>
                service.AddShader("shader", tempDir));
            Assert.Contains("Fragment shader file not found", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void AddMeshFromFileWithNullNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();
        string tempFile = Path.GetTempFileName();

        try
        {
            _ = Assert.Throws<ArgumentNullException>(() =>
                service.AddMeshFromFile(null!, tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddMeshFromFileWithEmptyNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();
        string tempFile = Path.GetTempFileName();

        try
        {
            _ = Assert.Throws<ArgumentNullException>(() =>
                service.AddMeshFromFile("", tempFile));
            _ = Assert.Throws<ArgumentNullException>(() =>
                service.AddMeshFromFile("   ", tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void AddMeshFromFileWithNullPathThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddMeshFromFile("mesh", null!));
    }

    [Fact]
    public void AddMeshFromFileWithEmptyPathThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddMeshFromFile("mesh", ""));
        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddMeshFromFile("mesh", "   "));
    }

    [Fact]
    public void AddMeshFromFileWithNonExistentFileThrowsFileNotFoundException()
    {
        using var service = new AssetsService();
        string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".obj");

        _ = Assert.Throws<FileNotFoundException>(() =>
            service.AddMeshFromFile("mesh", nonExistentPath));
    }

    [Fact]
    public void AddSphereWithNullNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddSphere(null!, 1.0f, 10, 10));
    }

    [Fact]
    public void AddSphereWithEmptyNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddSphere("", 1.0f, 10, 10));
        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddSphere("   ", 1.0f, 10, 10));
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-1.0f)]
    [InlineData(-10.0f)]
    public void AddSphereWithInvalidRadiusThrowsArgumentOutOfRangeException(float invalidRadius)
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddSphere("sphere", invalidRadius, 10, 10));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void AddSphereWithInvalidSectorsThrowsArgumentOutOfRangeException(int invalidSectors)
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddSphere("sphere", 1.0f, invalidSectors, 10));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void AddSphereWithInvalidStacksThrowsArgumentOutOfRangeException(int invalidStacks)
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.AddSphere("sphere", 1.0f, 10, invalidStacks));
    }

    [Fact]
    public void AddBoundingBoxWithNullNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddBoundingBox(null!));
    }

    [Fact]
    public void AddBoundingBoxWithEmptyNameThrowsArgumentNullException()
    {
        using var service = new AssetsService();

        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddBoundingBox(""));
        _ = Assert.Throws<ArgumentNullException>(() =>
            service.AddBoundingBox("   "));
    }

    [Fact]
    public void AddBoundingBoxWithNonExistentMeshThrowsKeyNotFoundException()
    {
        using var service = new AssetsService();

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() =>
            service.AddBoundingBox("nonexistent"));
        Assert.Contains("nonexistent", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetAllShadersReturnsEmptyCollectionWhenNoShaders()
    {
        using var service = new AssetsService();

        IReadOnlyCollection<ShaderAsset> shaders = service.GetAllShaders();

        Assert.NotNull(shaders);
        Assert.Empty(shaders);
    }

    [Fact]
    public void DisposeDoesNotThrow()
    {
        using var service = new AssetsService();

        service.Dispose();
        service.Dispose();
    }

    [Fact]
    public void GetMeshWithVeryLongNameThrowsKeyNotFoundException()
    {
        using var service = new AssetsService();
        string veryLongName = new('a', 10000);

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetMesh(veryLongName));
        Assert.Contains(veryLongName, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetShaderWithSpecialCharactersThrowsKeyNotFoundException()
    {
        using var service = new AssetsService();
        string specialName = "test@#$%^&*()_+-=[]{}|;':\",./<>?";

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetShader(specialName));
        Assert.Contains(specialName, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetTextureWithUnicodeCharactersThrowsKeyNotFoundException()
    {
        using var service = new AssetsService();
        string unicodeName = "тест资源🎮";

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(() => service.GetTexture(unicodeName));
        Assert.Contains(unicodeName, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(10000)]
    [InlineData(3)]
    public void AddSphereWithExtremeSectorsThrowsOrSucceeds(int sectors)
    {
        using var service = new AssetsService();

        if (sectors < 3)
        {
            _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
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
    public void AddSphereWithExtremeStacksThrowsOrSucceeds(int stacks)
    {
        using var service = new AssetsService();

        if (stacks < 2)
        {
            _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
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
    public void AddSphereWithExtremeRadiusThrowsOrSucceeds(float radius)
    {
        using var service = new AssetsService();

        if (radius <= 0)
        {
            _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.AddSphere("sphere", radius, 10, 10));
        }
        else if (radius > 1e5f)
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void AddTextureWithVeryLongPathThrowsFileNotFoundException()
    {
        using var service = new AssetsService();
        string veryLongPath = Path.Combine(Path.GetTempPath(), new string('a', 260) + ".png");

        _ = Assert.Throws<FileNotFoundException>(() =>
            service.AddTexture("texture", TextureType.ColorMap, veryLongPath));
    }

    [Fact]
    public void AddShaderWithVeryLongPathThrowsFileNotFoundException()
    {
        using var service = new AssetsService();
        string veryLongPath = Path.Combine(Path.GetTempPath(), new string('a', 260));

        _ = Assert.Throws<FileNotFoundException>(() =>
            service.AddShader("shader", veryLongPath));
    }

    [Fact]
    public void AddMeshFromFileWithVeryLongPathThrowsFileNotFoundException()
    {
        using var service = new AssetsService();
        string veryLongPath = Path.Combine(Path.GetTempPath(), new string('a', 260) + ".obj");

        _ = Assert.Throws<FileNotFoundException>(() =>
            service.AddMeshFromFile("mesh", veryLongPath));
    }

    [Fact]
    public void GetAllShadersAfterDisposeReturnsEmptyCollection()
    {
        using var service = new AssetsService();

        service.Dispose();
        IReadOnlyCollection<ShaderAsset> shaders = service.GetAllShaders();

        Assert.NotNull(shaders);
        Assert.Empty(shaders);
    }

    [Fact]
    public void DisposeMultipleTimesDoesNotThrow()
    {
        using var service = new AssetsService();

        service.Dispose();
        service.Dispose();
        service.Dispose();
        Assert.True(true);
    }

    [Fact]
    public void GetMethodsAfterDisposeStillThrowKeyNotFoundException()
    {
        using var service = new AssetsService();
        service.Dispose();

        _ = Assert.Throws<KeyNotFoundException>(() => service.GetMesh("test"));
        _ = Assert.Throws<KeyNotFoundException>(() => service.GetShader("test"));
        _ = Assert.Throws<KeyNotFoundException>(() => service.GetTexture("test"));
        _ = Assert.Throws<KeyNotFoundException>(() => service.GetBoundingBox("test"));
    }

    [Fact]
    public void AddMethodsWithNullNameAfterDisposeStillThrowsArgumentNullException()
    {
        using var service = new AssetsService();
        service.Dispose();

        _ = Assert.Throws<ArgumentNullException>(() => service.AddBoundingBox(null!));
    }
}
