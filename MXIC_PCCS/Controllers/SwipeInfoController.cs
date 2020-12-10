﻿using MXIC_PCCS.DataUnity.BusinessUnity;
using MXIC_PCCS.DataUnity.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MXIC_PCCS.Controllers
{
    [Authorize]
    public class SwipeInfoController : Controller
    {
        ISwipeInfo _ISwipeInfo = new SwipeInfo();

        // GET: SwipeInfo
        public ActionResult Index()
        {
            var id = HttpContext.User.Identity.Name;
            ViewBag.ID = id;
            return View();
        }

        public string CheckinList(string VendorName, string EmpID, string EmpName, DateTime? StartTime, DateTime? EndTime, string AttendTypeSelect)
        {              
            string str = _ISwipeInfo.CheckinList(VendorName, EmpID, EmpName, StartTime, EndTime, AttendTypeSelect);

            return str;
        }

        public string SwipeInfoDetail(string EditID)
        {
            string str = _ISwipeInfo.SwipeInfoDetail(EditID);

            return str;
        }
        public string EditSwipe(string EditID, string AttendTypeSelect, string Hour)
        {
            string str = _ISwipeInfo.EditSwipe(EditID, AttendTypeSelect, Hour);

            return str;
        }

        public string MxicTest()
        {
            string str = _ISwipeInfo.transform();

            return str;
        }

        public ActionResult transform(string StartTime,string EndTime)
        {

            string responseStr = "日期選擇不完整!";

            StringBuilder SB = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(StartTime) || !string.IsNullOrWhiteSpace(EndTime))
            {
                DateTime start = Convert.ToDateTime(StartTime);

                DateTime end = Convert.ToDateTime(EndTime);

                var result = DateTime.Compare(start, end);
                if (result != 1)
                { 
                _ISwipeInfo.transform2(StartTime, EndTime);

                return RedirectToAction("Index", "SwipeInfo");
                }
                else {

                    responseStr = "日期選擇異常!";
                    SB.Clear();
                SB.AppendFormat("<script>alert('{0}');window.location.href='../ScheduleSetting/Index';</script>", responseStr);
                return Content(SB.ToString());
                }
            }
            else
            {
                SB.Clear();
                SB.AppendFormat("<script>alert('{0}');window.location.href='../ScheduleSetting/Index';</script>", responseStr);

                return Content(SB.ToString());
            }

          


            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrWhiteSpace(StartTime) && !string.IsNullOrEmpty(EndTime) && !string.IsNullOrWhiteSpace(EndTime))
            {
                _ISwipeInfo.transform2(StartTime, EndTime);
                return RedirectToAction("Index", "SwipeInfo");
            }
            else
            {
                TempData["message"] = "請輸入開始和結束日期";
                return RedirectToAction("Index", "ScheduleSetting");
            }
>>>>>>> ad894dd58cab2135e4c170106305606a3e429102
        }
    }
}