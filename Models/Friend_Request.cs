//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocialNet.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Friend_Request
    {
        public System.Guid RequestID { get; set; }
        public Nullable<bool> Status { get; set; }
        public string FromUserLogin { get; set; }
        public string ToUserLogin { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
