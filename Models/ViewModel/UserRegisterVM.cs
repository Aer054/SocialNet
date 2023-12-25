using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SocialNet.Models.ViewModel
{
    [Table("Community")]
    public class UserRegisterVM
    {
        [Required]
        [DisplayName("Логин")]
        public string Login { get; set; }
        [Required]
        [DisplayName("Пароль")]
        public string Password { get; set; }
        [Required]
        [DisplayName("Фамилия")]
        public string Last_Name { get; set; }
        [Required]
        [DisplayName("Имя")]
        public string First_Name { get; set; }
        [DisplayName("Отчество")]
        public string Patronomic { get; set; }
    }
}