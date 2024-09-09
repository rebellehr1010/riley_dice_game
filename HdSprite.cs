using Godot;
using System;

public partial class HdSprite : Sprite2D
{
    [Export]
    public float TextureScale
    {
        get => _textureScale;
        set => SetTextureScale(value);
    }

    private float _textureScale = 1.0f;

    public float GetTextureScale()
    {
        return _textureScale;
    }

    public void SetTextureScale(float newTextureScale)
    {
        _textureScale = newTextureScale;
        Scale = Vector2.One / _textureScale;
    }
}
