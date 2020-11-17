﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using MXIC_PCCS.DataUnity.Interface;
using MXIC_PCCS.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;

namespace MXIC_PCCS.DataUnity.BusinessUnity
{
    public class ScheduleSetting : IScheduleSetting, IDisposable
    {

        public PCCSContext _db = new PCCSContext();

        //關閉資料庫
        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }

        public string AddSchedul(ScheduleProperty Model)
        {
            string Str = "匯入成功";

            var AddSchedul = new Models.ScheduleSetting()
            {
                Date = Model.WorkDate,
                EmpName = Model.EmpName,
                DayWeek = Model.DayWeek,
                PoNo = Model.PoNo,
                WorkShift = Model.WorkShift,
                ScheduleID = Guid.NewGuid(),
                WorkGroup = Model.WorkGroup
            };

            _db.MXIC_ScheduleSettings.Add(AddSchedul);

            _db.SaveChanges();

            return (Str);
        }

        public string ImportSchedul(HttpPostedFileBase file)
        {
            string str = "匯入成功";

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                if (file != null && file.ContentLength > 0)
                {
                    //年,月,PO號碼
                    string Year, Month, PoNo;
                    
                    //以下是讀檔
                    using (var excelPkg = new ExcelPackage(file.InputStream))
                    {
                        ExcelWorksheet sheet = excelPkg.Workbook.Worksheets["出勤班表"];//取得Sheet

                        int startRowIndex = sheet.Dimension.Start.Row + 5;//起始列
                        int endRowIndex = sheet.Dimension.End.Row;//結束列

                        int startColumn = sheet.Dimension.Start.Column;//開始欄
                        int endColumn = sheet.Dimension.End.Column;//結束欄

                        Year = sheet.Cells[3, 14].Text;
                        Month = sheet.Cells[3, 17].Text;
                        PoNo = sheet.Cells[3, 2].Text;

                        //清除當月資料
                        CleanSchedul(Year, Month, PoNo);

                        for (int currentRow = startRowIndex; currentRow <= endRowIndex; currentRow++)
                        {
                            //排班組別,姓名,班別,上班日期,星期
                            string WorkGroup = "", EmpName = "", WorkShift = "", workdate = "", DayWeek = "";
                            for (int currentColumn = startColumn; currentColumn <= endColumn; currentColumn++)
                            {
                                if(string.IsNullOrWhiteSpace(sheet.Cells[currentRow, 1].Text))
                                {
                                    continue;
                                }
                                if (currentColumn == 1)
                                {
                                    WorkGroup = sheet.Cells[currentRow, currentColumn].Text;
                                }
                                if (currentColumn == 2)
                                {
                                    EmpName = sheet.Cells[currentRow, currentColumn].Text;
                                }
                                if (currentColumn >= 3)
                                {
                                    if (!string.IsNullOrEmpty(sheet.Cells[4, currentColumn].Text) && !string.IsNullOrWhiteSpace(sheet.Cells[4, currentColumn].Text))
                                    {
                                        WorkShift = sheet.Cells[currentRow, currentColumn].Text;
                                        DayWeek = "星期" + sheet.Cells[5, currentColumn].Text;
                                        workdate = Year + '/' + Month + '/' + sheet.Cells[4, currentColumn].Text;

                                        ScheduleProperty model = new ScheduleProperty();
                                        model.PoNo = PoNo;
                                        model.WorkDate = Convert.ToDateTime(workdate);
                                        model.DayWeek = DayWeek;
                                        model.EmpName = EmpName;
                                        model.WorkShift = WorkShift;
                                        model.WorkGroup = WorkGroup;

                                        //新增進資料庫
                                        str = AddSchedul(model);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                str = ex.ToString();
            }

            return str;
        }

        public string ScheduleList(DateTime? StartTime, DateTime? EndTime, string PoNo)
        {
            var _ScheduleList = _db.MXIC_ScheduleSettings.Select(x => new { x.PoNo, x.Date, x.DayWeek, x.WorkShift, x.EmpName }).OrderBy(x=>new { x.Date,x.EmpName});

            if(!string.IsNullOrWhiteSpace(PoNo))
            {
                _ScheduleList = _ScheduleList.Where(x => x.PoNo.Contains(PoNo)).OrderBy(x => new { x.Date, x.EmpName });
            }
            if (!string.IsNullOrWhiteSpace(StartTime.ToString()))
            {
                DateTime ScheduleDateEnd = Convert.ToDateTime(EndTime).AddDays(1);
                _ScheduleList = _ScheduleList.Where(x => x.Date >= StartTime && x.Date < ScheduleDateEnd).OrderBy(x => new { x.Date, x.EmpName });
            }
            
            string str = JsonConvert.SerializeObject(_ScheduleList, Formatting.Indented);
            str = str.Replace("T00:00:00", "");
            return str;
        }

        public void CleanSchedul(string Year, string Month, string PoNo)
        {
            var _ScheduleList = _db.MXIC_ScheduleSettings.Where(x => x.Date.Year.ToString() == Year && x.Date.Month.ToString() == Month && x.PoNo == PoNo);

            _db.MXIC_ScheduleSettings.RemoveRange(_ScheduleList);
            _db.SaveChanges();
        }

        public MemoryStream ExportSchedul(string Year, string Month, string PoNo)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ep.Workbook.Worksheets.Add("班表產出");
            ExcelWorksheet sheet = ep.Workbook.Worksheets["班表產出"];

            var rowStart = 1; //設定起始行
            var colStart = 1; //設定起始欄
            sheet.DefaultColWidth = 4; //預設列寬
            sheet.Column(1).Width = 5.5; //第一列列寬
            //sheet.Cells.Style.Font.Name = "新細明體"; //預設字體
            //sheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; //文字水平置中
            //sheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//文字垂直置中

            #region 有年月的那一行
            sheet.Row(rowStart).Height = 30; //第一行行高
            sheet.Row(rowStart).Style.Font.Size = 18; //第一行字體大小
            sheet.Cells[rowStart, 1].Value = "PoNo : "+ PoNo;
            sheet.Cells[rowStart, 19, rowStart, 23].Merge = true;
            sheet.Cells[rowStart, 19].Value = "TGCM";
            sheet.Cells[rowStart, 25, rowStart, 29].Merge = true;
            sheet.Cells[rowStart, 25].Value = Year + "年";
            sheet.Cells[rowStart, 31, rowStart, 33].Merge = true;
            sheet.Cells[rowStart, 31].Value = Month + "月";
            sheet.Cells[rowStart, 31].Style.Font.Color.SetColor(Color.Red);
            sheet.Cells[rowStart, 35, rowStart, 40].Merge = true;
            sheet.Cells[rowStart, 35].Value = "出勤月報表";
            #endregion

            #region 日期的那一行
            var DaysInMonth = DateTime.DaysInMonth(Convert.ToInt32(Year), Convert.ToInt32(Month)); //當月有幾天
            var RowDate = rowStart + 1; //日期的那一行
            var ColDate = 3; //日期後起使列

            sheet.Cells[RowDate, 1, RowDate, 2].Merge = true;

            sheet.Cells[RowDate, 1].Value = "日期";
            sheet.Cells[RowDate, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[RowDate, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(224,224,224));//設定背景顏色

            for (int i = 1; i <= DaysInMonth; i++)
            {
                //從第三列開始,每兩列合併
                sheet.Cells[RowDate, ColDate, RowDate, ColDate + 1].Merge = true;

                sheet.Cells[RowDate, ColDate].Value = String.Format("{0:00}", i);
                sheet.Cells[RowDate, ColDate].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[RowDate, ColDate].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(224, 224, 224));//設定背景顏色
                ColDate += 2;
            }
            #endregion

            #region 星期的那一行
            var RowDayofWeek = rowStart + 2; //星期的那一行
            var ColDayofWeek = 3; //日期後起使列
            sheet.Row(RowDayofWeek).Height = 23; //星期的那行高

            sheet.Cells[RowDayofWeek, 1, RowDayofWeek, 2].Merge = true;
            sheet.Cells[RowDayofWeek, 1].Value = "星期";
            //將英文的星期對照至中文
            string[] Day = new string[] { "日", "一", "二", "三", "四", "五", "六" };
            for (int i = 1; i <= DaysInMonth; i++)
            {
                DateTime DayofWeekDate = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(i));
                string DayofWeek = Day[Convert.ToInt32(DayofWeekDate.DayOfWeek)];
                //從第三列開始,每兩列合併
                sheet.Cells[RowDayofWeek, ColDayofWeek, RowDayofWeek, ColDayofWeek + 1].Merge = true;
                sheet.Cells[RowDayofWeek, ColDayofWeek].Value = DayofWeek;
                if (DayofWeek == "日" || DayofWeek == "六")
                {
                    sheet.Cells[RowDayofWeek, ColDayofWeek].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[RowDayofWeek, ColDayofWeek].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 153, 204));//設定背景顏色
                }
                ColDayofWeek += 2;
            }
            #endregion

            #region 上下班那行
            bool ChangeBackColor = false;//是否要整行變色
            var RowWorkGroup = rowStart + 3; //上下班的那一行
            var ColWorkGroup = 3; //班別/人名後起使列

            sheet.Row(RowWorkGroup).Height = 40; //上下班的那行高
            sheet.Cells[RowWorkGroup, 1, RowWorkGroup, 2].Merge = true;
            sheet.Cells[RowWorkGroup, 1].Value = "班別/人名";
            sheet.Cells[RowWorkGroup, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[RowWorkGroup, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(56, 172, 255));//設定背景顏色

            for (int i = 0; i < DaysInMonth; i++)
            {

                sheet.Cells[RowWorkGroup, ColWorkGroup + i].Style.TextRotation = 255; //轉成垂直
                sheet.Cells[RowWorkGroup, ColWorkGroup + i].Value = "上班";
                sheet.Cells[RowWorkGroup, ColWorkGroup + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[RowWorkGroup, ColWorkGroup + i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(56, 172, 255));//設定背景顏色


                sheet.Cells[RowWorkGroup, ColWorkGroup + i + 1].Style.TextRotation = 255;//轉成垂直
                sheet.Cells[RowWorkGroup, ColWorkGroup + i + 1].Value = "下班";
                sheet.Cells[RowWorkGroup, ColWorkGroup + i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[RowWorkGroup, ColWorkGroup + i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(56, 172, 255));//設定背景顏色



                ColWorkGroup += 1;
            }
            #endregion

            #region 很多人員的那幾行
            var RowAttendant = rowStart + 4; //人員出勤開始的那一行

            var FirstShiftDate = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), 1);//要判斷的月份
            var LastDayShiftDate = FirstShiftDate.AddMonths(1).AddDays(-FirstShiftDate.AddMonths(1).Day);
            //從廠商管理撈出PONO裡的人員↓
            var _VendorManagement = _db.MXIC_VendorManagements.Where(x => x.PoNo == PoNo).Select(x => new { x.EmpName, x.Shifts });
            foreach (var ListVendorName in _VendorManagement)
            {
                #region 前兩格的組別跟人名
                sheet.Row(RowAttendant).Height = 30;
                sheet.Row(RowAttendant + 1).Height = 30;

                sheet.Cells[RowAttendant, 1, RowAttendant + 1, 1].Merge = true;

                sheet.Cells[RowAttendant, 1, RowAttendant + 1, 1].Value = ListVendorName.Shifts;
                sheet.Cells[RowAttendant, 1, RowAttendant + 1, 1].Style.TextRotation = 255;//文字轉直
                sheet.Cells[RowAttendant, 2, RowAttendant + 1, 2].Merge = true;

                sheet.Cells[RowAttendant, 2, RowAttendant + 1, 2].Value = ListVendorName.EmpName;
                sheet.Cells[RowAttendant, 2, RowAttendant + 1, 2].Style.TextRotation = 255;//文字轉直
                if (ChangeBackColor)//每兩團變色
                {
                    sheet.Cells[RowAttendant, 1, RowAttendant + 1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[RowAttendant, 1, RowAttendant + 1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(199, 235, 250));//設定背景顏色
                    sheet.Cells[RowAttendant, 2, RowAttendant + 1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[RowAttendant, 2, RowAttendant + 1, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(199, 235, 250));//設定背景顏色
                }
                #endregion

                var _FAC_ATTENDLIST = _db.FAC_ATTENDLISTs.OrderBy(x => x.WORK_DATETIME).Where(x => x.WORK_DATETIME <= LastDayShiftDate && x.WORK_DATETIME >= FirstShiftDate && x.WORKER_NAME == ListVendorName.EmpName).Select(x => new { x.ENTRANCE_DATETIME, x.EXIT_DATETIME, x.WORK_DATETIME, x.WORKER_NAME });
                //var _ScheduleSetting = _db.MXIC_ScheduleSettings.OrderBy(x => x.Date).Where(x => x.EmpName == ListVendorName.EmpName && x.Date >= FirstShiftDate && x.Date <= LastDayShiftDate);
                foreach (var ListAttendlist in _FAC_ATTENDLIST)
                {
                    var ColAttendant = 3; //3是日期那行的第三列,就是有日期的那一格,前面的變數ColDate被用掉了，所以不能共用

                    string WORK_DATE = ListAttendlist.WORK_DATETIME.ToString("dd"); //從刷卡紀錄取得日期

                    for (int i = 1; i <= DaysInMonth; i++)
                    {
                        sheet.Cells[RowAttendant + 1, ColAttendant, RowAttendant + 1, ColAttendant + 1].Merge = true; //上下班時間下面那格合併
                        if (ChangeBackColor)//每兩團變色
                        {
                            sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(199, 235, 250));//設定背景顏色
                        }
                        
                        if (sheet.Cells[RowDate, ColAttendant].Value.ToString() == WORK_DATE)
                        {
                            int _Year = Convert.ToInt32(Year);
                            int _Month = Convert.ToInt32(Month);
                            int _WORK_DATE = Convert.ToInt32(WORK_DATE);
                            var _MXIC_SwipeInfos = _db.MXIC_SwipeInfos.Where(x => x.EmpName == ListVendorName.EmpName && x.SwipeTime.Value.Year == _Year && x.SwipeTime.Value.Month == _Month && x.SwipeTime.Value.Day == _WORK_DATE).Select(x => new { x.EmpName, x.CheckType, x.AttendType });
                            string _AttendType = "正常";
                            foreach (var ListSwipeInfos in _MXIC_SwipeInfos)
                            {
                                if (ListSwipeInfos.AttendType != "正常")
                                {
                                    _AttendType = ListSwipeInfos.AttendType;
                                    break;
                                }
                                
                            }
                            switch (_AttendType)
                            {
                                case "正常":
                                    break;
                                case "異常":
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 90, 90));//設定背景顏色
                                    break;
                                case "代日":
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 163, 223));//設定背景顏色
                                    break;
                                case "代早":
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(11, 158, 0));//設定背景顏色
                                    break;
                                case "代夜":
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200, 148, 255));//設定背景顏色
                                    break;
                                case "請假":
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(212, 212, 212));//設定背景顏色
                                    break;
                                case "遲到":
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 251, 36));//設定背景顏色
                                    break;
                                case "加班":
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(61, 103, 255));//設定背景顏色
                                    break;
                                default:
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    sheet.Cells[RowAttendant, ColAttendant, RowAttendant + 1, ColAttendant + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 90, 90));//設定背景顏色
                                    break;
                            }
                            sheet.Cells[RowAttendant + 1, ColAttendant].Value = _AttendType;

                            DateTime _ENTRANCE_DATETIME = Convert.ToDateTime(ListAttendlist.ENTRANCE_DATETIME); //上班時間
                            sheet.Cells[RowAttendant, ColAttendant].Value = _ENTRANCE_DATETIME.ToString("HH:mm");
                            sheet.Cells[RowAttendant, ColAttendant].Style.Font.Size = 8;

                            DateTime _EXIT_DATETIME = Convert.ToDateTime(ListAttendlist.EXIT_DATETIME); //下班時間
                            sheet.Cells[RowAttendant, ColAttendant + 1].Value = _EXIT_DATETIME.ToString("HH:mm");
                            sheet.Cells[RowAttendant, ColAttendant + 1].Style.Font.Size = 8;
                        }
                        //因為有合併要一次跳兩格
                        ColAttendant += 2;
                    }
                }

                RowAttendant += 2; //有合併所以一次跳兩行
                ChangeBackColor = !ChangeBackColor;//每兩行換一次背景顏色
            }

            #endregion

            #region 最後的備註處與圖例

            //從最後一行給合併起來
            sheet.Cells[sheet.Dimension.End.Row + 1, 1, sheet.Dimension.End.Row + 6, sheet.Dimension.End.Column].Merge = true;
            //插入圖片
            Image img = Image.FromFile(HttpContext.Current.Server.MapPath(@"~\Content\image\產出班表圖例.png"));
            ExcelPicture pic = sheet.Drawings.AddPicture("圖例", img);
            pic.From.Row = sheet.Dimension.End.Row + 1;
            pic.From.Column = 20;
            //pic.SetPosition(sheet.Dimension.End.Row + 1, 1);
            #endregion

            //邊框顏色與文字置中
            using (ExcelRange range = sheet.Cells[sheet.Dimension.Start.Row + 1, sheet.Dimension.Start.Column, sheet.Dimension.End.Row, sheet.Dimension.End.Column])
            {
                range.Style.Font.Name = "新細明體"; //預設字體
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; //文字水平置中
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//文字垂直置中
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Color.SetColor(Color.FromArgb(56, 172, 255));
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Color.SetColor(Color.FromArgb(56, 172, 255));
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Color.SetColor(Color.FromArgb(56, 172, 255));
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Color.SetColor(Color.FromArgb(56, 172, 255));
            }

            MemoryStream fileStream = new MemoryStream();
            ep.SaveAs(fileStream);
            ep.Dispose();
            fileStream.Position = 0;
            return fileStream;
        }


        public string DelSchedule(string ScheduleDate, string SchedulePoNo)
        {
            string MSG = "刪除失敗!";
            try {
                if (!string.IsNullOrWhiteSpace(ScheduleDate) && !string.IsNullOrWhiteSpace(SchedulePoNo))
                {
                    DateTime TheMonthStart = DateTime.Parse(ScheduleDate);

                    DateTime TheMonthEnd = new DateTime(TheMonthStart.Year, TheMonthStart.Month, DateTime.DaysInMonth(TheMonthStart.Year, TheMonthStart.Month)).AddDays(1).AddSeconds(-1); //本月月底

                    var history = _db.MXIC_ScheduleSettings.Where(x => x.Date >= TheMonthStart && x.Date < TheMonthEnd);
                    if (history.Any())
                    {
                        foreach (var item in history)
                        {
                            _db.MXIC_ScheduleSettings.Remove(item);
                        }

                        _db.SaveChanges();
                        MSG = "刪除成功";
                    }
                    else
                    {

                        MSG = "查無班表資料";
                    }
                }
                else
                {

                    MSG = "欄位未填";
                }

            }
            catch(Exception ex)
            {
                MSG = ex.ToString();

            }
            return MSG;
        }


    }
}