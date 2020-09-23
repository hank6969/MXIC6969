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
            return View();
        }

        public string CheckinList(string VendorName, string EmpID, string EmpName, DateTime? StartTime, DateTime? EndTime, string AttendTypeSelect)
        {               //使用ISwipeInfo UserList方法
            string str = _ISwipeInfo.CheckinList( VendorName,  EmpID,  EmpName, StartTime,  EndTime,  AttendTypeSelect);


            return str;
        }

        public string SwipeInfoDetail(string EditID)
        {
            string str = _ISwipeInfo.SwipeInfoDetail(EditID);

            return str;

        }
        public string EditSwipe(string EditID,string AttendTypeSelect,string Hour)
        {

            string str = _ISwipeInfo.EditSwipe(EditID, AttendTypeSelect, Hour);

            return str;
        }
      
        public void transform()
        {
            _ISwipeInfo.transform();
        }
    }
}