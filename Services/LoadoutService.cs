using HellDiversLoadoutRandomizer.Data;
using HellDiversLoadoutRandomizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellDiversLoadoutRandomizer.Services;

public class LoadoutService
{
    private readonly StratagemContext _context;
    private readonly Random _random;

    public LoadoutService(StratagemContext context)
    {
        _context = context;
        _random = new Random();
    }

    // Generate a random loadout of 4 stratagems
    public async Task<List<Stratagem>> GenerateRandomLoadoutAsync()
    {
        var allStratagems = await _context.Stratagems.ToListAsync();
        return allStratagems.OrderBy(x => _random.Next()).Take(4).ToList();
    }

    // Evaluate synergy score for a given loadout
    public async Task<double> EvaluateLoadoutSynergyAsync(List<int> stratagemIds)
    {
        if (stratagemIds.Count != 4)
            throw new ArgumentException("Loadout must contain exactly 4 stratagems");

        var stratagemIdSet = stratagemIds.ToHashSet();
        double totalSynergy = 0;

        // Get all hyperedges that are satisfied by this loadout
        var relevantHyperedges = await _context.Hyperedges
            .Include(h => h.Members)
            .Where(h => h.Members.All(m => stratagemIdSet.Contains(m.StratagemId)))
            .ToListAsync();

        foreach (var hyperedge in relevantHyperedges)
        {
            // Only count hyperedges where ALL members are in the loadout
            var hyperedgeStratagemIds = hyperedge.Members.Select(m => m.StratagemId).ToHashSet();
            if (hyperedgeStratagemIds.IsSubsetOf(stratagemIdSet))
            {
                totalSynergy += hyperedge.SynergyWeight;
            }
        }

        return totalSynergy;
    }

    // Get stratagems with specific tags
    public async Task<List<Stratagem>> GetStratagemsByTagAsync(string tag)
    {
        return await _context.Stratagems
            .Where(s => s.Tags.Any(t => t.Tag == tag))
            .ToListAsync();
    }

    // Get loadout balance analysis
    public async Task<LoadoutAnalysis> AnalyzeLoadoutBalanceAsync(List<int> stratagemIds)
    {
        var stratagems = await _context.Stratagems
            .Include(s => s.Tags)
            .Where(s => stratagemIds.Contains(s.Id))
            .ToListAsync();

        var analysis = new LoadoutAnalysis
        {
            StratagemIds = stratagemIds,
            SynergyScore = await EvaluateLoadoutSynergyAsync(stratagemIds)
        };

        // Analyze tag distribution
        var allTags = stratagems.SelectMany(s => s.Tags.Select(t => t.Tag)).ToList();
        analysis.TagCounts = allTags.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());

        // Check for role coverage
        analysis.HasAntiTank = allTags.Any(t => t.Contains("Anti-Tank") || t.Contains("Anti-Armor"));
        analysis.HasCrowdControl = allTags.Any(t => t.Contains("Crowd Control") || t.Contains("Area"));
        analysis.HasSupport = allTags.Any(t => t.Contains("Support") || t.Contains("Defensive"));

        return analysis;
    }

    // Save a loadout test result
    public async Task SaveLoadoutTestAsync(List<int> stratagemIds, double effectivenessRating,
        string missionType, string difficulty, string notes = "")
    {
        var test = new LoadoutTest
        {
            TestDate = DateTime.UtcNow,
            StratagemIds = string.Join(",", stratagemIds),
            SynergyScore = await EvaluateLoadoutSynergyAsync(stratagemIds),
            EffectivenessRating = effectivenessRating,
            MissionType = missionType,
            Difficulty = difficulty,
            Notes = notes
        };

        _context.LoadoutTests.Add(test);
        await _context.SaveChangesAsync();
    }
}

public class LoadoutAnalysis
{
    public List<int> StratagemIds { get; set; } = new();
    public double SynergyScore { get; set; }
    public Dictionary<string, int> TagCounts { get; set; } = new();
    public bool HasAntiTank { get; set; }
    public bool HasCrowdControl { get; set; }
    public bool HasSupport { get; set; }

    public string GetBalanceDescription()
    {
        var issues = new List<string>();

        if (!HasAntiTank) issues.Add("No anti-tank capability");
        if (!HasCrowdControl) issues.Add("Limited crowd control");
        if (!HasSupport) issues.Add("No support options");

        return issues.Count == 0 ? "Well-balanced loadout" : $"Issues: {string.Join(", ", issues)}";
    }
}
