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
        public PCCSContext _db = new PCCSContext();

        //關閉資料庫
        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }

        public string SearchDepToVen(string DepNo, string DepName)
        {
            var _DepSearchCondition = _db.MXIC_DepartmentManagements.Select(x => new { x.DeleteID, x.DepName, x.VendorName,  x.DepNo }).OrderBy(x=>x.DepName);

            //如果DepNo不為空
            if (!string.IsNullOrWhiteSpace(DepNo))
            {
                _DepSearchCondition = _DepSearchCondition.Where(x => x.DepNo.Contains(DepNo)).OrderBy(x => x.DepName);
            }

            //如果DepName不為空
            if (!string.IsNullOrWhiteSpace(DepName))
            {
                _DepSearchCondition = _DepSearchCondition.Where(x => x.DepName.Contains(DepName)).OrderBy(x => x.DepName);
            }

            string responseStr = JsonConvert.SerializeObject(_DepSearchCondition, Formatting.Indented);

            return (responseStr);
        }

        public string AddDepToVen(string DepName, string VendorName)
        {
            string MessageStr = "新增成功!";

            if (!string.IsNullOrWhiteSpace(DepName) && !string.IsNullOrWhiteSpace(VendorName))
            {
                try
                {
                    string[] VendorList = null;

                    var DepNo = _db.MXIC_UserManagements.Where(x => x.DepName == DepName).Select(x => x.DepNo).FirstOrDefault();

                    
                        VendorName = VendorName.Substring(0, VendorName.Length - 1);

                        VendorList = VendorName.Split(',');

                    var OriginalVendor = _db.MXIC_DepartmentManagements.Where(y => y.DepName == DepName );



                    for (int x = 0; x < VendorList.Length; x++)
                    {
                        var AddVendorName = VendorList[x];
                        OriginalVendor = OriginalVendor.Where(y => y.VendorName == AddVendorName);
                        if (OriginalVendor.Any())
                        {
                            MessageStr = VendorList[x]+"已加入!";

                        }
                        else { 
                      


                        var AddVendor = new Models.DepartmentManagement();
                        AddVendor.DepName = DepName;
                        AddVendor.DepNo = DepNo;
                        AddVendor.VendorName = VendorList[x];
                        AddVendor.DepID = Guid.NewGuid();
                        AddVendor.DeleteID= Guid.NewGuid();
                        AddVendor.EditID= Guid.NewGuid();
                        _db.MXIC_DepartmentManagements.Add(AddVendor);

                        }
                    }

                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageStr = ex.ToString();
                }
            }
            else { MessageStr = "欄位未填"; }

            return (MessageStr);
        }

        //public string EditDepToVen(string EditID, string VendorName)
        //{
        //    string MessageStr = "修改成功";

        //    //var EditDepToVenListItem = _db.MXIC_DepartmentManagements.Where(x => x.EditID.ToString() == EditID).FirstOrDefault();

        //    //try
        //    //{
        //    //    EditDepToVenListItem.VendorName = VendorName;
        //    //    _db.SaveChanges();
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    MessageStr = e.ToString();
        //    //}

        //    return (MessageStr);
        //}

        public string DeleteDepToVen(string DeleteID)
        {
            string MessageStr = "刪除失敗!";

            try
            {
                if (!string.IsNullOrWhiteSpace(DeleteID))
                { 
                string[] DeleteIDList = null;
                DeleteID = DeleteID.Replace("jqg_grid_gb1_","").TrimEnd(',');
                DeleteIDList = DeleteID.Split(',');
                foreach(var item  in DeleteIDList) { 
                Models.DepartmentManagement DeleteDepToVenList = _db.MXIC_DepartmentManagements.Where(x => x.DeleteID.ToString() == item).FirstOrDefault();

                _db.MXIC_DepartmentManagements.Remove(DeleteDepToVenList);
                }
                _db.SaveChanges();

                MessageStr = "刪除成功!";
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
        public string Venderdata()
        {
            var _Venderdata = _db.MXIC_VendorManagements.Select(x => new { x.VendorName }).Distinct().OrderBy(x => x.VendorName).ToList();

            string responseStr = JsonConvert.SerializeObject(_Venderdata, Formatting.Indented);

            return (responseStr);
        }
    }
}