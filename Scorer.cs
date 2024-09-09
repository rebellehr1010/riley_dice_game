using Godot;
using Godot.Collections;
using System.Linq;

public partial class Scorer : Node
{
    [Signal] public delegate void ValidScoreEventHandler(bool isValid);

    [Signal] public delegate void AllDiceKeptEventHandler();

    [Signal] public delegate void NoScoreEventHandler();

    private int[] _diceValues = new int[6];

    private bool[] _diceKept = new bool[6];

    private Label _scoreLabel;
    public int PreviousScore = 0;
    public int CurrentScore = 0;
    public int TotalScore = 0;

    public override void _Ready()
    {
        for (int i = 1; i <= 6; i++)
        {
            // Find each Dice node by name
            Dice dice = GetNode<Dice>($"../Dice{i}");
            if (dice != null)
            {
                dice.DiceKept += OnDiceKept;
                dice.DiceSelected += OnDiceSelected;
            }
        }
        HUD hud = GetNode<HUD>("../HUD");
        if (hud != null)
        {
            hud.UpdateScore += OnUpdateScore;
        }
        else
        {
            GD.PrintErr("HUD node not found");
        }

    }

    // Called when the node enters the scene tree for the first time.
    public void OnDiceSelected(string diceName, int rolledNumber)
    {
        int index = GetDiceIndexFromName(diceName);

        if (index >= 0 && index < _diceValues.Length)
        {
            _diceValues[index] = rolledNumber;
            GD.Print("Dice values updated: ", string.Join(", ", _diceValues));
            UpdateScore();
            PreviousScore = CurrentScore;
        }
        else
        {
            GD.PrintErr($"Dice name '{diceName}' is not valid or index out of range: {index}");
        }
    }

    public void OnDiceKept(string diceName, bool isKept)
    {
        int index = GetDiceIndexFromName(diceName);

        if (index >= 0 && index < _diceKept.Length)
        {
            _diceKept[index] = isKept;
            GD.Print("Dice kept updated: ", string.Join(", ", _diceKept));
            if (_diceKept.All(kept => kept))
            {
                GD.Print("All dice kept.");
                EmitSignal(nameof(AllDiceKept));
                for (int i = 0; i < _diceKept.Length; i++)
                {
                    _diceKept[i] = false;
                }
            }
        }
        else
        {
            GD.PrintErr($"Dice name '{diceName}' is not valid or index out of range: {index}");
        }
    }

    public int GetDiceIndexFromName(string name)
    {
        // Assuming names are in the format Dice1, Dice2, etc.
        if (name.StartsWith("Dice"))
        {
            string indexStr = name.Substring(4); // Extracts the number part
            if (int.TryParse(indexStr, out int index))
            {
                return index - 1; // Convert to 0-based index
            }
        }
        return -1; // Invalid index
    }

    public void OnUpdateScore()
    {
        GD.Print("Current score kept: ", CurrentScore);
        if (CurrentScore == 0)
        {
            GD.Print("No valid score to keep. You Lose!");
            EmitSignal(nameof(NoScore));
            return;
        }
        GD.Print("Score kept: ", CurrentScore);
        TotalScore += CurrentScore;
        CurrentScore = 0;
        GetNode<Label>("ScoreLabel").Text = $"Total Score: {TotalScore}\nCurrent Score: {CurrentScore}";
        // EmitSignal(nameof(ValidScore), false);
        // UpdateScore();
    }

    private void UpdateScore()
    {
        CurrentScore = CalculateScore();
        GetNode<Label>("ScoreLabel").Text = $"Total Score: {TotalScore}\nCurrent Score: {CurrentScore}";
        GD.Print("Current score kept: ", CurrentScore);
        EmitSignal(nameof(ValidScore), CurrentScore > 0);

    }

    private int CalculateScore()
    {
        // Dictionary to count occurrences of each dice value
        var diceCount = new Dictionary<int, int>();

        foreach (int value in _diceValues)
        {
            if (diceCount.ContainsKey(value))
            {
                diceCount[value]++;
            }
            else
            {
                diceCount[value] = 1;
            }
        }

        int score = 0;

        // Calculate score based on the counts
        foreach (var kvp in diceCount)
        {
            int value = kvp.Key;
            int count = kvp.Value;
            int baseScore = 0;

            if (count >= 3)
            {
                // Set base score for triples
                if (value == 1)
                {
                    baseScore = 1000; // Triple 1s worth 1000 points
                }
                else
                {
                    baseScore = value * 100; // Other triples worth value * 100 points
                }

                if (count == 4)
                {
                    score += baseScore * 2; // Quadruple score for four of a kind
                }
                else if (count >= 5)
                {
                    score += baseScore * 4; // Quadruple the score again for five of a kind
                }
                else
                {
                    score += baseScore; // Regular triple score
                }

                // Remove all counted dice for 3 or more of a kind
                count -= count;
            }

            // For remaining 1s and 5s after scoring triples or more
            if (value == 1)
            {
                score += count * 100; // Each remaining 1 is worth 100 points
            }
            else if (value == 5)
            {
                score += count * 50; // Each remaining 5 is worth 50 points
            }
        }

        return score;
    }



}
