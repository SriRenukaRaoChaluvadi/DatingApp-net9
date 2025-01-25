using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext dataContext, 
ITokenService tokenService, IMapper mapper):BaseController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register( [FromBody]RegisterDto dto){
        if(await UserExists(dto.Username)){
            return BadRequest("Username taken");
        }
        
        using var hmac=new HMACSHA512();//we're using "using" keyword here so that obj will be destroyed as soon as scope ends
        //without depending on gc
        // var user=new AppUser{
        //     UserName=dto.Username.ToLower(),
        //     PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
        //     PasswordSalt=hmac.Key
        // };
        var user=mapper.Map<AppUser>(dto);
        user.UserName=user.UserName.ToLower();
        user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
        user.PasswordSalt=hmac.Key;
        dataContext.Users.Add(user);
        await dataContext.SaveChangesAsync();
        
        return new UserDto{
            Username=user.UserName,
            Token=tokenService.CreateToken(user),
            KnownAs=user.KnownAs
        };
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> loginUser([FromBody]LoginDto loginDto){
        var user=await dataContext.Users
        .Include(p=>p.Photos)
        .FirstOrDefaultAsync(x=>x.UserName==loginDto.Username.ToLower());
        if(user==null)
        return Unauthorized("Invalid Username");
        using var hmac=new HMACSHA512(user.PasswordSalt);
        var computedHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for(int i=0;i<computedHash.Length;i++){
            if(computedHash[i]!=user.PasswordHash[i])return Unauthorized("Invalid password");
        }
        return new UserDto{
            Username=user.UserName,
            Token=tokenService.CreateToken(user),
            PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
            KnownAs=user.KnownAs
        };

    }
    private async Task<bool> UserExists(string username){
            return await dataContext.Users.AnyAsync(x=>x.UserName.ToLower()==username.ToLower());
    }
}
