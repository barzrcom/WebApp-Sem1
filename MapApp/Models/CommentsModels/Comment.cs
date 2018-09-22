using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MapApp.Models.CommentsModels
{
    public class Comment
    {
        public int ID { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Header { get; set; }

        [StringLength(500)]
        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime EditTime { get; set; }

        // The user who created the comment
        public string User { get; set; }

        // The location the comment belongs to
        public int Location { get; set; }
    }
}
