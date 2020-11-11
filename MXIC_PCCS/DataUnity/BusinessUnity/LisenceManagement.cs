﻿using MXIC_PCCS.DataUnity.Interface;
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
        public PCCSContext _db = new PCCSContext();

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

            responseStr = responseStr.Replace("T00:00:00", "");

            return (responseStr);
        }

        public string AddLisence(string PoNo, string EmpName, string LicName, DateTime EndDate)
        {
            string MessageStr ;

            if (!string.IsNullOrWhiteSpace(PoNo) && !string.IsNullOrWhiteSpace(EmpName) && !string.IsNullOrWhiteSpace(LicName))
            {
                var check = _db.MXIC_LisenceManagements.Where(x => x.PoNo == PoNo && x.EmpName == EmpName && x.LicName == LicName);

                if (check.Any())
                {
                    MessageStr = "資料重複";
                }
                else
                {
                    var AddLisenceItem = new Models.LisenceManagement()
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

                    MessageStr = "新增成功";
                }
            }
            else
            {

                MessageStr = "新增失敗，請輸入所有資料。";
                

            }
            return (MessageStr);

        }

        public string EditLisence(string EditID, string PoNo, string EmpName, string LicName, DateTime EndDate)
        {
            string MessageStr = "修改成功";

            var EditLisenceItem = _db.MXIC_LisenceManagements.Where(x => x.EditID.ToString() == EditID).FirstOrDefault();

            try
            {
                EditLisenceItem.PoNo = PoNo;
                EditLisenceItem.EmpName = EmpName;
                EditLisenceItem.LicName = LicName;
                EditLisenceItem.EndDate = EndDate;
                EditLisenceItem.UpDateTime = DateTime.Now;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageStr = ex.ToString();
            }

            return (MessageStr);
        }

        public string EditLisenceDetail(string EditID)
        {
            var LisenceDetail = _db.MXIC_LisenceManagements.Where(x => x.EditID.ToString() == EditID).Select(x => new { x.PoNo, x.EmpName, x.LicName, x.EndDate });

            string responseStr = JsonConvert.SerializeObject(LisenceDetail, Formatting.Indented);

            responseStr= responseStr.Replace("T00:00:00", "");

            return (responseStr);
        }

        public string DeleteLisence(string DeleteID)
        {
            string MessageStr = "刪除失敗";
            try
            {
                if (!string.IsNullOrWhiteSpace(DeleteID))
                {
                    string[] DeleteIDList = null;
                    DeleteID = DeleteID.Replace("jqg_grid_gb1_", "").TrimEnd(',');
                    DeleteIDList = DeleteID.Split(',');
                    foreach (var item in DeleteIDList)
                    {
                        Models.LisenceManagement DeleteLisenceList = _db.MXIC_LisenceManagements.Where(x => x.DeleteID.ToString() == item).FirstOrDefault();

                        _db.MXIC_LisenceManagements.Remove(DeleteLisenceList);
                    }
                _db.SaveChanges();

                MessageStr = "刪除成功";
                }
                else
                {
                    MessageStr = "刪除失敗!請勾選刪除資料。";
                }
            }
            catch (Exception ex)
            {
                MessageStr = ex.ToString();
            }
            return (MessageStr);
        }

        public string ImportLisence(string PoNo, List<LisenceProperty> Property_ListModel)
        {
            string MessageStr = "匯入成功";
            DateTime ImportTime = DateTime.Now;

            for (int i = 0; i < Property_ListModel.Count; i++)
            {
                var AddLisenceItem = new Models.LisenceManagement()
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

        public string ClearTable(string PoNo)
        {
            string MessageStr = "判讀結束!";
            try
            {
                var Rows = _db.MXIC_LisenceManagements.Where(x => x.PoNo == PoNo);
                if (Rows.Count() > 0)
                {
                    foreach (var DataRow in Rows)
                    {
                        _db.MXIC_LisenceManagements.Remove(DataRow);
                    }
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageStr = ex.ToString();
            }
            return (MessageStr);
        }
    }
}