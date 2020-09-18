using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MXIC_PCCS.DataUnity.Interface;
using MXIC_PCCS.DataUnity.BusinessUnity;

namespace MXIC_PCCS.Controllers
{
    public class ScheduleSettingController : Controller
    {

        IScheduleSetting _IScheduleSetting = new ScheduleSetting();
        // GET: ScheduleSetting
        public ActionResult Index()
        {
            return View();
        }

        public string ScheduleList(DateTime? Date, string PoNo)
        {
            string str = _IScheduleSetting.ScheduleList(Date, PoNo);

            return str;
        }

        [HttpPost]
        public string UploadSchedule(HttpPostedFileBase file)
        {
            string ff = _IScheduleSetting.ImportSchedul(file);

            return ff;
        }
    }
}