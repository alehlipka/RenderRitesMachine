using OpenTK.Mathematics;
using StbTrueTypeSharp;

namespace RenderRitesMachine.Services.Gui;

/// <summary>
/// Bitmap font baked via stb_truetype for rendering text onto the GUI surface.
/// </summary>
public sealed class GuiFont
{
    private const int FirstChar = 32;
    private const int LastChar = 0x04FF; // 1279
    private const int CharCount = LastChar - FirstChar + 1; // 1248 characters

    private readonly Dictionary<char, Glyph> _glyphs;
    internal byte[] Atlas { get; }

    public int PixelHeight { get; }
    public int AtlasWidth { get; }
    public int AtlasHeight { get; }
    public float LineHeight { get; }
    public float Baseline { get; }

    private GuiFont(int pixelHeight, int atlasWidth, int atlasHeight, byte[] atlas, Dictionary<char, Glyph> glyphs, float lineHeight, float baseline)
    {
        PixelHeight = pixelHeight;
        AtlasWidth = atlasWidth;
        AtlasHeight = atlasHeight;
        Atlas = atlas;
        _glyphs = glyphs;
        LineHeight = lineHeight;
        Baseline = baseline;
    }

    public static GuiFont LoadFromFile(string path, int pixelHeight = 20, int atlasWidth = 1024, int atlasHeight = 1024)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Font file not found at '{path}'.", path);
        }

        byte[] fontData = File.ReadAllBytes(path);
        return LoadFromMemory(fontData, pixelHeight, atlasWidth, atlasHeight);
    }

    public static GuiFont LoadFromMemory(byte[] fontData, int pixelHeight = 20, int atlasWidth = 1024, int atlasHeight = 1024)
    {
        ArgumentNullException.ThrowIfNull(fontData);

        byte[] atlas = new byte[atlasWidth * atlasHeight];
        var glyphs = new Dictionary<char, Glyph>();

        float lineHeight = pixelHeight * 1.25f;
        float baseline = pixelHeight;

        unsafe
        {
            fixed (byte* fontPtr = fontData)
            fixed (byte* atlasPtr = atlas)
            {
                var bakedChars = new StbTrueType.stbtt_bakedchar[CharCount];
                fixed (StbTrueType.stbtt_bakedchar* bakedPtr = bakedChars)
                {
                    int result = StbTrueType.stbtt_BakeFontBitmap(fontPtr, 0, pixelHeight, atlasPtr, atlasWidth, atlasHeight, FirstChar, CharCount, bakedPtr);
                    if (result <= 0)
                    {
                        throw new InvalidOperationException("Failed to bake font bitmap.");
                    }

                    for (int i = 0; i < CharCount; i++)
                    {
                        char ch = (char)(FirstChar + i);
                        StbTrueType.stbtt_bakedchar baked = bakedChars[i];

                        bool hasBitmap = baked.x1 > baked.x0 && baked.y1 > baked.y0;
                        bool hasAdvance = Math.Abs(baked.xadvance) > float.Epsilon;
                        if (!hasBitmap && !hasAdvance)
                        {
                            continue;
                        }

                        glyphs[ch] = new Glyph(
                            baked.x0,
                            baked.y0,
                            baked.x1,
                            baked.y1,
                            baked.xoff,
                            baked.yoff,
                            baked.xadvance
                        );
                    }
                }
            }
        }

        return new GuiFont(pixelHeight, atlasWidth, atlasHeight, atlas, glyphs, lineHeight, baseline);
    }

    public Vector2 MeasureText(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Vector2.Zero;
        }

        float lineWidth = 0f;
        float maxWidth = 0f;
        int lines = 1;

        foreach (char ch in text)
        {
            if (ch == '\n')
            {
                maxWidth = Math.Max(maxWidth, lineWidth);
                lineWidth = 0f;
                lines++;
                continue;
            }

            if (TryGetGlyph(ch, out Glyph glyph))
            {
                lineWidth += glyph.XAdvance;
            }
        }

        maxWidth = Math.Max(maxWidth, lineWidth);
        float height = lines * LineHeight;
        return new Vector2(maxWidth, height);
    }

    internal bool TryGetGlyph(char character, out Glyph glyph)
    {
        return _glyphs.TryGetValue(character, out glyph);
    }

    internal readonly struct Glyph
    {
        public Glyph(int x0, int y0, int x1, int y1, float xOffset, float yOffset, float xAdvance)
        {
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
            XOffset = xOffset;
            YOffset = yOffset;
            XAdvance = xAdvance;
        }

        public int X0 { get; }
        public int Y0 { get; }
        public int X1 { get; }
        public int Y1 { get; }
        public float XOffset { get; }
        public float YOffset { get; }
        public float XAdvance { get; }
        public int Width => X1 - X0;
        public int Height => Y1 - Y0;
    }
}
