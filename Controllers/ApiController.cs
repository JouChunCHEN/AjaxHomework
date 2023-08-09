using AjaxDemo.Models;
using AjaxDemo.NWModels;
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
        public IActionResult Fetch()
        {
            return Content("Hello Fetch.");
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

        public IActionResult CheckAccount(string name)
        {
            if(_context.Members.Where(x=>x.Name==name).Count() >0)
            {
                return Content("此會員名稱已存在");
            }
            else
            {
                return Content("");
            }
        }

        public IActionResult autoComplete(string? keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return Json(null);
            }

            NWContext db = new NWContext();
            var products = db.Products.Where(x=>x.ProductName.ToUpper().StartsWith(keyword.ToUpper())).Select(x=>x.ProductName);
                return Json(products);
         }

        public IActionResult getImageById(int id=1)
        {
            Members? member = _context.Members.Find(id);
            byte[]? img = member.FileData;

            return File(img, "image/jpeg");
        }

        //回傳城市的JSON資料
        public IActionResult City()
        {
            var cities = _context.Address.Select(x => x.City).Distinct();
            return Json(cities);
        }

        //根據城市名稱，回傳城市的鄉鎮區JSON資料
        public IActionResult District(string city)
        {
            var districts = _context.Address.Where(x=>x.City == city).Select(x => x.SiteId).Distinct();
            return Json(districts);
        }

        //根據鄉鎮區名稱，回傳鄉鎮區的路名JSON資料
        public IActionResult Road(string siteId)
        {
            var roads = _context.Address.Where(x => x.SiteId == siteId).Select(x => x.Road).Distinct();
            return Json(roads);
        }
    }
}
