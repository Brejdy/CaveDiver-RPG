using System.Globalization;
using System.Text;
using CaveDiver.Models;

namespace CaveDiver.Engine;

internal static class CommandParser
{
    public enum ExplorationAction
    {
        Unknown,
        LookAround,
        Trade,
        Battle,
        Travel,
        Rest,
        Talk,
        Exit
    }

    public enum BattleAction
    {
        Unknown,
        Attack,
        Run
    }

    public enum MeetingAction
    {
        Unknown,
        Join,
        Decline,
        Talk
    }

    public sealed class BattleCommand
    {
        public BattleAction Action { get; }
        public int? TargetIndex { get; }

        public BattleCommand(BattleAction action, int? targetIndex = null)
        {
            Action = action;
            TargetIndex = targetIndex;
        }
    }

    public static ExplorationAction ParseExplorationAction(string? input, bool hasMerchant, bool hasEnemies, bool hasTravel)
    {
        var normalized = Normalize(input);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return ExplorationAction.Unknown;
        }

        if (normalized == "1" || ContainsAny(normalized, "look", "observe", "around"))
        {
            return ExplorationAction.LookAround;
        }

        if (hasMerchant && (normalized == "2" || ContainsAny(normalized, "trade", "merchant", "shop", "buy", "sell")))
        {
            return ExplorationAction.Trade;
        }

        if (hasEnemies && (normalized == "3" || ContainsAny(normalized, "battle", "fight", "attack", "combat")))
        {
            return ExplorationAction.Battle;
        }

        if (hasTravel && (normalized == "4" || ContainsAny(normalized, "travel", "move", "go", "walk")))
        {
            return ExplorationAction.Travel;
        }

        if (normalized == "5" || ContainsAny(normalized, "rest", "sleep", "recover"))
        {
            return ExplorationAction.Rest;
        }

        if (normalized == "7" || ContainsAny(normalized, "talk", "speak", "chat"))
        {
            return ExplorationAction.Talk;
        }

        if (normalized == "6" || ContainsAny(normalized, "leave", "exit", "quit", "end"))
        {
            return ExplorationAction.Exit;
        }

        return ExplorationAction.Unknown;
    }

    public static BattleCommand ParseBattleCommand(string? input, List<Enemy> enemies)
    {
        var normalized = Normalize(input);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return new BattleCommand(BattleAction.Unknown);
        }

        if (ContainsAny(normalized, "run", "escape", "flee"))
        {
            return new BattleCommand(BattleAction.Run);
        }

        if (normalized.StartsWith("attack") || normalized == "a")
        {
            var targetIndex = ResolveEnemyTarget(normalized, enemies);
            return new BattleCommand(BattleAction.Attack, targetIndex);
        }

        return new BattleCommand(BattleAction.Unknown);
    }

    public static bool TryResolveTravelDestination(string? input, List<Location> connectedLocations, out int index, out bool stayHere)
    {
        index = -1;
        stayHere = false;

        var normalized = Normalize(input);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return false;
        }

        if (ContainsAny(normalized, "stay", "cancel"))
        {
            stayHere = true;
            return true;
        }

        var numberToken = ExtractFirstNumber(normalized);
        if (numberToken.HasValue)
        {
            var selected = numberToken.Value - 1;
            if (selected >= 0 && selected < connectedLocations.Count)
            {
                index = selected;
                return true;
            }

            if (numberToken.Value == connectedLocations.Count + 1)
            {
                stayHere = true;
                return true;
            }

            return false;
        }

        for (int i = 0; i < connectedLocations.Count; i++)
        {
            var locationName = Normalize(connectedLocations[i].Name);
            if (normalized.Contains(locationName))
            {
                index = i;
                return true;
            }
        }

        return false;
    }

    private static int? ResolveEnemyTarget(string normalizedInput, List<Enemy> enemies)
    {
        var numberToken = ExtractFirstNumber(normalizedInput);
        if (numberToken.HasValue)
        {
            var byNumber = numberToken.Value - 1;
            if (byNumber >= 0 && byNumber < enemies.Count)
            {
                return byNumber;
            }
        }

        var aliveEnemies = enemies
            .Select((enemy, idx) => new { Enemy = enemy, Index = idx })
            .Where(x => x.Enemy.IsAlive)
            .ToList();

        foreach (var candidate in aliveEnemies)
        {
            var enemyName = Normalize(candidate.Enemy.Name);
            if (normalizedInput.Contains(enemyName))
            {
                return candidate.Index;
            }
        }

        var words = normalizedInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var candidate in aliveEnemies)
        {
            var enemyWords = Normalize(candidate.Enemy.Name).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (enemyWords.Any(w => words.Contains(w)))
            {
                return candidate.Index;
            }
        }

        return aliveEnemies.FirstOrDefault()?.Index;
    }

    private static int? ExtractFirstNumber(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (int.TryParse(part, out var value))
            {
                return value;
            }
        }

        return null;
    }

    private static bool ContainsAny(string input, params string[] terms)
    {
        return terms.Any(input.Contains);
    }

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
