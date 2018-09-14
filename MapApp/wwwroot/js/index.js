
$(document).ready(function () {
    // map is a global google map object

    refreshLocations();

    $("#refresh_locations").click(function () {
        refreshLocations();
    });

    $("#clear_locations").click(function () {
        clearLocations();
    });

})

