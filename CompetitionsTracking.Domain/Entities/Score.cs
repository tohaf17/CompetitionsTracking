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
        [ForeignKey(nameof(EntryId))]
        public virtual Entry Entry { get; set; }
        [ForeignKey(nameof(JudgeId))]
        public virtual Judge Judge { get; set; }
    }
    public enum ScoreType
    {
        D,
        E,
        A,
        Penalty
    }
}
