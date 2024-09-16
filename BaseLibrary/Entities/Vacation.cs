using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class Vacation : OtherBaseEntity
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public int NumberOfDays { get; set; }
        [Required]
        public DateTime EndDate => StartDate.AddDays(NumberOfDays);

        public VacationType VacationType { get; set; } = null!;
        [Required]
        public int VacationTypeId { get; set; }
    }
}