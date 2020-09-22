using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MXIC_PCCS.DataUnity.Interface;
using MXIC_PCCS.Models;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace MXIC_PCCS.DataUnity.BusinessUnity
{
    public class ScheduleSetting : IScheduleSetting, IDisposable
    {

        public MXIC_PCCSContext _db = new MXIC_PCCSContext();
        //關閉資料庫
        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }

        public string AddSchedul(MXIC_ScheduleProperty Model)
        {
            string Str = "匯入成功";

            var AddSchedul = new MXIC_ScheduleSetting()
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
                        CleanSchedul(Year, Month);
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
                                    WorkShift = sheet.Cells[currentRow, currentColumn].Text;
                                    DayWeek = "星期" + sheet.Cells[5, currentColumn].Text;
                                    workdate = Year +'/'+ Month +'/'+ sheet.Cells[4, currentColumn].Text;

                                    MXIC_ScheduleProperty model = new MXIC_ScheduleProperty();
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
            catch (Exception ex)
            {
                str = ex.ToString();
            }
            return str;
        }

        public string ScheduleList(DateTime? ScheduleDate, string PoNo)
        {
            var _ScheduleList = _db.MXIC_ScheduleSettings.Select(x => new { x.PoNo, x.Date, x.DayWeek, x.WorkShift, x.EmpName });

            if(!string.IsNullOrWhiteSpace(PoNo))
            {
                _ScheduleList = _ScheduleList.Where(x => x.PoNo.Contains(PoNo));
            }
            if (!string.IsNullOrWhiteSpace(ScheduleDate.ToString()))
            {
                DateTime ScheduleDateEnd = Convert.ToDateTime(ScheduleDate).AddDays(1);
                _ScheduleList = _ScheduleList.Where(x => x.Date >= ScheduleDate && x.Date < ScheduleDateEnd);
            }
            
            string str = JsonConvert.SerializeObject(_ScheduleList, Formatting.Indented);
            str = str.Replace("T00:00:00", "");
            return str;

        }

        public void CleanSchedul(string Year, string Month)
        {
            var _ScheduleList = _db.MXIC_ScheduleSettings.Where(x => x.Date.Year.ToString() == Year && x.Date.Month.ToString() == Month);


            _db.MXIC_ScheduleSettings.RemoveRange(_ScheduleList);
            _db.SaveChanges();


        }
    }
}