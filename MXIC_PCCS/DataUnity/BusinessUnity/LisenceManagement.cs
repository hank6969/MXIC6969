using MXIC_PCCS.DataUnity.Interface;
using MXIC_PCCS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MXIC_PCCS.DataUnity.BusinessUnity
{
    public class LisenceManagement : ILisenceManagement, IDisposable
    {
        //開啟資料庫連結
        public MXIC_PCCSContext _db = new MXIC_PCCSContext();

        //關閉資料庫
        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }
        public string SearchLisence(string PoNo, string EmpName, string LicName)
        {

            var _LisenceSearchCondition = _db.MXIC_LisenceManagements.Select(x => new { x.PoNo, x.EmpName, x.LicName, x.EndDate,x.EditID,x.DeleteID });


            //如果PoNo不為空
            if (!string.IsNullOrWhiteSpace(PoNo))
            {
                _LisenceSearchCondition = _LisenceSearchCondition.Where(x => x.PoNo.Contains(PoNo));

            }
            //如果EmpName不為空
            if (!string.IsNullOrWhiteSpace(EmpName))
            {
                _LisenceSearchCondition = _LisenceSearchCondition.Where(x => x.EmpName.Contains(EmpName));

            }
            //如果LicName不為空
            if (!string.IsNullOrWhiteSpace(LicName))
            {
                _LisenceSearchCondition = _LisenceSearchCondition.Where(x => x.LicName.Contains(LicName));

            }

            //_LisenceList轉換成json字串
            string responseStr = JsonConvert.SerializeObject(_LisenceSearchCondition, Formatting.Indented);

            return (responseStr);

        }

        public string AddLisence(string PoNo, string EmpName, string LicName, DateTime EndDate)
        {
            string MessageStr = "新增成功";

            var AddLisenceItem = new MXIC_LisenceManagement()
            {
                LicID = Guid.NewGuid(),
                PoNo = PoNo,
                EmpName = EmpName,
                LicName = LicName,
                EndDate = EndDate,
                LicPossess = true,
                UpDateTime = DateTime.Now,
                EditID = Guid.NewGuid(),
                DeleteID = Guid.NewGuid()

            };

            _db.MXIC_LisenceManagements.Add(AddLisenceItem);

            _db.SaveChanges();

            return (MessageStr);
        }

        public string EditLisence(string EditID, string PoNo, string EmpName, string LicName, DateTime EndDate)
        {
            string MessageStr = "修改成功";

            var EditLisenceList = _db.MXIC_LisenceManagements.Where(x => x.EditID.ToString() == EditID).FirstOrDefault();

            try
            {
                EditLisenceList.PoNo = PoNo;
                EditLisenceList.EmpName = EmpName;
                EditLisenceList.LicName = LicName;
                EditLisenceList.EndDate = EndDate;
                EditLisenceList.UpDateTime = DateTime.Now;

                _db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageStr = e.ToString();
            }
            return (MessageStr);
        }

        public string EditLisenceDetail(string EditID)
        {
            var LisenceDetail = _db.MXIC_LisenceManagements.Where(x => x.EditID.ToString() == EditID).Select(x => new { x.PoNo, x.EmpName, x.LicName, x.EndDate });

            string responseStr = JsonConvert.SerializeObject(LisenceDetail, Formatting.Indented);

            return (responseStr);
        }

        public string DeleteLisence(string DeleteID)
        {
            string MessageStr = "刪除失敗";
            try
            {
                MXIC_LisenceManagement DeleteLisenceList = _db.MXIC_LisenceManagements.Where(x => x.DeleteID.ToString() == DeleteID).FirstOrDefault();

                _db.MXIC_LisenceManagements.Remove(DeleteLisenceList);
                _db.SaveChanges();

                MessageStr = "刪除成功";
            }
            catch (Exception e)
            {
                MessageStr = e.ToString();
            }
            return (MessageStr);
        }

        public string ImportLisence(string PoNo, List<MXIC_LisenceProperty> Property_ListModel)
        {
            string MessageStr = "匯入成功";
            DateTime ImportTime = DateTime.Now;

            for (int i = 0; i < Property_ListModel.Count; i++)
            {
                var AddLisenceItem = new MXIC_LisenceManagement()
                {
                    LicID = Guid.NewGuid(),
                    PoNo = PoNo,
                    EmpName = Property_ListModel[i].EmpName,
                    LicName = Property_ListModel[i].LicName,
                    EndDate = Convert.ToDateTime(Property_ListModel[i].EndDate),
                    LicPossess = true,
                    UpDateTime = ImportTime,
                    EditID = Guid.NewGuid(),
                    DeleteID = Guid.NewGuid(),
                };

                _db.MXIC_LisenceManagements.Add(AddLisenceItem);
            }
            _db.SaveChanges();

            return (MessageStr);
        }
    }
}