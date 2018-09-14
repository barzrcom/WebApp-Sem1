$(document).ready(function () {
	// Note: This example requires that you consent to location sharing when
	// prompted by your browser. If you see the error "The Geolocation service
	// failed.", it means you probably did not give permission for the browser to
	// locate you.
	$('#map').css({
		'height': '400px', /* The height is 400 pixels */
		'width': '50%' /* The width is the width of the web page */
	});


	var markers = [];



	// Try HTML5 geolocation.
	if (navigator.geolocation) {
		navigator.geolocation.getCurrentPosition(function (position) {
			var pos = {
				lat: position.coords.latitude,
				lng: position.coords.longitude
			};

			map.setCenter(pos);
		}, function () {
			handleLocationError(true, infoWindow, map.getCenter());
		});
	} else {
		// Browser doesn't support Geolocation
		handleLocationError(false, infoWindow, map.getCenter());
	}

	// This event listener will call addMarker() when the map is clicked.
	map.addListener('click', function (event) {
		addMarker(event.latLng);
	});

	// Adds a marker to the map and push to the array.
	function addMarker(location) {
		console.log('You clicked on: ' + event.latLng);
		var marker = new google.maps.Marker({
			position: location,
			map: map
		});
		markers.push(marker);
	}

	// Adds a marker to the map and push to the array.
	function addMarker(location) {
		var marker = new google.maps.Marker({
			position: location,
			map: map
		});
		deleteMarkers();
		markers.push(marker);
		$("#Latitude").val(location.lat);
		$("#Longitude").val(location.lng);


		showMarkers();
	}

	// Sets the map on all markers in the array.
	function setMapOnAll(map) {
		for (var i = 0; i < markers.length; i++) {
			markers[i].setMap(map);
		}
	}

	// Removes the markers from the map, but keeps them in the array.
	function clearMarkers() {
		setMapOnAll(null);
	}

	// Shows any markers currently in the array.
	function showMarkers() {
		setMapOnAll(map);
	}

	// Deletes all markers in the array by removing references to them.
	function deleteMarkers() {
		clearMarkers();
		markers = [];
	}

	function handleLocationError(browserHasGeolocation, infoWindow, pos) {
		infoWindow.setPosition(pos);
		infoWindow.setContent(browserHasGeolocation ?
			'Error: The Geolocation service failed.' :
			'Error: Your browser doesn\'t support geolocation.');
		infoWindow.open(map);
	}

})