using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("disciplines")]
    public class Discipline
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int ApparatusId { get; set; }

        [ForeignKey(nameof(ApparatusId))]
        public virtual Apparatus Apparatus { get; set; }
        public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    }
}
