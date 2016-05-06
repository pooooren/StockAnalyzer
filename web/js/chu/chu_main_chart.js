// JavaScript source code

function chu_main_chart() { }

chu_main_chart.showdetail = function (stock,start) {
    var url = "/rest/rest/info/id/" + stock;
    $.ajaxSetup({ async: false });
    var t = $.getJSON(url, function (data) {
        var weight = data["weight"];
        $("#BigSelect").empty();
        $("#BigSelect").append("<option>" + 500 * weight + "</option><option  selected='selected'>" + 1000 * weight + "</option><option>" + 2000 * weight + "</option>");
        var name = data["name"];
        var namestring = "<a href='http://quote.eastmoney.com/" + stock + ".html'>" + name + "</a>";

        $("#name").html(namestring);

        var first = data["firstlevel"];
        var second = data["secondlevel"];
        var location = data["location"];
        $('#industry1').attr('href', '/web/company.html?industry1=' + first);
        $('#industry1').text(first);

        $('#industry2').attr('href', '/web/company.html?industry1=' + first + '&industry2=' + second);
        $('#industry2').text(second);

        $('#location').attr('href', '/web/company.html?location=' + location);
        $('#location').text(location);
		
		$('#trend').attr('href', '/web/single_statistics.html?sid='+stock+'&start='+start+'&big=500' + location);
        $('#trend').text("排名走势");
    });

    //chu_main_chart.showfinance(stock);
		//$('#finance').load('/web/finance.html?sid=' + stock);
    $("#finance").html("<iframe src=/web/finance.html?sid=" + stock + " width=1200 height=35 frameborder=0></iframe>");
};

//chu_main_chart.showfinance = function (stock) {
//    var url = "/rest/rest/infoext/id/" + stock;
//    var html = "";
//    $.ajaxSetup({ async: false });
//    var t = $.getJSON(url, function (data) {
//        html += " shiyinglv:" + data["shiyinglv"];
//        html += " shijinjinglv:" + data["shijinglv"];
//        html += " ROE:" + data["ROE"];
//        html += " meiguweifenpeilirun:" + data["meiguweifenpeilirun"];
//        html += " shourutongbi:" + data["shourutongbi"];
//        html += " jingzichantongbi:" + data["jingliruntongbi"];
//    });
//    $('#finance').text(html);
//}

chu_main_chart.showchart = function (stock,big,type,start) {

    var seriesOptions,
    yAxisOptions = [],
    seriesCounter = 0,
    colors = Highcharts.getOptions().colors;
				
    var tag = [];
    var totalshare = [];
    var sellshare = [];
    var buyshare = [];
    var incrementalBuyMoney = [];
    var incrementalSellMoney = [];
    var diff = [];
    var diff_share = [];
    var big_share_rate = [];
    var close = [];

	var rank=[];
	
    $.ajaxSetup({ async: false });
    var url = "/rest/rest/query/id/" + stock + "?big=" + big + "&type=" + type + "&start=" + start;
    var t = $.getJSON(url, function (data) {
        for (var i in data) {
            tag[i] = data[i]["tag"];
            totalshare[i] = data[i]["totalshare"];
            sellshare[i] = data[i]["sellshare"];
            buyshare[i] = data[i]["buyshare"];
            incrementalBuyMoney[i] = data[i]["incrementalBuyMoney"];
            incrementalSellMoney[i] = data[i]["incrementalSellMoney"];
            diff[i] = incrementalBuyMoney[i] - incrementalSellMoney[i];
            diff_share[i] = data[i]["incrementalBuyShare"] - data[i]["incrementalSellShare"];
            close[i] = data[i]["close"];
            big_share_rate[i] = (sellshare[i] + buyshare[i]) / totalshare[i];
            big_share_rate[i] = chu_common.formatnumber(big_share_rate[i],3);
			
        }
    });

	  var url = "/rest/rest/analyzebyDate?sid=" + stock +"&start=" + start;
    var t = $.getJSON(url, function (data1) {
        for (var i in data1) {
            rank[i] = data1[i]["rank"];
            
			
        }
    });

    seriesOptions = {
        name: "test",
        data: totalshare
    };

    $('#container').highcharts({
        chart: {
            zoomType: 'xy'
        },
        title: {
            text: 'analyze chart',
            x: -20 //center
        },
        subtitle: {
            text: '',
            x: -20
        },
        xAxis: {
            categories: tag
        },
        yAxis: [{
            title: {
                text: 'trade data'
            },
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }]
        }, //first Y
        {
            title: {
                text: 'price'
            },
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }],
            opposite: true
        } //second Y

        ],
        tooltip: {
            shared: true
        },
        legend: {
            layout: 'vertical',
            align: 'left',
            x: 120,
            verticalAlign: 'top',
            y: 100,
            floating: true,
            backgroundColor: '#FFFFFF'
        },
        series: [{
            name: 's',
            yAxis: 0,
            type: 'column',
            data: sellshare,
            color: 'green'
        }, {
            name: 'b',
            yAxis: 0,
            type: 'column',
            data: buyshare,
            color: 'red'
        },
        {
            name: 'close',
            yAxis: 1,
            type: 'line',
            data: close,
            color: 'blue'
        }]
    }); //high chart1

    $('#container2').highcharts({
        chart: {
            zoomType: 'xy'
        },
        title: {
            text: 'analyze chart',
            x: -20 //center
        },
        subtitle: {
            text: '',
            x: -20
        },
        xAxis: {
            categories: tag
        },
        yAxis: [{
            title: {
                text: 'trade data'
            },
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }]
        }, //first Y
        {
            title: {
                text: 'price'
            },
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }],
            opposite: true
        } //second Y

        ],
        tooltip: {
            shared: true
        },
        legend: {
            layout: 'vertical',
            align: 'left',
            x: 120,
            verticalAlign: 'top',
            y: 100,
            floating: true,
            backgroundColor: '#FFFFFF'
        },
        series: [
        {
            name: 'diff money',
            yAxis: 0,
            data: diff,
            type: 'line',
            color: 'blue'
        }
        ,
         {
             name: 'big share rate',
             yAxis: 1,
             data: big_share_rate,
             type: 'line',
             color: 'red'
         }

//        {
//        name: 'big share rate',
//    yAxis: 1,
//    data: diff_share,
//    type: 'line',
//    color: 'red'
//}
        ]
    });//high chart 2
	
	    $('#container3').highcharts({
        chart: {
            zoomType: 'xy'
        },
        title: {
            text: 'analyze chart',
            x: -20 //center
        },
        subtitle: {
            text: '',
            x: -20
        },
        xAxis: {
            categories: tag
        },
        yAxis: [{
            title: {
                text: 'trade data'
            },
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }]
        }, //first Y
        {
            title: {
                text: 'price'
            },
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }],
            opposite: true
        } //second Y

        ],
        tooltip: {
            shared: true
        },
        legend: {
            layout: 'vertical',
            align: 'left',
            x: 120,
            verticalAlign: 'top',
            y: 100,
            floating: true,
            backgroundColor: '#FFFFFF'
        },
        series: [
        {
            name: 'diff money',
            yAxis: 0,
            data: rank,
            type: 'line',
            color: 'blue'
        }

//        {
//        name: 'big share rate',
//    yAxis: 1,
//    data: diff_share,
//    type: 'line',
//    color: 'red'
//}
        ]
    });//high chart 2
};
