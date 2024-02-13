using System.Diagnostics;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using StarfishTestWebApp2.Models;

namespace StarfishTestWebApp2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _conf;

    public HomeController(ILogger<HomeController> logger, IConfiguration conf)
    {
        _logger = logger;
        _conf = conf;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ListBlobs()
    {
        BlobContainerClient blobStorage = new BlobContainerClient(_conf.GetValue<string>("ConnectionStrings:BlobStorage"),
            "testblob");
        var list = blobStorage.GetBlobs();
        
        return View(list);
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    
    [HttpPost]
    public async Task<IActionResult> UploadImage(UploadImage model, IFormFile ImageFile)
    {
        if (ModelState.IsValid)
        {
            BlobContainerClient blobStorage = new BlobContainerClient(_conf.GetValue<string>("ConnectionStrings:BlobStorage"),
                "testblob");

            
            var filename = ContentDispositionHeaderValue.Parse(ImageFile.ContentDisposition).FileName.Trim('"');

            var response = blobStorage.UploadBlob(filename, ImageFile.OpenReadStream());

            Console.WriteLine(response.Value.ToString());
        }        

        return RedirectToAction("Index","Home");   
    }
    
}