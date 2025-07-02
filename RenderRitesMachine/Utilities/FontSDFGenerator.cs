using StbTrueTypeSharp;

namespace RenderRitesMachine.Utilities;

public class FontSDFGenerator
{
    public static Texture2D GenerateSDFFontAtlas(string fontPath, int fontSize, int textureSize, 
        out Dictionary<char, FontCharacterInfo> charInfos)
    {
        charInfos = new Dictionary<char, FontCharacterInfo>();
        
        // Загрузка шрифта
        byte[] fontData = File.ReadAllBytes(fontPath);
        StbTrueType.stbtt_fontinfo font = new StbTrueType.stbtt_fontinfo();
        if (!StbTrueType.stbtt_InitFont(font, fontData, 0))
            throw new Exception("Failed to load font");
        
        // Масштаб для заданного размера шрифта
        float scale = StbTrueType.stbtt_ScaleForPixelHeight(font, fontSize);
        
        // Определение глифов для рендеринга (включая кириллицу)
        var charsToRender = GetCyrillicChars();
        
        // Создание битмапа для атласа
        byte[] bitmap = new byte[textureSize * textureSize];
        
        // Позиция текущего глифа в атласе
        int x = 1, y = 1, maxY = 0;
        
        foreach (char c in charsToRender)
        {
            int glyph = StbTrueType.stbtt_FindGlyphIndex(font, c);
            if (glyph == 0) continue;
            
            // Получение метрик глифа
            int advance, lsb;
            StbTrueType.stbtt_GetGlyphHMetrics(font, glyph, out advance, out lsb);
            
            int x0, y0, x1, y1;
            StbTrueType.stbtt_GetGlyphBitmapBox(font, glyph, scale, scale, out x0, out y0, out x1, out y1);
            
            int glyphWidth = x1 - x0;
            int glyphHeight = y1 - y0;
            
            // Перенос на следующую строку, если не хватает места
            if (x + glyphWidth + 1 >= textureSize)
            {
                x = 1;
                y += maxY + 1;
                maxY = 0;
            }
            
            // Рендеринг SDF глифа
            byte[] glyphBitmap = new byte[glyphWidth * glyphHeight];
            StbTrueType.stbtt_MakeGlyphSDF(font, scale, glyph, 5, 0.5f, glyphBitmap, glyphWidth, glyphHeight, 0);
            
            // Копирование глифа в атлас
            for (int gy = 0; gy < glyphHeight; gy++)
            {
                for (int gx = 0; gx < glyphWidth; gx++)
                {
                    int atlasPos = (y + gy) * textureSize + (x + gx);
                    bitmap[atlasPos] = glyphBitmap[gy * glyphWidth + gx];
                }
            }
            
            // Сохранение информации о глифе
            charInfos[c] = new FontCharacterInfo
            {
                X = x,
                Y = y,
                Width = glyphWidth,
                Height = glyphHeight,
                XOffset = x0,
                YOffset = y0,
                XAdvance = (int)(advance * scale)
            };
            
            x += glyphWidth + 1;
            if (glyphHeight > maxY) maxY = glyphHeight;
        }
        
        // Создание текстуры OpenGL
        var texture = new Texture2D();
        texture.LoadFromMemory(bitmap, textureSize, textureSize, PixelInternalFormat.R8, PixelFormat.Red);
        texture.SetFilter(TextureMinFilter.Linear, TextureMagFilter.Linear);
        
        return texture;
    }
    
    private static IEnumerable<char> GetCyrillicChars()
    {
        // Базовый набор кириллицы
        for (char c = 'А'; c <= 'я'; c++)
            yield return c;
        
        // Дополнительные символы
        yield return 'Ё';
        yield return 'ё';
        
        // Базовый набор ASCII
        for (char c = ' '; c <= '~'; c++)
            yield return c;
    }
}