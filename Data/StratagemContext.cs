using HellDiversLoadoutRandomizer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellDiversLoadoutRandomizer.Data;

public class StratagemContext : DbContext
{
    public DbSet<Stratagem> Stratagems { get; set; }
    public DbSet<StratagemTag> StratagemTags { get; set; }
    public DbSet<Hyperedge> Hyperedges { get; set; }
    public DbSet<HyperedgeMember> HyperedgeMembers { get; set; }
    public DbSet<LoadoutTest> LoadoutTests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=helldivers_stratagems.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships
        modelBuilder.Entity<StratagemTag>()
            .HasOne(st => st.Stratagem)
            .WithMany(s => s.Tags)
            .HasForeignKey(st => st.StratagemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HyperedgeMember>()
            .HasOne(hm => hm.Hyperedge)
            .WithMany(h => h.Members)
            .HasForeignKey(hm => hm.HyperedgeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HyperedgeMember>()
            .HasOne(hm => hm.Stratagem)
            .WithMany(s => s.HyperedgeMembers)
            .HasForeignKey(hm => hm.StratagemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Create unique index for stratagem tags
        modelBuilder.Entity<StratagemTag>()
            .HasIndex(st => new { st.StratagemId, st.Tag })
            .IsUnique();

        // Create unique index for hyperedge members
        modelBuilder.Entity<HyperedgeMember>()
            .HasIndex(hm => new { hm.HyperedgeId, hm.StratagemId })
            .IsUnique();
    }
}