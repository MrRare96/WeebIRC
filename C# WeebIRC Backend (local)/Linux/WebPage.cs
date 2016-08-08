using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebIRCWebEdition
{
    class WebPage
    {
        public static string GetHtml(string ip) {

            return @"
                <html>
<head>
<title> WeebIRC </title>
</head>
 <!--Import Google Icon Font-->
<link href=""http://fonts.googleapis.com/icon?family=Material+Icons"" rel=""stylesheet"">
<!--Import materialize.css-->
<link type=""text/css"" rel=""stylesheet"" href=""/materialize/css/materialize.min.css"" media=""screen,projection"" />
<!--Let browser know website is optimized for mobile-->
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />

<script type=""text/javascript"">

    

    var loggedin = false;
    var webSocketServer = new WebSocket(""ws://"" + myIP + "":3125"");

    window.onbeforeunload = function (event) {
        webSocketServer.send(""disconnect"");
        webSocketServer.onclose = function () { }; // disable onclose handler first
        webSocketServer.close()
    };
</script>
<body onunload=""closecon()"">
    <ul id=""animeSeasonsFS"" style=""width: 200px;"" class=""dropdown-content"">
    </ul>
    <nav>
        <div class=""nav-wrapper customBlue"">
            <a href=""#chat"" class=""brand-logo hide-on-med-and-down"">WeebIRC</a>
            <center><span class=""center-align brand-logo"" id=""page""> Chat </span></center>

            <a href=""#"" data-activates=""slide-out"" class=""button-collapse""><i class=""material-icons"">menu</i></a>
            <ul id=""nav-mobile"" class=""right hide-on-med-and-down"">
                <li><a href=""#"" onclick=""onAnimeClick()"">Anime</a></li>
                <li><a href=""#"" class=""dropdown-button""  data-activates=""animeSeasonsFS""><i class=""material-icons right"">arrow_drop_down</i></a></li>
                <li><a href=""#"" onclick=""onChatClick()"">Chat</a></li>
                <li><a href=""#"" onclick=""onDlClick()"">Downloads</a></li>
                <li><a class=""modal-trigger"" onclick=""getSettings()"" href=""#settings"">Settings</a></li>
                

            </ul>


            <ul id=""slide-out"" class=""side-nav"">
                <li class=""no-padding"">
                    <ul class=""collapsible collapsible-accordion"">
                        <li>
                            <a href=""#"" onclick=""onAnimeClick()"">Anime</a><a class=""collapsible-header"">Anime<i class=""material-icons right"">arrow_drop_down</i></a>
                            <div class=""collapsible-body"">
                                <ul class=""animeSeasons"">
                                </ul>
                            </div>
                        </li>
                    </ul>
                </li>
                <li><a href=""#"" onclick=""onChatClick()"">Chat</a></li>
                <li><a href=""#"" onclick=""onDlClick()"">Downloads</a></li>
                <li><a class=""modal-trigger"" onclick=""getSettings()"" href=""#settings"">Settings</a></li>
            </ul>

        </div>
    </nav>


    <div class=""container"" id=""loading"">
        <div class=""row"" style=""width: 100%; height:100%;"">
            <div class=""col s12 valign-wrapper"" style=""width: 100%; height:100%;"">
                    <div style=""margin-left: 50%; margin-right: 50%;"">
                        <div class=""preloader-wrapper big active valign"">
                            <div class=""spinner-layer spinner-green-only"">
                                <div class=""circle-clipper left"">
                                    <div class=""circle""></div>
                                </div><div class=""gap-patch"">
                                    <div class=""circle""></div>
                                </div><div class=""circle-clipper right"">
                                    <div class=""circle""></div>
                                </div>
                            </div>
                        </div>
                    </div>
            </div>
        </div>
    </div>

    <div class=""container"" id=""chatwindow"" >
        <div class=""row"">
            <div class=""col s12"">
                <div id=""container1"">
                    <div id=""chat"">
                    </div>
                </div>
            </div>
        </div>
        <div class=""row"">
            <div class=""col s12"">
                <input id=""sendmsg"" class=""inputFocus"" style=""width: 85%"" type=""text"" />
                <button class=""btn waves-effect waves-light"" style=""width: 14%"" onclick=""onSendClick()""> Send </button>
            </div>
        </div>
    </div>

    <div class=""container"" id=""dlpage"">
        <div class=""row"">
            <div class=""col s12"">
                <table class=""highlight"">
                    <thead>
                        <tr>
                            <th data-field=""id"">File</th>
                            <th data-field=""name"">Status</th>
                            <th data-field=""price"">Progress</th>
                            <th data-field=""price"">Speed</th>
                            <th data-field=""price"">Options</th>
                        </tr>
                    </thead>

                    <tbody id=""tablebody"">
                       
                    </tbody>
                </table>

            </div>
        </div>
    </div>

    <div class=""col s12"">
        <div id=""anime""></div>
    </div>

    <div class=""container"" id=""animedetail"" >
    </div>

        
        <!-- Modal Structure -->
        <div id=""modal1"" class=""modal"">
            <div class=""modal-content"">
                <h4>IRC Setup</h4>
                <input class=""server inputFocus"" style=""width: 100%"" type=""text""><br />
                <input class=""channel inputFocus"" style=""width: 100%"" type=""text""><br />
                <input class=""username inputFocus"" style=""width: 100%"" type=""text""><br />
                <span class=""left""> Auto Connect: </span>  
                <div class=""switch right"">
                    <label>
                        No
                        <input class=""autoconnectsw"" onclick=""setAutoConnect()"" type=""checkbox"">
                        <span class=""lever""></span>
                        Yes
                    </label>
                </div>
            </div>
            <div class=""modal-footer"">

                <button class=""btn waves-effect waves-light"" style=""width: 100%"" onclick=""setTimeout(connectirc(), 0); saveSettings();"">Save and Connect </button>
            </div>
        </div>
        
        <div id=""settings"" class=""modal"">
            <div class=""modal-content"">
                <h4>SETTINGS</h4>
                <p>
                    <span class=""left""> Reset all synonyms: </span>  <a href=""#"" class=""btn waves-effect waves-light right"" onclick=""onLocalStorageReset()"">Reset</a><br />
                    <input class=""server inputFocus"" style=""width: 100%""  type=""text""><br />
                    <input class=""channel inputFocus"" style=""width: 100%"" type=""text""><br />
                    <input class=""username inputFocus"" style=""width: 100%""  type=""text""><br />
                    <span class=""left""> Auto Connect: </span>  
                    <div class=""switch right"">
                        <label>
                            No
                            <input class=""autoconnectsw"" onclick=""setAutoConnect()"" type=""checkbox"">
                            <span class=""lever""></span>
                            Yes
                        </label>
                    </div>
                </p>
                
            </div>
            <div class=""modal-footer"">

                <button class=""btn waves-effect waves-light"" style=""width: 100%"" onclick=""saveSettings()""> Save </button>
            </div>
        </div>


    <!---->


    <script type=""text/javascript"" src=""https://code.jquery.com/jquery-2.1.1.min.js""></script>
    <script type=""text/javascript"" src=""materialize/js/materialize.min.js""></script>
    <script type=""text/javascript"">
    //materialcss loading stuff
    $(document).ready(function () {
        // the ""href"" attribute of .modal-trigger must specify the modal ID that wants to be triggered
        $('.modal-trigger').leanModal();
    });
    $('.button-collapse').sideNav({
        closeOnClick: true // Closes side-nav on <a> clicks, useful for Angular/Meteor
    });
    
    //to make sure a new websocket connection can be made on reload or closing of webpage
    $(window).on('beforeunload', function () {
        sendircmsg(""/quit"");
        sendmsg(""IRCCLOSE"");
        webSocketServer.send(""disconnect"");
    });
    
    //check if textfields are focussed, prevents priviousPage from executing when typing backspace in textfields
    var isThereFocus = false;

    $(document).focusin(function () {
        isThereFocus = true;
        console.log(""focusin"");
    });
    $(document).focusout(function () {
        isThereFocus = false;
        console.log(""focusout"");
    });

    //global values
    var received = false;
    var tempMsgSend = """";
    var currentAnime = """";
    var animeEpisodes = JSON.parse(""{}"");
    var resolution = ""720"";
    var botselected = ""CR-HOLLAND|NEW"";
    var lastPage = """";
    var currentPage = ""ANIMES"";
    var loginIsRunning = false;

    //check if localstorage is available
    var localStorage;
    function storageAvailable(type) {
        try {
            localStorage = window[type],
                x = '__storage_test__';
            localStorage.setItem(x, x);
            localStorage.removeItem(x);
            return true;
        }
        catch (e) {
            return false;
        }
    }
    //send msg to websocket server
    function sendmsg(text) {
        webSocketServer.send(text);
        tempMsgSend = text;
    }
    //send irc 
    function onSendClick() {
        var msg = $('#sendmsg').val();
        sendircmessage(msg);
    }
    
    //send msg with irc prefix
    function sendircmessage(text) {
        var msg = ""irc:"" + text;
        tempMsgSend = msg;
        console.log(""sending irc: "" + msg);
        sendmsg(msg);
        $('#sendmsg').val("""");
    }

    //reset local storage
    function onLocalStorageReset() {
        if (storageAvailable('localStorage')) {
            localStorage.clear();
            console.log(""local storage cleared"");
        }
    }

    //go to previous page
    function goToPrevious() {
        console.log(""PREVIOUS PRESSED"");
        console.log(currentPage);
        console.log(lastPage);
        console.log(currentAnime);
        if (!isThereFocus) {
            if (lastPage != """") {
                if (lastPage == ""ANIMES"") {
                    onAnimeClick();
                } else if (lastPage == ""CHAT"") {
                    onChatClick();
                } else if (lastPage == ""DOWNLOADS"") {
                    onDlClick();
                } else if (lastPage == ""ANIMEDETAILS"") {
                    animeClicked(currentAnime);
                }
            }
        } else {
            console.log(""Something has been focussed, previous disabled :D"");
        }
       
    }
    
    //set auto connect to irc server on page load
    function setAutoConnect() {
        if (storageAvailable('localStorage')) {
            if ($('.autoconnectsw').is(':checked')) {
                localStorage.setItem(""autoconnect"", ""1"");
                Materialize.toast('Auto Connect Enabled', 4000);
            } else {
                localStorage.setItem('autoconnect', ""0"");
                Materialize.toast('Auto Connect Disabled', 4000);
            }
        }
    }

    //save local settings
    function saveSettings() {
        if (storageAvailable('localStorage')) {
            var server = $('.server').val();
            var channel = $('.channel').val();
            var username = $('.username').val();
            console.log(currentPage);
            console.log(lastPage);
            console.log(currentAnime);
            localStorage.setItem(""server"", server);
            localStorage.setItem(""channel"", channel);
            localStorage.setItem(""username"", username);
            Materialize.toast('Saved Settings!', 4000);
            $('#settings').closeModal();
        } else {
            Materialize.toast('Could not save Settings!', 4000);
            $('#settings').closeModal();
        }
    }

    //get local settings
    function getSettings() {
        if (storageAvailable('localStorage')) {
            if ((localStorage.getItem(""server"") != null) && (localStorage.getItem(""channel"") != null) && (localStorage.getItem(""username"") != null)) {

                var server = localStorage.getItem(""server"");
                var channel = localStorage.getItem(""channel"");
                var username = localStorage.getItem(""username"");

                $('.server').val(server);
                $('.channel').val(channel);
                $('.username').val(username);
                if ((localStorage.getItem(""autoconnect"") != null) && (localStorage.getItem(""autoconnect"") == ""1"")) {
                    $('.autoconnectsw').prop('checked', true);
                    if (!loginIsRunning) {
                        connectirc();
                    }
                } else {
                    $('.autoconnectsw').prop('checked', false);
                }
            }

        } else {
            Materialize.toast('Could not load Settings!', 4000);
        }
    }


    var tryIrcConnect;
    function connectirc() {
        tryIrcConnect = setInterval(keepConnectingUntilConnectedToIrc(), 500);
    }
    
    //retry connection to irc when failed to get response    
    function keepConnectingUntilConnectedToIrc() {
        var server = $('.server').val();
        var channel = $('.channel').val();
        var username = $('.username').val();
        if (!loginIsRunning) {
            loginIsRunning = true;
        }

      
       
        sendmsg(""server: "" + server + "" channel: "" + channel + "" username: "" + username + "" junk: this is junk"");
        if (loggedin) {
            Materialize.toast('Succesfully logged in onto your IRC server!', 4000);
            loginIsRunning = false;
            clearInterval(tryIrcConnect);
            return true;
        } 
        /* later */
    }

    webSocketServer.onopen = function (event) {
        webSocketServer.send(""YO!"");
        console.log(""YUS WORKS"");
        setTimeout(getSettings(), 0);

        Materialize.toast('Log into your IRC server!', 4000);
        $('#modal1').openModal();
    };

    webSocketServer.onmessage = function (event) {
        console.log(event.data);
        //login on irc success checker

        if (event.data.indexOf(""ping"") > -1) {
            sendmsg(""pong~pong~pong"");
        }
        if (event.data.indexOf(""loginsucces"") > -1) {
            loggedin = true;
            $('#modal1').closeModal();
            Materialize.toast('Succesfully logged in on IRC server!', 4000);

        }
        //send confirmation checker
        if (event.data.indexOf(""received:"") > -1) {
            console.log(""RECEIVED CONFIRMED"");
            if (event.data.substring(event.data.indexOf(""received:"") + 1)) {
                sendmsg(tempMsgSend + "":msgisok"");
            } else {
                sendmsg(tempMsgSend);
            }
            received = true;
        }

        //irc information
        if (event.data.indexOf(""irc:"") > -1) {
            loggedin = true;
            $('#modal1').closeModal();
            $('#chat').append(event.data.substring(event.data.indexOf("":"") + 1) + ""<br>"");
        }

        //download information
        if (event.data.indexOf(""download"") > -1 && (event.data.indexOf(""NO DOWNLOAD"") == -1)) {
            var dldata = event.data.split('~');
            var downloadid = dldata[1];
            var file = dldata[2];
            var fileStatus = dldata[3];
            var fileProgress = dldata[4];
            var fileSpeed = dldata[5];
            var fileAddress = dldata[6];
            var filePack = dldata[7];

            if (!$(""#"" + downloadid).length) {
                Materialize.toast('Download ' + file + ' started!', 4000);
                var tableRow = ""<tr id="" + downloadid + "">                                                                                                                        \
                                        <td>"" + file + ""</td>                                                                                                   \
                                        <td>"" + fileStatus + ""</td>                                                                                             \
                                        <td>"" + fileProgress + ""%</td>                                                                                           \
                                        <td>"" + fileSpeed + ""</td>                                                                                              \
                                        <td> <a id=\""button"" + downloadid + ""\""  onclick=\""\"" class=\""btn waves-effect waves-light\"" target=\""_blank\"" href=\""http://"" + myIP + "":8018/"" + fileAddress + ""\""> Get File </a></td>   \
                                    </tr> \
                                 <tr><td colspan=\""5\""><div class=\""progress\"" id=\""progressbar\"">  <div class=\""determinate\"" style=\""width: "" + fileProgress + ""%\""></div></div></td></tr>\
                    "";

                $('#tablebody').append(tableRow);
                console.log(""added new entry to table"");
                onDlClick();
            } else {
                var tableRowContent = ""<td>"" + file + ""</td>                                                                                                   \
                                        <td>"" + fileStatus + ""</td>                                                                                             \
                                        <td>"" + fileProgress + ""%</td>                                                                                           \
                                        <td>"" + fileSpeed + "" kb/s</td>                                                                                              \
                                        <td> <a id=\""button"" + downloadid + ""\""  onclick=\""\"" class=\""btn waves-effect waves-light\"" target=\""_blank\"" href=\""http://"" + myIP + "":8018/"" + fileAddress + ""\""> Get File </a></td>   \
                                       \
                    "";
                $('#' + downloadid).html(tableRowContent);
                $('#progressbar').html(' <div class=\""determinate\"" style=\""width: ' + fileProgress + '%\""></div>');
                console.log(""refreshed current download :?"");
            }

            if (event.data.indexOf(""COMPLETED"") > -1) {

                Materialize.toast('Download ' + file + ' done!', 4000);
                var tableRowContent = ""<td>"" + file + ""</td>                                                                                                   \
                                        <td>"" + fileStatus + ""</td>                                                                                             \
                                        <td>100%</td>                                                                                           \
                                        <td></td>                                                                                              \
                                        <td> <a id=\""button"" + downloadid + ""\""  onclick=\""\"" class=\""btn waves-effect waves-light\"" target=\""_blank\"" href=\""http://"" + myIP + "":8018/"" + fileAddress + ""\""> Get File </a></td>   \
                                       \
                    "";
                $('#' + downloadid).html(tableRowContent);
                $('#progressbar').html(' <div class=\""determinate\"" style=\""width: 100%\""></div>');
            } else if (event.data.indexOf(""FAILED"") > -1) {
                Materialize.toast('Download ' + file + ' failed  :(', 4000);
            }
        }
    }


    $(document).ready(setTimeout(function () {

        //load anime
        setTimeout(function () { getAllSeasons(); getAndPrintAnime('http://myanimelist.net/anime/season'); }, 0);
        //print anime
        setTimeout(onAnimeClick(), 0);
        //listens for backspace pressed
        jQuery(function ($) {
            var input = $(document);
            input.on('keydown', function () {
                var key = event.keyCode || event.charCode;

                if (key == 8 || key == 46)
                    goToPrevious();
            });
        });
        //android back button listener
        document.addEventListener('backbutton', onBackKeyDown, false);
        //goes to previous page 
        function onBackKeyDown(event) {
            // Handle the back button
            event.preventDefault();
            goToPrevious();
        }
        //send message in chat;
        $(""#sendmsg"").keyup(function (event) {
            if (event.keyCode == 13) {
                var msg = $('#sendmsg').val();
                console.log(""sending irc: "" + msg);
                sendircmessage(msg);
                $('#sendmsg').val("""");
            }
        });

       
        
    }, 0));

    //show currently downloading list
    function onDlClick() {
        $(document).ready(function () {
            lastPage = currentPage;
            currentPage = ""DOWNLOADS"";
            $(""#dlpage"").show();
            $(""#chatwindow"").hide();
            $(""#anime"").hide();
            $(""#animedetail"").hide();
            $(""#loading"").hide();
            $(""#page"").html(""Downloads"");
        });
    }
    
    //opens chat page (normally not used often)
    function onChatClick() {
        $(document).ready(function () {
            lastPage = currentPage;
            currentPage = ""CHAT"";
            $(""#chatwindow"").show();
            $(""#dlpage"").hide();
            $(""#anime"").hide();
            $(""#animedetail"").hide();
            $(""#loading"").hide();
            $(""#page"").html(""Chat"");
        });
    }
    
    //opens page with anime covers (default page)
    function onAnimeClick() {
        $(document).ready(function () {
            lastPage = currentPage;
            currentPage = ""ANIMES"";
            $(""#anime"").show();
            $(""#chatwindow"").hide();
            $(""#dlpage"").hide();
            $(""#animedetail"").hide();
            $(""#loading"").hide();
            $(""#page"").html(""Anime"");
        });
    }
    
    function showLoadScreen() {
        $(document).ready(function () {
            $(""#anime"").hide();
            $(""#chatwindow"").hide();
            $(""#dlpage"").hide();
            $(""#animedetail"").hide();
            $(""#loading"").show();
            $(""#page"").html(""Anime"");
        });
    }
        //get all anime covers from myanimelist url


    function getAllSeasons() {
        var url = ""?url:http://myanimelist.net/anime/season/archive"";
        $.ajax({
            url: url, success: function (data) {
                var htmlMalSeason = data.replace(/(\r\n|\n|\r)/gm, """");
                // then you can manipulate your text as you wish
                var div = document.createElement('div');

                div.innerHTML = htmlMalSeason;

                var seasonsByUrl = div.getElementsByClassName(""anime-seasonal-byseason"")[0].getElementsByTagName('a');

                
                var output = """";

                var amountOfTags = seasonsByUrl.length;
                for (var i = 0; i < amountOfTags; i++) {

                    var animeseason = (seasonsByUrl[i].innerText || seasonsByUrl[i].textContent).trim();
                    var animeSeasonlink = (seasonsByUrl[i].getAttribute(""href"")).trim();

                 
                    output = output + '<li><a href=""#!"" onclick=""setTimeout(function(){showLoadScreen(); getAndPrintAnime(\'' + animeSeasonlink + '\'); onAnimeClick();}, 0);"">' + animeseason + '</a></li>';
                }

                $(document).ready(function () {
                    $(""#animeSeasonsFS"").html(output);
                    $("".dropdown-button"").dropdown();

                    $("".animeSeasons"").html(output);
                });
            }
        });
    }

    function getAndPrintAnime(rawurl) {
        setTimeout(showLoadScreen(), 0);
        var url = ""?url:"" + rawurl;
        $.ajax({
            url: url, success: function (data) {
                htmlMalSeason = data.replace(/(\r\n|\n|\r)/gm, """");
                // then you can manipulate your text as you wish
                var div = document.createElement('div');

                div.innerHTML = htmlMalSeason;

                var animeimg = div.getElementsByClassName(""image"");
                var animesynopsis = div.getElementsByClassName(""synopsis js-synopsis"");
                var animegenres = div.getElementsByClassName(""genres-inner js-genre-inner"");
                var output = '<div class=""container""><div class=""row""><div class=""col s12""><p><input id=""searchInput"" class=""inputFocus"" style=""width: 100%"" type=""text"" value=""Search"" /></p></div></div></div>';
                for (var i = 0; i < animeimg.length; i++) {

                    var style = animeimg[i].currentStyle || window.getComputedStyle(animeimg[i], false);
                    var animecover = animeimg[i].getAttribute(""style"").match(/url\([""|']?([^""']*)[""|']?\)/)[1];
                    var animetitles = (animeimg[i].innerText || animeimg[i].textContent).trim();
                    var animeLinkTag = animeimg[i].getElementsByClassName(""link-image"");
                    var animeLink = (animeLinkTag[0].getAttribute(""href"")).trim();
                    var synopsis = (animesynopsis[i].innerText || animesynopsis[i].textContent).trim();

                    var synopsisParsed = synopsis.replace(/[`~!@#$%^&*()_|+\-=?;:'"",.<>\{\}\[\]\\\/]/gi, '').replace(/(\r\n|\n|\r)/gm, ""<br>"");

                    var genresUnparsed = animegenres[i].innerText || animegenres[i].textContent;
                    var genres = genresUnparsed.replace(/\s+/g,' ');
                    var id = i;
                    output = output + '<div class=""hide-on-med-and-down selectionHover"">\
                                    <div class=""card customCard"" imgid=""' + id + '""> \
                                        <div class=""card-image customCard-image"">\
                                            <img class=""customCard-image"" onclick=""animeClicked(\'' + animeLink + '~' + animecover + '~' + animetitles + '~' + synopsisParsed + '~' + genres + '\')"" src=""' + animecover + '"" />\
                                            <span class=""card-title alpha60 customCard-title"">\
                                                <h6>' + animetitles + '</h6>\
                                            </span>\
                                        </div>\
                                      </div>\
                                    </div>'
                    +
                                    '<div class=""hide-on-large-only row valign-wrapper selectionHover"" onclick=""animeClicked(\'' + animeLink + '~' + animecover + '~' + animetitles + '~' + synopsisParsed + '~' + genres + '\')"">\
                                        <div class=""col s12"" style=""margin-top: 2%;"">\
                                            <div class=""col s3"">\
                                                <img class=""mobileCustomImg"" src=""' + animecover + '"" /> \
                                            </div>\
                                            <div class=""col s9"">\
                                                <span>\
                                                    <center><h5 class=""valign center-align"">' + animetitles + '</h6><hr>\
                                                    ' + genres + '\
                                               </center> </span>\
                                           </div>\
                                        </div>\
                                        <hr>\
                                    </div>';
                }


                $(document).ready(function () {
                    onAnimeClick();
                    $(""#anime"").html(output);

                    $(""#searchInput"").keyup(function (event) {
                        if (event.keyCode == 13) {
                            var searchparam = $('#searchInput').val().replace(' ', '%20');
                            var url = 'http://myanimelist.net/anime.php?q=' + searchparam;
                            console.log(""Searching url: "" + url);
                            setTimeout(getAndPrintSearchedAnime(url), 0);
                        }
                    });
                    $(""#searchInput"").click(function (event) {
                        $('#searchInput').val("""");
                    });
                });
            }
        });
    }


    function getAndPrintSearchedAnime(rawurl) {
        setTimeout(showLoadScreen(), 0);
        var url = ""?url:"" + rawurl;
        $.ajax({
            url: url, success: function (data) {
                htmlMalSeason = data.replace(/(\r\n|\n|\r)/gm, """");
                // then you can manipulate your text as you wish
                var div = document.createElement('div');

                div.innerHTML = htmlMalSeason;

                var animeimg = div.getElementsByTagName(""img"");
                var animesynopsis = div.getElementsByClassName(""pt4"");
                var animelink = div.getElementsByClassName(""hoverinfo_trigger"");
                
                var output = '<div class=""container""><div class=""row""><div class=""col s12""><p><input id=""searchInput"" class=""inputFocus"" style=""width: 100%"" type=""text"" value=""Search"" /></p></div></div></div>';
                var animeimglength = animeimg.length;
                for (var i = 0; i < animeimglength; i++) {

                    var animecover = animeimg[i].getAttribute(""src"").trim().replace(/t([^t]*)$/,'$1');
                    console.log(animecover);
                    var animetitles = animeimg[i].getAttribute(""alt"").trim();
                    console.log(animetitles);
                    var animeLink = animelink[i].getAttribute(""href"").trim();


                    var synopsis = """";
                    try{
                        synopsis = (animesynopsis[i].innerText || animesynopsis[i].textContent).trim();
                    } catch(e) {
                        synopsis = ""unknown"";
                    }

                    var synopsisParsed = synopsis.replace(/[`~!@#$%^&*()_|+\-=?;:'"",.<>\{\}\[\]\\\/]/gi, '').replace(/(\r\n|\n|\r)/gm, ""<br>"");
                    var genres = ""WIP"";
                    var id = i;
                    output = output + '<div class=""hide-on-med-and-down selectionHover"">\
                                    <div class=""card customCard"" imgid=""' + id + '""> \
                                        <div class=""card-image customCard-image"">\
                                            <img class=""customCard-image"" onclick=""animeClicked(\'' + animeLink + '~' + animecover + '~' + animetitles + '~' + synopsisParsed + '~' + genres + '\')"" src=""' + animecover + '"" />\
                                            <span class=""card-title alpha60 customCard-title"">\
                                                <h6>' + animetitles + '</h6>\
                                            </span>\
                                        </div>\
                                      </div>\
                                    </div>'
                    +
                                    '<div class=""hide-on-large-only row valign-wrapper selectionHover"" onclick=""animeClicked(\'' + animeLink + '~' + animecover + '~' + animetitles + '~' + synopsisParsed + '~' + genres + '\')"">\
                                        <div class=""col s12"" style=""margin-top: 2%;"">\
                                            <div class=""col s3"">\
                                                <img class=""mobileCustomImg"" src=""' + animecover + '"" /> \
                                            </div>\
                                            <div class=""col s9"">\
                                                <span>\
                                                    <center><h5 class=""valign center-align"">' + animetitles + '</h6><hr>\
                                                    ' + genres + '\
                                               </center> </span>\
                                           </div>\
                                        </div>\
                                        <hr>\
                                    </div>';
                }


                $(document).ready(function () {
                    onAnimeClick();
                    $(""#anime"").html(output);

                    $(""#searchInput"").keyup(function (event) {
                        if (event.keyCode == 13) {
                            var searchparam = $('#searchInput').val().replace(' ', '%20');
                            var url = 'http://myanimelist.net/anime.php?q=' + searchparam;
                            console.log(""Searching url: "" + url);
                            setTimeout(getAndPrintAnime(url), 0);
                        }
                    });
                    $(""#searchInput"").click(function (event) {
                        $('#searchInput').val("""");
                    });
                });
            }
        });
    }

    //prints more information about the anime(cover) you clicked
    function animeClicked(animeInfo) {
        lastPage = currentPage;
        currentPage = ""ANIMEDETAILS"";

        var infoPage, cover, title, synopsis, genres;
        try{
            var seperateInfo = animeInfo.split('~');
            infoPage = seperateInfo[0];
            cover = seperateInfo[1];
            title = seperateInfo[2];
            synopsis = seperateInfo[3];
            genres = seperateInfo[4];
            currentAnime = animeInfo;

            var synopsisParsed = synopsis.split(""039"").join('\'');
            var synonyms = """";
            if (storageAvailable('localStorage')) {
                // Yippee! We can use localStorage awesomeness
                if (localStorage.getItem(title) != null) {
                    var synonymsJsonString = localStorage.getItem(title);
                    var synonymsJson = JSON.parse(synonymsJsonString);
                    $.each(synonymsJson, function (key, value) {
                        synonyms = synonyms + "", "" + value;
                    });
                    synonyms = synonyms.substring(2);
                } else {
                    synonyms = ""None"";
                }
            }
            else {
                // Too bad, no localStorage for us
                synonyms = ""Unable to load."";
                Materialize.toast('Can\'t load your preferences :( ', 4000);
            }
            console.log(""anime clicked"");
            var output = '  <div class=""row"">\
                            <div class=""col s3 left"">\
                                <div class=""section"">\
                                    <div class=""card"" href=""' + infoPage + '"" style=""max-width: 225px; float: left;""> \
                                        <div class=""card-image hide-on-med-and-down"" id=""' + infoPage + '"" onclick=""animeClicked(' + infoPage + ', ' + cover + ', ' + title + ', ' + synopsisParsed + ', ' + genres + ')"" style=""height: 316px;"">\
                                            <img src=""' + cover + '"" /> <span class=""card-title alpha60"" style=""width: 100%;"">\
                                            <h6>' + title + '</h6>\
                                        </div>\
                                        <div class=""card-image mobileCustomImg hide-on-large-only"" id=""' + infoPage + '"" onclick=""animeClicked(' + infoPage + ', ' + cover + ', ' + title + ', ' + synopsisParsed + ', ' + genres + ')"">\
                                            <img src=""' + cover + '"" />\<span class=""card-title custom-title alpha60"" style=""width: 100%;"">\
                                            <h6>' + title + '</h6>\
                                        </div>\
                                        <div class=""card-action"">\
                                            <label for=""genres"">Genres:</label>\
                                            <div id=""genres"" style=""color: #4886ff; word-wrap: break-word;"">' + genres + '</div>\
                                        </div>\
                                         <div class=""card-action"">\
                                            <label class=""left"" for=""allsynonyms"">Synonyms:</label><span class=""right""><a onclick=""showAddInput()""><i class=""material-icons"" >add</i></a></span><br>\
                                            <div id=""showAddInput""> </div>\
                                            <div id=""allsynonyms"" style=""color: #4886ff; word-wrap: break-word;"">' + synonyms + '</div>\
                                        </div>\
                                    </div>\
                                </div>\
                            </div>\
                            <div class=""col s9 right"">\
                                <div class=""section"">\
                                    <h4> ' + title + ' </h4>\
                                    <hr> ' + synopsisParsed + ' \
                                </div>\
                            </div>\
                        </div>\
                        <div class=""divider""></div>\
                        <div class=""row"">\
                            <div class=""input-field col s6"">\
                                        <select onchange=""onBotChange()"" id=""botlist"">\
                                            <option value="""" disabled selected>Choose your preffered Bot</option>\
                                        </select>\
                                    </div>\
                            <div class=""input-field col s6"">\
                                <select onchange=""changeResolution()"" id=""resolutionSelected"">\
                                    <option value=""1080"">1080p</option>\
                                    <option value=""720"" selected>720p</option>\
                                    <option value=""480"">480p</option>\
                                    <option value=""none"">Everything</option>\
                                </select>\
                            </div>\
                        </div>\
                        <div class=""divider""></div>\
                        <div class=""row"">\
                            <div class=""col s12"">\
                                <div class=""section"">\
                                    <ul class=""collapsible popout"" id=""episodes"" data-collapsible=""accordion""> </u>\
                                </div>\
                            </div>\
                        </div>';

            $(document).ready(function () {
                $(""#animedetail"").html(output);
                $(""#anime"").hide();
                $(""#chatwindow"").hide();
                $(""#dlpage"").hide();
                $(""#animedetail"").show();


                $('select').material_select();
                setTimeout(searchOnNibl(title, true), 0);

                if (storageAvailable('localStorage')) {
                    // Yippee! We can use localStorage awesomeness
                    if (localStorage.getItem(currentAnime.split('~')[2]) != null) {
                        var synonymsJsonString = localStorage.getItem(title);
                        var synonymsJson = JSON.parse(synonymsJsonString);
                        $.each(synonymsJson, function (key, value) {
                            setTimeout(searchOnNibl(value, false), 0);
                        });
                    }
                }
            });


        } catch (E) {
            console.log(""COULD NOT PARSE ANIME INFO"");
            Materialize.toast('Could not load anime information!', 4000);
        }

    }

    //input for synonyms 
    function showAddInput(animeid) {
        var output = '<input id=""add_synonym"" class=""inputFocus"" type=""text"">\
                      <label for=""add_synonym"">Synonym</label>';
        $(""#showAddInput"").html(output);

        $(""#add_synonym"").keyup(function (event) {
            console.log(""synonym adding"");
            if (event.keyCode == 13) {
                var synonym = $('#add_synonym').val();
                if (storageAvailable('localStorage')) {

                    var synonyms = """";
                    if (localStorage.getItem(currentAnime.split('~')[2]) == null) {
                        localStorage.setItem(currentAnime.split('~')[2], '{""' + synonym + '"":""' + synonym + '""}');
                        synonyms = ""None"";
                        var synonymsJsonString = localStorage.getItem(currentAnime.split('~')[2]);
                        var synonymsJson = JSON.parse(synonymsJsonString);
                        $.each(synonymsJson, function (key, value) {
                            synonyms = synonyms + "", "" + value;
                        });
                        synonyms = synonyms.substring(2);
                    } else {
                        var currentSynonyms = localStorage.getItem(currentAnime.split('~')[2]);
                        var newSynonyms = currentSynonyms.Replace('}', ',') + '""' + synonym + '"":""' + synonym + '""}';
                        localStorage.setItem(currentAnime.split('~')[2], newSynonyms);


                        var synonymsJsonString = localStorage.getItem(currentAnime.split('~')[2]);
                        var synonymsJson = JSON.parse(synonymsJsonString);
                        $.each(synonymsJson, function (key, value) {
                            synonyms = synonyms + "", "" + value;
                            setTimeout(searchOnNibl(value, false), 0);
                        });
                        synonyms = synonyms.substring(4);
                    }
                    Materialize.toast('Succesfully stored your synonym!', 4000);
                }
                else {
                    // Too bad, no localStorage for us
                    Materialize.toast('Can\'t store your preferences :( ', 4000);
                }
                console.log(""CURRENT SYNONYMS:"" + synonyms);
                $(document).ready(function () {
                    $('#allsynonyms').html(synonyms);
                    $(""#showAddInput"").html("""");
                });
            }
        });
    }

    //search nibl for anime
    function searchOnNibl(title, replace) {

        var searchQuery = title.trim().replace(/\s+/g, '+');
        var url = ""?url:http://nibl.co.uk/bots.php?search="" + searchQuery;
        var htmlMalSeason = """";
        console.log(""searching url: "" + url);



        $.ajax({
            url: url, success: function (data) {
                htmlMalSeason = data;
                // then you can manipulate your text as you wish
                var div = document.createElement('div');

                div.innerHTML = htmlMalSeason;

                var botnameall = div.getElementsByClassName(""botname"");
                var packnumberall = div.getElementsByClassName(""packnumber"");
                var filenameall = div.getElementsByClassName(""filename"");

                var previousBot = """";
                var jsonbots = """";

                if (replace) {
                    console.log(""Think its time to create a json object with bots and episodes :I"");
                    var bots = ""{ \""bots\"": {"";
                    var botSpecific;

                    for (var i = 0; i < filenameall.length; i++) {

                        var botname = (botnameall[i].innerText || botnameall[i].textContent).trim().replace(""[s]"", """").trim();
                        var packnumber = (packnumberall[i].innerText || packnumber[i].textContent).trim();
                        var filename = (filenameall[i].innerText || filenameall[i].textContent).replace(""[s]"", """").replace(""[get]"", """").trim();
                        var bot;
                        if (previousBot == botname) {
                            bots = bots + "",{\""filename\"":\"""" + filename + ""\"", \""packnumber\"":\"""" + packnumber + ""\""}"";
                        } else {
                            if (i == 0) {
                                bots = bots + ""\"""" + botname + ""\"": ["";
                            } else {
                                bots = bots + ""],\"""" + botname + ""\"": ["";
                            }

                            bots = bots + ""{\""filename\"":\"""" + filename + ""\"", \""packnumber\"":\"""" + packnumber + ""\""}"";
                        }

                        previousBot = botname;
                    }
                    bots += ""]}}"";
                    jsonbots = bots.replace(/(\r\n|\n|\r)/gm, """");
                    try{
                        animeEpisodes = JSON.parse(jsonbots);
                        Materialize.toast('Episodes found for <br> ' + title + ' :)', 4000);
                    } catch (e){
                        animeEpisodes = JSON.parse(""{}"");
                        Materialize.toast('No episodes found for <br> ' + title + ' :(', 4000);
                    }
                } else if (!replace) {
                    try{
                        console.log(""will try to append instead of replacing :I"");
                        var currentbots = animeEpisodes;
                        console.log(currentbots);
                        for (var i = 0; i < filenameall.length; i++) {
                            var botname = (botnameall[i].innerText || botnameall[i].textContent).trim().replace(""[s]"", """").trim();
                            var packnumber = (packnumberall[i].innerText || packnumber[i].textContent).trim();
                            var filename = (filenameall[i].innerText || filenameall[i].textContent).replace(""[s]"", """").replace(""[get]"", """").trim();
                            var bot;

                            currentbots.bots[botname][Object.keys(currentbots.bots[botname]).length].filename = filename;
                            currentbots.bots[botname][Object.keys(currentbots.bots[botname]).length].packnumber = packnumber;
                            previousBot = botname;
                            console.log(""APPEND TO JSON"");
                        }
                        if(currenbots == animeEpisodes){
                            Materialize.toast('No episodes found for <br> ' + title + ' :(', 4000);
                            console.log(currentbots);
                        } else {
                            Materialize.toast('Episodes found for <br> ' + title + ' :)', 4000);
                            animeEpisodes = currentbots;
                            console.log(currentbots);
                            console.log(animeEpisodes);
                        }

                    } catch (e) {
                        console.log(""Cannot replace because there is nothing there to replace :I"");
                        var bots = ""{ \""bots\"": {"";
                        var botSpecific;
                        for (var i = 0; i < filenameall.length; i++) {

                            var botname = (botnameall[i].innerText || botnameall[i].textContent).trim().replace(""[s]"", """").trim();
                            var packnumber = (packnumberall[i].innerText || packnumber[i].textContent).trim();
                            var filename = (filenameall[i].innerText || filenameall[i].textContent).replace(""[s]"", """").replace(""[get]"", """").trim();
                            var bot;
                            if (previousBot == botname) {
                                bots = bots + "",{\""filename\"":\"""" + filename + ""\"", \""packnumber\"":\"""" + packnumber + ""\""}"";
                            } else {
                                if (i == 0) {
                                    bots = bots + ""\"""" + botname + ""\"": ["";
                                } else {
                                    bots = bots + ""],\"""" + botname + ""\"": ["";
                                }

                                bots = bots + ""{\""filename\"":\"""" + filename + ""\"", \""packnumber\"":\"""" + packnumber + ""\""}"";
                            }

                            previousBot = botname;
                        }
                        bots += ""]}}"";
                        jsonbots = bots.replace(/(\r\n|\n|\r)/gm, """");
                        try {
                            animeEpisodes = JSON.parse(jsonbots);
                            Materialize.toast('Episodes found for <br> ' + title , 4000);
                        } catch (e) {
                            animeEpisodes = JSON.parse(""{}"");
                            Materialize.toast('No episodes found for <br> ' + title + '<br> Try adding a synonym!', 4000);
                        }
                    }
                }


                var eplist = """";


                var tempBotSelected = """";
                var doesBotSelectedExist = false;
                $.each(animeEpisodes.bots, function (key, value) {
                    var output;
                    if (key == botselected) {
                        output = ""<option value="" + key + "" selected>"" + key + ""</option>"";
                        if (!doesBotSelectedExist) {
                            doesBotSelectedExist = true;
                        }
                    } else {
                        output = ""<option value="" + key + "">"" + key + ""</option>"";
                        tempBotSelected = key;
                    }
                    $(document).ready(function () {
                        $(""#botlist"").append(output);
                    });
                });

                if (!doesBotSelectedExist) {
                    botselected = tempBotSelected;
                }

                console.log(botselected);
                console.log(animeEpisodes.bots[botselected]);
                var jsonString = JSON.stringify(animeEpisodes.bots[botselected]);
                eplist = printDownloadListItem(jsonString);


                $(document).ready(function () {
                    if (loggedin) {
                        $(""#episodes"").html(eplist);
                    } else {
                        $(""#episodes"").html('<center><h4> You are not connected to an irc server! <br> Download disabled... ');
                    }
                    $('.collapsible').collapsible({
                        accordion: false // A setting that changes the collapsible behavior to expandable instead of the default accordion style
                    });
                    $('select').material_select();
                });
            }
        });
    }

    //when use selects another resolution
    function changeResolution() {
        resolution = document.getElementById('resolutionSelected').selectedOptions[0].value;
        console.log(resolution);
        var eplist = printDownloadListItem(JSON.stringify(animeEpisodes.bots[botselected]));
        $(""#episodes"").html(eplist);
    }

    //when user selects a different bot
    function onBotChange() {
        botselected = document.getElementById('botlist').selectedOptions[0].text;
        var eplist = printDownloadListItem(JSON.stringify(animeEpisodes.bots[botselected]));
        $(""#episodes"").html(eplist);
    }

    //prints xdcc file list
    function printDownloadListItem(jsonToParse) {
        var output = """";
        var jsonparsed = JSON.parse(jsonToParse);
        console.log(jsonparsed);
        $.each(jsonparsed, function (key, value) {
            if (value.filename.indexOf(resolution) > -1) {
                output = output + '  <li> \
                    <div class=""collapsible-header truncate"">\
                        <i class=""material-icons"">cloud</i>\
                        ' + value.filename + '\
                    </div>\
                    <div class=""collapsible-body center"">\
                        <p id=""' + value.packnumber + '"">\
                            <button  onclick=""sendircmessage(\'/msg ' + botselected + ' xdcc send #' + value.packnumber + '\'); replaceWithCircular(' + value.packnumber + ');"" class=""waves-effect waves-light btn grey""><i class=""material-icons right"">file_download</i>Download</a>\
                         </p>\
                    </div>\
                </li> </ul>';
            } else if (resolution == ""none"") {
                output = output + '  <li> \
                    <div class=""collapsible-header truncate"">\
                        <i class=""material-icons"">cloud</i>\
                        ' + value.filename + '\
                    </div>\
                    <div class=""collapsible-body center"">\
                        <p id=""' + value.packnumber + '"">\
                            <button onclick=""sendircmessage(\'/msg ' + botselected + ' xdcc send #' + value.packnumber + '\'); replaceWithCircular(\'' + value.packnumber + '\');"" class=""waves-effect waves-light btn grey""><i class=""material-icons right"">file_download</i>Download</a>\
                        </p>\
                    </div>\
                </li> </ul>';
            }
        });

        return output;
    }
    
    //print small circular loader
    function printCircular() {
        return '<div class=""preloader-wrapper small active"">\
                    <div class=""spinner-layer spinner-green-only"">\
                      <div class=""circle-clipper left"">\
                        <div class=""circle""></div>\
                      </div><div class=""gap-patch"">\
                        <div class=""circle""></div>\
                      </div><div class=""circle-clipper right"">\
                        <div class=""circle""></div>\
                      </div>\
                    </div>\
                  </div>';

        console.log(""REPLACING WITH CIRCULAR :D"");
    }

    //replace id element with circular
    function replaceWithCircular(id) {
        console.log(""REPLACING WITH CIRCULAR :D"");
        $(""#"" + id).html(printCircular());
    }

   
    </script>
</body>
</html>
                ".Replace("myIP", ip);
        }

    }
}
