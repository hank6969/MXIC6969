using MXIC_PCCS.DataUnity.Interface;
using MXIC_PCCS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MXIC_PCCS.DataUnity.BusinessUnity
{
    public class SwipeInfo : ISwipeInfo, IDisposable
    {
        public PCCSContext _db = new PCCSContext();

        public MxicTestContext _dbMXIC = new MxicTestContext();

        //關閉資料庫
        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }
        public string CheckinList(string VendorName, string EmpID, string EmpName, DateTime? StartTime, DateTime? EndTime, string AttendTypeSelect)
        {
            var _List = _db.MXIC_View_Swipes.Select(x => new { x.PoNo, x.VendorName, x.EmpID, x.CheckType, x.EmpName, x.SwipeTime, x.EditID, x.AttendType,x.WorkShift }).OrderBy(x => new { x.PoNo, x.EmpID, x.SwipeTime });

            if (!string.IsNullOrWhiteSpace(VendorName))
            {
                _List = _List.Where(x => x.VendorName.ToLower().Contains(VendorName.ToLower())).OrderBy(x => new { x.PoNo, x.EmpID, x.SwipeTime });
            }

            if (!string.IsNullOrWhiteSpace(EmpID))
            {
                _List = _List.Where(x => x.EmpID.ToLower().Contains(EmpID.ToLower())).OrderBy(x => new { x.PoNo, x.EmpID, x.SwipeTime });
            }

            if (!string.IsNullOrWhiteSpace(EmpName))
            {
                _List = _List.Where(x => x.EmpName.ToLower().Contains(EmpName.ToLower())).OrderBy(x => new { x.PoNo, x.EmpID, x.SwipeTime });
            }

            if (!string.IsNullOrWhiteSpace(StartTime.ToString()) && !string.IsNullOrWhiteSpace(EndTime.ToString()))
            {   //日期加一天 再減去一秒
                DateTime End2 = EndTime.Value.AddDays(1).AddSeconds(-1);
                _List = _List.Where(x => x.SwipeTime >= StartTime && x.SwipeTime <= End2).OrderBy(x => new { x.PoNo, x.EmpID, x.SwipeTime });
            }

            if (!string.IsNullOrWhiteSpace(AttendTypeSelect))
            {
                _List = _List.Where(x => x.AttendType == AttendTypeSelect).OrderBy(x => new { x.PoNo, x.EmpID, x.SwipeTime });
            }

            string Str = JsonConvert.SerializeObject(_List, Formatting.Indented);

            return (Str);
        }

        public string SwipeInfoDetail(string EditID)
        {
            var SwipeInfoDetail = _db.MXIC_SwipeInfos.Where(x => x.EditID.ToString() == EditID).Select(x => new { x.AttendType, x.Hour });

            string Str = JsonConvert.SerializeObject(SwipeInfoDetail, Formatting.Indented);

            return (Str);
        }

        public string EditSwipe(string EditID, string AttendTypeSelect, string Hour)
        {
            string str = "修改失敗";

            try
            {
                var EditSwipeInfo = _db.MXIC_SwipeInfos.Where(x => x.EditID.ToString() == EditID).FirstOrDefault();

                double time;

                if (double.TryParse(Hour, out time))
                {
                    EditSwipeInfo.AttendType = AttendTypeSelect;

                    EditSwipeInfo.Hour = time;
                }
                else
                {
                    str = "時數格式錯誤";
                }

                _db.SaveChanges();

                str = "修改成功";
            }
            catch (Exception e)
            {
                str = e.ToString();
            }

            return (str);
        }

        public string transform()
        {
            var ATTENDLIST = _dbMXIC.FAC_ATTENDLISTs;
            string Str = JsonConvert.SerializeObject(ATTENDLIST, Formatting.Indented);

            return (Str);


           
        }

        public void transform2(string StartTime, string EndTime)
        {
            //DateTime Date = DateTime.Parse(UserDate);
            DateTime StartDate = DateTime.Parse(StartTime);
            DateTime EndDate = DateTime.Parse(EndTime).AddDays(1).AddSeconds(-1);
            //DateTime Date = Convert.ToDateTime("2020-08-01");

            //DateTime TheMonthStart = new DateTime(Date.Year, Date.Month, 1);//本月初1號
            //DateTime TheMonthEnd = new DateTime(Date.Year, Date.Month, DateTime.DaysInMonth(Date.Year, Date.Month));//本月初月底

            // 設定查詢月份
          //DateTime StartDate = Convert.ToDateTime("2020-08-10");
            //DateTime StartDate = TheMonthStart;
          //DateTime EndDate = Convert.ToDateTime("2020-09-08");
           // DateTime EndDate = TheMonthEnd.AddDays(1).AddSeconds(-1);
            var history = _db.MXIC_SwipeInfos.Where(x => x.WORK_DATETIME >= StartDate && x.WORK_DATETIME < EndDate).ToList();
            if (history.Any())
            {
                foreach(var item in history)
                {
                    _db.MXIC_SwipeInfos.Remove(item);
                }

                _db.SaveChanges();
            }
            
            // 班表
            var UserSchedule = _db.MXIC_ScheduleSettings.Where(x => x.Date >= StartDate && x.Date < EndDate);
         // var UserSchedule = _db.MXIC_ScheduleSettings;

            foreach (var item in UserSchedule)
            {
                string InAttendType = "正常";
                string OutAttendType = "正常";

                //某人某天的打卡紀錄
                var ATTENDLIST = _dbMXIC.FAC_ATTENDLISTs.Where(x => x.WORKER_NAME == item.EmpName && x.WORK_DATETIME == item.Date).FirstOrDefault();

                if (ATTENDLIST == null && item.WorkShift == "休")
                {

                }
                else
                {
                    DateTime ENTRANCE;

                    DateTime EXIT;

                    //如果某人某天無打卡紀錄又不是休假的話出席狀態異常
                    if (ATTENDLIST == null && item.WorkShift != "休")
                    {
                        ENTRANCE = item.Date;

                        EXIT = item.Date;

                        if (item.WorkShift == "請假")
                        {
                            InAttendType = "請假";

                            OutAttendType = "請假";
                            var SwipeIN = new Models.SwipeInfo();
                            SwipeIN.CheckType = "CHECKIN";
                            SwipeIN.SwipeTime = ENTRANCE;
                            SwipeIN.EmpName = item.EmpName;
                            SwipeIN.EditID = Guid.NewGuid();
                            SwipeIN.SwipID = Guid.NewGuid();
                            SwipeIN.Hour = 8;
                            SwipeIN.AttendType = InAttendType;
                            SwipeIN.valid = "true";
                            SwipeIN.WORK_DATETIME = item.Date;
                            _db.MXIC_SwipeInfos.Add(SwipeIN);

                            var SwipeOUT = new Models.SwipeInfo();
                            SwipeOUT.CheckType = "CHECKOUT";
                            SwipeOUT.SwipeTime = EXIT;
                            SwipeOUT.EmpName = item.EmpName;
                            SwipeOUT.EditID = Guid.NewGuid();
                            SwipeOUT.SwipID = Guid.NewGuid();
                            SwipeOUT.Hour = 8;
                            SwipeOUT.AttendType = OutAttendType;
                            SwipeOUT.valid = "true";
                            SwipeOUT.WORK_DATETIME = item.Date;
                            _db.MXIC_SwipeInfos.Add(SwipeOUT);
                        }
                        else
                        {
                            InAttendType = "曠職";

                            OutAttendType = "曠職";
                            var SwipeIN = new Models.SwipeInfo();
                            SwipeIN.CheckType = "CHECKIN";
                            SwipeIN.SwipeTime = ENTRANCE;
                            SwipeIN.EmpName = item.EmpName;
                            SwipeIN.EditID = Guid.NewGuid();
                            SwipeIN.SwipID = Guid.NewGuid();
                            SwipeIN.Hour = 0;
                            SwipeIN.AttendType = InAttendType;
                            SwipeIN.valid = "true";
                            SwipeIN.WORK_DATETIME = item.Date;
                            _db.MXIC_SwipeInfos.Add(SwipeIN);

                            var SwipeOUT = new Models.SwipeInfo();
                            SwipeOUT.CheckType = "CHECKOUT";
                            SwipeOUT.SwipeTime = EXIT;
                            SwipeOUT.EmpName = item.EmpName;
                            SwipeOUT.EditID = Guid.NewGuid();
                            SwipeOUT.SwipID = Guid.NewGuid();
                            SwipeOUT.Hour = 0;
                            SwipeOUT.AttendType = OutAttendType;
                            SwipeOUT.valid = "true";
                            SwipeOUT.WORK_DATETIME = item.Date;
                            _db.MXIC_SwipeInfos.Add(SwipeOUT);
                        }
                    }
                    //有打卡紀錄
                    else
                    {
                       
                        //上班時間                      
                        //如果CHECKINtime不是空
                        if (!string.IsNullOrWhiteSpace(ATTENDLIST.ENTRANCE_DATETIME.ToString()))
                        {
                            DateTime CHECKINtime = Convert.ToDateTime(ATTENDLIST.ENTRANCE_DATETIME.Value.ToString("HH:mm"));
                            ENTRANCE = ATTENDLIST.ENTRANCE_DATETIME.Value;

                            switch (item.WorkShift)
                            {   //如果此人是早班
                                case "早":
                                    DateTime StartTimeDay = Convert.ToDateTime("06:30");
                                  
                                    DateTime LateTimeDay = Convert.ToDateTime("07:00");
                                   
                                    //上班時間比06:30早=加班 上班時間比07:00晚=遲到
                                   
                                        if (CHECKINtime < StartTimeDay || CHECKINtime > LateTimeDay)
                                        {
                                            InAttendType = "異常";
                                        }
                                                                      
                                    break;
                                //如果此人是常日班
                                case "日":
                                    DateTime StartTimeNormal = Convert.ToDateTime("08:00");
                                   
                                    DateTime LateTimeNormal = Convert.ToDateTime("08:30");
                                   
                                    //上班時間比08:00早=加班 上班時間比08:30晚=遲到
                                    if (CHECKINtime < StartTimeNormal || CHECKINtime > LateTimeNormal)
                                    {
                                        InAttendType = "異常";
                                    }                                    
                                    break;
                                //如果此人是夜班
                                case "夜":
                                    DateTime StartTimeNight = Convert.ToDateTime("18:30");
                                   
                                    DateTime LateTimeNight = Convert.ToDateTime("19:00");
                                   
                                    //上班時間比18:30早=加班 上班時間比19:00晚=遲到
                                    if (CHECKINtime < StartTimeNight || CHECKINtime > LateTimeNight)
                                    {
                                        InAttendType = "異常";
                                    }                                   
                                    break;
                                //如果此人是代早代晚或是班表休假但是有打卡紀錄
                                default:
                                    InAttendType = "異常";                                   
                                    break;
                            }
                            var SwipeIN = new Models.SwipeInfo();
                            SwipeIN.CheckType = "CHECKIN";
                            SwipeIN.SwipeTime = ENTRANCE;
                            SwipeIN.EmpName = item.EmpName;
                            SwipeIN.EditID = Guid.NewGuid();
                            SwipeIN.SwipID = Guid.NewGuid();
                            SwipeIN.Hour = 0;
                            SwipeIN.AttendType = InAttendType;
                            SwipeIN.valid = "true";
                            SwipeIN.WORK_DATETIME = item.Date;
                            _db.MXIC_SwipeInfos.Add(SwipeIN);


                        }
                        else
                        {
                            InAttendType = "異常";
                            ENTRANCE = item.Date;
                            var SwipeIN = new Models.SwipeInfo();
                            SwipeIN.CheckType = "CHECKIN";
                            SwipeIN.SwipeTime = ENTRANCE;
                            SwipeIN.EmpName = item.EmpName;
                            SwipeIN.EditID = Guid.NewGuid();
                            SwipeIN.SwipID = Guid.NewGuid();
                            SwipeIN.Hour = 0;
                            SwipeIN.AttendType = InAttendType;
                            SwipeIN.valid = "true";
                            SwipeIN.WORK_DATETIME = item.Date;
                            _db.MXIC_SwipeInfos.Add(SwipeIN);

                        }                        
                        //下班時間
                        if (!string.IsNullOrWhiteSpace(ATTENDLIST.EXIT_DATETIME.ToString()))
                        {
                            DateTime CHECKOUTtime = Convert.ToDateTime(ATTENDLIST.EXIT_DATETIME.Value.ToString("HH:mm"));
                            EXIT = ATTENDLIST.EXIT_DATETIME.Value;

                            switch (item.WorkShift)
                            {   //如果此人是早班
                                case "早":
                                    
                                    DateTime EndTimeDay = Convert.ToDateTime("19:30");
                                    
                                    DateTime EarlyTimeDay = Convert.ToDateTime("19:00");
                                    //上班時間比06:30早=加班 上班時間比07:00晚=遲到

                                    //下班時間比19:30晚=加班 下班時間比19:00早=早退
                                    if (CHECKOUTtime > EndTimeDay || CHECKOUTtime < EarlyTimeDay)
                                    {
                                        OutAttendType = "異常";
                                    }
                                    break;
                                //如果此人是常日班
                                case "日":
                                   
                                    DateTime EndTimeNormal = Convert.ToDateTime("18:00");
                                    
                                    DateTime EarlyTimeNormal = Convert.ToDateTime("17:30");

                                    //下班時間比18:00晚=加班 下班時間比17:30早=早退
                                    if (CHECKOUTtime > EndTimeNormal || CHECKOUTtime < EarlyTimeNormal)
                                    {
                                        OutAttendType = "異常";
                                    }
                                    break;
                                //如果此人是夜班
                                case "夜":
                                   
                                    DateTime EndTimeNight = Convert.ToDateTime("07:30");
                                   
                                    DateTime EarlyTimeNight = Convert.ToDateTime("07:00");
                                    //上班時間比18:30早=加班 上班時間比19:00晚=遲到

                                    //下班時間比07:30晚=加班 下班時間比07:00早=早退
                                    if (CHECKOUTtime > EndTimeNight || CHECKOUTtime < EarlyTimeNight)
                                    {
                                        OutAttendType = "異常";
                                    }
                                    break;
                                //如果此人是代早代晚或是班表休假但是有打卡紀錄
                                default:
                      
                                    OutAttendType = "異常";
                                    break;
                            }
                            var SwipeOUT = new Models.SwipeInfo();
                            SwipeOUT.CheckType = "CHECKOUT";
                            SwipeOUT.SwipeTime = EXIT;
                            SwipeOUT.EmpName = item.EmpName;
                            SwipeOUT.EditID = Guid.NewGuid();
                            SwipeOUT.SwipID = Guid.NewGuid();
                            SwipeOUT.Hour = 0;
                            SwipeOUT.AttendType = OutAttendType;
                            SwipeOUT.valid = "true";
                            SwipeOUT.WORK_DATETIME = item.Date;
                            _db.MXIC_SwipeInfos.Add(SwipeOUT);

                        }
                        else
                        {
                            EXIT = item.Date;
                            OutAttendType = "異常";
                            var SwipeOUT = new Models.SwipeInfo();
                            SwipeOUT.CheckType = "CHECKOUT";
                            SwipeOUT.SwipeTime = EXIT;
                            SwipeOUT.EmpName = item.EmpName;
                            SwipeOUT.EditID = Guid.NewGuid();
                            SwipeOUT.SwipID = Guid.NewGuid();
                            SwipeOUT.Hour = 0;
                            SwipeOUT.AttendType = OutAttendType;
                            SwipeOUT.valid = "true";
                            SwipeOUT.WORK_DATETIME = item.Date;
                            _db.MXIC_SwipeInfos.Add(SwipeOUT);

                        }
                      
                    }
                  

                }
            }
            _db.SaveChanges();
        }
    }
}