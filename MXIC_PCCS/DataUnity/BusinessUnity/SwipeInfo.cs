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
        public MXIC_PCCSContext _db = new MXIC_PCCSContext();

        public MxicTestContext _dbMXIC = new MxicTestContext();

        //關閉資料庫
        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }
        public string CheckinList(string VendorName, string EmpID, string EmpName, DateTime? StartTime, DateTime? EndTime,string AttendTypeSelect)
        {
            var _List = _db.MXIC_View_Swipes.Select(x=>new { x.PoNo ,x.VendorName ,x.EmpID ,x.EmpName ,x.CheckType,x.SwipeTime ,x.EditID ,x.AttendType}).OrderBy(x => new { x.PoNo, x.EmpID, x.SwipeTime});

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
            {

                _List = _List.Where(x => x.SwipeTime >= StartTime && x.SwipeTime <= EndTime).OrderBy(x => new { x.PoNo, x.EmpID, x.SwipeTime });

            }

            if (!string.IsNullOrWhiteSpace(AttendTypeSelect))
            {
                _List = _List.Where(x => x.AttendType== AttendTypeSelect).OrderBy(x => new { x.PoNo, x.EmpID, x.SwipeTime });

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
                
            try {
                var EditSwipeInfo = _db.MXIC_SwipeInfos.Where(x => x.EditID.ToString() == EditID).FirstOrDefault();
                double time;
                if (double.TryParse(Hour, out time)) {
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

            catch(Exception e)
            {

                str= e.ToString();
            }
            
            
            
         


            return (str);
        }

        public string transform() {

            var ATTENDLIST = _dbMXIC.FAC_ATTENDLISTs;

           

            string AttendType = "正常";

            foreach (var item in ATTENDLIST)
            { 
                MXIC_ScheduleSetting UserSchedule = _db.MXIC_ScheduleSettings.Where(x => x.Date == item.WORK_DATETIME&&x.EmpName==item.WORKER_NAME).FirstOrDefault();

              

                if (UserSchedule != null) {

                    string WorkShift = UserSchedule.WorkShift.ToString();
                    DateTime CHECKINtime = Convert.ToDateTime(item.ENTRANCE_DATETIME.Value.ToString("HH:mm"));
                    DateTime CHECKOUTtime = Convert.ToDateTime(item.EXIT_DATETIME.Value.ToString("HH:mm"));
                    if (CHECKINtime == null || CHECKOUTtime == null)
                    {
                        AttendType = "異常";
                    }
                    else
                    {
                        switch (WorkShift)
                        {
                            case "日":
                                DateTime StartTimeDay = Convert.ToDateTime("06:30");
                                DateTime EndTimeDay = Convert.ToDateTime("19:30");
                                DateTime LateTimeDay = Convert.ToDateTime("07:00");
                                DateTime EarlyTimeDay = Convert.ToDateTime("19:00");
                                if (CHECKINtime < StartTimeDay || CHECKINtime > LateTimeDay || CHECKOUTtime > EndTimeDay || CHECKOUTtime < EarlyTimeDay)
                                {

                                    AttendType = "異常";
                                }

                                break;

                            case "常日":
                                DateTime StartTimeNormal = Convert.ToDateTime("08:00");
                                DateTime EndTimeNormal = Convert.ToDateTime("18:00");
                                DateTime LateTimeNormal = Convert.ToDateTime("08:30");
                                DateTime EarlyTimeNormal = Convert.ToDateTime("17:30");
                                if (CHECKINtime < StartTimeNormal || CHECKINtime > LateTimeNormal || CHECKOUTtime > EndTimeNormal || CHECKOUTtime < EarlyTimeNormal)
                                {

                                    AttendType = "異常";
                                }
                                break;

                            case "夜":
                                DateTime StartTimeNight = Convert.ToDateTime("18:30");
                                DateTime EndTimeNight = Convert.ToDateTime("07:30");
                                DateTime LateTimeNight = Convert.ToDateTime("19;00");
                                DateTime EarlyTimeNight = Convert.ToDateTime("07:00");
                                if (CHECKINtime < StartTimeNight || CHECKINtime > LateTimeNight || CHECKOUTtime > EndTimeNight || CHECKOUTtime < EarlyTimeNight)
                                {

                                    AttendType = "異常";
                                }
                                break;

                            case "休":

                                AttendType = "異常";

                                break;
                        }
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        var Swipe = new MXIC_SwipeInfo();
                        if (i == 0)
                    {   
                            Swipe.CheckType = "CHECKIN";
                            Swipe.SwipeTime = item.ENTRANCE_DATETIME;
                            Swipe.EmpName = item.WORKER_NAME;
                            Swipe.EditID = Guid.NewGuid();
                            Swipe.SwipID = Guid.NewGuid();
                            Swipe.Hour = 0;
                            Swipe.AttendType = AttendType;
                            Swipe.valid = "true";
                        }
                    else
                    {
                            Swipe.CheckType = "CHECKOUT";
                            Swipe.SwipeTime = item.EXIT_DATETIME;
                            Swipe.EmpName = item.WORKER_NAME;
                            Swipe.EditID = Guid.NewGuid();
                            Swipe.SwipID = Guid.NewGuid();
                            Swipe.Hour = 0;
                            Swipe.AttendType = AttendType;
                            Swipe.valid = "true";
                        }
                        _db.MXIC_SwipeInfos.Add(Swipe);
                        _db.SaveChanges();

                    }
            }
            }

           
         
         


            string str = "修改失敗";


            return (str); }

    }
}