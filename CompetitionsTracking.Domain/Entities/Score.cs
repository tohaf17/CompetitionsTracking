using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("scores")]
    public class Score
    {
        [Key]
        public int Id { get; set; }
        public int EntryId { get; set; }
        public int JudgeId { get; set; }
        public ScoreType Type { get; set; }
        public float ScoreValue { get; set; }

        // Breakdown fields for DB (Difficulty Body) validation
        public int? JumpCount { get; set; }
        public int? BalanceCount { get; set; }
        public int? RotationCount { get; set; }
        public int? DynamicRotationCount { get; set; } // "R" elements
        public int? ElementCount { get; set; } // Total counted elements (max 8)

        [ForeignKey(nameof(EntryId))]
        public virtual Entry Entry { get; set; }
        
        [ForeignKey(nameof(JudgeId))]
        public virtual Judge Judge { get; set; }
    }
    public enum ScoreType
    {
        D,  // General Difficulty
        DA, // Apparatus Difficulty
        DB, // Body Difficulty
        E,  // Execution
        A,  // Artistry
        Penalty
    }
}

