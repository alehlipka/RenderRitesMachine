namespace RenderRitesMachine.Utilities;

public struct FontCharacterInfo
{
    public int X, Y;          // Позиция в атласе
    public int Width, Height; // Размеры глифа
    public int XOffset;       // Смещение по X
    public int YOffset;       // Смещение по Y
    public int XAdvance;      // Смещение курсора
}