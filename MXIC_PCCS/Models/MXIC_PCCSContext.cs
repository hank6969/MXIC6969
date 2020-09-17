namespace MXIC_PCCS.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MXIC_PCCSContext : DbContext
    {
        public MXIC_PCCSContext()
            : base("name=MXIC_PCCS")
        {
        }
        public virtual DbSet<MXIC_DepartmentManagement> MXIC_DepartmentManagements { get; set; }
        public virtual DbSet<MXIC_LisenceManagement> MXIC_LisenceManagements { get; set; }
        public virtual DbSet<MXIC_Quotation> MXIC_Quotations { get; set; }
        public virtual DbSet<MXIC_ScheduleSetting> MXIC_ScheduleSettings { get; set; }
        public virtual DbSet<MXIC_SwipeInfo> MXIC_SwipeInfos { get; set; }
        public virtual DbSet<MXIC_UserManagement> MXIC_UserManagements { get; set; }
        public virtual DbSet<MXIC_InputGenerate> MXIC_InputGenerates { get; set; }
        public virtual DbSet<MXIC_VendorManagement> MXIC_VendorManagements { get; set; }

        public virtual DbSet<MXIC_View_Swipe> MXIC_View_Swipes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
