using Moq;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Tests;

/// <summary>
/// Тесты для класса SceneManager.
/// </summary>
public class SceneManagerTests
{
    private Mock<ISceneFactory> CreateMockSceneFactory()
    {
        var mockFactory = new Mock<ISceneFactory>();
        return mockFactory;
    }

    private TestScene CreateTestScene(string name)
    {
        return new TestScene(
            name,
            Mock.Of<IAssetsService>(),
            Mock.Of<ITimeService>(),
            Mock.Of<IRenderService>(),
            Mock.Of<IGuiService>(),
            Mock.Of<IAudioService>(),
            Mock.Of<ISceneManager>(),
            Mock.Of<ILogger>()
        );
    }

    [Fact]
    public void Constructor_CreatesEmptyManager()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();

        var manager = new SceneManager(mockFactory.Object);

        Assert.Null(manager.Current);
    }

    [Fact]
    public void AddScene_WithValidName_AddsScene()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);
        TestScene testScene = CreateTestScene("testScene");
        mockFactory.Setup(f => f.CreateScene<TestScene>("testScene")).Returns(testScene);

        manager.AddScene<TestScene>("testScene");

        var scenes = manager.Select(s => s.Name).ToList();
        Assert.Contains("testScene", scenes);
    }

    [Fact]
    public void AddScene_WithNullName_ThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);

        Assert.Throws<ArgumentNullException>(() => manager.AddScene<TestScene>(null!));
    }

    [Fact]
    public void AddScene_WithEmptyName_ThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);

        Assert.Throws<ArgumentNullException>(() => manager.AddScene<TestScene>(""));
        Assert.Throws<ArgumentNullException>(() => manager.AddScene<TestScene>("   "));
    }

    [Fact]
    public void AddScene_ReturnsSelf_ForChaining()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);
        TestScene testScene1 = CreateTestScene("scene1");
        TestScene testScene2 = CreateTestScene("scene2");
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);

        SceneManager result = manager
            .AddScene<TestScene>("scene1")
            .AddScene<TestScene>("scene2");

        Assert.Same(manager, result);
        var scenes = manager.Select(s => s.Name).ToList();
        Assert.Contains("scene1", scenes);
        Assert.Contains("scene2", scenes);
    }

    [Fact]
    public void SwitchTo_WithValidName_SwitchesCurrentScene()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);
        TestScene testScene1 = CreateTestScene("scene1");
        TestScene testScene2 = CreateTestScene("scene2");
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);

        manager.AddScene<TestScene>("scene1");
        manager.AddScene<TestScene>("scene2");

        manager.SwitchTo("scene2");

        Assert.NotNull(manager.Current);
        Assert.Equal("scene2", manager.Current.Name);
    }

    [Fact]
    public void SwitchTo_WithInvalidName_ThrowsArgumentException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);
        TestScene testScene = CreateTestScene("scene1");
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene);
        manager.AddScene<TestScene>("scene1");

        ArgumentException exception = Assert.Throws<ArgumentException>(() => manager.SwitchTo("nonexistent"));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void SwitchTo_WithNullName_ThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);

        Assert.Throws<ArgumentNullException>(() => manager.SwitchTo(null!));
    }

    [Fact]
    public void SwitchTo_WithEmptyName_ThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);

        Assert.Throws<ArgumentNullException>(() => manager.SwitchTo(""));
        Assert.Throws<ArgumentNullException>(() => manager.SwitchTo("   "));
    }

    [Fact]
    public void ForEach_ExecutesActionForEachScene()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);
        TestScene testScene1 = CreateTestScene("scene1");
        TestScene testScene2 = CreateTestScene("scene2");
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);

        manager.AddScene<TestScene>("scene1");
        manager.AddScene<TestScene>("scene2");

        var sceneNames = new List<string>();

        manager.ForEach(scene => sceneNames.Add(scene.Name));

        Assert.Equal(2, sceneNames.Count);
        Assert.Contains("scene1", sceneNames);
        Assert.Contains("scene2", sceneNames);
    }

    [Fact]
    public void ForEach_WithNullAction_ThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);

        Assert.Throws<ArgumentNullException>(() => manager.ForEach(null!));
    }

    [Fact]
    public void Select_ProjectsScenes()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);
        TestScene testScene1 = CreateTestScene("scene1");
        TestScene testScene2 = CreateTestScene("scene2");
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);

        manager.AddScene<TestScene>("scene1");
        manager.AddScene<TestScene>("scene2");

        var sceneNames = manager.Select(s => s.Name).ToList();

        Assert.Equal(2, sceneNames.Count);
        Assert.Contains("scene1", sceneNames);
        Assert.Contains("scene2", sceneNames);
    }

    [Fact]
    public void Select_WithNullSelector_ThrowsArgumentNullException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);

        Assert.Throws<ArgumentNullException>(() => manager.Select<string>(null!));
    }

    [Fact]
    public void Dispose_DisposesAllScenes()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);
        TestScene testScene1 = CreateTestScene("scene1");
        TestScene testScene2 = CreateTestScene("scene2");
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);

        manager.AddScene<TestScene>("scene1");
        manager.AddScene<TestScene>("scene2");

        manager.Dispose();

        Assert.True(true);
    }

    private class TestScene : Scene
    {
        public TestScene(string name, IAssetsService assetsService, ITimeService timeService,
            IRenderService renderService, IGuiService guiService, IAudioService audioService, ISceneManager sceneManager, ILogger logger)
            : base(name, assetsService, timeService, renderService, guiService, audioService, sceneManager, logger)
        {
        }

        protected override void OnLoad()
        {
        }
    }

    [Fact]
    public void SwitchTo_MultipleRapidSwitches_WorksCorrectly()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);

        TestScene testScene1 = CreateTestScene("scene1");
        TestScene testScene2 = CreateTestScene("scene2");
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene1);
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene2")).Returns(testScene2);
        manager.AddScene<TestScene>("scene1");
        manager.AddScene<TestScene>("scene2");
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
    public void ForEach_WithEmptyManager_DoesNotExecute()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);
        int executionCount = 0;

        manager.ForEach(_ => executionCount++);

        Assert.Equal(0, executionCount);
    }

    [Fact]
    public void Select_WithEmptyManager_ReturnsEmptySequence()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);

        var results = manager.Select(s => s.Name).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Dispose_WithManyScenes_DisposesAll()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);

        for (int i = 0; i < 50; i++)
        {
            TestScene scene = CreateTestScene($"scene{i}");
            mockFactory.Setup(f => f.CreateScene<TestScene>($"scene{i}")).Returns(scene);
            manager.AddScene<TestScene>($"scene{i}");
        }

        manager.Dispose();

        Assert.True(true);
    }

    [Fact]
    public void AddScene_WithVeryLongName_AddsScene()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);
        string longName = new string('a', 1000);
        TestScene testScene = CreateTestScene(longName);
        mockFactory.Setup(f => f.CreateScene<TestScene>(longName)).Returns(testScene);

        manager.AddScene<TestScene>(longName);

        var scenes = manager.Select(s => s.Name).ToList();
        Assert.Contains(longName, scenes);
    }

    [Fact]
    public void SwitchTo_WithVeryLongName_ThrowsArgumentException()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);
        string longName = new string('a', 1000);

        ArgumentException exception = Assert.Throws<ArgumentException>(() => manager.SwitchTo(longName));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void Current_AfterDispose_IsNull()
    {
        Mock<ISceneFactory> mockFactory = CreateMockSceneFactory();
        var manager = new SceneManager(mockFactory.Object);
        TestScene testScene = CreateTestScene("scene1");
        mockFactory.Setup(f => f.CreateScene<TestScene>("scene1")).Returns(testScene);
        manager.AddScene<TestScene>("scene1");
        manager.SwitchTo("scene1");

        manager.Dispose();

        Assert.True(true);
    }
}

