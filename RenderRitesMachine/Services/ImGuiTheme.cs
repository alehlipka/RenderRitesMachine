using ImGuiNET;
using System.Numerics;

namespace RenderRitesMachine.Services;

/// <summary>
/// Кастомная тема для ImGui.
/// </summary>
public static class ImGuiTheme
{
    /// <summary>
    /// Применяет кастомную тему к текущему контексту ImGui.
    /// </summary>
    public static void Apply()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        
        // Цвета темы - темная тема с акцентами
        style.Colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.95f, 0.95f, 1.00f);
        style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.10f, 0.10f, 0.12f, 0.94f);
        style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.08f, 0.08f, 0.10f, 0.00f);
        style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.12f, 0.12f, 0.14f, 0.94f);
        style.Colors[(int)ImGuiCol.Border] = new Vector4(0.25f, 0.25f, 0.30f, 0.50f);
        style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.20f, 0.20f, 0.24f, 0.54f);
        style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.30f, 0.30f, 0.35f, 0.54f);
        style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.35f, 0.35f, 0.40f, 0.54f);
        style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.08f, 0.08f, 0.10f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.15f, 0.15f, 0.18f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.08f, 0.08f, 0.10f, 0.51f);
        style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.12f, 0.12f, 0.14f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.08f, 0.08f, 0.10f, 0.53f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.30f, 0.30f, 0.35f, 0.54f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.40f, 0.40f, 0.45f, 0.54f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.50f, 0.50f, 0.55f, 0.54f);
        style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.40f, 0.70f, 1.00f, 1.00f);
        style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.40f, 0.70f, 1.00f, 1.00f);
        style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.50f, 0.75f, 1.00f, 1.00f);
        style.Colors[(int)ImGuiCol.Button] = new Vector4(0.25f, 0.25f, 0.30f, 0.54f);
        style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.35f, 0.35f, 0.40f, 0.54f);
        style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.45f, 0.45f, 0.50f, 0.54f);
        style.Colors[(int)ImGuiCol.Header] = new Vector4(0.25f, 0.25f, 0.30f, 0.54f);
        style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.35f, 0.35f, 0.40f, 0.54f);
        style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.40f, 0.70f, 1.00f, 0.54f);
        style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.25f, 0.25f, 0.30f, 0.50f);
        style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.35f, 0.35f, 0.40f, 0.50f);
        style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.40f, 0.70f, 1.00f, 0.50f);
        style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.25f, 0.25f, 0.30f, 0.54f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.35f, 0.35f, 0.40f, 0.54f);
        style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.40f, 0.70f, 1.00f, 0.54f);
        style.Colors[(int)ImGuiCol.Tab] = new Vector4(0.20f, 0.20f, 0.24f, 0.86f);
        style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(0.35f, 0.35f, 0.40f, 0.86f);
        style.Colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.40f, 0.70f, 1.00f, 0.30f);
        style.Colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(0.08f, 0.08f, 0.10f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.61f, 0.61f, 0.61f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.00f, 0.43f, 0.35f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.40f, 0.70f, 1.00f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.50f, 0.75f, 1.00f, 1.00f);
        style.Colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.15f, 0.15f, 0.18f, 1.00f);
        style.Colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.25f, 0.25f, 0.30f, 1.00f);
        style.Colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.20f, 0.20f, 0.24f, 1.00f);
        style.Colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        style.Colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1.00f, 1.00f, 1.00f, 0.06f);
        style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.40f, 0.70f, 1.00f, 0.35f);
        style.Colors[(int)ImGuiCol.DragDropTarget] = new Vector4(1.00f, 1.00f, 0.00f, 0.90f);
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
        style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
        style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);
        
        // Настройки стиля
        style.WindowPadding = new Vector2(8.0f, 8.0f);
        style.FramePadding = new Vector2(4.0f, 3.0f);
        style.CellPadding = new Vector2(4.0f, 2.0f);
        style.ItemSpacing = new Vector2(8.0f, 4.0f);
        style.ItemInnerSpacing = new Vector2(4.0f, 4.0f);
        style.TouchExtraPadding = new Vector2(0.0f, 0.0f);
        style.IndentSpacing = 21.0f;
        style.ScrollbarSize = 14.0f;
        style.GrabMinSize = 10.0f;
        
        style.WindowBorderSize = 1.0f;
        style.ChildBorderSize = 1.0f;
        style.PopupBorderSize = 1.0f;
        style.FrameBorderSize = 0.0f;
        style.TabBorderSize = 0.0f;
        
        style.WindowRounding = 4.0f;
        style.ChildRounding = 4.0f;
        style.FrameRounding = 3.0f;
        style.PopupRounding = 4.0f;
        style.ScrollbarRounding = 9.0f;
        style.GrabRounding = 3.0f;
        style.LogSliderDeadzone = 4.0f;
        style.TabRounding = 4.0f;
        
        style.WindowTitleAlign = new Vector2(0.0f, 0.5f);
        style.WindowMenuButtonPosition = ImGuiDir.Right;
        style.ColorButtonPosition = ImGuiDir.Right;
        style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
        style.SelectableTextAlign = new Vector2(0.0f, 0.0f);
        style.DisplaySafeAreaPadding = new Vector2(3.0f, 3.0f);
        
        style.AntiAliasedLines = true;
        style.AntiAliasedLinesUseTex = true;
        style.AntiAliasedFill = true;
        style.CurveTessellationTol = 1.25f;
        style.CircleTessellationMaxError = 0.30f;
    }
}

