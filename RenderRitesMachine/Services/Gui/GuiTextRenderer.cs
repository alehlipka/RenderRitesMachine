using OpenTK.Mathematics;

namespace RenderRitesMachine.Services.Gui;

internal sealed class GuiTextRenderer
{
    public void DrawText(GuiSurface surface, GuiFont font, string? text, int x, int y, Color4 color)
    {
        if (surface == null)
        {
            throw new ArgumentNullException(nameof(surface));
        }

        if (font == null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        float cursorX = x;
        float cursorY = y + font.Baseline;

        foreach (char ch in text)
        {
            if (ch == '\n')
            {
                cursorX = x;
                cursorY += font.LineHeight;
                continue;
            }

            if (!font.TryGetGlyph(ch, out GuiFont.Glyph glyph))
            {
                continue;
            }

            int destX = (int)MathF.Round(cursorX + glyph.XOffset);
            int destY = (int)MathF.Round(cursorY + glyph.YOffset);
            BlitGlyph(surface, font, glyph, destX, destY, color);
            cursorX += glyph.XAdvance;
        }
    }

    private static void BlitGlyph(GuiSurface surface, GuiFont font, GuiFont.Glyph glyph, int destX, int destY, Color4 color)
    {
        for (int row = 0; row < glyph.Height; row++)
        {
            int srcY = glyph.Y0 + row;
            for (int col = 0; col < glyph.Width; col++)
            {
                int srcX = glyph.X0 + col;
                int atlasIndex = (srcY * font.AtlasWidth) + srcX;
                byte coverage = font.Atlas[atlasIndex];
                float alpha = coverage / 255f;
                if (alpha <= 0f)
                {
                    continue;
                }

                surface.BlendPixel(destX + col, destY + row, color, alpha);
            }
        }
    }
}

