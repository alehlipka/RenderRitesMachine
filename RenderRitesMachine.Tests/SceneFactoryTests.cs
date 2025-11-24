using System.Diagnostics.CodeAnalysis;
using Moq;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса SceneFactory (edge cases).
/// </summary>
public sealed class SceneFactoryTests
{
    private static SceneFactory CreateSceneFactory()
    {
        IAssetsService assetsService = Mock.Of<IAssetsService>();
        ITimeService timeService = Mock.Of<ITimeService>();
        IRenderService renderService = Mock.Of<IRenderService>();
        IAudioService audioService = Mock.Of<IAudioService>();
        IGuiService guiService = Mock.Of<IGuiService>();
        ILogger logger = Mock.Of<ILogger>();
        ISceneManager sceneManager = Mock.Of<ISceneManager>();

        var factory = new SceneFactory(assetsService, timeService, renderService, audioService, guiService, logger);
        factory.SetSceneManager(sceneManager);
        return factory;
    }

    [Fact]
    public void CreateSceneWithoutSetSceneManagerThrowsInvalidOperationException()
    {
        IAssetsService assetsService = Mock.Of<IAssetsService>();
        ITimeService timeService = Mock.Of<ITimeService>();
        IRenderService renderService = Mock.Of<IRenderService>();
        IAudioService audioService = Mock.Of<IAudioService>();
        IGuiService guiService = Mock.Of<IGuiService>();
        ILogger logger = Mock.Of<ILogger>();

        var factory = new SceneFactory(assetsService, timeService, renderService, audioService, guiService, logger);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => factory.CreateScene<TestScene>("test"));
        Assert.Contains("SceneManager must be set", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CreateSceneWithNullNameHandlesCorrectly()
    {
        SceneFactory factory = CreateSceneFactory();

        try
        {
            TestScene scene = factory.CreateScene<TestScene>(null!);
            Assert.NotNull(scene);
        }
        catch (ArgumentNullException)
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void CreateSceneWithEmptyNameCreatesScene()
    {
        SceneFactory factory = CreateSceneFactory();

        TestScene scene = factory.CreateScene<TestScene>("");

        Assert.NotNull(scene);
        Assert.Equal("", scene.Name);
    }

    [Fact]
    public void CreateSceneWithVeryLongNameCreatesScene()
    {
        SceneFactory factory = CreateSceneFactory();
        string longName = new('a', 10000);

        TestScene scene = factory.CreateScene<TestScene>(longName);

        Assert.NotNull(scene);
        Assert.Equal(longName, scene.Name);
    }

    [Fact]
    public void CreateSceneMultipleTimesCreatesDifferentInstances()
    {
        SceneFactory factory = CreateSceneFactory();

        TestScene scene1 = factory.CreateScene<TestScene>("test1");
        TestScene scene2 = factory.CreateScene<TestScene>("test2");

        Assert.NotSame(scene1, scene2);
        Assert.Equal("test1", scene1.Name);
        Assert.Equal("test2", scene2.Name);
    }

    [Fact]
    public void SetSceneManagerMultipleTimesUpdatesManager()
    {
        SceneFactory factory = CreateSceneFactory();
        ISceneManager newManager = Mock.Of<ISceneManager>();

        factory.SetSceneManager(newManager);
        TestScene scene = factory.CreateScene<TestScene>("test");

        Assert.NotNull(scene);
    }

    [SuppressMessage("Performance", "CA1812", Justification = "Instantiated via SceneFactory.CreateScene<>")]
    private sealed class TestScene(string name, IAssetsService assetsService, ITimeService timeService,
        IRenderService renderService, IAudioService audioService, IGuiService guiService, ISceneManager sceneManager, ILogger logger)
        : Scene(name, assetsService, timeService, renderService, audioService, guiService, sceneManager, logger)
    {
        protected override void OnLoad()
        {
        }
    }
}
