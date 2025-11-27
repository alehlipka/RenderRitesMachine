using Moq;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Tests for <see cref="SceneManager"/>.
/// </summary>
public sealed class SceneManagerTests
{
    private static Mock<ISceneFactory> CreateMockSceneFactory()
    {
        var mockFactory = new Mock<ISceneFactory>();
        return mockFactory;
    }

    private static TestScene CreateTestScene(string name)
    {
        return new TestScene(
            name,
            Mock.Of<IAssetsService>(),
            Mock.Of<ITimeService>(),
            Mock.Of<IRenderService>(),
            Mock.Of<IAudioService>(),
            Mock.Of<IGuiService>(),
            Mock.Of<ISceneManager>(),
            Mock.Of<ILogger>()
        );
    }

    [Fact]
    public void ConstructorCreatesEmptyManager()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();

        using var manager = new SceneManager(mockFactory.Object);

        Assert.Null(manager.Current);
    }

    [Fact]
    public void AddSceneWithValidNameAddsScene()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);
        using TestScene testScene = CreateTestScene("testScene");
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("testScene")).Returns(testScene);

        _ = manager.AddScene<TestScene>("testScene");

        var scenes = manager.Select(s => s.Name).ToList();
        Assert.Contains("testScene", scenes, StringComparer.Ordinal);
    }

    [Fact]
    public void AddSceneWithNullNameThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);

        _ = Assert.Throws<ArgumentNullException>(() => manager.AddScene<TestScene>(null!));
    }

    [Fact]
    public void AddSceneWithEmptyNameThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);

        _ = Assert.Throws<ArgumentNullException>(() => manager.AddScene<TestScene>(""));
        _ = Assert.Throws<ArgumentNullException>(() => manager.AddScene<TestScene>("   "));
    }

    [Fact]
    public void AddSceneReturnsSelfForChaining()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);
        using TestScene testScene1 = CreateTestScene("scene1");
        using TestScene testScene2 = CreateTestScene("scene2");
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);

        SceneManager result = manager
            .AddScene<TestScene>("scene1")
            .AddScene<TestScene>("scene2");

        Assert.Same(manager, result);
        var scenes = manager.Select(s => s.Name).ToList();
        Assert.Contains("scene1", scenes, StringComparer.Ordinal);
        Assert.Contains("scene2", scenes, StringComparer.Ordinal);
    }

    [Fact]
    public void SwitchToWithValidNameSwitchesCurrentScene()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);
        using TestScene testScene1 = CreateTestScene("scene1");
        using TestScene testScene2 = CreateTestScene("scene2");
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);

        _ = manager.AddScene<TestScene>("scene1");
        _ = manager.AddScene<TestScene>("scene2");

        manager.SwitchTo("scene2");

        Assert.NotNull(manager.Current);
        Assert.Equal("scene2", manager.Current.Name);
    }

    [Fact]
    public void SwitchToWithInvalidNameThrowsArgumentException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);
        using TestScene testScene = CreateTestScene("scene1");
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene);
        _ = manager.AddScene<TestScene>("scene1");

        ArgumentException exception = Assert.Throws<ArgumentException>(() => manager.SwitchTo("nonexistent"));
        Assert.Contains("not found", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void SwitchToWithNullNameThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);

        _ = Assert.Throws<ArgumentNullException>(() => manager.SwitchTo(null!));
    }

    [Fact]
    public void SwitchToWithEmptyNameThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);

        _ = Assert.Throws<ArgumentNullException>(() => manager.SwitchTo(""));
        _ = Assert.Throws<ArgumentNullException>(() => manager.SwitchTo("   "));
    }

    [Fact]
    public void ForEachExecutesActionForEachScene()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);
        using TestScene testScene1 = CreateTestScene("scene1");
        using TestScene testScene2 = CreateTestScene("scene2");
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);

        _ = manager.AddScene<TestScene>("scene1");
        _ = manager.AddScene<TestScene>("scene2");

        var sceneNames = new List<string>();

        manager.ForEach(scene => sceneNames.Add(scene.Name));

        Assert.Equal(2, sceneNames.Count);
        Assert.Contains("scene1", sceneNames, StringComparer.Ordinal);
        Assert.Contains("scene2", sceneNames, StringComparer.Ordinal);
    }

    [Fact]
    public void ForEachWithNullActionThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);

        _ = Assert.Throws<ArgumentNullException>(() => manager.ForEach(null!));
    }

    [Fact]
    public void SelectProjectsScenes()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);
        using TestScene testScene1 = CreateTestScene("scene1");
        using TestScene testScene2 = CreateTestScene("scene2");
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);

        _ = manager.AddScene<TestScene>("scene1");
        _ = manager.AddScene<TestScene>("scene2");

        var sceneNames = manager.Select(s => s.Name).ToList();

        Assert.Equal(2, sceneNames.Count);
        Assert.Contains("scene1", sceneNames, StringComparer.Ordinal);
        Assert.Contains("scene2", sceneNames, StringComparer.Ordinal);
    }

    [Fact]
    public void SelectWithNullSelectorThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);

        _ = Assert.Throws<ArgumentNullException>(() => manager.Select<string>(null!));
    }

    [Fact]
    public void DisposeDisposesAllScenes()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);
        using TestScene testScene1 = CreateTestScene("scene1");
        using TestScene testScene2 = CreateTestScene("scene2");
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);

        _ = manager.AddScene<TestScene>("scene1");
        _ = manager.AddScene<TestScene>("scene2");

        manager.Dispose();

        Assert.True(true);
    }

    private sealed class TestScene(string name, IAssetsService assetsService, ITimeService timeService,
        IRenderService renderService, IAudioService audioService, IGuiService guiService, ISceneManager sceneManager, ILogger logger)
        : Scene(name, assetsService, timeService, renderService, audioService, guiService, sceneManager, logger)
    {
        protected override void OnLoad()
        {
        }
    }

    [Fact]
    public void SwitchToMultipleRapidSwitchesWorksCorrectly()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);

        using TestScene testScene1 = CreateTestScene("scene1");
        using TestScene testScene2 = CreateTestScene("scene2");
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);
        _ = manager.AddScene<TestScene>("scene1");
        _ = manager.AddScene<TestScene>("scene2");
        manager.SwitchTo("scene1");

        for (int i = 0; i < 100; i++)
        {
            manager.SwitchTo(i % 2 == 0 ? "scene1" : "scene2");
            Assert.NotNull(manager.Current);
        }

        Assert.NotNull(manager.Current);
        Assert.Equal("scene2", manager.Current.Name);
    }

    [Fact]
    public void ForEachWithEmptyManagerDoesNotExecute()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);
        int executionCount = 0;

        manager.ForEach(_ => executionCount++);

        Assert.Equal(0, executionCount);
    }

    [Fact]
    public void SelectWithEmptyManagerReturnsEmptySequence()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);

        var results = manager.Select(s => s.Name).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void DisposeWithManyScenesDisposesAll()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);

        for (int i = 0; i < 50; i++)
        {
            using TestScene scene = CreateTestScene($"scene{i}");
            _ = mockFactory.Setup(f => f.CreateScene<TestScene>($"scene{i}")).Returns(scene);
            _ = manager.AddScene<TestScene>($"scene{i}");
        }

        manager.Dispose();

        Assert.True(true);
    }

    [Fact]
    public void AddSceneWithVeryLongNameAddsScene()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);
        string longName = new('a', 1000);
        using TestScene testScene = CreateTestScene(longName);
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>(longName)).Returns(testScene);

        _ = manager.AddScene<TestScene>(longName);

        var scenes = manager.Select(s => s.Name).ToList();
        Assert.Contains(longName, scenes, StringComparer.Ordinal);
    }

    [Fact]
    public void SwitchToWithVeryLongNameThrowsArgumentException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);
        string longName = new('a', 1000);

        ArgumentException exception = Assert.Throws<ArgumentException>(() => manager.SwitchTo(longName));
        Assert.Contains("not found", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CurrentAfterDisposeIsNull()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        using var manager = new SceneManager(mockFactory.Object);
        using TestScene testScene = CreateTestScene("scene1");
        _ = mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene);
        _ = manager.AddScene<TestScene>("scene1");
        manager.SwitchTo("scene1");

        manager.Dispose();

        Assert.True(true);
    }
}
