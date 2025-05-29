using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellDiversLoadoutRandomizer.Models;

// Hyperedges for stratagem synergies
public class Hyperedge
{
    [Key]
    public int Id { get; set; }

    public double SynergyWeight { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(100)]
    public string SynergyType { get; set; } = string.Empty; // "Anti-Tank Combo", "Area Denial", etc.

    // Navigation properties
    public virtual ICollection<HyperedgeMember> Members { get; set; } = new List<HyperedgeMember>();
}

// Junction table for many-to-many relationship between Hyperedges and Stratagems
public class HyperedgeMember
{
    [Key]
    public int Id { get; set; }

    public int HyperedgeId { get; set; }
    public int StratagemId { get; set; }

    // Foreign keys
    [ForeignKey(nameof(HyperedgeId))]
    public virtual Hyperedge Hyperedge { get; set; } = null!;

    [ForeignKey(nameof(StratagemId))]
    public virtual Stratagem Stratagem { get; set; } = null!;
}