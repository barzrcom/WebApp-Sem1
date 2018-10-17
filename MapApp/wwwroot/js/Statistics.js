$(document).ready(function () {

    //options
    var options = {
        responsive: true,
        title: {
            display: true,
            position: "top",
            text: "Locations Categories",
            fontSize: 18,
            fontColor: "#111"
        },
        legend: {
            display: true,
            position: "bottom",
            labels: {
                fontColor: "#333",
                fontSize: 16
            }
        }
    };

    location_category = []

    // get Categories dynamicly
    $.get("/Locations/Categories", function (data) {
        location_category = data;
 
        chartdata = {
            datasets: [{
                data: [],
                backgroundColor: [
                    "#F7464A",
                    "#46BFBD",
                    "#FDB45C",
                    "#F5DEB3",
                    "#9ACD32"
                ],
                borderColor: [
                    "#FFFFFF",
                    "#FFFFFF",
                    "#FFFFFF",
                    "#FFFFFF",
                    "#FFFFFF"
                ],
                borderWidth: [1, 1, 1, 1, 1]
            }],
            // These labels appear in the legend and in the tooltips when hovering different arcs
            labels: []
        };
 

        $.each(location_category, function (key, value) {
            chartdata.datasets[0].data.push(value);
            chartdata.labels.push(key);
        });



        var ctx = document.getElementById("myChart").getContext('2d');

        // And for a doughnut chart
        var myDoughnutChart = new Chart(ctx, {
            type: 'doughnut',
            data: chartdata,
            options: options
        });

    });



})

