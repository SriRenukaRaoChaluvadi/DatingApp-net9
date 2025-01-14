using System;
using System.Diagnostics.CodeAnalysis;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController(DataContext dataContext):BaseController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth(){
        return "Secret Text";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound(){
        var t=dataContext.Users.Find(-1);

        return t==null?NotFound():t;
    }
    [HttpGet("server-error")]
    public ActionResult<AppUser> GetServerError(){
        try{
        var t=dataContext.Users.Find(-1)??throw new Exception("A bad thing happened");       
        return t;
        }
        catch(Exception ex){
            return StatusCode(500, "Computer says no!");
        }        
    }
    
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest(){
        return BadRequest("this is not a good request");
    }


        
    


}
