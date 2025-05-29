using HellDiversLoadoutRandomizer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellDiversLoadoutRandomizer.Data;

public class DataSeeder
{
    private readonly StratagemContext _context;

    public DataSeeder(StratagemContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        // Check if data already exists
        if (await _context.Stratagems.AnyAsync())
        {
            Console.WriteLine("Database already seeded.");
            return;
        }

        Console.WriteLine("Seeding Helldivers 2 stratagems...");

        // Support Weapons
        var supportWeapons = new List<(string name, int cooldown, string code, List<string> tags)>
            {
                ("AC-8 Autocannon", 480, "↓↓↑←→", new() {"Support Weapon", "Anti-Tank", "Anti-Armor", "Explosive"}),
                ("APW-1 Anti-Materiel Rifle", 480, "↓←→↑↓", new() {"Support Weapon", "Anti-Tank", "Precision", "Long-Range"}),
                ("ARC-3 Arc Thrower", 480, "→↑←↓→", new() {"Support Weapon", "Anti-Personnel", "Chain Lightning", "Crowd Control"}),
                ("EAT-17 Expendable Anti-Tank", 120, "↓↓←↑→", new() {"Support Weapon", "Anti-Tank", "Single-Use", "Disposable"}),
                ("FAF-14 Spear", 480, "↓↓↑↓↓", new() {"Support Weapon", "Anti-Tank", "Guided Missile", "Lock-On"}),
                ("FLAM-40 Flamethrower", 480, "↓←↑↓→", new() {"Support Weapon", "Anti-Personnel", "Fire", "Close-Range"}),
                ("GR-8 Recoilless Rifle", 480, "→←→↓←", new() {"Support Weapon", "Anti-Tank", "Team Reload", "Explosive"}),
                ("LAS-98 Laser Cannon", 480, "↓←↓↑←", new() {"Support Weapon", "Energy", "Sustained Beam", "Anti-Armor"}),
                ("LAS-99 Quasar Cannon", 480, "↓↓↑←→", new() {"Support Weapon", "Energy", "Charged Shot", "Anti-Tank"}),
                ("MG-43 Machine Gun", 480, "↓←↓↑→", new() {"Support Weapon", "Suppression", "Anti-Personnel", "High Ammo"}),
                ("MG-206 Heavy Machine Gun", 480, "↓←↓↓↑", new() {"Support Weapon", "Suppression", "Anti-Personnel", "High Damage"}),
                ("RS-422 Railgun", 480, "→←→↓←", new() {"Support Weapon", "Anti-Tank", "Precision", "Armor Piercing"})
            };

        // Orbital Stratagems
        var orbitalStratagems = new List<(string name, int cooldown, string code, List<string> tags)>
            {
                ("Orbital Precision Strike", 90, "→→↑", new() {"Orbital", "Precision", "Anti-Tank", "Single Target"}),
                ("Orbital Airburst Strike", 90, "→→→", new() {"Orbital", "Anti-Personnel", "Area Damage", "Shrapnel"}),
                ("Orbital 120MM HE Barrage", 240, "→→↓←→↓", new() {"Orbital", "Area Bombardment", "High Explosive", "Multiple Hits"}),
                ("Orbital 380MM HE Barrage", 240, "→↓↑↑←↓↓", new() {"Orbital", "Massive Bombardment", "High Explosive", "Wide Area"}),
                ("Orbital Walking Barrage", 240, "→↓→↓→↓", new() {"Orbital", "Moving Bombardment", "Area Denial", "Continuous"}),
                ("Orbital Laser", 200, "→↓↑→↓", new() {"Orbital", "Sustained Beam", "High Damage", "Limited Uses"}),
                ("Orbital Railcannon Strike", 210, "→↑↓↓→", new() {"Orbital", "Auto-Target", "Anti-Tank", "Precision"}),
                ("Orbital Gatling Barrage", 180, "→↓←↑→", new() {"Orbital", "Rapid Fire", "Anti-Personnel", "Suppression"}),
                ("Orbital Smoke Barrage", 120, "→→↓→", new() {"Orbital", "Concealment", "Utility", "Area Denial"}),
                ("Orbital EMS Mortar", 120, "→→←↓", new() {"Orbital", "EMP", "Electronics Disable", "Crowd Control"})
            };

        // Eagle Stratagems
        var eagleStratagems = new List<(string name, int cooldown, string code, List<string> tags)>
            {
                ("Eagle Airstrike", 15, "→→↓→", new() {"Eagle", "Fast Strike", "Precision", "Anti-Personnel"}),
                ("Eagle Cluster Bomb", 15, "→→↓↓→", new() {"Eagle", "Wide Area", "Anti-Personnel", "Multiple Explosions"}),
                ("Eagle 500KG Bomb", 15, "→←↓↓↓", new() {"Eagle", "Massive Explosion", "Anti-Tank", "Large Radius"}),
                ("Eagle Napalm Airstrike", 15, "→→↓↑", new() {"Eagle", "Fire", "Area Denial", "Damage Over Time"}),
                ("Eagle Smoke Strike", 15, "→→↓←", new() {"Eagle", "Concealment", "Utility", "Fast Deployment"}),
                ("Eagle 110MM Rocket Pods", 15, "→→↓↑→", new() {"Eagle", "Multiple Rockets", "Anti-Armor", "Sustained Fire"}),
                ("Eagle Strafing Run", 15, "→→↓←", new() {"Eagle", "Machine Gun", "Anti-Personnel", "Line Attack"})
            };

        // Defensive/Utility Stratagems
        var defensiveStratagems = new List<(string name, int cooldown, string code, List<string> tags)>
            {
                ("Shield Generator Pack", 480, "↓↑↓↑←→", new() {"Backpack", "Defensive", "Energy Shield", "Utility"}),
                ("Supply Pack", 480, "↓←↓↑↑↓", new() {"Backpack", "Utility", "Extra Ammo", "Support"}),
                ("Jump Pack", 480, "↓↑↑↓↑", new() {"Backpack", "Mobility", "Utility", "Vertical Movement"}),
                ("Ballistic Shield Backpack", 480, "↓←↓↓↑←", new() {"Backpack", "Defensive", "Shield", "Protection"}),
                ("Resupply", 0, "↓↓↑→", new() {"Utility", "Ammo", "Stims", "Essential"}),
                ("Reinforce", 0, "↑→↓←↑", new() {"Utility", "Respawn", "Essential", "Team Support"}),
                ("Hellbomb", 0, "↓↑←↓↑→→", new() {"Utility", "Objective", "Massive Explosion", "Mission Specific"})
            };

        // Sentry Stratagems
        var sentryStratagems = new List<(string name, int cooldown, string code, List<string> tags)>
            {
                ("MG Sentry", 180, "↓↑→→↑", new() {"Sentry", "Anti-Personnel", "Machine Gun", "Defensive"}),
                ("Gatling Sentry", 180, "↓↑→←→", new() {"Sentry", "Anti-Personnel", "Rapid Fire", "High Damage"}),
                ("Mortar Sentry", 180, "↓↑→→↓", new() {"Sentry", "Indirect Fire", "Area Damage", "Long Range"}),
                ("Autocannon Sentry", 180, "↓↑→↑←↑", new() {"Sentry", "Anti-Tank", "High Damage", "Versatile"}),
                ("Rocket Sentry", 180, "↓↑→→←", new() {"Sentry", "Anti-Vehicle", "Explosive", "High Damage"}),
                ("EMS Mortar Sentry", 180, "↓↑→↓→", new() {"Sentry", "EMP", "Electronics Disable", "Area Control"}),
                ("Anti-Personnel Minefield", 180, "↓←↑→", new() {"Sentry", "Area Denial", "Mines", "Defensive"})
            };

        // Exosuits
        var exosuits = new List<(string name, int cooldown, string code, List<string> tags)>
            {
                ("EXO-45 Patriot Exosuit", 600, "←↓→↑←↓↓", new() {"Exosuit", "Mech", "Anti-Personnel", "Machine Gun"}),
                ("EXO-49 Emancipator Exosuit", 600, "←↓→↑←↓↓", new() {"Exosuit", "Mech", "Anti-Tank", "Autocannon"})
            };

        // Combine all stratagems
        var allStratagemData = new List<(string category, List<(string name, int cooldown, string code, List<string> tags)>)>
            {
                ("Support Weapon", supportWeapons),
                ("Orbital", orbitalStratagems),
                ("Eagle", eagleStratagems),
                ("Defensive", defensiveStratagems),
                ("Sentry", sentryStratagems),
                ("Exosuit", exosuits)
            };

        // Add all stratagems to database
        int id = 1;
        foreach (var (category, stratagems) in allStratagemData)
        {
            foreach (var (name, cooldown, code, tags) in stratagems)
            {
                var stratagem = new Stratagem
                {
                    Id = id++,
                    Name = name,
                    Category = category,
                    CooldownSeconds = cooldown,
                    Uses = cooldown == 0 ? -1 : (category == "Eagle" ? 2 : 1), // Eagles typically have 2 uses
                    InputCode = code,
                    Description = $"{category} stratagem: {name}"
                };

                _context.Stratagems.Add(stratagem);

                // Add tags for this stratagem
                foreach (var tag in tags)
                {
                    _context.StratagemTags.Add(new StratagemTag
                    {
                        StratagemId = stratagem.Id,
                        Tag = tag
                    });
                }
            }
        }

        await _context.SaveChangesAsync();

        // Seed some initial hyperedges (synergies)
        await SeedInitialSynergiesAsync();

        Console.WriteLine($"Seeded {id - 1} stratagems with tags and synergies.");
    }

