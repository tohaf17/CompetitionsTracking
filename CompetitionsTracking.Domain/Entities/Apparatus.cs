using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("apparatus")]
    public class Apparatus
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();
    }
}
