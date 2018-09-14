// Initialize and add the map
var map = null;
var markerList = [];

function initMap() {
    // The location of Colman
    var colman = { lat: 31.968987, lng: 34.770724 };
    // The map, centered at Colman
    map = new google.maps.Map(document.getElementById('map'), { zoom: 18, center: colman });
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
        console.log(data);
        clearLocations();
        $.each(data, function (index, data) {
            var pos = { lat: data.latitude, lng: data.longitude };
            markerList.push(new google.maps.Marker({ position: pos, map: map }));
        });

    });
};