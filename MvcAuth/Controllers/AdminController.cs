using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MvcAuth.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();   //这里的View直接定位到Views下的Admin目录下的Index.cshtml文件。（如果Admin文件夹不存在，或者Admin目录的文件的文件名不是Index.cshtml,View()方法都不能成功定位）
        }
    }
}