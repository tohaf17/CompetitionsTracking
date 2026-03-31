using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetitionsTracking.Domain.Entities
{
    [Table("appeals")]
    public class Appeal
    {
        [Key]
        public int Id { get; set; }
        public int ResultId { get; set; }
        public string Reason { get; set; }
        public AppealStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ResolvedAt { get; set; }

        [ForeignKey(nameof(ResultId))]
        public virtual Result Result { get; set; }

    }
    public enum AppealStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
