using System;
using System.Collections.Generic;
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

		public string Name { get; set; }

		public string Description { get; set; }

		public LocationType Type { get; set; }

		public long Latitude { get; set; }

		public long Longitude { get; set; }

		public string District { get; set; }

		public Seasons Seasons { get; set; }

		public DateTime OpenTime { get; set; }

		public DateTime CloseTime { get; set; }

		public DateTime LastVisit { get; set; }

		public float Duration { get; set; }  // Duration in Hours
	}
}
