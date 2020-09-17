namespace MXIC_PCCS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MXIC_InputGenerate
    {   [Key]
        public int InputGenerateID { get; set; }
        public string TableName { get; set; }
        public string COLUMN_NAME { get; set; }
        public string Remarks { get; set; }
        public int Generate { get; set; }
        public int PopGenerate { get; set; }
        public string GenerateType { get; set; }
        public string AddPopGenerate { get; set; }
        public string EditPopGenerate { get; set; }
        public int? GridTitleGenerate { get; set; }

        public string GridFormatter { get; set; }

        public int? Sequence { get; set; }



    }
}
