using MXIC_PCCS.DataUnity.BusinessUnity;
using MXIC_PCCS.DataUnity.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
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
      
        public ActionResult transform()
        {
            _ISwipeInfo.transform2();

            return RedirectToAction("Index", "SwipeInfo");
        }
    }
}