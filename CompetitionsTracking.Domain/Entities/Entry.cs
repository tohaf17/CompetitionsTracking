using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("entries")]
    public class Entry
    {
        [Key]
        public int Id { get; set; }
        public int CompetitionId { get; set; }
        public int ParticipantId { get; set; }
        public int DisciplineId { get; set; }
        public int CategoryId { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
        public EntryStatus EntryStatus { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(CompetitionId))]
        public virtual Competition Competition { get; set; }
        [ForeignKey(nameof(ParticipantId))]
        public virtual Participant Participant { get; set; }
        [ForeignKey(nameof(DisciplineId))]
        public virtual Discipline Discipline { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }


        public virtual Result Result { get; set; }
        public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
    }
    public enum ApplicationStatus
    {
        Pending,
        Accepted,
        Rejected,
        Resubmitted
    }
    public enum EntryStatus
    {
        Registered,
        Active,
        Finished,
        DNS
    }
}
