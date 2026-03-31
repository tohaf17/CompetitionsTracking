using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("results")]
    public class Result
    {
        [Key]
        public int Id { get; set; }
        public int EntryId { get; set; }
        public int Place {get;set;}
        public float FinalScore{get;set;}
        public string AwardedMedal { get; set; }

        [ForeignKey(nameof(EntryId))]
        public virtual Entry Entry { get; set; }

        public virtual ICollection<Appeal> Appeals { get; set; } = new List<Appeal>();
    }
}
