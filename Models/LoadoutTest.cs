using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellDiversLoadoutRandomizer.Models;

public class LoadoutTest
{
    [Key]
    public int Id { get; set; }

    public DateTime TestDate { get; set; }

    [MaxLength(200)]
    public string StratagemIds { get; set; } = string.Empty; // Comma-separated IDs

    public double SynergyScore { get; set; }

    public double EffectivenessRating { get; set; } // User-provided or calculated rating

    [MaxLength(50)]
    public string MissionType { get; set; } = string.Empty; // "Terminid", "Automaton", "Mixed"

    [MaxLength(20)]
    public string Difficulty { get; set; } = string.Empty; // "Trivial" to "Helldive"

    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}