using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("participants")]
    public abstract class Participant
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    }
}
