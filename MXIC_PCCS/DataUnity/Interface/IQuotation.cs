using MXIC_PCCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MXIC_PCCS.DataUnity.Interface
{
    interface IQuotation
    {
        /// <summary>
        /// 報價單查詢
        /// </summary>
        /// <param name="VendorName"></param>
        /// <param name="PoNo"></param>
        /// <param name="PoClassID"></param>
        /// <returns></returns>
        string SearchQuotation(string VendorName, string PoNo, string PoClassID);
        /// <summary>
        /// 匯入報價單
        /// </summary>
        /// <param name="VendorName"></param>
        /// <param name="PoNo"></param>
        /// <param name="Property_ListModel"></param>
        /// <returns></returns>
        string ImportQuotation(string VendorName, string PoNo, List<MXIC_QuotationProperty> Property_ListModel);
        /// <summary>
        /// 匯入之前先清空資料
        /// </summary>
        /// <returns></returns>
        string ClearTable();
    }
}
