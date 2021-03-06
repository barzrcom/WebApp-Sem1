﻿// Initialize and add the map
var map = null;
var markerList = [];

function initMap() {
    // The location of Colman
    var colman = { lat: 31.968987, lng: 34.770724 };
    // The map, centered at Colman
	if (map == null) {
		map = new google.maps.Map(document.getElementById('map'), { zoom: 6, center: colman });
		var controlDiv = document.getElementById('floating-panel');
		map.controls[google.maps.ControlPosition.BOTTOM_CENTER].push(controlDiv);
	}
		// The marker, positioned at Colman
    //var marker = new google.maps.Marker({ position: colman, map: map });
}

function clearLocations() {
    $.each(markerList, function (index, data) {
        data.setMap(null);
    });
    markerList = [];
};


function refreshLocations() {
    $.get("/Locations/Data", function (data) {
        clearLocations();
		$.each(data, function (index, data) {
			// Extract Geo Position from data
			var pos = { lat: data.latitude, lng: data.longitude };
			// Add info Window Object based on the specific data
			var infowindow = new google.maps.InfoWindow({ content: FormatInfoWindowContent(data) });
			// Create a Google map marker and add it to the list
			var marker = new google.maps.Marker({ position: pos, map: map, title: data.name })
			markerList.push(marker);
			// Add click function to Marker
            marker.addListener('click', function () { infowindow.open(map, marker); });
            google.maps.event.addListener(infowindow, 'domready', function () {
                $('#barrating-' + data.id).barrating({
                    theme: 'fontawesome-stars-o',
                    readonly: true,
                    initialRating: data.rating,
                    allowEmpty: true,
                    emptyValue: 0
                });
            });
        });

    });
};


function FormatInfoWindowContent(data) {
	/*
	 * Function handle how the Info Windows content will look like
	 */
    console.log(data);
	var description = data.description;
	if (description == null) {
		description = 'Empty Description.';
	}
	if (data.image != null) {
		var image = '<img id="preview_image" src="data:image;base64,' +
			data.image + '" width="90px" height="90px" />';
	} else { var image = "";}
    var contentString = '<div id="content">' +
		'<div id="siteNotice">' +
		'</div>' +
        '<h2 id="firstHeading" class="firstHeading"><a href="Locations/Details/' + data.id +'">' + data.name +
		'</a></h2><div id="bodyContent">' +
        '<p>' + description + '</p>' +
        '<p> <select id="barrating-' + data.id +'" hidden> <option value="0"> 0 </option> <option value="1"> 1</option> <option value="2">2</option> <option value="3">3</option> <option value="4">4</option> <option value="5">5</option> </select >' + 
		'<p>' + image + '</p>' +
		'</div>' +
        '</div>';
	return contentString;
}
