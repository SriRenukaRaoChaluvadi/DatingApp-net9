using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class UsersController(DataContext context) : BaseController
{
    [AllowAnonymous]
    [HttpGet]//app/users
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
        return await context.Users.ToListAsync();
    }
    [Authorize]
    [HttpGet("{id:int}")]//app/users/id
    public async Task<ActionResult<AppUser>> GetUser(int id){
        var user=await context.Users.FindAsync(id);
        if(user==null)
        return  NotFound();
        return user;
    } 

}
