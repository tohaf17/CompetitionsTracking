using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("competitions")]
    public class Competition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public CompetitionStatus Status { get; set; }

        public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    }
    public enum CompetitionStatus
    {
        Planned,
        RegistrationOpen,
        Ongoing,
        Finished
    }
}
