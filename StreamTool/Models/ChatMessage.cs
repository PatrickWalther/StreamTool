using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StreamTool.Models
{
    public class ChatMessage
    {
        [Key]
        public int MsgId { get; set; }
        [Required]
        [Display(Name = "User")]
        public string Sender { get; set; }
        [Required]
        public string Message { get; set; }

        public override string ToString()
        {
            return Sender + ": " + Message;
        }
    }
}
