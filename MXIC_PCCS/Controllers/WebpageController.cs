using MXIC_PCCS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MXIC_PCCS.Controllers
{
    public class WebpageController : Controller
    {
        public MXIC_PCCSContext _db = new MXIC_PCCSContext();
        // GET: Webpage
        public ActionResult Index()
        {
            return View();
        }

        public string PageGenerate(string tablename,string COLUMN_NAME)
        {
            
               var list = _db.MXIC_InputGenerates.Where(x => x.TableName == tablename).OrderBy(x=>x.Sequence);

            if (!string.IsNullOrWhiteSpace(COLUMN_NAME))
            {
                 list = list.Where(x => x.COLUMN_NAME == COLUMN_NAME).OrderBy(x => x.Sequence); 

            }
            string Str = JsonConvert.SerializeObject(list, Formatting.Indented);

            return (Str);

        }

        public string Nav()
        {

            var list = _db.NavDatas.Select(x=>new { x.Name,x.Url,x.commonly_used}); 

            string Str = JsonConvert.SerializeObject(list, Formatting.Indented);

            return (Str);

        }
    }
}