using MXIC_PCCS.DataUnity.BusinessUnity;
using MXIC_PCCS.DataUnity.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MXIC_PCCS.Controllers
{
    public class UserManagementController : Controller
    {
        IUserManagement _IUserManagement = new UserManagement();
        // GET: UserManagement
        public ActionResult Index()
        {   
            return View();
        }
        public string UserList(string DepNo, string DepName, string UserID, string UserName)
        {               //呼叫IUserManagement介面中的 UserList方法
            string str = _IUserManagement.UserList( DepNo , DepName,  UserID , UserName);
                

            return str ;
        }
        //人員管理刪除使用者
        public string DeleteUser(string DeleteID)
        {               //呼叫IUserManagement介面中的 DeleteUser
            string str = _IUserManagement.DeleteUser(DeleteID);

            return str;

        }
        //新增使用者
        public string AddUser(string DepNo, string DepName, string UserID, string UserName, string Admin, string PassWord)
        {               //呼叫IUserManagement介面中的 DeleteUser
            string str = _IUserManagement.AddUser( DepNo,  DepName,  UserID,  UserName,  Admin,  PassWord);

            return str;

        }
        public string EditUserDetail(string EditID)
        {
            string str = _IUserManagement.EditUserDetail(EditID);

            return str;

        }

        public string EditUser(string EditID, string DepNo, string DepName, string UserID, string UserName, string Admin, string PassWord)
        {
            string str = _IUserManagement.EditUser( EditID,  DepNo,  DepName,  UserID,  UserName,  Admin,  PassWord);

            return str;


        }

    }
}