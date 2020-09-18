using MXIC_PCCS.DataUnity.BusinessUnity;
using MXIC_PCCS.DataUnity.Interface;
using MXIC_PCCS.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MXIC_PCCS.Controllers
{
    public class LisenceManagementController : Controller
    {
        ILisenceManagement _ILisenceManagement = new LisenceManagement();
        // GET: LisenceManagement
        public ActionResult Index()
        {
            return View();
        }

        public string SearchLisence(string PoNo, string EmpName, string LicName)
        {
            string str = _ILisenceManagement.SearchLisence(PoNo, EmpName, LicName);
           
            return str;
        }

        public string AddLisence(string PoNo, string EmpName, string LicName, DateTime EndDate)
        {
            string str = _ILisenceManagement.AddLisence(PoNo, EmpName, LicName, EndDate);

            return str;
        }

        public string EditLisence(string EditID, string PoNo, string EmpName, string LicName, DateTime EndDate)
        {
            string str = _ILisenceManagement.EditLisence(EditID, PoNo, EmpName, LicName, EndDate);

            return str;
        }

        public string EditLisenceDetail(string EditID)
        {
            string str = _ILisenceManagement.EditLisenceDetail(EditID);

            return str;
        }
        public string DeleteLisence(string DeleteID)
        {
            string str = _ILisenceManagement.DeleteLisence(DeleteID);

            return str;
        }

        [HttpPost]
        public ActionResult UploadLisence(HttpPostedFileBase file)
        {
            try
            {
                //網頁有新增修改功能，所以不清空資料
                //匯入EXCEL
                //EPPLUS 授權 (不可註解刪除)
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                //檢查匯入的EXCEL檔
                if (file != null && file.ContentLength > 0)
                {
                    //宣告儲存空間(儲存證照的內容)
                    string PoNo;
                    List<MXIC_LisenceProperty> Property_ListModel = new List<MXIC_LisenceProperty>();

                    using (var excelPkg = new ExcelPackage(file.InputStream))
                    {
                        //取得Sheet
                        ExcelWorksheet sheet = excelPkg.Workbook.Worksheets["證照"];

                        //PO直接讀取對應位置的內容
                        //抓不到就先直接return 
                        if (sheet.Cells[1, 1].Text.Contains("PO No."))
                        {
                            PoNo = sheet.Cells[1, 2].Text;
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }

                        //剩下的資料範圍
                        int startRowIndex = 3;//起始列
                        int endRowIndex = sheet.Dimension.Rows;//結束列
                        int startColumn = 1;//開始欄
                        int endColumn = 4;//結束欄

                        //直接讀取剩下的資料
                        for (int currentRow = startRowIndex; currentRow <= endRowIndex; currentRow++)
                        {
                            MXIC_LisenceProperty Property_Model = new MXIC_LisenceProperty();
                            for (int currentColumn = startColumn; currentColumn <= endColumn; currentColumn++)
                            {
                                if (sheet.Cells[currentRow, currentColumn].Text == "")
                                {
                                    break;
                                }
                                else
                                {
                                    //Response.Write(sheet.Cells[currentRow, currentColumn].Text);
                                    //Response.Write("<br/>");

                                    switch (currentColumn)
                                    {
                                        case 1:
                                            Property_Model.EmpName = sheet.Cells[currentRow, currentColumn].Text;
                                            break;
                                        case 2:
                                            Property_Model.LicName = sheet.Cells[currentRow, currentColumn].Text;
                                            break;
                                        case 3:
                                            Property_Model.EndDate = sheet.Cells[currentRow, currentColumn].Text;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            Property_ListModel.Add(Property_Model);
                        }
                       string MessageStr = _ILisenceManagement.ImportLisence(PoNo, Property_ListModel);
                    }
                }
            }
            catch (Exception ex)
            {
                //網頁跳出錯誤訊息 或是 寫ErrorLog

            }
            return RedirectToAction("Index");
        }

        public ActionResult DownloadLisenceExample()
        {
            try
            {
                string filepath = Server.MapPath("~/Content/證照範本.xlsx");
                string filename = Path.GetFileName(filepath);
                Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/xlsx", filename);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}