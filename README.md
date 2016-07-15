StockAnalyzer
=============

DownloadData.exe 600228 2013-01-01 2013-06-30


http://114.215.102.172/web/single_dynamic.html?sid=sz600000&big=2000

http://114.215.102.172/rest/rest/info/id/sh600000

http://localhost/rest/rest/statistics?type=name&end=20150304&level=1

http://localhost/rest/rest/statistics?start=20140101&end=20150304&level=1

http://localhost/web/analyze-statistics.html?start=20140101&level=1&end=20150304


http://my.oschina.net/u/209016/blog/79000


update test.analyzedata set month=TIMESTAMPDIFF(MONTH,startdate,enddate);

ALTER TABLE `test`.`sh600000` ADD INDEX `time` (`time` ASC);