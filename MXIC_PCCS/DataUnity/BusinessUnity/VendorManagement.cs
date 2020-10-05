using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MXIC_PCCS.DataUnity.Interface;
using MXIC_PCCS.Models;
using Newtonsoft.Json;

namespace MXIC_PCCS.DataUnity.BusinessUnity
{
    public class VendorManagement : IVendorManagement, IDisposable
    {
        //開啟資料庫連結
        public MXIC_PCCSContext _db = new MXIC_PCCSContext();

        //關閉資料庫
        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }

        public string AddVendor(string PoNo, string VendorName, string EmpID, string EmpName, string Shifts)
        {
            string Str = "新增成功";
            if (!string.IsNullOrWhiteSpace(PoNo) && !string.IsNullOrWhiteSpace(VendorName) && !string.IsNullOrWhiteSpace(EmpID) && !string.IsNullOrWhiteSpace(EmpName) && !string.IsNullOrWhiteSpace(Shifts))
            {
                var OriginalEmp = _db.MXIC_VendorManagements.Where(x => x.EmpID == EmpID);
                if (OriginalEmp.Any())
                {

                    Str = "此駐廠人員編號已存在";
                }
                else { 
                var AddUser = new MXIC_VendorManagement()
                {
                    VenID = Guid.NewGuid(),
                    PoNo = PoNo,
                    VendorName = VendorName,
                    EmpID = EmpID,
                    EmpName = EmpName,
                    Shifts = Shifts,
                    DeleteID = Guid.NewGuid(),
                    EditID = Guid.NewGuid()

                };

                _db.MXIC_VendorManagements.Add(AddUser);
                _db.SaveChanges();
            }
            }
            return (Str);
        }

        public string DeleteVendor(string DeleteID)
        {
            string Str = "刪除失敗";
            try
            {
                MXIC_VendorManagement Vendor = _db.MXIC_VendorManagements.Where(x => x.DeleteID.ToString() == DeleteID).FirstOrDefault();
                //User.UserDisable = false;
                _db.MXIC_VendorManagements.Remove(Vendor);
                _db.SaveChanges();
                Str = "刪除成功";
            }
            catch (Exception e)
            {
                Str = e.ToString();
            }
            return (Str);
        }

        public string EditVendor(string EditID, string PoNo, string VendorName, string EmpID, string EmpName, string Shifts)
        {
            string Str = "修改成功";

            var EditVendor = _db.MXIC_VendorManagements.Where(x => x.EditID.ToString() == EditID).FirstOrDefault();

            try
            {
                if (!string.IsNullOrWhiteSpace(PoNo))
                {
                    EditVendor.PoNo = PoNo;
                }

                if (!string.IsNullOrWhiteSpace(VendorName))
                {
                    EditVendor.VendorName = VendorName;
                }

                if (!string.IsNullOrWhiteSpace(EmpID))
                {
                    EditVendor.EmpID = EmpID;
                }

                if (!string.IsNullOrWhiteSpace(EmpName))
                {
                    EditVendor.EmpName = EmpName;
                }

                if (!string.IsNullOrWhiteSpace(Shifts))
                {
                    EditVendor.Shifts = Shifts;
                }
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                Str = e.ToString();
            }
            return (Str);
        }

        public string VendorList( string PoNo, string VendorName, string EmpID, string EmpName, string Shifts)
        {
            var _VenderList = _db.MXIC_VendorManagements.Select(x=>new { x.PoNo,x.VendorName,x.EmpID,x.EmpName,x.Shifts,x.EditID,x.DeleteID});

            //如果PoNo不為空
            if (!string.IsNullOrWhiteSpace(PoNo))
            {
                _VenderList = _VenderList.Where(x => x.PoNo.ToLower().Contains(PoNo.ToLower()));
            }
            //如果VendorName不為空
            if (!string.IsNullOrWhiteSpace(VendorName))
            {
                _VenderList = _VenderList.Where(x => x.VendorName.ToLower().Contains(VendorName.ToLower()));
            }
            //如果EmpID不為空
            if (!string.IsNullOrWhiteSpace(EmpID))
            {
                _VenderList = _VenderList.Where(x => x.EmpID.ToLower().Contains(EmpID.ToLower()));
            }
            //如果EmpName不為空
            if (!string.IsNullOrWhiteSpace(EmpName))
            {
                _VenderList = _VenderList.Where(x => x.EmpName.ToLower().Contains(EmpName.ToLower()));
            }
            //如果Shifts不為空
            if (!string.IsNullOrWhiteSpace(Shifts))
            {
                _VenderList = _VenderList.Where(x => x.Shifts.ToLower().Contains(Shifts.ToLower()));
            }

            string Str = JsonConvert.SerializeObject(_VenderList, Formatting.Indented);

            return (Str);
        }

        public string EditVendorDetail (string EditID)
        {
            var VendorDetail = _db.MXIC_VendorManagements.Where(x => x.EditID.ToString() == EditID).Select(x => new { x.PoNo, x.VendorName, x.EmpID, x.EmpName,x.Shifts });

            string Str = JsonConvert.SerializeObject(VendorDetail, Formatting.Indented);

            return (Str);
        }
    }
}