namespace MXIC_PCCS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MXIC_LisenceManagement
    {
        [Key]
        public Guid LicID { get; set; }

        [Required]
        public string PoNo { get; set; }

        [Required]
        [StringLength(10)]
        public string EmpName { get; set; }

        [Required]
        public string LicName { get; set; }

        public DateTime EndDate { get; set; }

        public bool LicPossess { get; set; }

        public DateTime UpDateTime { get; set; }

        public Guid EditID { get; set; }

        public Guid DeleteID { get; set; }
    }
}
