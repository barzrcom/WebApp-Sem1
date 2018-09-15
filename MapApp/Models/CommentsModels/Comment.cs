using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapApp.Models.CommentsModels
{
    public class Comment
    {
        public int ID { get; set; }

        public string Header { get; set; }

        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime EditTime { get; set; }

        // The user who created the comment
        public string User { get; set; }

        // The location the comment belongs to
        public int Location { get; set; }
    }
}
