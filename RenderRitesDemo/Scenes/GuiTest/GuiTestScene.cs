using System.Numerics;
using RenderRitesDemo.ECS;
using RenderRitesDemo.ECS.Features.SceneSwitch;
using RenderRitesMachine.ECS.Systems;
using RenderRitesMachine.Output;

namespace RenderRitesDemo.Scenes.GuiTest;

/// <summary>
/// Сцена для тестирования различных элементов GUI через ImGui.
/// </summary>
public class GuiTestScene(string name) : Scene(name)
{
    // Переменные для различных GUI элементов (публичные для доступа из системы рендеринга)
    public bool Checkbox1 = false;
    public bool Checkbox2 = true;
    public int RadioButton = 0;
    public float SliderFloat = 0.5f;
    public int SliderInt = 50;
    public string TextInput = "Введите текст...";
    public int ComboSelection = 0;
    public readonly string[] ComboItems = { "Вариант 1", "Вариант 2", "Вариант 3", "Вариант 4" };
    public int ListBoxSelection = 0;
    public readonly string[] ListBoxItems = { "Элемент A", "Элемент B", "Элемент C", "Элемент D" };
    public Vector3 ColorPicker = new(1.0f, 0.5f, 0.0f);
    public bool ShowDemoWindow = false;
    public bool ShowStyleEditor = false;

    protected override void OnLoad()
    {
        Camera.Position = new OpenTK.Mathematics.Vector3(0.0f, 0.0f, 10.0f);

        ResizeSystems.Add(new MainResizeSystem());
        UpdateSystems.Add(new SceneSwitchSystem());
        RenderSystems.Add(new GuiTestRenderSystem());
    }
}

