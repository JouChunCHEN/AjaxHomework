using AjaxDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AjaxDemo.Controllers
{
    public class ApiController : Controller
    {
        private readonly DemoContext _context;
        private readonly IWebHostEnvironment _environ;

        public ApiController(DemoContext context, IWebHostEnvironment environ)
        {
            _context = context;
            _environ = environ;
        }

        public IActionResult Index()
        {
            System.Threading.Thread.Sleep(3000); //強制停止3秒鐘，再往下執行>>測試讀取效果
            return Content("Hello Ajax.");
        }

        public IActionResult getDemo(ViewModels.CUserViewModel vm)
        {
            if(string.IsNullOrEmpty(vm.name))
            {
                vm.name = "Guest";
            }
            return Content($"Hello {vm.name}, your are {vm.age} years old.");
        }

        public IActionResult Register(Members member, IFormFile file)
        {
            //透過WebRootPath動態取得wwwroot的實際路徑     //ContentRootPath >>專案資料夾的實際路徑
            string filePath = Path.Combine(_environ.WebRootPath, "uploads", file.FileName);
            //上傳檔案到uploads資料夾中
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            //return Content($"已上傳檔案至{filePath}");

            //將圖片轉為二進位
            byte[] imgBytes = null;
            using (MemoryStream memeoryStream = new MemoryStream())
            {
                file.CopyTo(memeoryStream);
                imgBytes = memeoryStream.ToArray();
            }

            //將圖片資料存至DB
            member.FileName = file.FileName;
            member.FileData = imgBytes;
            _context.Members.Add(member);
            _context.SaveChanges();

            return Content("新增成功!");
        }
    }
}
