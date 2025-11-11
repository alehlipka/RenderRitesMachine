using System.Numerics;
using ImGuiNET;
using Leopotam.EcsLite;
using RenderRitesDemo.Scenes.GuiTest;
using RenderRitesMachine;
using RenderRitesMachine.Debug;
using RenderRitesMachine.ECS;

namespace RenderRitesDemo.ECS;

/// <summary>
/// Система рендеринга для тестирования GUI элементов.
/// </summary>
public class GuiTestRenderSystem : IEcsRunSystem
{
    private bool _showMetricsWindow = false;

    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        // Убеждаемся, что контекст ImGui установлен
        IntPtr context = shared.Gui.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
        }

        // Получаем ссылку на сцену
        var scene = shared.SceneManager.Current as GuiTestScene;
        if (scene == null) return;

        // Главное меню
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Сцены"))
            {
                string currentScene = shared.SceneManager.Current?.Name ?? "";
                bool isPreloader = currentScene == "preloader";
                bool isGuiTest = currentScene == "guitest";

                if (ImGui.MenuItem("Главная сцена", "F1", isPreloader))
                {
                    shared.SceneManager.SwitchTo("preloader");
                }
                if (ImGui.MenuItem("GUI Тест", "F2", isGuiTest))
                {
                    shared.SceneManager.SwitchTo("guitest");
                }
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Окна"))
            {
                if (ImGui.MenuItem("Демо окно ImGui", null, scene.ShowDemoWindow))
                {
                    scene.ShowDemoWindow = !scene.ShowDemoWindow;
                }
                if (ImGui.MenuItem("Редактор стилей", null, scene.ShowStyleEditor))
                {
                    scene.ShowStyleEditor = !scene.ShowStyleEditor;
                }
                if (ImGui.MenuItem("Метрики", "Ctrl+M", _showMetricsWindow))
                {
                    _showMetricsWindow = !_showMetricsWindow;
                }
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        // Демо окно ImGui
        if (scene.ShowDemoWindow)
        {
            ImGui.ShowDemoWindow(ref scene.ShowDemoWindow);
        }

        // Редактор стилей
        if (scene.ShowStyleEditor)
        {
            if (ImGui.Begin("Редактор стилей", ref scene.ShowStyleEditor))
            {
                ImGui.ShowStyleEditor();
            }
            ImGui.End();
        }

        // Окно метрик
        if (_showMetricsWindow)
        {
            if (ImGui.Begin("Метрики", ref _showMetricsWindow))
            {
                ImGui.Text($"FPS: {FpsCounter.GetFps():F2}");
                ImGui.Text($"Время кадра: {1000.0 / FpsCounter.GetFps():F2} мс");

                ImGui.Separator();
                ImGui.Text("Статистика ImGui:");
                ImGui.Text($"Активных окон: {ImGui.GetIO().MetricsRenderWindows}");
                ImGui.Text($"Активных виджетов: {ImGui.GetIO().MetricsActiveWindows}");
            }
            ImGui.End();
        }

        // Главное окно тестирования GUI
        if (ImGui.Begin("Тестирование GUI элементов", ImGuiWindowFlags.None))
        {
            // Информация о переключении сцен
            ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), "Переключение сцен:");
            ImGui.BulletText("F1 - Главная сцена");
            ImGui.BulletText("F2 - GUI Тест");
            ImGui.Separator();

            // Tab Bar
            if (ImGui.BeginTabBar("GuiElements"))
            {
                // Вкладка: Основные элементы
                if (ImGui.BeginTabItem("Основные"))
                {
                    RenderBasicElements(scene);
                    ImGui.EndTabItem();
                }

                // Вкладка: Ввод данных
                if (ImGui.BeginTabItem("Ввод данных"))
                {
                    RenderInputElements(scene);
                    ImGui.EndTabItem();
                }

                // Вкладка: Выбор
                if (ImGui.BeginTabItem("Выбор"))
                {
                    RenderSelectionElements(scene);
                    ImGui.EndTabItem();
                }

                // Вкладка: Цвета
                if (ImGui.BeginTabItem("Цвета"))
                {
                    RenderColorElements(scene);
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }
        ImGui.End();

        // Окно с информацией
        if (ImGui.Begin("Информация"))
        {
            ImGui.Text($"FPS: {FpsCounter.GetFps():F2}");
            ImGui.Text($"Текущая сцена: {shared.SceneManager.Current?.Name ?? "Нет"}");
            ImGui.Separator();
            ImGui.TextWrapped("Это демонстрационная сцена для тестирования различных элементов GUI ImGui.");
        }
        ImGui.End();
    }

    private void RenderBasicElements(GuiTestScene scene)
    {
        ImGui.Text("Основные элементы управления:");
        ImGui.Separator();

        // Кнопки
        if (ImGui.Button("Обычная кнопка"))
        {
            // Действие при нажатии
        }
        ImGui.SameLine();
        if (ImGui.Button("Маленькая кнопка", new Vector2(100, 0)))
        {
            // Действие при нажатии
        }

        ImGui.Spacing();

        // Checkboxes
        ImGui.Checkbox("Чекбокс 1", ref scene.Checkbox1);
        ImGui.Checkbox("Чекбокс 2 (включен)", ref scene.Checkbox2);

        ImGui.Spacing();

        // Radio Buttons
        ImGui.Text("Радио кнопки:");
        if (ImGui.RadioButton("Вариант A", scene.RadioButton == 0))
            scene.RadioButton = 0;
        ImGui.SameLine();
        if (ImGui.RadioButton("Вариант B", scene.RadioButton == 1))
            scene.RadioButton = 1;
        ImGui.SameLine();
        if (ImGui.RadioButton("Вариант C", scene.RadioButton == 2))
            scene.RadioButton = 2;

        ImGui.Spacing();

        // Текст
        ImGui.Text("Обычный текст");
        ImGui.TextColored(new Vector4(1.0f, 0.0f, 0.0f, 1.0f), "Цветной текст (красный)");
        ImGui.TextDisabled("Отключенный текст");
        ImGui.TextWrapped("Это длинный текст, который будет автоматически переноситься на новую строку, если он не помещается в доступную ширину окна.");

        ImGui.Spacing();

        // Разделители
        ImGui.Separator();
        ImGui.Text("Разделитель выше");
    }

    private void RenderInputElements(GuiTestScene scene)
    {
        ImGui.Text("Элементы ввода данных:");
        ImGui.Separator();

        // Text Input
        ImGui.Text("Текстовое поле:");
        byte[] textBuffer = System.Text.Encoding.UTF8.GetBytes(scene.TextInput);
        Array.Resize(ref textBuffer, 256);
        if (ImGui.InputText("##textinput", textBuffer, (uint)textBuffer.Length))
        {
            scene.TextInput = System.Text.Encoding.UTF8.GetString(textBuffer).TrimEnd('\0');
        }

        ImGui.Spacing();

        // Sliders
        ImGui.Text("Слайдеры:");
        ImGui.SliderFloat("Слайдер (float)", ref scene.SliderFloat, 0.0f, 1.0f);
        ImGui.SliderInt("Слайдер (int)", ref scene.SliderInt, 0, 100);

        ImGui.Spacing();

        // Drag
        ImGui.Text("Перетаскивание:");
        float dragFloat = scene.SliderFloat;
        if (ImGui.DragFloat("Перетащить (float)", ref dragFloat, 0.01f, 0.0f, 1.0f))
        {
            scene.SliderFloat = dragFloat;
        }

        ImGui.Spacing();

        // Input Numbers
        ImGui.Text("Числовые поля:");
        int inputInt = scene.SliderInt;
        if (ImGui.InputInt("Число (int)", ref inputInt))
        {
            scene.SliderInt = Math.Clamp(inputInt, 0, 100);
        }
    }

    private void RenderSelectionElements(GuiTestScene scene)
    {
        ImGui.Text("Элементы выбора:");
        ImGui.Separator();

        // Combo
        ImGui.Text("Выпадающий список:");
        if (ImGui.BeginCombo("Комбо", scene.ComboItems[scene.ComboSelection]))
        {
            for (int i = 0; i < scene.ComboItems.Length; i++)
            {
                bool isSelected = scene.ComboSelection == i;
                if (ImGui.Selectable(scene.ComboItems[i], isSelected))
                {
                    scene.ComboSelection = i;
                }
                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndCombo();
        }

        ImGui.Spacing();

        // List Box
        ImGui.Text("Список:");
        if (ImGui.BeginListBox("Список элементов", new Vector2(-1, 100)))
        {
            for (int i = 0; i < scene.ListBoxItems.Length; i++)
            {
                bool isSelected = scene.ListBoxSelection == i;
                if (ImGui.Selectable(scene.ListBoxItems[i], isSelected))
                {
                    scene.ListBoxSelection = i;
                }
                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndListBox();
        }

        ImGui.Spacing();

        // Tree
        ImGui.Text("Дерево элементов:");
        if (ImGui.TreeNode("Узел 1"))
        {
            if (ImGui.TreeNode("Подузел 1.1"))
            {
                ImGui.Text("Содержимое подузла 1.1");
                ImGui.TreePop();
            }
            if (ImGui.TreeNode("Подузел 1.2"))
            {
                ImGui.Text("Содержимое подузла 1.2");
                ImGui.TreePop();
            }
            ImGui.TreePop();
        }
        if (ImGui.TreeNode("Узел 2"))
        {
            ImGui.Text("Содержимое узла 2");
            ImGui.TreePop();
        }
    }

    private void RenderColorElements(GuiTestScene scene)
    {
        ImGui.Text("Элементы работы с цветом:");
        ImGui.Separator();

        // Color Picker
        ImGui.Text("Выбор цвета:");
        Vector3 color = scene.ColorPicker;
        if (ImGui.ColorEdit3("Цвет", ref color))
        {
            scene.ColorPicker = color;
        }

        ImGui.Spacing();

        // Color Button
        Vector4 color4 = new Vector4(color.X, color.Y, color.Z, 1.0f);
        if (ImGui.ColorButton("Кнопка цвета", color4))
        {
            // Действие при нажатии
        }

        ImGui.Spacing();

        // Color Picker (full)
        Vector4 fullColor = new Vector4(color.X, color.Y, color.Z, 1.0f);
        if (ImGui.ColorPicker4("Полный выбор цвета", ref fullColor))
        {
            scene.ColorPicker = new Vector3(fullColor.X, fullColor.Y, fullColor.Z);
        }

        ImGui.Spacing();

        // Показ выбранного цвета
        ImGui.Text("Выбранный цвет:");
        ImGui.SameLine();
        ImGui.ColorButton("##preview", color4, ImGuiColorEditFlags.NoTooltip, new Vector2(50, 20));
        ImGui.Text($"RGB: ({color.X:F2}, {color.Y:F2}, {color.Z:F2})");
    }
}

