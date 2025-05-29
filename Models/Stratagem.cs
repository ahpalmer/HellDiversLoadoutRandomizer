using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellDiversLoadoutRandomizer.Models;

public class Stratagem
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // "Support Weapon", "Orbital", "Eagle", etc.

    public int CooldownSeconds { get; set; }

    public int Uses { get; set; } // -1 for unlimited, specific count otherwise

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(20)]
    public string InputCode { get; set; } = string.Empty; // Arrow key sequence like "↓↓↑←→"

    // Navigation properties
    public virtual ICollection<StratagemTag> Tags { get; set; } = new List<StratagemTag>();
    public virtual ICollection<HyperedgeMember> HyperedgeMembers { get; set; } = new List<HyperedgeMember>();
}

// Tags for flexible categorization
public class StratagemTag
{
    [Key]
    public int Id { get; set; }

    public int StratagemId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Tag { get; set; } = string.Empty;

    // Foreign key
    [ForeignKey(nameof(StratagemId))]
    public virtual Stratagem Stratagem { get; set; } = null!;
}