using MXIC_PCCS.DataUnity.Interface;
using MXIC_PCCS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MXIC_PCCS.DataUnity.BusinessUnity
{
    public class DepartmentManagement : IDepartmentManagement, IDisposable
    {
        //開啟資料庫連結
        public MXIC_PCCSContext _db = new MXIC_PCCSContext();

        //關閉資料庫
        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }

        public string SearchDepToVen(string DepNo, string DepName)
        {
            var _DepSearchCondition = _db.MXIC_DepartmentManagements.Select(x => new { x.DepName, x.VendorName, x.EditID, x.DeleteID, x.DepNo });

            //如果DepNo不為空
            if (!string.IsNullOrWhiteSpace(DepNo))
            {
                _DepSearchCondition = _DepSearchCondition.Where(x => x.DepNo.Contains(DepNo));
            }

            //如果DepName不為空
            if (!string.IsNullOrWhiteSpace(DepName))
            {
                _DepSearchCondition = _DepSearchCondition.Where(x => x.DepName.Contains(DepName));
            }

            string responseStr = JsonConvert.SerializeObject(_DepSearchCondition, Formatting.Indented);

            return (responseStr);
        }

        public string AddDepToVen(string DepNo, string DepName, string VendorName)
        {
            string MessageStr = "新增成功";

            //var AddDepToVenItem = new MXIC_DepartmentManagement()
            //{
            //    DepID = Guid.NewGuid(),
            //    DepNo = DepNo,
            //    DepName = DepName,
            //    VendorName = VendorName,
            //    EditID = Guid.NewGuid(),
            //    DeleteID = Guid.NewGuid()

            //};

            //_db.MXIC_DepartmentManagements.Add(AddDepToVenItem);
            //_db.SaveChanges();

            return (MessageStr);
        }

        public string EditDepToVen(string EditID, string VendorName)
        {
            string MessageStr = "修改成功";

            //var EditDepToVenListItem = _db.MXIC_DepartmentManagements.Where(x => x.EditID.ToString() == EditID).FirstOrDefault();

            //try
            //{
            //    EditDepToVenListItem.VendorName = VendorName;
            //    _db.SaveChanges();
            //}
            //catch (Exception e)
            //{
            //    MessageStr = e.ToString();
            //}

            return (MessageStr);
        }

        public string DeleteDepToVen(string DeleteID)
        {
            string MessageStr = "刪除失敗";

            try
            {
                MXIC_DepartmentManagement DeleteDepToVenList = _db.MXIC_DepartmentManagements.Where(x => x.DeleteID.ToString() == DeleteID).FirstOrDefault();

                _db.MXIC_DepartmentManagements.Remove(DeleteDepToVenList);
                _db.SaveChanges();

                MessageStr = "刪除成功";
            }
            catch (Exception e)
            {
                MessageStr = e.ToString();
            }

            return (MessageStr);
        }
    }
}