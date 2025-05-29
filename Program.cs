using HellDiversLoadoutRandomizer.Data;
using HellDiversLoadoutRandomizer.Models;
using HellDiversLoadoutRandomizer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HellDiversLoadoutRandomizer;

class Program
{
    // This builds the host immediately because this is just a console app.  If you want to deploy this to Azure you'll need to swap to a var host = HostBuilder.Configure...().ConfigureAppConfig()... type setup.
    static async Task Main(string[] args)
    {
        Console.WriteLine("🔥 HELLDIVERS 2 STRATAGEM SYNERGY ANALYZER 🔥");
        Console.WriteLine("==============================================");
        Console.WriteLine();

        // Set up dependency injection
        var host = CreateHostBuilder(args).Build();

        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StratagemContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        var hypergraph = scope.ServiceProvider.GetRequiredService<StratagemHypergraph>();

        // Initialize database
        await context.Database.EnsureCreatedAsync();
        await seeder.SeedDataAsync();

        // Main menu loop
        bool running = true;
        while (running)
        {
            DisplayMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await GenerateRandomLoadout(hypergraph);
                    break;
                case "2":
                    await AnalyzeCustomLoadout(hypergraph, context);
                    break;
                case "3":
                    await ViewStratagemsByTag(context);
                    break;
                case "4":
                    await TestLoadout(hypergraph);
                    break;
                case "5":
                    await ViewTestResults(context);
                    break;
                case "6":
                    await ViewAllStratagems(context);
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            if (running)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        Console.WriteLine("\nFor Super Earth! 🌎");
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<StratagemContext>();
                services.AddScoped<DataSeeder>();
                services.AddScoped<StratagemHypergraph>();
            });

    static void DisplayMenu()
    {
        Console.WriteLine("\n🎯 STRATAGEM ANALYZER MENU");
        Console.WriteLine("1. Generate Random Loadout");
        Console.WriteLine("2. Analyze Custom Loadout");
        Console.WriteLine("3. View Stratagems by Tag");
        Console.WriteLine("4. Test Loadout Effectiveness");
        Console.WriteLine("5. View Test Results");
        Console.WriteLine("6. View All Stratagems");
        Console.WriteLine("0. Exit");
        Console.Write("\nChoose an option: ");
    }

