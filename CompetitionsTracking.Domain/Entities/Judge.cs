using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("judges")]
    public class Judge
    {
        [Key]
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string QualificationLevel { get; set; } = string.Empty;
        [ForeignKey(nameof(PersonId))]
        public virtual Person Person { get; set; }
        public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
    }
}
