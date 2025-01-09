using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Controller]
[Route("api/[controller]")]
public class UsersController(DataContext context) : ControllerBase
{
    [HttpGet]//app/users
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
        return await context.Users.ToListAsync();
    }
    [HttpGet("{id:int}")]//app/users/id
    public async Task<ActionResult<AppUser>> GetUser(int id){
        var user=await context.Users.FindAsync(id);
        if(user==null)
        return  NotFound();
        return user;
    } 

}
