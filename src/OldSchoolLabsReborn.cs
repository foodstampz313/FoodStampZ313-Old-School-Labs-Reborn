using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;

namespace FoodStampZ313s.OldSchoolLabsReborn;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.foodstampz313.oldschoollabsreborn";
    public override string Name { get; init; } = "FoodStampZ313's Old School Labs Reborn";
    public override string Author { get; init; } = "FoodStampZ313";
    public override List<string>? Contributors { get; init; } = ["OpenAI ChatGPT - technical framework assistance", "MrFums - original concept inspiration"];
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class OldSchoolLabsReborn(
    DatabaseServer databaseServer,
    ISptLogger<OldSchoolLabsReborn> logger,
    ModHelper modHelper) : IOnLoad
{
    public Task OnLoad()
    {
        var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var cfg = modHelper.GetJsonDataFromFile<ModConfig>(modPath, "config/config.json");
        var tables = databaseServer.GetTables();

        var items = tables.Templates.Items;
        dynamic templates = tables.Templates;
        dynamic prices = templates.Prices;
        dynamic handbookItems = templates.Handbook.Items;

        var changed = 0;
        var prefix = $"[{cfg.LoggerPrefix}]";
        var usageValue = Math.Max(0, cfg.UnlimitedUsesValue);

        foreach (var pair in cfg.Keycards)
        {
            var id = new MongoId(pair.Key);
            var card = pair.Value;

            if (!items.TryGetValue(id, out TemplateItem? item) || item is null)
            {
                logger.LogWithColor($"{prefix} Missing {card.Name} keycard: {pair.Key}", LogTextColor.Yellow, LogBackgroundColor.Black);
                continue;
            }

            if (cfg.EnableUnlimitedUses)
            {
                item.Properties.MaximumNumberOfUsage = usageValue;
            }

            if (cfg.EnablePriceChanges)
            {
                TrySetDynamicDictionaryValue(prices, id, card.Price);
                TrySetHandbookPrice(handbookItems, id, card.Price);
            }

            changed++;
            logger.LogWithColor($"{prefix} Loaded {card.Name} keycard", LogTextColor.Green, LogBackgroundColor.Black);
        }

        logger.Success($"{prefix} Successfully modified {changed} Labs keycards.");
        return Task.CompletedTask;
    }

    private void TrySetDynamicDictionaryValue(dynamic dictionary, MongoId id, int price)
    {
        try
        {
            dictionary[id] = price;
            return;
        }
        catch
        {
            // Some SPT dictionaries are keyed by MongoId and some by string depending on the table.
        }

        try
        {
            dictionary[id.ToString()] = price;
        }
        catch (Exception ex)
        {
            logger.LogWithColor($"[FoodStampZ313 Labs Reborn] Could not update price table for {id}: {ex.Message}", LogTextColor.Yellow, LogBackgroundColor.Black);
        }
    }

    private void TrySetHandbookPrice(dynamic handbookItems, MongoId id, int price)
    {
        try
        {
            foreach (dynamic entry in handbookItems)
            {
                if ($"{entry.Id}" == id.ToString())
                {
                    entry.Price = price;
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWithColor($"[FoodStampZ313 Labs Reborn] Could not update handbook price for {id}: {ex.Message}", LogTextColor.Yellow, LogBackgroundColor.Black);
        }
    }
}

public record ModConfig
{
    public bool EnableUnlimitedUses { get; set; } = true;
    public int UnlimitedUsesValue { get; set; } = 999999;
    public bool EnablePriceChanges { get; set; } = true;
    public bool EnableSpawnBuffs { get; set; } = false;
    public bool EnableTherapistSales { get; set; } = false;
    public string LoggerPrefix { get; set; } = "FoodStampZ313 Labs Reborn";
    public Dictionary<string, KeycardConfig> Keycards { get; set; } = new();
}

public record KeycardConfig
{
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
}
