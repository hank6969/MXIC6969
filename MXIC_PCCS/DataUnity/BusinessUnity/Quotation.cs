using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MXIC_PCCS.DataUnity.Interface;
using MXIC_PCCS.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Web.Mvc;
using System.Text;

namespace MXIC_PCCS.DataUnity.BusinessUnity
{
    public class Quotation : IQuotation, IDisposable
    {
        //開啟資料庫連結
        public MXIC_PCCSContext _db = new MXIC_PCCSContext();

        //關閉資料庫
        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }

        public string SearchQuotation(string VendorName, string PoNo, string PoClassID)
        {
            var _QuotationSearchCondition = _db.MXIC_Quotations.OrderBy(x => new { x.PoNo ,x.Sequence }).Select(x=>new { x.PoNo , x.PoClassID, x.PoClassName, x.Unit, x.Amount, x.VendorName});
            
            //如果VendorName不為空
            if (!string.IsNullOrWhiteSpace(VendorName))
            {
                _QuotationSearchCondition = _QuotationSearchCondition.Where(x => x.VendorName.Contains(VendorName));

            }
            //如果PoNo不為空
            if (!string.IsNullOrWhiteSpace(PoNo))
            {
                _QuotationSearchCondition = _QuotationSearchCondition.Where(x => x.PoNo.Contains(PoNo));

            }
            //如果PoClassID不為空
            if (!string.IsNullOrWhiteSpace(PoClassID))
            {
                _QuotationSearchCondition = _QuotationSearchCondition.Where(x => x.PoClassID.ToLower().Contains(PoClassID.ToLower()));

            }

            //_SearchCondition轉換成json字串
            string responseStr = JsonConvert.SerializeObject(_QuotationSearchCondition, Formatting.Indented);

            return (responseStr);
        }

        public string ImportQuotation(string VendorName, string PoNo, List<MXIC_QuotationProperty> Property_ListModel)
        {
            string MessageStr = "匯入成功";

            for (int i=0; i< Property_ListModel.Count;i++)
            {
                var AddQuotationItem = new MXIC_Quotation()
                {
                    QuotationID = Guid.NewGuid(),
                    PoNo = PoNo,
                    VendorName = VendorName,
                    PoClassID = Property_ListModel[i].PoClassID,
                    PoClassName = Property_ListModel[i].PoClassName,
                    LicPossess = "有",
                    Unit = Property_ListModel[i].Unit,
                    Amount = Property_ListModel[i].Amount,
                    Sequence = 1+i
                };

                _db.MXIC_Quotations.Add(AddQuotationItem);
            }
            _db.SaveChanges();

            return (MessageStr);
        }

        public string ClearTableCheck(HttpPostedFileBase file)
        {
            //StringBuilder SB = new StringBuilder();
            string Result = "0";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPkg = new ExcelPackage(file.InputStream))
            {
                string PoNo;
                ExcelWorksheet sheet = excelPkg.Workbook.Worksheets["PO報價"];
                if (sheet.Cells[7, 7].Text.Contains("PO NO.") && !string.IsNullOrWhiteSpace(sheet.Cells[7, 9].Text))
                {
                    PoNo = sheet.Cells[7, 9].Text;

                    var Rows = _db.MXIC_Quotations.Where(x => x.PoNo == PoNo);

                    if (Rows.Count() > 0)
                    {
                        //Po已存在
                        Result ="1";
                    }
                  
                }
                else
                {
                    Result = "0";

                }

                

                return (Result);


            }

            
        }

        public string ClearTable(string PoNo)
        {
            string MessageStr = "判讀結束!";
            try
            {
                var Rows = _db.MXIC_Quotations.Where(x => x.PoNo == PoNo);

                if (Rows.Count() > 0)
                {
                    foreach (var DataRow in Rows)
                    {
                        _db.MXIC_Quotations.Remove(DataRow);
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