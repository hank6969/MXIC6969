using MXIC_PCCS.DataUnity.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MXIC_PCCS.DataUnity.BusinessUnity;

namespace MXIC_PCCS.Controllers
{
    public class VendorManagementController : Controller
    {
        IVendorManagement _IVendorManagement = new VendorManagement();

        // GET: VendorManagement
        public ActionResult Index()
        {
            return View();
        }

        public string VendorList(string PoNo, string VendorName, string EmpID, string EmpName)
        {
            var str = _IVendorManagement.VendorList(PoNo, VendorName, EmpID, EmpName);

            return str;
        }

        public string AddVendor(string PoNo, string VendorName, string EmpID, string EmpName)
        {
            string str = _IVendorManagement.AddVendor(PoNo, VendorName, EmpID, EmpName);

            return str;
        }

        public string DeleteVendor(string DeleteID)
        {
            string str = _IVendorManagement.DeleteVendor(DeleteID);

            return str;
        }

        public string EditVendor(string EditID, string PoNo, string VendorName, string EmpID, string EmpName)
        {
            string str = _IVendorManagement.EditVendor(EditID, PoNo, VendorName, EmpID, EmpName);

            return str;
        }

        public string EditVendorDetail(string EditID)
        {
            string str = _IVendorManagement.EditVendorDetail(EditID);

            return (str);
        }
    }
}