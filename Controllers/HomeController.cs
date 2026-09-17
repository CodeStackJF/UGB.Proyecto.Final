using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UGB.Proyecto.Final.Interfaces;

namespace UGB.Proyecto.Final.Controllers;

public class HomeController(IUsersRepository usersRepository) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Privacy()
    {        
        return Ok(await usersRepository.GetAll());
    }

}
