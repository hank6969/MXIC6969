namespace MXIC_PCCS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SwipeInfoSelect")]
    public partial class SwipeInfoSelect
    {


        [Key]
        public Guid id { get; set; }

        public string name { get; set; }

      
        public string value { get; set; }

     
    }
}
