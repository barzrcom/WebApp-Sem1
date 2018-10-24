using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MapApp.Models.ViewModels
{
	public class View
	{
		public int ID { get; set; }

		public string UserId { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }

		public DateTime Date { get; set; }
	}
}
