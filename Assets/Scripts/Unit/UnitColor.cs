using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitColor : MonoBehaviour
{
    public enum ColorType
    {
        BLUE,
        PURPLE,
        PINK,
        RED,
        YELLOW,
        ANY,
        COUNT,
    };

    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    };

    public ColorSprite[] colorSprites;

    private Dictionary<ColorType, Sprite> colorSpriteDict;

    private ColorType color;

    public ColorType Color
    {
        get { return color; }
        set { SetColor(value); }
    }
    public int NumColors
    {
        get { return colorSprites.Length; }
    }

    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = transform.Find("enemy").GetComponent<SpriteRenderer>();

        colorSpriteDict = new Dictionary<ColorType, Sprite>();
        {
            for (int i = 0; i < colorSprites.Length; i++)
            {
                if (!colorSpriteDict.ContainsKey(colorSprites[i].color))
                {
                    colorSpriteDict.Add(colorSprites[i].color, colorSprites[i].sprite);
                }
            }
        }
    }

    public void SetColor(ColorType newColor)
    {
        color = newColor;
        if (colorSpriteDict.ContainsKey (newColor))
        {
            sprite.sprite = colorSpriteDict[newColor];
        }
    }
}
