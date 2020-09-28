using MXIC_PCCS.DataUnity.BusinessUnity;
using MXIC_PCCS.DataUnity.Interface;
using MXIC_PCCS.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MXIC_PCCS.Controllers
{
    [Authorize]
    public class QuotationController : Controller
    {
        IQuotation _IQuotation = new Quotation();
        StringBuilder SB = new StringBuilder();

        // GET: Quotation
        public ActionResult Index()
        {
            var id = HttpContext.User.Identity.Name;
            ViewBag.ID = id;
            return View();
        }

        [HttpPost]
        public ActionResult UploadQuotation(HttpPostedFileBase file)
        {
            try
            {
                //先清空資料庫
                _IQuotation.ClearTable();

                //再匯入EXCEL
                //EPPLUS 授權 (不可註解刪除)
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 

                //檢查匯入的EXCEL檔
                if (file != null && file.ContentLength > 0)
                {
                    //宣告儲存空間(儲存報價單的內容)
                    string VendorName, PoNo;

                    List<MXIC_QuotationProperty> Property_ListModel = new List<MXIC_QuotationProperty>();
             
                    //以下是EXCEL讀檔
                    using (var excelPkg = new ExcelPackage(file.InputStream))
                    {
                        //取得Sheet
                        ExcelWorksheet sheet = excelPkg.Workbook.Worksheets["PO報價"];

                        //廠商和PO 直接讀取對應位置的內容
                        //抓不到就先直接return 
                        if (sheet.Cells[5, 7].Text.Contains("供應商Vendor") && string.IsNullOrWhiteSpace(sheet.Cells[5, 9].Text))
                        {
                            VendorName = sheet.Cells[5, 9].Text;
                        }
                        else
                        {
                            SB.Clear();
                            SB.AppendFormat("<script>alert('找不到供應商名稱!');window.location.href='../Quotation/Index';</script>");
                            return Content(SB.ToString());
                        }
                        
                        if (sheet.Cells[7, 7].Text.Contains("PO NO.") && string.IsNullOrWhiteSpace(sheet.Cells[7, 9].Text))
                        {
                            PoNo = sheet.Cells[7, 9].Text;
                        }
                        else
                        {
                            SB.Clear();
                            SB.AppendFormat("<script>alert('找不到PO Number!');window.location.href='../Quotation/Index';</script>");
                            return Content(SB.ToString());
                        }

                        //剩下的資料範圍
                        int startRowIndex = 9;//起始列
                        int endRowIndex = 33;//結束列
                        int startColumn = 4;//開始欄
                        int endColumn = 7;//結束欄

                        //直接讀取剩下的資料
                        for (int currentRow = startRowIndex; currentRow <= endRowIndex; currentRow++)
                        {
                            MXIC_QuotationProperty Property_Model = new MXIC_QuotationProperty();
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
                                        case 4:
                                            Property_Model.PoClassID = sheet.Cells[currentRow, currentColumn].Text;
                                            break;
                                        case 5:
                                            Property_Model.PoClassName = sheet.Cells[currentRow, currentColumn].Text;
                                            break;
                                        case 6:
                                            Property_Model.Unit = sheet.Cells[currentRow, currentColumn].Text;
                                            break;
                                        case 7:
                                            Property_Model.Amount = sheet.Cells[currentRow, currentColumn].Text;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            Property_ListModel.Add(Property_Model);
                        }
                        _IQuotation.ImportQuotation(VendorName, PoNo, Property_ListModel);
                    }
                }
            }
            catch (Exception ex)
            {
                SB.Clear();
                SB.AppendFormat(ex.ToString());
            }
            return Content(SB.ToString());
        }

        public string SearchQuotation(string VendorName, string PoNo, string PoClassID)
        {
            //呼叫IQuotation介面中的QuotationList方法
            string jsonStr = _IQuotation.SearchQuotation(VendorName, PoNo, PoClassID);

            return jsonStr;
        }

        public ActionResult DownloadQuotationExample()
        {
            try
            {
                string filepath = Server.MapPath("~/Content/報價單範本.xlsx");
                string filename = Path.GetFileName(filepath);
                Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/xlsx", filename);
            }
            catch (Exception ex)
            {
                SB.Clear();
                SB.AppendFormat("<script>alert('下載失敗!');window.location.href='../Quotation/Index';</script>");
                return Content(SB.ToString());
            }
        }
    }
}