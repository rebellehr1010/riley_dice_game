using Godot;
using System;

public partial class Dice : Node2D
{
    [Signal] public delegate void DiceSelectedEventHandler(string Name, int rolledNumber);

    [Signal] public delegate void DiceKeptEventHandler(string Name, bool diceKept);

    [Export] public NodePath SymbolPath { get; set; }

    [Export] public NodePath SymbolAnimationPlayerPath { get; set; }

    private Node2D _symbol;
    private AnimationPlayer _symbolAnimationPlayer;
    private RandomNumberGenerator _rng = new RandomNumberGenerator();

    [Export] public string backgroundTexturePath = "res://dice/background.svg";

    [Export] public string pressedBackgroundTexturePath = "res://dice/pressed_background.svg";

    private Texture2D _backgroundTexture;
    private Texture2D _pressedBackgroundTexture;

    public int RolledNumber { get; private set; }

    private bool _diceKept = false;

    public override void _Ready()
    {
        _symbol = GetNode<Node2D>(SymbolPath);
        _symbolAnimationPlayer = GetNode<AnimationPlayer>(SymbolAnimationPlayerPath);
        
        _backgroundTexture = GD.Load<Texture2D>(backgroundTexturePath);
        _pressedBackgroundTexture = GD.Load<Texture2D>(pressedBackgroundTexturePath);

        Scorer scorer = GetNode<Scorer>("../Scorer");
        if (scorer != null)
        {
            scorer.AllDiceKept += OnAllDiceKept;
        }
        else
        {
            GD.PrintErr("Scorer node not found");
        }
        

        HUD hud = GetNode<HUD>("../HUD");
        if (hud != null)
        {
            hud.StartGame += OnHUDStartGame;
            hud.KeepDiceRequested += OnKeepDiceRequested;
            hud.DiceRollRequested += OnHUDDiceRollRequested;
        }
        else
        {
            GD.PrintErr("HUD node not found");
        }

        Hide();
    }

    public int Roll()
    {
        GetNode<Sprite2D>("Visual/Background/HDSprite").Texture = _backgroundTexture;
        GetNode<Button>("DiceButton").Disabled = false; 
        GetNode<Button>("DiceButton").ToggleMode = false; 
        // EmitSignal(nameof(DiceSelected), Name, 0);
        Show();
        _rng.Randomize();
        RolledNumber = _rng.RandiRange(1, 6);
        _symbolAnimationPlayer.Play("Roll");
        // EmitSignal(nameof(DiceRolled), RolledNumber);
        GetNode<Button>("DiceButton").ToggleMode = true; 
        return RolledNumber;
    }

    public void OnDiceButtonToggled(bool isPressed)
    {   
        GetNode<Sprite2D>("Visual/Background/HDSprite").Texture = isPressed ? _pressedBackgroundTexture : _backgroundTexture;
        // If the dice is pressed, send its current value as a signal
        EmitSignal(nameof(DiceSelected), Name, isPressed ? RolledNumber : 0);
    }

    public void OnHUDStartGame()
    {
        Show();
        GetNode<Button>("DiceButton").ToggleMode = false; 
        GetNode<Button>("DiceButton").Disabled = true; 
    }

    public void OnKeepDiceRequested()
    {
        if (GetNode<Button>("DiceButton").IsPressed())
        {
            _diceKept = true;
            // GD.Print("Hiding dice ", Name, DiceKept);
            Hide();
            EmitSignal(nameof(DiceKept), Name, true);
        }
        GetNode<Button>("DiceButton").Disabled = true; 
        GetNode<Button>("DiceButton").ToggleMode = false; 
        
    }

    public void OnAllDiceKept()
    {
        _diceKept = false;
        Show();
        GetNode<Button>("DiceButton").Disabled = false; 
        GetNode<Button>("DiceButton").ToggleMode = false; 
    }

    public void UpdateSymbol()
    {
        _symbol.Call("SetSymbol", RolledNumber);
    }

    public void OnHUDDiceRollRequested()
    {
        // GD.Print(Name, "DiceKept", DiceKept);
        if (!_diceKept)
        {
            Roll();
        }
    }
}
