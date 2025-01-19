using System;
using System.Security.Claims;
using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService) : BaseController
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
    [HttpPut]
    public async Task<ActionResult> UpdateUser([FromBody]MemberUpdateDto memberUpdateDto){
        var user=await userRepository.GetUserByUsernameAsync(User.GetUserName());
        if(user==null){
            return BadRequest("Could not find user");
            
        }
        mapper.Map(memberUpdateDto,user);
        if(await userRepository.SaveAllAsync())return NoContent();
        return BadRequest("Failed to update user");
    } 
    [HttpPost("add-photo")]  
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file) {
        var user=await userRepository.GetUserByUsernameAsync(User.GetUserName());
        if(user==null) return BadRequest("Cannot update user");
        var result=await photoService.AddPhotoAsync(file);
        if(result.Error!=null)
        return BadRequest(result.Error.Message);
        var photo=new Photo{
            Url=result.SecureUrl.AbsoluteUri,
            PublicId=result.PublicId
        };
        user.Photos.Add(photo);
        object obj=new {username=user.UserName};
        if(await userRepository.SaveAllAsync())        
        return CreatedAtAction(nameof(GetUser), new{username=user.UserName},
         mapper.Map<PhotoDto>(photo));
        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId){
        var user=await userRepository.GetUserByUsernameAsync(User.GetUserName());
        if(user==null)return BadRequest("Could not find user");
        var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
        if(photo==null||photo.IsMain)return BadRequest("Cannot use this as main photo");
        var currentMain=user.Photos.FirstOrDefault(x=>x.IsMain);
        if(currentMain!=null)currentMain.IsMain=false;
        photo.IsMain=true;
        if(await userRepository.SaveAllAsync())return NoContent();
        return BadRequest("Problem setting main photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId){
        var user=await userRepository.GetUserByUsernameAsync(User.GetUserName());
        if(user==null)
            return BadRequest("User not found");
        var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
        if(photo==null||photo.IsMain)
            return BadRequest("this pic can't be deleted");
        if(photo.PublicId!=null){
            var result=await photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error!=null){
                return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if(await userRepository.SaveAllAsync())return Ok();
            return BadRequest("problem deleting photo");
        }
        return BadRequest("problem deleting photo");
    }


}
