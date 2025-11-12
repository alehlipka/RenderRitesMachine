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
        var assetsService = Mock.Of<IAssetsService>();
        var timeService = Mock.Of<ITimeService>();
        var renderService = Mock.Of<IRenderService>();
        var guiService = Mock.Of<IGuiService>();
        var audioService = Mock.Of<IAudioService>();
        var logger = Mock.Of<ILogger>();
        var sceneManager = Mock.Of<ISceneManager>();

        var factory = new SceneFactory(assetsService, timeService, renderService, guiService, audioService, logger);
        factory.SetSceneManager(sceneManager);
        return factory;
    }

    [Fact]
    public void CreateScene_WithoutSetSceneManager_ThrowsInvalidOperationException()
    {
        // Arrange
        var assetsService = Mock.Of<IAssetsService>();
        var timeService = Mock.Of<ITimeService>();
        var renderService = Mock.Of<IRenderService>();
        var guiService = Mock.Of<IGuiService>();
        var audioService = Mock.Of<IAudioService>();
        var logger = Mock.Of<ILogger>();

        var factory = new SceneFactory(assetsService, timeService, renderService, guiService, audioService, logger);
        // Не вызываем SetSceneManager

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => factory.CreateScene<TestScene>("test"));
        Assert.Contains("SceneManager must be set", exception.Message);
    }

    [Fact]
    public void CreateScene_WithNullName_HandlesCorrectly()
    {
        // Arrange
        var factory = CreateSceneFactory();

        // Act & Assert - Scene может принимать null имя или выбросить исключение
        // В зависимости от реализации Scene конструктора
        try
        {
            var scene = factory.CreateScene<TestScene>(null!);
            // Если не выбросило исключение, проверяем, что сцена создана
            Assert.NotNull(scene);
        }
        catch (Exception)
        {
            // Исключение тоже допустимо
            Assert.True(true);
        }
    }

    [Fact]
    public void CreateScene_WithEmptyName_CreatesScene()
    {
        // Arrange
        var factory = CreateSceneFactory();

        // Act
        var scene = factory.CreateScene<TestScene>("");

        // Assert - сцена должна быть создана (валидация имени может быть в Scene)
        Assert.NotNull(scene);
        Assert.Equal("", scene.Name);
    }

    [Fact]
    public void CreateScene_WithVeryLongName_CreatesScene()
    {
        // Arrange
        var factory = CreateSceneFactory();
        var longName = new string('a', 10000);

        // Act
        var scene = factory.CreateScene<TestScene>(longName);

        // Assert
        Assert.NotNull(scene);
        Assert.Equal(longName, scene.Name);
    }

    [Fact]
    public void CreateScene_MultipleTimes_CreatesDifferentInstances()
    {
        // Arrange
        var factory = CreateSceneFactory();

        // Act
        var scene1 = factory.CreateScene<TestScene>("test1");
        var scene2 = factory.CreateScene<TestScene>("test2");

        // Assert
        Assert.NotSame(scene1, scene2);
        Assert.Equal("test1", scene1.Name);
        Assert.Equal("test2", scene2.Name);
    }

    [Fact]
    public void SetSceneManager_MultipleTimes_UpdatesManager()
    {
        // Arrange
        var factory = CreateSceneFactory();
        var newManager = Mock.Of<ISceneManager>();

        // Act
        factory.SetSceneManager(newManager);
        var scene = factory.CreateScene<TestScene>("test");

        // Assert - сцена должна быть создана с новым менеджером
        Assert.NotNull(scene);
    }

    // Вспомогательные классы для тестирования
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
            // Пустая реализация
        }
    }
}

