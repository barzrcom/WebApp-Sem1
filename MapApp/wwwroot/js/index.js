
$(document).ready(function () {
    // map is a global google map object

    refreshLocations();

    $("#refresh_locations").click(function () {
        refreshLocations();
    });

    $("#clear_locations").click(function () {
        clearLocations();
    });

	$('.class--map').css({
		'position': 'absolute',
		'top': '50px',
		'left': '0',
		'bottom': '0px',
		'right': '0'
	});

	$('.container--map').css({
		'padding': '5px',
		'margin': '5px'
	});
})


