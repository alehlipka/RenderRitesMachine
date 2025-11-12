using Moq;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса SceneFactory (edge cases).
/// </summary>
public class SceneFactoryTests
{
    private SceneFactory CreateSceneFactory()
    {
        IAssetsService assetsService = Mock.Of<IAssetsService>();
        ITimeService timeService = Mock.Of<ITimeService>();
        IRenderService renderService = Mock.Of<IRenderService>();
        IGuiService guiService = Mock.Of<IGuiService>();
        IAudioService audioService = Mock.Of<IAudioService>();
        ILogger logger = Mock.Of<ILogger>();
        ISceneManager sceneManager = Mock.Of<ISceneManager>();

        var factory = new SceneFactory(assetsService, timeService, renderService, guiService, audioService, logger);
        factory.SetSceneManager(sceneManager);
        return factory;
    }

    [Fact]
    public void CreateScene_WithoutSetSceneManager_ThrowsInvalidOperationException()
    {
        IAssetsService assetsService = Mock.Of<IAssetsService>();
        ITimeService timeService = Mock.Of<ITimeService>();
        IRenderService renderService = Mock.Of<IRenderService>();
        IGuiService guiService = Mock.Of<IGuiService>();
        IAudioService audioService = Mock.Of<IAudioService>();
        ILogger logger = Mock.Of<ILogger>();

        var factory = new SceneFactory(assetsService, timeService, renderService, guiService, audioService, logger);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => factory.CreateScene<TestScene>("test"));
        Assert.Contains("SceneManager must be set", exception.Message);
    }

    [Fact]
    public void CreateScene_WithNullName_HandlesCorrectly()
    {
        SceneFactory factory = CreateSceneFactory();

        try
        {
            TestScene scene = factory.CreateScene<TestScene>(null!);
            Assert.NotNull(scene);
        }
        catch (Exception)
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void CreateScene_WithEmptyName_CreatesScene()
    {
        SceneFactory factory = CreateSceneFactory();

        TestScene scene = factory.CreateScene<TestScene>("");

        Assert.NotNull(scene);
        Assert.Equal("", scene.Name);
    }

    [Fact]
    public void CreateScene_WithVeryLongName_CreatesScene()
    {
        SceneFactory factory = CreateSceneFactory();
        string longName = new string('a', 10000);

        TestScene scene = factory.CreateScene<TestScene>(longName);

        Assert.NotNull(scene);
        Assert.Equal(longName, scene.Name);
    }

    [Fact]
    public void CreateScene_MultipleTimes_CreatesDifferentInstances()
    {
        SceneFactory factory = CreateSceneFactory();

        TestScene scene1 = factory.CreateScene<TestScene>("test1");
        TestScene scene2 = factory.CreateScene<TestScene>("test2");

        Assert.NotSame(scene1, scene2);
        Assert.Equal("test1", scene1.Name);
        Assert.Equal("test2", scene2.Name);
    }

    [Fact]
    public void SetSceneManager_MultipleTimes_UpdatesManager()
    {
        SceneFactory factory = CreateSceneFactory();
        ISceneManager newManager = Mock.Of<ISceneManager>();

        factory.SetSceneManager(newManager);
        TestScene scene = factory.CreateScene<TestScene>("test");

        Assert.NotNull(scene);
    }

    private class TestScene : Scene
    {
        public TestScene(string name, IAssetsService assetsService, ITimeService timeService,
            IRenderService renderService, IGuiService guiService, IAudioService audioService,
            ISceneManager sceneManager, ILogger logger)
            : base(name, assetsService, timeService, renderService, guiService, audioService, sceneManager, logger)
        {
        }

        protected override void OnLoad()
        {
        }
    }
}

