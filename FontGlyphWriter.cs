using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FontGlyphWriter : MonoBehaviour
{
    [SerializeField] Font font;
    [SerializeField] string charset = "";
    
    [ContextMenu("Write glyphs")]
    public void Write()
    {
        if (font.dynamic)
            font.RequestCharactersInTexture(charset);

        CharacterInfo[] infos       = font.characterInfo;
        Texture2D       fontTexture = (Texture2D) font.material.mainTexture;
        RenderTexture   tmp         = new RenderTexture(fontTexture.width, fontTexture.height, 0);
        Graphics.Blit(fontTexture, tmp);

        Texture2D fontAtlas = new Texture2D(fontTexture.width, fontTexture.height);
        RenderTexture.active = tmp;

        fontAtlas.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        fontAtlas.Apply();

        string path;
        if (infos.Length == 0)
        {
            Debug.LogError("There's no glyphs in the font");
            return;
        }

        path = EditorUtility.OpenFolderPanel("Load glyphs", "", "");

        foreach (CharacterInfo info in infos)
        {
            Texture2D glyph;

            bool rotated = Mathf.Abs(info.uvBottomLeft.y - info.uvBottomRight.y) > Mathf.Epsilon;

            Rect rect;

            if (rotated)
            {
                glyph = new Texture2D(info.maxY - info.minY, info.maxX - info.minX);

                float width  = info.uvTopLeft.x    - info.uvBottomLeft.x;
                float height = info.uvBottomLeft.y - info.uvBottomRight.y;

                rect = new Rect(
                        info.uvBottomLeft.x  * fontTexture.width,
                        info.uvBottomRight.y * fontTexture.height,
                        width                * fontTexture.width,
                        height               * fontTexture.height
                    );
            }
            else
            {
                glyph = new Texture2D(info.maxX - info.minX, info.maxY - info.minY);
                float width  = Math.Abs(info.uvBottomRight.x - info.uvBottomLeft.x);
                float height = Math.Abs(info.uvBottomLeft.y  - info.uvTopLeft.y);

                rect = new Rect(
                        info.uvTopLeft.x * fontTexture.width,
                        info.uvTopLeft.y * fontTexture.height,
                        width            * fontTexture.width,
                        height           * fontTexture.height
                    );
            }

            glyph.ReadPixels(rect, 0, 0);

            if (rotated)
                glyph = RotateTexture(glyph, false);
            else
            {
                glyph = FlipTexture(glyph, false);
                glyph.SetPixels(glyph.GetPixels().Reverse().ToArray());
            }

            byte[] bytes = glyph.EncodeToPNG();

            File.WriteAllBytes(string.Format("{0}/glyph{1}.png", path, Convert.ToChar(info.index)), bytes);
        }
    }

    Texture2D FlipTexture(Texture2D original, bool upSideDown = true)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);

        int xN = original.width;
        int yN = original.height;


        for (int i = 0; i < xN; i++)
        {
            for (int j = 0; j < yN; j++)
            {
                if (upSideDown)
                {
                    flipped.SetPixel(j, xN - i - 1, original.GetPixel(j, i));
                }
                else
                {
                    flipped.SetPixel(xN - i - 1, j, original.GetPixel(i, j));
                }
            }
        }

        flipped.Apply();

        return flipped;
    }

    Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated  = new Color32[original.Length];
        int       w        = originalTexture.width;
        int       h        = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated          = (i + 1) * h - j - 1;
                iOriginal         = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
}
