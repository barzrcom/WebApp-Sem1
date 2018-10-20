$(document).ready(function () {

    //options
    var options1 = {
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

    var options2 = {
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true
                }
            }]
        },
        responsive: true,
        title: {
            display: true,
            position: "top",
            text: "Top Views",
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

    var options3 = {
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true
                }
            }],
			xAxes: [{
				type: 'time',
				distribution: 'series',
				time: {
					unit: 'day',
					unitStepSize: 1,
					displayFormats: {
						'day': 'MMM DD'
					}
				}
            }]
        },
        responsive: true,
        title: {
            display: true,
            position: "top",
            text: "Activity per Day (Views)",
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

    // get Categories dynamicly
    $.get("/Locations/Categories", function (data) {
        location_category = data;
 
        chartdata1 = {
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
            chartdata1.datasets[0].data.push(value);
            chartdata1.labels.push(key);
        });



        var ctx = document.getElementById("myChart1").getContext('2d');

        // And for a doughnut chart
        var myDoughnutChart = new Chart(ctx, {
            type: 'doughnut',
            data: chartdata1,
            options: options1
        });

    });

    // get Categories dynamicly
    $.get("/Views/ByLocation", function (data) {
        location_category = data;

        chartdata2 = {
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
            chartdata2.datasets[0].data.push(value);
            chartdata2.labels.push(key);
        });



        var ctx = document.getElementById("myChart2").getContext('2d');

        // And for a doughnut chart
        var myDoughnutChart = new Chart(ctx, {
            type: 'bar',
            data: chartdata2,
            options: options2
        });

    });


    // get Categories dynamicly
    $.get("/Views/ByDate", function (data) {
        location_category = data;

        chartdata3 = {
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
            chartdata3.datasets[0].data.push(value);
            chartdata3.labels.push(key);
        });



        var ctx = document.getElementById("myChart3").getContext('2d');

        // And for a doughnut chart
        var myDoughnutChart = new Chart(ctx, {
            type: 'line',
            data: chartdata3,
            options: options3
        });

    });


})

