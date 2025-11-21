using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.Exceptions;

namespace RenderRitesMachine.Tests;

public sealed class ExceptionsTests
{
    [Fact]
    public void DuplicateResourceExceptionStoresDetails()
    {
        var exception = new DuplicateResourceException("mesh", "cube");

        Assert.Equal("mesh", exception.ResourceType);
        Assert.Equal("cube", exception.ResourceName);
        Assert.Contains("cube", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void OpenGLErrorExceptionStoresErrorInfo()
    {
        var exception = new OpenGLErrorException("Compile", ErrorCode.InvalidEnum);

        Assert.Equal("Compile", exception.Operation);
        Assert.Equal(ErrorCode.InvalidEnum, exception.ErrorCode);
        Assert.Contains("InvalidEnum", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ShaderCompilationExceptionStoresInfo()
    {
        var exception = new ShaderCompilationException("FragmentShader", "shader.frag", "syntax error");

        Assert.Equal("FragmentShader", exception.ShaderType);
        Assert.Equal("shader.frag", exception.ShaderPath);
        Assert.Equal("syntax error", exception.CompilationLog);
    }

    [Fact]
    public void ShaderLinkingExceptionStoresInfo()
    {
        var exception = new ShaderLinkingException("MainShader", "link error");

        Assert.Equal("MainShader", exception.ShaderName);
        Assert.Equal("link error", exception.LinkingLog);
    }
}

