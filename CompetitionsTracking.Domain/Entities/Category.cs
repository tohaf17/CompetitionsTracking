using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("categories")]
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Type { get; set; } = string.Empty;
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    }
}
