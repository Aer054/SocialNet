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
    public class CommunityVM
    {
        public System.Guid CommunityID { get; set; }
        [Required]
        [DisplayName("Название сообщества")]
        public string CommunityName { get; set; }
        [Required]
        [DisplayName("Это блог?")]
        public bool IsItBLog { get; set; }
       
    }
}