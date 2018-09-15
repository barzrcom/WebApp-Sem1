$(document).ready(function () {
	// Note: This example requires that you consent to location sharing when
	// prompted by your browser. If you see the error "The Geolocation service
	// failed.", it means you probably did not give permission for the browser to
	// locate you.
	$('#map').css({
		'height': '400px', /* The height is 400 pixels */
		'width': '50%' /* The width is the width of the web page */
	});

	var latitude = parseFloat($("#Latitude").prop('innerText'));
	var longitude = parseFloat($("#Longitude").prop('innerText'));

	var pos = {
		lat: latitude,
		lng: longitude
	};
	addMarker(pos);


	// Adds a marker to the map and push to the array.
	function addMarker(location) {
		var marker = new google.maps.Marker({
			position: location,
			map: map
		});
		marker.setMap(map);
	}
})