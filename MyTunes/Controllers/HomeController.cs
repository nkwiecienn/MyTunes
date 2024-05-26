using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyTunes.Data;
using MyTunes.Models;
using SQLitePCL;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace MyTunes.Controllers;

public class HomeController : Controller
{
    //private readonly ILogger<HomeController> _logger;
    private readonly MyTunesContext _context;

    // public HomeController(ILogger<HomeController> logger)
    // {
    //     _logger = logger;
    // }

    public HomeController(MyTunesContext context)
{
    _context = context ?? throw new ArgumentNullException(nameof(context));
    
    try
    {
        var canConnect = _context.Database.CanConnect();
        if (!canConnect)
        {
            throw new InvalidOperationException("Unable to connect to the database.");
        }
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException("An error occurred while connecting to the database.", ex);
    }
}


    [Route("/")]
    public IActionResult Index()
    {
        ViewData["Username"] = HttpContext.Session.GetString("Username");

        return View();
    }

    [Route("/profile")]
    public IActionResult Profile()
    {
        ViewData["Username"] = HttpContext.Session.GetString("Username");
        return View();
    }

    [Route("/privacy")]
    public IActionResult Privacy()
    {
        ViewData["Username"] = HttpContext.Session.GetString("Username");
        return View();
    }

    [Route("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [Route("/login")]
    public IActionResult Login()
    {
        ViewData["Username"] = HttpContext.Session.GetString("Username");

        return View();
    }

    [HttpPost]
    [Route("/login")]
    public IActionResult Login(IFormCollection form)
    {
        String username = form["username"].ToString();
        String password = form["password"].ToString();
        var user = _context.User.Where(u => u.Username == username && u.Password == MD5Hash(password));

        if(user.Count() < 1) {
            ViewData["Message"] = "Invalid username or password";
        } else {
            ViewData["Username"] = username;
            HttpContext.Session.SetString("Username", username);
            int userID = user.First().UserId;
            HttpContext.Session.SetInt32("UserID", userID);
            return RedirectToAction("Profile");
        }

        return View();
    }

    [Route("/register")]
    public IActionResult Register()
    {
        ViewData["Username"] = HttpContext.Session.GetString("Username");
        return View();
    }

    [HttpPost]
    [Route("/register")]
    public async Task<IActionResult> Register([Bind("UserId,Username,Password")] User user, IFormCollection form)
    {
        if (form is null)
            return View();

        String username = form["username"].ToString();
        String password = form["password1"].ToString();

        if (password != form["password2"].ToString())
        {
            ViewData["Message"] = "Passwords do not match";
            return View();
        }

        if (_context.User.Any(u => u.Username == username))
        {
            ViewData["Message"] = "Username already exists";
            return View();
        }

        user.Username = username;
        user.Password = MD5Hash(password);

        _context.Add(user);
        await _context.SaveChangesAsync();

        ViewData["Message"] = "Registration successful";
        return View();
    }

    private string MD5Hash(string input)
    {
        using (var md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
