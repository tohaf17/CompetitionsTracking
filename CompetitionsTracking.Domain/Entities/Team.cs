using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("teams")]
    public class Team:Participant
    {
        [Required]
        public string Name { get; set; }= string.Empty;
        public int CoachId { get; set; }

        [ForeignKey(nameof(CoachId))]
        public virtual Person Coach { get; set; }
        
        public virtual ICollection<Person> Members { get; set; } = new List<Person>();
    }
}