    private async Task SeedInitialSynergiesAsync()
    {
        var synergies = new List<(string description, string type, double weight, List<string> stratagemNames)>
            {
                // Anti-tank combinations
                ("Railgun + Shield Generator for sustained anti-tank", "Anti-Tank Combo", 8.5,
                 new() {"RS-422 Railgun", "Shield Generator Pack"}),

                ("Autocannon + Supply Pack for extended firepower", "Sustained Combat", 7.8,
                 new() {"AC-8 Autocannon", "Supply Pack"}),

                ("Quasar + EAT for massive anti-tank potential", "Heavy Anti-Tank", 9.2,
                 new() {"LAS-99 Quasar Cannon", "EAT-17 Expendable Anti-Tank"}),
                
                // Area control combinations
                ("Orbital Laser + Eagle 500KG for area devastation", "Area Devastation", 8.7,
                 new() {"Orbital Laser", "Eagle 500KG Bomb"}),

                ("Napalm + Mortar Sentry for area denial", "Area Denial", 7.5,
                 new() {"Eagle Napalm Airstrike", "Mortar Sentry"}),
                
                // Defensive combinations
                ("Shield Generator + Jump Pack for survival", "Mobility Defense", 8.0,
                 new() {"Shield Generator Pack", "Jump Pack"}),

                ("Autocannon Sentry + Gatling Sentry for defensive line", "Defensive Line", 8.3,
                 new() {"Autocannon Sentry", "Gatling Sentry"}),
                
                // Support combinations
                ("Arc Thrower + Stun Grenades for crowd control", "Crowd Control", 7.2,
                 new() {"ARC-3 Arc Thrower"}), // Note: Stun Grenades not in our list yet
                
                ("Eagle Cluster + Orbital Airburst for anti-personnel", "Anti-Personnel Combo", 7.0,
                 new() {"Eagle Cluster Bomb", "Orbital Airburst Strike"}),
                
                // Versatile loadouts
                ("Balanced loadout: Anti-tank + Area + Support + Defense", "Balanced Loadout", 9.5,
                 new() {"AC-8 Autocannon", "Eagle 500KG Bomb", "Resupply", "Shield Generator Pack"})
            };

        foreach (var (description, type, weight, stratagemNames) in synergies)
        {
            var hyperedge = new Hyperedge
            {
                Description = description,
                SynergyType = type,
                SynergyWeight = weight
            };

            _context.Hyperedges.Add(hyperedge);
            await _context.SaveChangesAsync(); // Save to get the ID

            // Add stratagem members to this hyperedge
            foreach (var stratagemName in stratagemNames)
            {
                var stratagem = await _context.Stratagems
                    .FirstOrDefaultAsync(s => s.Name == stratagemName);

                if (stratagem != null)
                {
                    _context.HyperedgeMembers.Add(new HyperedgeMember
                    {
                        HyperedgeId = hyperedge.Id,
                        StratagemId = stratagem.Id
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}