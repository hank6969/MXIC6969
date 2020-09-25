using MXIC_PCCS.DataUnity.BusinessUnity;
using MXIC_PCCS.DataUnity.Interface;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MXIC_PCCS.Controllers
{
    public class ExportPOController : Controller
    {
        IExportPO _ExportPO = new ExportPO();

        // GET: Test
        public ActionResult Index()
        {
            var id = HttpContext.User.Identity.Name;
            ViewBag.ID = id;
            return View();
        }

        public ActionResult DownloadQuotation(string PONumber, string Month)
        {
            string responseStr; 
             StringBuilder SB = new StringBuilder();
            try
            {
                System.IO.File.Copy(Properties.Resources.ExampleDirectory, Properties.Resources.DowloadDirectory, true);
                responseStr = ExportPO(PONumber,Month);
                if (responseStr == "寫入成功")
                {
                    string filepath = Server.MapPath("~/Content/計價單.xlsx");
                    string filename = Path.GetFileName(filepath);
                    Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/xlsx", filename);
                }
                else
                {
                    SB.AppendFormat("<script>alert('{0}');window.location.href='../ExportPO/Index';</script>", responseStr);
                }
            }
            catch (Exception ex)
            {
                SB.Clear();
                responseStr = "匯出失敗";
                SB.AppendFormat(ex.ToString());
            }
            return Content(SB.ToString());
        }

        public string ExportPO(string PONumber, string Month)
        {
            var responseStr = _ExportPO.CalcuationPO(PONumber, Convert.ToDateTime(Month));
            return responseStr;
        }
    }
}