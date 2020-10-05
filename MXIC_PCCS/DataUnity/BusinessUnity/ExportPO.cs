using MXIC_PCCS.DataUnity.Interface;
using MXIC_PCCS.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace MXIC_PCCS.DataUnity.BusinessUnity
{
    public class ExportPO : IExportPO, IDisposable
    {
        //開啟資料庫連結
        public MXIC_PCCSContext _db = new MXIC_PCCSContext();

        //關閉資料庫
        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }

        public void DownloadExcel(string PoNo, string VendorName,List<MXIC_CalculationQuotation> CalculationQuotation)
        {
            //Step 1. 寫入EXCEL 
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //TODO : 要使用記得修改路徑 因為寫死阿阿阿QQ
            var ExcelPath  = Properties.Resources.DowloadDirectory;
            var DownloadPath = new FileInfo(ExcelPath);
            using (var Excel = new ExcelPackage(DownloadPath))
            {
                ExcelWorksheet WorkSheet = Excel.Workbook.Worksheets["PO報價"];

                //Step 1-1.把數量寫入計價單   
                foreach (var QuotationItem in CalculationQuotation)
                {
                    if (QuotationItem.Count != 0)
                    {
                        switch (QuotationItem.PoClassID)
                        {                           
                            #region 把數量寫入計價單
                            case "10-1":
                                WorkSheet.Cells[9, 8].Value = QuotationItem.Count;
                                break;
                            case "10-2":
                                WorkSheet.Cells[10, 8].Value = QuotationItem.Count;
                                break;
                            case "10-3":
                                WorkSheet.Cells[11, 8].Value = QuotationItem.Count;
                                break;
                            case "10-4":
                                WorkSheet.Cells[12, 8].Value = QuotationItem.Count;
                                break;
                            case "10-5":
                                WorkSheet.Cells[13, 8].Value = QuotationItem.Count;
                                break;
                            case "10-6":
                                WorkSheet.Cells[14, 8].Value = QuotationItem.Count;
                                break;
                            case "10-7":
                                WorkSheet.Cells[15, 8].Value = QuotationItem.Count;
                                break;
                            case "10-8":
                                WorkSheet.Cells[16, 8].Value = QuotationItem.Count;
                                break;
                            case "10-9":
                                WorkSheet.Cells[17, 8].Value = QuotationItem.Count;
                                break;
                            case "10-10":
                                WorkSheet.Cells[18, 8].Value = QuotationItem.Count;
                                break;
                            case "10-11":
                                WorkSheet.Cells[19, 8].Value = QuotationItem.Count;
                                break;
                            case "10-12":
                                WorkSheet.Cells[20, 8].Value = QuotationItem.Count;
                                break;
                            case "10-13":
                                WorkSheet.Cells[21, 8].Value = QuotationItem.Count;
                                break;
                            case "10-14":
                                WorkSheet.Cells[22, 8].Value = QuotationItem.Count;
                                break;
                            case "10-15":
                                WorkSheet.Cells[23, 8].Value = QuotationItem.Count;
                                break;
                            case "10-16":
                                WorkSheet.Cells[24, 8].Value = QuotationItem.Count;
                                break;
                            case "10-17":
                                WorkSheet.Cells[25, 8].Value = QuotationItem.Count;
                                break;
                            case "10-18":
                                WorkSheet.Cells[26, 8].Value = QuotationItem.Count;
                                break;
                            case "10-19":
                                WorkSheet.Cells[27, 8].Value = QuotationItem.Count;
                                break;
                            case "10-20":
                                WorkSheet.Cells[28, 8].Value = QuotationItem.Count;
                                break;
                            case "10-21":
                                WorkSheet.Cells[29, 8].Value = QuotationItem.Count;
                                break;
                            case "10-22":
                                WorkSheet.Cells[30, 8].Value = QuotationItem.Count;
                                break;
                            case "10-23":
                                WorkSheet.Cells[31, 8].Value = QuotationItem.Count;
                                break;
                            case "10-24":
                                WorkSheet.Cells[32, 8].Value = QuotationItem.Count;
                                break;
                            case "10-25":
                                WorkSheet.Cells[33, 8].Value = QuotationItem.Count;
                                break;
                            default:
                                break;
                                #endregion
                        }

                    }

                }

                //Vendor
                WorkSheet.Cells[5, 9].Value = VendorName;
                 //PO
                WorkSheet.Cells[7, 9].Value = PoNo;

                Excel.Save();
            }
        }

        private void UsualFunction(int WithLisenceCount, string WorkGroup, List<MXIC_CalculationQuotation> QuotationList)
        {
            if (WithLisenceCount == 0)                //有證照
            {
                switch (WorkGroup)                    //班別
                {
                    case "組長":
                        //Console.WriteLine("10-1");
                        QuotationList[1 - 1].Count = QuotationList[1 - 1].Count + 1;
                        break;
                    case "常日":
                        //Console.WriteLine("10-10");
                        QuotationList[10 - 1].Count = QuotationList[10 - 1].Count + 1;
                        break;
                    case "早":
                        //Console.WriteLine("10-8");
                        QuotationList[8 - 1].Count = QuotationList[8 - 1].Count + 1;
                        break;
                    case "夜":
                        //Console.WriteLine("10-9");
                        QuotationList[9 - 1].Count = QuotationList[9 - 1].Count + 1;
                        break;
                    default:
                        break;
                }
            }
            else                                        //無證照
            {
                switch (WorkGroup)                      //班別
                {
                    case "組長":
                        //Console.WriteLine("10-13");
                        QuotationList[13 - 1].Count = QuotationList[13 - 1].Count + 1;
                        break;
                    case "常日":
                        //Console.WriteLine("10-13");
                        QuotationList[13 - 1].Count = QuotationList[13 - 1].Count + 1;
                        break;
                    case "早":
                        //Console.WriteLine("10-11");
                        QuotationList[11 - 1].Count = QuotationList[11 - 1].Count + 1;
                        break;
                    case "夜":
                        //Console.WriteLine("10-12");
                        QuotationList[12 - 1].Count = QuotationList[12 - 1].Count + 1;
                        break;
                    default:
                        break;

                }
            }
        } 

        public string CalcuationPO(string PoNo, DateTime Date)
        {
            // 這個月一半有證照 一半沒有的(X) 還不會寫 QQ 

            string reponseStr = "寫入成功";

            string PoNumber="", VendorName="";

            var AttendError = _db.MXIC_SwipeInfos.Where(x => x.AttendType == "異常");

            var Order = _db.MXIC_Quotations.Where(x => x.PoNo == PoNo);

            if (AttendError.Any())
            {
                reponseStr = "刷卡有異常資料未修改";
            }
            if (!Order.Any())
            {
                reponseStr = "無此PO";
            }
            else
            {
                try
                {
                   
                    // 撈出報價單資料
                    var QuotationList = _db.MXIC_Quotations.OrderBy(x => x.Sequence).Where(x => x.PoNo == PoNo).Select(x => new { x.PoNo, x.VendorName, x.PoClassID, x.Amount });

                    List<MXIC_CalculationQuotation> CalculationQuotation_ListModel = new List<MXIC_CalculationQuotation>();

                    foreach (var QuotationRow in QuotationList)
                    {
                        MXIC_CalculationQuotation CalculationQuotation_Model = new MXIC_CalculationQuotation();
                        CalculationQuotation_Model.PoClassID = QuotationRow.PoClassID;
                        CalculationQuotation_Model.Amount = QuotationRow.Amount;
                        CalculationQuotation_Model.Count = 0.0;
                        CalculationQuotation_ListModel.Add(CalculationQuotation_Model);
                        PoNumber = QuotationRow.PoNo;
                        VendorName = QuotationRow.VendorName;
                    }
                    //本月初
                    DateTime TheMonthStart = new DateTime(Date.Year, Date.Month, 1);//本月初1號

                    //本月底
                    DateTime TheMonthEnd = new DateTime(Date.Year, Date.Month, DateTime.DaysInMonth(Date.Year, Date.Month));//本月初月底

                    // 設定查詢月份
                    DateTime StartDate = TheMonthStart;
                    DateTime EndDate = TheMonthEnd;
                    // 知道人 + 班別
                    var EmpList = _db.MXIC_ScheduleSettings.Where(x => x.PoNo == PoNo&&x.Date>= StartDate && x.Date <= EndDate).Select(x => new { x.EmpName, x.WorkGroup });
                 

                    // 給他查起來
                    foreach (var Emp in EmpList)
                    {
                        // 先查詢結果
                        var IsUnusual = _db.MXIC_SwipeInfos.Where(x => x.valid == "true" && x.EmpName == Emp.EmpName && (x.AttendType != "正常") && x.SwipeTime >= StartDate && x.SwipeTime <= EndDate);
                        var WithLisence = _db.MXIC_LisenceManagements.Where(x => x.EmpName == Emp.EmpName && x.EndDate > EndDate);
                        var WorkGroup = Emp.WorkGroup;

                        // 之後再判斷
                        if (IsUnusual.Count() == 0)                   // 正常
                        {
                            UsualFunction(WithLisence.Count(), WorkGroup, CalculationQuotation_ListModel);
                        }
                        else                                          // 異常 
                        {
                            var WorkHour = 240.0;
                            foreach (var UnusualRow in IsUnusual)
                            {
                                #region 遲到+早退 時間計算
                                if (UnusualRow.AttendType == "遲到" || UnusualRow.AttendType == "早退")
                                {
                                    WorkHour = WorkHour - UnusualRow.Hour;
                                }
                                #endregion
                                else
                                {
                                    switch (UnusualRow.AttendType)
                                    {
                                        #region 加班
                                        case "加班":
                                            if (WithLisence.Count() == 0)                    //有證照
                                            {
                                                switch (WorkGroup)                           //班別
                                                {
                                                    case "組長":
                                                        //Console.WriteLine("10-4(填入時數)"); //UnusualRow.Hour
                                                        CalculationQuotation_ListModel[4 - 1].Count = CalculationQuotation_ListModel[4 - 1].Count + UnusualRow.Hour;
                                                        break;
                                                    case "常日":
                                                        //Console.WriteLine("10-4(填入時數)"); //UnusualRow.Hour
                                                        CalculationQuotation_ListModel[4 - 1].Count = CalculationQuotation_ListModel[4 - 1].Count + UnusualRow.Hour;
                                                        break;
                                                    case "早":
                                                        //Console.WriteLine("10-2(填入時數)"); //UnusualRow.Hour
                                                        CalculationQuotation_ListModel[2 - 1].Count = CalculationQuotation_ListModel[2 - 1].Count + UnusualRow.Hour;
                                                        break;
                                                    case "夜":
                                                        //Console.WriteLine("10-3(填入時數)"); //UnusualRow.Hour
                                                        CalculationQuotation_ListModel[3 - 1].Count = CalculationQuotation_ListModel[3 - 1].Count + UnusualRow.Hour;
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            else                                             //無證照
                                            {
                                                switch (WorkGroup)                           //班別
                                                {
                                                    case "組長":
                                                        //Console.WriteLine("10-7(填入時數)"); //UnusualHour
                                                        CalculationQuotation_ListModel[7 - 1].Count = CalculationQuotation_ListModel[7 - 1].Count + UnusualRow.Hour;
                                                        break;
                                                    case "常日":
                                                        //Console.WriteLine("10-7(填入時數)"); //UnusualHour
                                                        CalculationQuotation_ListModel[7 - 1].Count = CalculationQuotation_ListModel[7 - 1].Count + UnusualRow.Hour;
                                                        break;
                                                    case "早":
                                                        // Console.WriteLine("10-5(填入時數)"); //UnusualHour
                                                        CalculationQuotation_ListModel[5 - 1].Count = CalculationQuotation_ListModel[5 - 1].Count + UnusualRow.Hour;
                                                        break;
                                                    case "夜":
                                                        //Console.WriteLine("10-6(填入時數)"); //UnusualHour
                                                        CalculationQuotation_ListModel[6 - 1].Count = CalculationQuotation_ListModel[6 - 1].Count + UnusualRow.Hour;
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            break;
                                        #endregion

                                        #region 代早
                                        case "代早":
                                            if (WithLisence.Count() == 0)                    //有證照
                                            {
                                                //Console.WriteLine("10-14");
                                                CalculationQuotation_ListModel[14 - 1].Count = CalculationQuotation_ListModel[14 - 1].Count + UnusualRow.Hour;

                                            }
                                            else                                             //無證照
                                            {
                                                //Console.WriteLine("10-16");
                                                CalculationQuotation_ListModel[16 - 1].Count = CalculationQuotation_ListModel[16 - 1].Count + UnusualRow.Hour;
                                            }
                                            break;
                                        #endregion

                                        #region 代晚
                                        case "代晚":
                                            if (WithLisence.Count() == 0)                    //有證照
                                            {
                                                //Console.WriteLine("10-15");
                                                CalculationQuotation_ListModel[15 - 1].Count = CalculationQuotation_ListModel[15 - 1].Count + UnusualRow.Hour;

                                            }
                                            else                                             //無證照
                                            {
                                                //Console.WriteLine("10-17");
                                                CalculationQuotation_ListModel[17 - 1].Count = CalculationQuotation_ListModel[17 - 1].Count + UnusualRow.Hour;
                                            }
                                            break;
                                            #endregion
                                    }
                                }
                            }

                            #region 遲到+早退 or 加班/支援 給本薪 金額計算
                            if (WorkHour == 240.0)                                            //代表他都是加班或支援
                            {
                                UsualFunction(WithLisence.Count(), WorkGroup, CalculationQuotation_ListModel);
                            }
                            else                                                              //遲到早退
                            {
                                if (WithLisence.Count() == 0)                                 //有證照
                                {
                                    if (WorkHour.ToString().Contains(".5"))
                                    {
                                        if (WorkGroup == "夜")
                                        {
                                            // Console.WriteLine("10-21");                        //+1
                                            CalculationQuotation_ListModel[21 - 1].Count = CalculationQuotation_ListModel[21 - 1].Count + 1;
                                        }
                                        else
                                        {
                                            //Console.WriteLine("10-19");                        //+1
                                            CalculationQuotation_ListModel[19 - 1].Count = CalculationQuotation_ListModel[19 - 1].Count + 1;
                                        }

                                    }
                                    switch (WorkGroup)                                         //班別
                                    {
                                        case "組長":
                                            //Console.WriteLine("10-18(填入時數)"); //+Math.Floor(WorkHour);
                                            CalculationQuotation_ListModel[18 - 1].Count = CalculationQuotation_ListModel[18 - 1].Count + Math.Floor(WorkHour);
                                            break;
                                        case "常日":
                                            //Console.WriteLine("10-18(填入時數)"); //+Math.Floor(WorkHour);
                                            CalculationQuotation_ListModel[18 - 1].Count = CalculationQuotation_ListModel[18 - 1].Count + Math.Floor(WorkHour);
                                            break;
                                        case "早":
                                            //Console.WriteLine("10-18(填入時數)"); //+Math.Floor(WorkHour);
                                            CalculationQuotation_ListModel[18 - 1].Count = CalculationQuotation_ListModel[18 - 1].Count + Math.Floor(WorkHour);
                                            break;
                                        case "夜":
                                            // Console.WriteLine("10-20(填入時數)"); //+Math.Floor(WorkHour);
                                            CalculationQuotation_ListModel[20 - 1].Count = CalculationQuotation_ListModel[20 - 1].Count + Math.Floor(WorkHour);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else                                              //無證照
                                {
                                    if (WorkHour.ToString().Contains(".5"))
                                    {
                                        if (WorkGroup == "夜")
                                        {
                                            //Console.WriteLine("10-25"); //+1
                                            CalculationQuotation_ListModel[25 - 1].Count = CalculationQuotation_ListModel[25 - 1].Count + 1;
                                        }
                                        else
                                        {
                                            //Console.WriteLine("10-23"); //+1
                                            CalculationQuotation_ListModel[23 - 1].Count = CalculationQuotation_ListModel[23 - 1].Count + 1;
                                        }

                                    }
                                    switch (WorkGroup)                            //班別
                                    {
                                        case "組長":
                                            //Console.WriteLine("10-22(填入時數)"); //+Math.Floor(WorkHour);
                                            CalculationQuotation_ListModel[22 - 1].Count = CalculationQuotation_ListModel[22 - 1].Count + Math.Floor(WorkHour);
                                            break;
                                        case "常日":
                                            //Console.WriteLine("10-22(填入時數)"); //+Math.Floor(WorkHour);
                                            CalculationQuotation_ListModel[22 - 1].Count = CalculationQuotation_ListModel[22 - 1].Count + Math.Floor(WorkHour);
                                            break;
                                        case "早":
                                            //Console.WriteLine("10-22(填入時數)"); //+Math.Floor(WorkHour);
                                            CalculationQuotation_ListModel[22 - 1].Count = CalculationQuotation_ListModel[22 - 1].Count + Math.Floor(WorkHour);
                                            break;
                                        case "夜":
                                            //Console.WriteLine("10-24(填入時數)"); //+Math.Floor(WorkHour);
                                            CalculationQuotation_ListModel[24 - 1].Count = CalculationQuotation_ListModel[24 - 1].Count + Math.Floor(WorkHour);
                                            break;
                                        default:
                                            break;
                                    }
                                }

                            }
                            #endregion
                        }
                    }
                    DownloadExcel(PoNumber, VendorName, CalculationQuotation_ListModel); 
                }
                catch (Exception ex)
                {
                    reponseStr = "寫入失敗";
                }
            }
            return  (reponseStr);
        }
    }
}