using System;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository) : BaseController
{
    
    [HttpGet]//app/users
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers(){
        var users=await userRepository.GetMembersAsync();
        return Ok(users);
    }
    
    [HttpGet("{username}")]//app/users/id
    public async Task<ActionResult<MemberDto>> GetUser(string username){
        var user=await userRepository.GetMemberAsync(username);
        if(user==null)
        return  NotFound();
        return user;
    } 

}
