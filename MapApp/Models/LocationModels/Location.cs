﻿using System.ComponentModel.DataAnnotations;

public enum LocationCategory : byte
{ ViewPoint, Restaurant, Museum }


namespace MapApp.Models.LocationModels
{

	public class Location
	{
		public int ID { get; set; }

		public string User { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public LocationCategory Category { get; set; }

		public float Latitude { get; set; }

		public float Longitude { get; set; }

		public byte[] Image { get; set; }

        [Range(0, 5)]
        public float Rating { get; set; }
    }
}
