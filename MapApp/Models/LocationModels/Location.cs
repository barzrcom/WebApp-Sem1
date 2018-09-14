using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

public enum LocationType : byte
{ Type1, Type2 }

public enum Seasons : byte
{ Winter, Spring, Summer, Autumn }

namespace MapApp.Models.LocationModels
{

	public class Location
	{
		public int ID { get; set; }

		public string User { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public LocationType Type { get; set; }

		public float Latitude { get; set; }

		public float Longitude { get; set; }

		public string District { get; set; }

		public Seasons Seasons { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:MM}")]
		[DisplayName("Opening Time")]
		public DateTime OpenTime { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:MM}")]
		[DisplayName("Closing Time")]
		public DateTime CloseTime { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
		[DisplayName("Visit Date")]
		public DateTime LastVisit { get; set; }

		public float Duration { get; set; }  // Duration in Hours
	}
}
