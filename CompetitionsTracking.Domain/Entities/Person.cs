using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("persons")]
    public class Person:Participant
    {
        [Required]
        public string Name { get; set; }= string.Empty;
        [Required]
        public string Surname { get; set; }= string.Empty;
        public string Country { get; set; }= string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int? MentorId { get; set; }
        public Gender Gender { get; set; }

        [ForeignKey(nameof(MentorId))]
        public virtual Person Mentor { get; set; }
        public virtual ICollection<Person> Mentees { get; set; } = new List<Person>();
        public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
    }
    public enum Gender
    {
        Male,Female
    }
}
