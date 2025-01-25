using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTO;

public class RegisterDto
{
    [Required(AllowEmptyStrings = false)]
    public required string Username{get;set;}=string.Empty;

    [Required(AllowEmptyStrings = false)]
    public required string KnownAs{get;set;}=string.Empty;

    [Required(AllowEmptyStrings = false)]
    public required string City{get;set;}=string.Empty;


    [Required(AllowEmptyStrings = false)]
    public required string Gender{get;set;}=string.Empty;


    [Required(AllowEmptyStrings = false)]
    public required string Country{get;set;}=string.Empty;


    [Required(AllowEmptyStrings = false)]
    public required string DateOfBirth{get;set;}=string.Empty;

    [Required(AllowEmptyStrings = false)]
    [StringLength(8, MinimumLength =4)]
    public required string Password{get;set;}=string.Empty;



}
