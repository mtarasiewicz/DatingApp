using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace API.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "Username cannot be empty.")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Password cannot be empty.")]
    public string Password { get; set; }
}
