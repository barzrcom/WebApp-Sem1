
$(document).ready(function () {
    // map is a global google map object

    refreshLocations();

    $("#refresh_locations").click(function () {
        refreshLocations();
    });

    $("#clear_locations").click(function () {
        clearLocations();
    });

	$('#map').css({
		'height': '400px', /* The height is 400 pixels */
		'width': '100%' /* The width is the width of the web page */
	});
})

