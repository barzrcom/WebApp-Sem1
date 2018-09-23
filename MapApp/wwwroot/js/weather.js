
$(document).ready(function () {
    var lat, lon, api_url;
    lat = $.trim($('#Latitude').html());
    lon = $.trim($('#Longitude').html());

    api_url = 'http://api.openweathermap.org/data/2.5/weather?lat=' +
        lat + '&lon=' +
        lon + '&units=metric&appid=42f325f217132a2fa4b282b44d4a873e';

    $.ajax({
        url: api_url,
        method: 'GET',
        success: function (data) {

            var tempr = data.main.temp;
            var icon = data.weather[0].icon;

            $('#weather').text(tempr + '°');
            $('#weather').append('<img src=http://openweathermap.org/img/w/' + icon + '.png>');

        }
    });


});