using Godot;
using System;

public partial class Symbol : Node2D
{
    private Sprite2D _sprite;

    // Enumeration for symbols
    public enum SymbolEnum
    {
        NONE = 0,
        ONE = 1,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX
    }

    public override void _Ready()
    {
        // Initialize _sprite with the node from the scene
        _sprite = GetNode<Sprite2D>("HDSprite");
    }

    public void SetSymbol(int newSymbol)
    {
        // Set the frame of the sprite to the new symbol value
        _sprite.Frame = newSymbol;
    }
}
