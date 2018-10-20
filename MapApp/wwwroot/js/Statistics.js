var dynamicColors = function () {
    var r = Math.floor(Math.random() * 255);
    var g = Math.floor(Math.random() * 255);
    var b = Math.floor(Math.random() * 255);
    return "rgb(" + r + "," + g + "," + b + ")";
}

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
        },
        events: ["mousemove", "mouseout", "click", "touchstart", "touchmove", "touchend"],
        onClick: function (e, data) {
            if (data[0] != undefined) {
                window.location.href = "../Locations?category=" + data[0]._model.label;
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
            text: "Most Viewed (5)",
            fontSize: 18,
            fontColor: "#111"
        },
        legend: {
            display: false,
        },
        events: ["mousemove", "mouseout", "click", "touchstart", "touchmove", "touchend"],
        onClick: function (e, data) {
            if (data[0] != undefined) {
                window.location.href = "../Locations/Details/" + data[0]._model.label;
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
                    tooltipFormat: 'MMM DD',
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
            display: false,
        },
    };

    // get Categories dynamicly
    $.get("/Locations/Categories", function (data) {
        location_category = data;

        chartdata1 = {
            datasets: [{
                data: [],
                backgroundColor: [],
                borderColor: "#FFFFFF",
                borderWidth: 1
            }],
            // These labels appear in the legend and in the tooltips when hovering different arcs
            labels: []
        };


        $.each(location_category, function (key, value) {
            chartdata1.datasets[0].data.push(value);
            chartdata1.datasets[0].backgroundColor.push(dynamicColors());
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
    $.get("/Views/ByLocation?limitResults=5", function (data) {
        location_category = data;

        chartdata2 = {
            datasets: [{
                data: [],
                backgroundColor: [],
                borderColor: "#FFFFFF",
                borderWidth: 1
            }],
            // These labels appear in the legend and in the tooltips when hovering different arcs
            labels: []
        };


        $.each(location_category, function (key, value) {
            chartdata2.datasets[0].data.push(value);
            chartdata2.datasets[0].backgroundColor.push(dynamicColors());
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
                pointBackgroundColor: "#6597ed"
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
