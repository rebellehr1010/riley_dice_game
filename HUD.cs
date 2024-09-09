using Godot;
using System;

public partial class HUD : CanvasLayer
{

    [Signal] public delegate void StartGameEventHandler();

    [Signal] public delegate void DiceRollRequestedEventHandler();

    [Signal] public delegate void UpdateScoreEventHandler();
    [Signal] public delegate void KeepDiceRequestedEventHandler();

    public override void _Ready()
    {
        Scorer scorer = GetNode<Scorer>("../Scorer");
        if (scorer != null)
        {
            scorer.ValidScore += OnValidScore;
        }
        else
        {
            GD.PrintErr("Scorer node not found");
        }

        UpdateButtonState("RollButton", false, false);
        UpdateButtonState("KeepButton", false, false);
    }

    public void ShowMessage(string text)
    {
        var message = GetNode<Label>("Message");
        message.Text = text;
        message.Show();
    }

	public void OnStartButtonPressed()
    {
        UpdateButtonState("StartButton", false, false);
        UpdateButtonState("RollButton", true, true);
        EmitSignal(SignalName.StartGame);
    }

    public void OnRollButtonPressed()
    {
        // GetNode<Label>("Message").Hide();
        UpdateButtonState("KeepButton", true, true);
        UpdateButtonState("RollButton", false, false);
        EmitSignal(SignalName.DiceRollRequested);
    }

    public void OnKeepButtonPressed()
    {
        // GetNode<Label>("Message").Hide();
        UpdateButtonState("KeepButton", false, false);
        UpdateButtonState("RollButton", true, true);
        EmitSignal(SignalName.UpdateScore);
        EmitSignal(SignalName.KeepDiceRequested);
    }

    public void OnValidScore(bool isValid)
    {
        GD.Print("Valid score: ", isValid);
        GetNode<Button>("KeepButton").Disabled = !isValid;
    }

    private void UpdateButtonState(string buttonName, bool isVisible, bool isEnabled)
    {
        var button = GetNode<Button>(buttonName);
        if (isVisible)
        {
            button.Show();
        }
        else
        {
            button.Hide();
        }
        button.Disabled = !isEnabled;
    }

    // public override void _UnhandledInput(InputEvent @event)
    // {
    //     if (@event.IsActionPressed("dice_roll"))
    //     {
    //         GetNode<Label>("Message").Hide();
    //     }
    // }

}