    static async Task GenerateRandomLoadout(StratagemHypergraph hypergraph)
    {
        Console.WriteLine("\n🎲 GENERATING RANDOM LOADOUT...");
        Console.WriteLine("================================");

        var loadout = await hypergraph.GenerateRandomLoadoutAsync();
        var stratagemIds = loadout.Select(s => s.Id).ToList();
        var synergyScore = await hypergraph.EvaluateLoadoutSynergyAsync(stratagemIds);
        var analysis = await hypergraph.AnalyzeLoadoutBalanceAsync(stratagemIds);

        Console.WriteLine("\n🔥 YOUR RANDOM LOADOUT:");
        for (int i = 0; i < loadout.Count; i++)
        {
            var stratagem = loadout[i];
            Console.WriteLine($"{i + 1}. {stratagem.Name} ({stratagem.Category})");
            Console.WriteLine($"   Code: {stratagem.InputCode} | Cooldown: {stratagem.CooldownSeconds}s");
        }

        Console.WriteLine($"\n📊 ANALYSIS:");
        Console.WriteLine($"Synergy Score: {synergyScore:F1}");
        Console.WriteLine($"Balance Assessment: {analysis.GetBalanceDescription()}");

        if (analysis.TagCounts.Any())
        {
            Console.WriteLine("\n🏷️  Tag Distribution:");
            foreach (var (tag, count) in analysis.TagCounts.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"   {tag}: {count}");
            }
        }
    }

    static async Task AnalyzeCustomLoadout(StratagemHypergraph hypergraph, StratagemContext context)
    {
        Console.WriteLine("\n⚙️  ANALYZE CUSTOM LOADOUT");
        Console.WriteLine("=========================");

        var allStratagems = await context.Stratagems.OrderBy(s => s.Name).ToListAsync();

        Console.WriteLine("\nAvailable Stratagems:");
        for (int i = 0; i < allStratagems.Count; i++)
        {
            Console.WriteLine($"{i + 1:D2}. {allStratagems[i].Name}");
        }

        var selectedIds = new List<int>();

        for (int slot = 1; slot <= 4; slot++)
        {
            Console.Write($"\nEnter stratagem number for slot {slot}: ");
            if (int.TryParse(Console.ReadLine(), out int choice) &&
                choice >= 1 && choice <= allStratagems.Count)
            {
                selectedIds.Add(allStratagems[choice - 1].Id);
                Console.WriteLine($"Selected: {allStratagems[choice - 1].Name}");
            }
            else
            {
                Console.WriteLine("Invalid selection. Skipping slot.");
                slot--; // Retry this slot
            }
        }

        if (selectedIds.Count == 4)
        {
            var synergyScore = await hypergraph.EvaluateLoadoutSynergyAsync(selectedIds);
            var analysis = await hypergraph.AnalyzeLoadoutBalanceAsync(selectedIds);

            Console.WriteLine("\n📊 CUSTOM LOADOUT ANALYSIS:");
            Console.WriteLine($"Synergy Score: {synergyScore:F1}");
            Console.WriteLine($"Balance Assessment: {analysis.GetBalanceDescription()}");

            if (analysis.TagCounts.Any())
            {
                Console.WriteLine("\n🏷️  Tag Distribution:");
                foreach (var (tag, count) in analysis.TagCounts.OrderByDescending(x => x.Value))
                {
                    Console.WriteLine($"   {tag}: {count}");
                }
            }
        }
    }

    static async Task ViewStratagemsByTag(StratagemContext context)
    {
        Console.WriteLine("\n🏷️  VIEW STRATAGEMS BY TAG");
        Console.WriteLine("=========================");

        var availableTags = await context.StratagemTags
            .Select(st => st.Tag)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        Console.WriteLine("\nAvailable Tags:");
        for (int i = 0; i < availableTags.Count; i++)
        {
            Console.WriteLine($"{i + 1:D2}. {availableTags[i]}");
        }

        Console.Write("\nEnter tag number to view stratagems: ");
        if (int.TryParse(Console.ReadLine(), out int choice) &&
            choice >= 1 && choice <= availableTags.Count)
        {
            var selectedTag = availableTags[choice - 1];
            var stratagems = await context.Stratagems
                .Where(s => s.Tags.Any(t => t.Tag == selectedTag))
                .Include(s => s.Tags)
                .OrderBy(s => s.Name)
                .ToListAsync();

            Console.WriteLine($"\n🎯 Stratagems with tag '{selectedTag}':");
            foreach (var stratagem in stratagems)
            {
                Console.WriteLine($"\n• {stratagem.Name} ({stratagem.Category})");
                Console.WriteLine($"  Code: {stratagem.InputCode} | Cooldown: {stratagem.CooldownSeconds}s");
                var otherTags = stratagem.Tags.Where(t => t.Tag != selectedTag).Select(t => t.Tag);
                if (otherTags.Any())
                {
                    Console.WriteLine($"  Other tags: {string.Join(", ", otherTags)}");
                }
            }
        }
    }

    static async Task TestLoadout(StratagemHypergraph hypergraph)
    {
        Console.WriteLine("\n🧪 TEST LOADOUT EFFECTIVENESS");
        Console.WriteLine("=============================");

        // Generate or get a loadout to test
        var loadout = await hypergraph.GenerateRandomLoadoutAsync();
        var stratagemIds = loadout.Select(s => s.Id).ToList();

        Console.WriteLine("Testing this loadout:");
        foreach (var stratagem in loadout)
        {
            Console.WriteLine($"• {stratagem.Name}");
        }

        var synergyScore = await hypergraph.EvaluateLoadoutSynergyAsync(stratagemIds);
        Console.WriteLine($"\nCalculated Synergy Score: {synergyScore:F1}");

        // Get user feedback
        Console.Write("\nHow effective was this loadout? (1-10): ");
        if (double.TryParse(Console.ReadLine(), out double effectiveness))
        {
            Console.Write("Mission type (Terminid/Automaton/Mixed): ");
            var missionType = Console.ReadLine() ?? "Mixed";

            Console.Write("Difficulty (Trivial/Easy/Medium/Challenging/Hard/Extreme/Suicide/Impossible/Helldive): ");
            var difficulty = Console.ReadLine() ?? "Medium";

            Console.Write("Notes (optional): ");
            var notes = Console.ReadLine() ?? "";

            await hypergraph.SaveLoadoutTestAsync(stratagemIds, effectiveness, missionType, difficulty, notes);
            Console.WriteLine("\n✅ Test result saved!");
        }
    }

    static async Task ViewTestResults(StratagemContext context)
    {
        Console.WriteLine("\n📈 TEST RESULTS");
        Console.WriteLine("===============");

        var tests = await context.LoadoutTests
            .OrderByDescending(t => t.TestDate)
            .Take(10)
            .ToListAsync();

        if (!tests.Any())
        {
            Console.WriteLine("No test results found. Run some loadout tests first!");
            return;
        }

        Console.WriteLine("Recent Test Results (Last 10):");
        foreach (var test in tests)
        {
            Console.WriteLine($"\n📅 {test.TestDate:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"   Stratagems: {test.StratagemIds}");
            Console.WriteLine($"   Synergy Score: {test.SynergyScore:F1}");
            Console.WriteLine($"   Effectiveness: {test.EffectivenessRating}/10");
            Console.WriteLine($"   Mission: {test.MissionType} ({test.Difficulty})");
            if (!string.IsNullOrEmpty(test.Notes))
            {
                Console.WriteLine($"   Notes: {test.Notes}");
            }
        }

        // Calculate averages
        var avgSynergy = tests.Average(t => t.SynergyScore);
        var avgEffectiveness = tests.Average(t => t.EffectivenessRating);

        Console.WriteLine($"\n📊 STATISTICS:");
        Console.WriteLine($"Average Synergy Score: {avgSynergy:F1}");
        Console.WriteLine($"Average Effectiveness: {avgEffectiveness:F1}/10");
    }

    static async Task ViewAllStratagems(StratagemContext context)
    {
        Console.WriteLine("\n📋 ALL STRATAGEMS");
        Console.WriteLine("=================");

        var stratagems = await context.Stratagems
            .Include(s => s.Tags)
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Name)
            .ToListAsync();

        var currentCategory = "";
        foreach (var stratagem in stratagems)
        {
            if (stratagem.Category != currentCategory)
            {
                currentCategory = stratagem.Category;
                Console.WriteLine($"\n🎯 {currentCategory.ToUpper()}:");
                Console.WriteLine(new string('-', currentCategory.Length + 5));
            }

            Console.WriteLine($"\n• {stratagem.Name}");
            Console.WriteLine($"  Code: {stratagem.InputCode} | Cooldown: {stratagem.CooldownSeconds}s | Uses: {(stratagem.Uses == -1 ? "Unlimited" : stratagem.Uses.ToString())}");

            var tags = stratagem.Tags.Select(t => t.Tag);
            if (tags.Any())
            {
                Console.WriteLine($"  Tags: {string.Join(", ", tags)}");
            }
        }

        Console.WriteLine($"\nTotal Stratagems: {stratagems.Count}");
    }
}