var timeStamp = 0 + ":" + 0 + ":" + 0 + "." + 0;
var subtitleArray = [];
var lineNumber = 0;
var currentTime = 0;
var interval;
var subtitleIsSet = false;

function getSubtitle(subtitleurl, vid) {

    $.ajax({
        url: subtitleurl,
        type: 'get',
        async: false,
        success: function (data) {
            var lines = data.split("\n");
            $.each(lines, function (key, line) {
                if (line.indexOf("Dialogue") > -1) {
                    var parts = line.split(',');
                    parts[parts.indexOf(parts[1])] = timeStampToSeconds(parts[1]);
                    parts[parts.indexOf(parts[2])] = timeStampToSeconds(parts[2]);
                    subtitleArray.push(parts);
                }
            });
            console.log("Succesfully read subtitle!");
            subtitleIsSet = true;
        },
        error: function (err) {
            subtitleIsSet = false;
        }
    });

    if (subtitleIsSet) {
        console.log("Starting timer");
        getTimeStamp(vid);

        vid.onseeking = function () {
            var index = 0;
            var time = vid.currentTime;
            $.each(subtitleArray, function (curText) {
                var timeStart = curText[1];
                var timeEnd = curText[2];
                console.log("INDEX SEARCH ON TIMESTART: " + timeStart + ", TIMEEND: " + timeEnd + " WITH CURRENT TIME: " + curText);
                if (time > timeStart && time < timeEnd) {
                    console.log("FOUND SUB POS AT: " + index);
                    lineNumber = index;
                    return index;
                } else {
                    index++;
                }

            });
        }
    }
    return subtitleIsSet;
}

function resetSubtitle() {
    subtitleIsSet = false;
    subtitleArray = [];
    $('#subtitle').html('');
    try{
        clearInterval(interval);
    } catch (e) {
        console.log("no interval running");
    }
}
       
function timeStampToSeconds(timestamp) {
    var parts = timestamp.split(':');
    var hour = parts[0];
    var minute = parts[1];
    var second = parts[2];
    var totalSeconds = hour * 3600 + minute * 60 + parseInt(second);
    return totalSeconds;
}

function getTimeStamp(vid) {
    interval = setInterval(function () {
        if (video.ended) {
            clearInterval(interval);
        }
        var curTimeSecond = vid.currentTime;
        currentTime = curTimeSecond;
        setTimeout(showSubtitle(curTimeSecond, vid), 0);               
    }, 100);
}

function showSubtitle(time, vid) {
    
    try{
        var currentText = subtitleArray[lineNumber];
        var secondOfTimeStart = currentText[1];
        var secondOfTimeEnd = currentText[2];
       
        if (time < secondOfTimeStart) {
            $('#subtitle').html('');
        } else {

            if (time > secondOfTimeEnd) {
                lineNumber++;
            } else {
                var fullText = "";
                for (var i = 9; i < currentText.length; i++) {
                    fullText = fullText + "," + currentText[i];
                }
                $('#subtitle').html(fullText.substring(1).replace("\\N", "<br />"));
            }
        }
    } catch (e) {
        clearInterval(interval);
    }
    
}
        