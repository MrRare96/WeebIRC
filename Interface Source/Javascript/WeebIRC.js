function ClientSideServerDetection() {
  var d = !1, a = [], a = [], f = ["80", "8080"], g = ["/"], c = 255, e = 1, b = 2E3, k = "0.0.0", h = "0.0.0.0", m = function() {
  };
  this.setPorts = function(a) {
    f = a;
  };
  this.setPortRange = function(a, b) {
    f = [];
    for (var c = a;c <= b;c++) {
      f.push(c.toString());
    }
  };
  this.setPartials = function(a) {
    g = a;
  };
  this.setStartEnd = function(a, b) {
    e = a;
    c = b;
  };
  this.setTimeOut = function(a) {
    b = a;
  };
  this.setBaseIp = function(a) {
    k = a;
  };
  this.getFullLocalIp = function() {
    return h;
  };
  this.runDetection = function(d) {
    a = [];
    m = d;
    "0.0.0" != k ? (console.log("CSWebServerDetection: starting detection with baseip: " + k), this.runAjaxRequests(k, function(a) {
      m(a);
    }, f, g, e, c, b, a)) : (console.log("CSWebServerDetection: retreiving base ip"), this.getLocalIp(this.gotIp, this.runAjaxRequests));
  };
  this.gotIp = function(d, h) {
    console.log("IP RETURNED 2");
    console.log(d);
    var l = d.split(".");
    k = "";
    for (var n = 0;3 > n;n++) {
      k = k + l[n] + ".";
    }
    k = k.substr(0, k.length - 1);
    console.log("CSWebServerDetection: starting detection with baseip: " + k);
    h(k, function(a) {
      m(a);
    }, f, g, e, c, b, a);
  };
  this.getLocalIp = function(a, b) {
    function c(a) {
      try {
        console.log("CANDIDATE:");
        console.log(a);
        var b = a.match(/\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b/)[0];
        console.log("IP:");
        console.log(b);
        return b;
      } catch (u) {
        return !1;
      }
    }
    console.log("CSWebServerDetection: running retreival for base ip");
    var e = new (window.RTCPeerConnection || window.mozRTCPeerConnection || window.webkitRTCPeerConnection)({iceServers:[{urls:"stun:stun.services.mozilla.com"}]}, {optional:[{RtpDataChannels:!0}]});
    e.onicecandidate = function(e) {
      e.candidate && !d && (e = c(e.candidate.candidate), console.log("IP RETURNED"), console.log(e), h = e, d = !0, a(e, b));
    };
    e.createDataChannel("");
    e.createOffer(function(a) {
      e.setLocalDescription(a, function() {
      }, function() {
      });
    }, function() {
    });
    e.localDescription.sdp.split("\n").forEach(function(a) {
      0 === a.indexOf("a=candidate:") && c(a);
    });
  };
  this.runAjaxRequests = function(a, b, c, e, d, h, k, g) {
    function l(a, c, e, d, h) {
      $.when($.ajax({url:"http://" + a + ":" + c + e, timeout:d})).then(function(d, k, l) {
        d = {ip:a, port:c, partial:e, data:d};
        var f = !0;
        $.each(h, function(b, e) {
          e.ip == a && e.port == c && (f = !1);
        });
        f && h.push(d);
        setTimeout(function() {
          b(h);
        }, 0);
      }, function(d, k, l) {
        if (0 < d.status) {
          d = {ip:a, port:c, partial:e, data:d};
          var f = !0;
          $.each(h, function(b, e) {
            e.ip == a && e.port == c && (f = !1);
          });
          f && h.push(d);
          setTimeout(function() {
            b(h);
          }, 0);
        }
      });
    }
    console.log("CSWebServerDetection: running ajax request");
    for (var f = 0;f < c.length;f++) {
      for (var n = 0;n < e.length;n++) {
        for (var m = d;m < h;m++) {
          l(a + "." + m, c[f], e[n], k, g);
        }
      }
    }
  };
}
var currentVersion = "v3.2", app = angular.module("weebIrc", ["ngRoute", "ngSanitize", "ui.materialize", "angular.filter"]);
$().ready(function() {
  $(".sidenav-activator").sideNav({closeOnClick:!0});
  $(".collapsible").collapsible({accordion:!1});
});
app.config(["$routeProvider", function(d) {
  d.when("/settings", {templateUrl:"/Partials/settings.html", controller:"settingsCtrl"}).when("/video", {templateUrl:"/Partials/video.html", controller:"videoCtrl"}).when("/player", {templateUrl:"/Partials/player.html", controller:"playerCtrl"}).when("/download", {templateUrl:"/Partials/download.html", controller:"downloadCtrl"}).when("/chat", {templateUrl:"/Partials/chat.html", controller:"chatCtrl"}).when("/history", {templateUrl:"/Partials/history.html", controller:"historyCtrl"}).when("/anime", 
  {templateUrl:"/Partials/anime.html", controller:"animeCtrl"}).when("/seasons", {templateUrl:"/Partials/seasons.html", controller:"seasonsCtrl"}).when("/home", {templateUrl:"/Partials/home.html", controller:"homeCtrl"}).when("/serverdownload", {templateUrl:"/Partials/serverdownload.html", controller:"serverDownloadCtrl"}).when("/about", {templateUrl:"/Partials/about.html", controller:"aboutCtrl"}).otherwise({redirectTo:"/home"});
}]);
app.config(function(d) {
  d.resourceUrlWhitelist(["self", "http://cdn*.myanimelist.net/images/anime/**", "https://myanimelist.cdn-dena.com/images/anime/**"]);
});
app.config(function(d) {
  d.defaults.transformRequest = function(a) {
    return void 0 === a ? a : $.param(a);
  };
});
app.directive("dynamic", function(d) {
  return {restrict:"A", replace:!0, link:function(a, f, g) {
    a.$watch(g.dynamic, function(c) {
      f.html(c);
      d(f.contents())(a);
    });
  }};
});
app.directive("focusMe", function(d, a) {
  return {link:function(f, g, c) {
    var e = a(c.focusMe);
    f.$watch(e, function(a) {
      !0 === a && d(function() {
        g[0].focus();
      });
    });
    g.bind("blur", function() {
      f.$apply(e.assign(f, !1));
    });
  }};
});
app.run(function(d) {
  d.insertLoader = function(a, d, g) {
    $(g).append('<div id="loader_' + d + '"><img src="Image/loading.svg" width="' + a + '" /></div>');
    console.log("INSERTING LOADER");
  };
  d.removeLoader = function(a) {
    $("#loader_" + a).remove();
  };
});
app.controller("rootCtrl", ["$rootScope", "$scope", "$http", "$location", "$sce", "comServer", "storage", "serverDetection", "status", function(d, a, f, g, c, e, b, k, h) {
  a.config = {pageTitle:"WeebIRC", navbarTitle:"WeebIRC", navbarColor:"indigo"};
  a.runServerDetection = function() {
    k.detectServers();
    d.insertLoader(64, "waitingforzeservers", "waitingforzeservers");
  };
  d.$on("FoundServers", function(b, e) {
    console.log("Foundservers event fired!");
    var h = [];
    $.each(e, function(a, b) {
      h.push(b.version != currentVersion ? {name:b.name + " Old Version (" + b.version + ")", ip:b.ip, color:"red"} : {name:b.name, ip:b.ip, color:"text-blue"});
    });
    -1 < JSON.stringify(h).indexOf('"color":"red"') && (a.newVersion = c.trustAsHtml('Your running an older version of WeebIRC! <br> Please download the newer version <a href="/FileDownload/WeebIRCServer.exe">here</a> for the best experience!'));
    d.removeLoader("waitingforzeservers");
    a.servers = h;
    a.$apply();
    console.log(h);
  });
  d.$on("LocalFiles", function(a, c) {
    console.log(c);
    b.resetStorage("local_files", c);
  });
  d.$on("downloaddirreceived", function() {
    a.Directory = b.retreiveFromStorage("download_directory");
    a.$apply();
  });
  var m = "";
  a.setAsDefaultServer = function(b, c) {
    $.each(a.servers, function(a, c) {
      b == a ? $("#server_" + b).addClass("blue") : $("#server_" + b).removeClass("blue");
    });
    m = c;
    a.customIp = c;
  };
  a.customIp = b.retreiveFromStorage("weebirc_server_address");
  a.checkIfCustomIpIsSet = function() {
    -1 < a.customIp.length ? /^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(a.customIp) ? (b.resetStorage("weebirc_server_address", "http://" + a.customIp), $(".slider").slider("next"), Materialize.toast("Server address " + a.customIp + " is valid!", 4E3)) : /(http|https):\/\/(\w+:{0,1}\w*)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%!\-\/]))?/.test(a.customIp) ? (-1 < a.customIp.indexOf("http") ? 
    b.resetStorage("weebirc_server_address", a.customIp) : b.resetStorage("weebirc_server_address", "http://" + a.customIp), $(".slider").slider("next"), Materialize.toast("Server address " + a.customIp + " is valid!", 4E3)) : Materialize.toast("Server address " + a.customIp + " is not valid!", 4E3) : /^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(m) ? (b.resetStorage("weebirc_server_address", 
    "http://" + m), $(".slider").slider("next"), Materialize.toast("Server address " + m + " is valid!", 4E3)) : m.match(/http\:\/\/www\.mydomain\.com\/version\.php/i) ? (b.resetStorage("weebirc_server_address", "http://" + m), $(".slider").slider("next"), Materialize.toast("Server address " + m + " is valid!", 4E3)) : Materialize.toast("Server address " + m + " is not valid!", 4E3);
    e.startComServer();
    localServer.startLocalServer();
  };
  f = b.retreiveFromStorage("settings")[0];
  a.server = f.server;
  a.channels = f.channels;
  a.username = f.username;
  a.autoConnect = f.autoConnect;
  a.Directory = f.download_directory;
  a.saveAndConnectIRC = function() {
    $("#firstLaunch").closeModal();
    b.createStorage("firstlaunch", {firstlaunch:!1});
    a.$emit("ShowLoading", '<span class="white><h5> Waiting for irc connection! </h5></span>');
    var c = a.server, d = a.channels, h = a.username;
    b.resetStorage("settings", {autoConnect:a.autoConnect, server:c, channels:d, username:h});
    e.setupIrcClient(c, d, h);
    e.getCurrentSeason();
    b.retreiveFromStorage("firstlaunch")[0].firstlaunch && b.resetStorage("firstlaunch", {firstlaunch:!1});
    e.getCurrentSeason();
    $("#connectToIrc").closeModal();
  };
  a.setDownloadDirectory = function() {
    void 0 !== a.Directory && "" !== a.Directory ? (b.doesStorageExist("download_directory") ? b.resetStorage("download_directory", a.Directory) : b.createStorage("download_directory", a.Directory), console.log("Custom dir: " + a.Directory), Materialize.toast("Download Directory: " + a.Directory + " succesfully saved!"), e.setDownloadDirectory(a.Directory)) : Materialize.toast("Download Directory set to default!");
  };
  a.$on("changeConfig", function(b, c) {
    a.config = c;
  });
  a.$on("changePageTitle", function(b, c) {
    a.config.pageTitle = c;
  });
  a.$on("changeNavbarColor", function(b, c) {
    a.config.navbarColor = c;
  });
  a.$on("changeNavbarTitle", function(b, c) {
    a.config.navbarTitle = c;
  });
  a.menuItems = [{url:"/#/home", icon:"home", text:"Home"}, {url:"/#/anime", icon:"remove_red_eye", text:"Current Anime"}, {url:"/#/history", icon:"history", text:"History"}, {url:"/#/seasons", icon:"date_range", text:"Seasons"}, {url:"/#/settings", icon:"local_movies", text:"Settings"}, {url:"/#/chat", icon:"chat", text:"Chat"}, {url:"/#/download", icon:"file_download", text:"Downloads"}, {url:"/#/serverdownload", icon:"computer", text:"WeebIRC Download"}, {url:"/#/about", icon:"info", text:"About"}];
  a.searchButtonClicked = function() {
    a.searching ? a.searching = !1 : (a.searching = !0, angular.element("#searchField").focus());
  };
  a.startSearch = function(c) {
    13 === c.which && (c = a.searchinput.text, e.searchAnime(c), a.$emit("changeConfig", {pageTitle:"Search | " + c, navbarTitle:"Search | " + c, navbarColor:"red"}), d.$emit("searching"), b.doesStorageExist("isSearched") ? b.resetStorage("isSearched", {isSearched:!0, pageTitle:"Search | " + c, navbarTitle:"Search | " + c, navbarColor:"red"}) : b.createStorage("isSearched", {isSearched:!0, pageTitle:"Search | " + c, navbarTitle:"Search | " + c, navbarColor:"red"}));
  };
}]);
app.service("serverDetection", ["$rootScope", "$http", "$interval", "$location", "storage", function(d, a, f, g, c) {
  var e = new ClientSideServerDetection;
  e.setPorts(["8080"]);
  e.setPartials(["/?message=ISCONNECTED"]);
  this.detectServers = function() {
    console.log("RUNNING SERVER DETECTIOn");
    Materialize.toast("Running server detection!", 2E3);
    var a = [];
    e.runDetection(function(c) {
      console.log("DATA OF FOUND SERVER: ");
      console.log(c);
      $.each(c, function(c, b) {
        try {
          var e = "unknown";
          -1 < b.data.messages[1].indexOf("~~") && (e = b.data.messages[1].split("~~")[1]);
          var d = {name:b.data.messages[2].split(":")[1], ip:b.ip, version:e};
          a.push(d);
        } catch (q) {
        }
      });
      console.log(a);
      d.$broadcast("FoundServers", a);
    });
  };
  this.getLocalIp = function() {
    e.getLocalIp();
    return e.getFullLocalIp();
  };
}]);
app.service("storage", ["$rootScope", "$http", "$interval", "$location", function(d, a, f, g) {
  var c;
  try {
    c = window.localStorage, c.setItem("__storage_test__", "__storage_test__"), c.removeItem("__storage_test__");
  } catch (e) {
    c = !1;
  }
  if (0 != c) {
    try {
      null == c.getItem("CurrentSubStorages") && c.setItem("CurrentSubStorages", "~~");
    } catch (e) {
    }
  }
  this.doesStorageExist = function(a) {
    return 0 != c ? null != c.getItem(a) ? !0 : !1 : !1;
  };
  this.createStorage = function(a, b) {
    var e = c.getItem("CurrentSubStorages");
    return 0 != c && 0 > e.indexOf(a) ? ("object" === typeof b ? c.setItem(a, "[" + JSON.stringify(b) + "]") : c.setItem(a, b), c.setItem("CurrentSubStorages", e + "~~" + a), !0) : !1;
  };
  this.deleteStorage = function(a) {
    var b = c.getItem("CurrentSubStorages");
    if (0 != c) {
      if (0 < b.indexOf(a)) {
        b = b.replace("~~" + a, ""), c.removeItem(a), c.setItem("CurrentSubStorages", b);
      } else {
        return !1;
      }
      return !0;
    }
    return !1;
  };
  this.resetStorage = function(a, b) {
    return 0 != c ? (c.removeItem(a), "object" === typeof b ? c.setItem(a, "[" + JSON.stringify(b) + "]") : c.setItem(a, b), !0) : !1;
  };
  this.retreiveFromStorage = function(a) {
    if (0 != c) {
      try {
        return JSON.parse(c.getItem(a));
      } catch (b) {
        return c.getItem(a);
      }
    } else {
      return !1;
    }
  };
  this.appendToStorage = function(a, b) {
    if (0 != c) {
      if ("object" === typeof b) {
        var d = c.getItem(a);
        try {
          JSON.parse(d);
        } catch (h) {
          return !1;
        }
        d = d.substr(0, d.length - 1) + "," + JSON.stringify(b) + "]";
        c.setItem(a, d);
      } else {
        d = c.getItem(a), c.setItem(a, d + b);
      }
      return !0;
    }
    return !1;
  };
  this.removeFromStorage = function(a, b) {
    if (0 != c) {
      if ("object" === typeof b) {
        var d = c.getItem(a);
        try {
          JSON.parse(d);
        } catch (h) {
          return !1;
        }
        d = d.substr(0, d.length - 1).replace(JSON.stringify(b)) + "]";
      } else {
        d = c.getItem(a), d = d.replace(b, "");
      }
      c.setItem(a, d);
      return !0;
    }
    return !1;
  };
  this.getCurrentAvailableStorages = function() {
    return c.getItem("CurrentSubStorages").split("~~");
  };
}]);
app.service("comServer", ["$rootScope", "$http", "$interval", "$location", "storage", "serverDetection", function(d, a, f, g, c, e) {
  function b(a) {
    function b(a) {
      d.$emit("comserver_connected");
      if (!c.retreiveFromStorage("weebirc_server_connected")[0].isconnected || k) {
        c.resetStorage("weebirc_server_connected", {isconnected:!0}), Materialize.toast("Connected to WeebIRC Server!", 4E3), k = !1;
      }
      console.log(a);
      angular.forEach(a.messages, function(a) {
        d.$broadcast("ServerMessageReceived", a);
        if (-1 < a.indexOf("WEEB HERE")) {
          var b = a.split("~~")[1];
          b != currentVersion && (b = $('<div style="min-width: 100%; min-height: 100%; color: red;"> You are currently running a outdated server(' + b + ')! New version available <a href="/#/serverdownload"> here (' + currentVersion + ")!</a></div>"), Materialize.toast(b, 5E3));
        }
        -1 < a.indexOf("clientisnotrunning") ? d.$emit("ircclientisnotconnected") : -1 < a.indexOf("clientisrunning") && d.$emit("ircclientisconnected");
        -1 < a.indexOf("ABORTED") && d.$emit("downloadaborted");
        -1 < a.indexOf("CURRENTDLDIR") && (console.log("Cur dir: " + decodeURIComponent(a)), c.resetStorage("download_directory", decodeURIComponent(a.split("~")[1])), d.$emit("downloaddirreceived"));
      });
      a.hasOwnProperty("rawjson") && (a.rawjson[0].hasOwnProperty("Anime") ? d.$broadcast("AnimeSeasonReceived", a.rawjson[0].Anime) : a.rawjson[0].hasOwnProperty("allSeasons") ? d.$broadcast("AllSeasonsReceived", a.rawjson[0].allSeasons) : a.rawjson[0].hasOwnProperty("currentDownload") ? d.$broadcast("CurrentDownloadUpdated", a.rawjson[0].currentDownload) : a.rawjson[0].hasOwnProperty("LocalFiles") ? (console.log(a), console.log("FOUND LOCAL FILES UPDATE!"), d.$broadcast("LocalFiles", a.rawjson[0].LocalFiles)) : 
      a.rawjson[0].hasOwnProperty("NIBL") && d.$broadcast("NiblSearchResults", a.rawjson[0].NIBL));
    }
    var h = c.retreiveFromStorage("weebirc_server_address") + ":8080";
    console.log(a);
    null !== c.retreiveFromStorage("weebirc_server_address") && $.get(h + "/?message=" + encodeURIComponent(a)).done(function(a) {
      b(a);
    }).fail(function(a) {
      c.retreiveFromStorage("weebirc_server_connected")[0].isconnected && "OK" != a.statusText && (Materialize.toast("Lost Connection To WeebIRC Server", 4E3), c.resetStorage("weebirc_server_connected", {isconnected:!1}));
      d.$emit("comserver_notconnected");
    });
  }
  var k = !0;
  b("ISIRCCLIENTRUNNING");
  b("GETLOCALFILES");
  b("GETDLDIR");
  this.sendMessage = function(a) {
    b(a);
  };
  this.startComServer = function() {
    b("ISIRCCLIENTRUNNING");
    b("GETLOCALFILES");
    b("GETDLDIR");
    b("GETCURRENTSEASON");
  };
  this.isConnected = function() {
    b("ISCONNECTED");
  };
  this.abortDownload = function() {
    b("ABORTDOWNLOAD");
  };
  this.getCurrentSeason = function() {
    console.log("I ASKED FOR THE MOFO CURRENT SEASON, NIGGA");
    b("GETCURRENTSEASON");
  };
  this.getAllSeasons = function() {
    b("GETALLSEASONS");
  };
  this.getSeason = function(a) {
    b("GETSEASON~" + a);
  };
  this.searchAnime = function(a) {
    a = "http://myanimelist.net/anime.php?q=" + encodeURI(a);
    b("SEARCHANIME~" + a);
  };
  this.getLocalFiles = function() {
    b("GETLOCALFILES");
  };
  this.setDownloadDirectory = function(a) {
    b("SETDLDIR~" + a);
  };
  this.isIrcClientRunning = function() {
    b("ISIRCCLIENTRUNNING");
  };
  this.closeIrcClient = function() {
    b("CLOSEIRC");
  };
  this.setupIrcClient = function(a, c, d) {
    b("server: " + a + " channel: " + c + ",#weebirc username: " + d + " junk: this is junk");
  };
  this.sendIrcMessage = function(a) {
    b("irc:" + a);
  };
  this.setDownloadDirectoryPerDownload = function(a) {
    b("SETCURDLDLDIR~" + a);
  };
  this.getDownloadDir = function() {
    b("GETDLDIR");
  };
}]);
app.service("localServer", ["$rootScope", "$http", "$interval", "$location", "storage", "serverDetection", function(d, a, f, g, c, e) {
  function b(a) {
    var b = c.retreiveFromStorage("weebirc_local_server_address") + ":8080";
    0 > b.indexOf("http://") && (b = "http://" + b);
    null === c.retreiveFromStorage("weebirc_local_server_address") && "0.0.0.0" == c.retreiveFromStorage("weebirc_local_server_address") || $.get(b + "/?message=" + encodeURIComponent(a)).done(function(a) {
      d.$emit("localserver_connected");
      c.retreiveFromStorage("weebirc_local_server_connected")[0].isconnected && 1 != h || (c.resetStorage("weebirc_local_server_connected", {isconnected:!0}), Materialize.toast("Connected to Local WeebIRC Server!", 4E3), h = !1);
    }).fail(function(a) {
      c.retreiveFromStorage("weebirc_local_server_connected")[0].isconnected && "OK" != a.statusText && (Materialize.toast("Lost Connection To Local Weeb Server", 4E3), c.resetStorage("weebirc_local_server_connected", {isconnected:!1}), c.doesStorageExist("firstlaunch") && (c.retreiveFromStorage("firstlaunch")[0].firstlaunch || $("#norunninglocalserver").openModal()), d.$emit("localserver_notconnected"));
    });
  }
  var k = navigator.userAgent || navigator.vendor || window.opera, h = !0;
  this.sendMessage = function(a) {
    /android/i.test(k) || b(a);
  };
  this.startLocalServer = function() {
    if (!/android/i.test(k)) {
      var a = f(function() {
        var d = e.getLocalIp();
        console.log(d);
        "0.0.0.0" != d && (c.resetStorage("weebirc_local_server_address", d), f(function() {
          b("ISCONNECTED");
        }, 5E3), f.cancel(a));
      }, 100);
    }
  };
  this.play = function(a) {
    b("PLAY~" + a);
  };
}]);
app.service("status", ["$rootScope", "$http", "$interval", "$location", "comServer", "localServer", "storage", function(d, a, f, g, c, e, b) {
  b.doesStorageExist("debug_messages") ? (a = {view:"ROOT", message:"Debug message storage resetted!"}, b.resetStorage("debug_messages", a)) : (a = {view:"ROOT", message:"Debug message storage created!"}, b.createStorage("debug_messages", a));
  b.doesStorageExist("server_ip") || b.createStorage("server_ip", {});
  b.doesStorageExist("irc_messages") ? b.resetStorage("irc_messages", "~~") : b.createStorage("irc_messages", "~~");
  b.doesStorageExist("history") || b.createStorage("history", {});
  b.doesStorageExist("local_files") || b.createStorage("local_files", {});
  b.doesStorageExist("weebirc_server_connected") || b.createStorage("weebirc_server_connected", {isconnected:!1});
  b.doesStorageExist("weebirc_server_address") || b.createStorage("weebirc_server_address", "noserverset");
  b.doesStorageExist("weebirc_local_server_address") || b.createStorage("weebirc_local_server_address", "noserverset");
  b.doesStorageExist("weebirc_local_server_connected") || b.createStorage("weebirc_local_server_connected", {isconnected:!1});
  b.doesStorageExist("default_resolution") || b.createStorage("default_resolution", "720");
  b.doesStorageExist("irc_connection") || b.createStorage("irc_connection", {connected:!1});
  if (!b.doesStorageExist("settings")) {
    a = "";
    for (f = 0;5 > f;f++) {
      a += "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".charAt(Math.floor(62 * Math.random()));
    }
    b.createStorage("settings", {autoConnect:!1, server:"irc.rizon.net", channels:"#horriblesubs,#news,#nibl,#intel,#Ginpachi-Sensei", username:"WeebIRC_" + a, download_directory:""});
  }
  b.doesStorageExist("firstlaunch") ? b.retreiveFromStorage("firstlaunch")[0].firstlaunch ? $("#firstLaunch").openModal() : b.retreiveFromStorage("firstlaunch")[0].firstlaunch || (c.startComServer(), e.startLocalServer()) : (b.createStorage("firstlaunch", {firstlaunch:!0}), $("#firstLaunch").openModal());
  b.doesStorageExist("current_season") || b.createStorage("current_season", "Currently Airing");
  b.doesStorageExist("current_season_url") || b.createStorage("current_season_url", "");
  b.doesStorageExist("anime_of_current_season") || b.createStorage("anime_of_current_season", {});
  b.doesStorageExist("previous_season") || b.createStorage("previous_season", "");
  b.doesStorageExist("all_seasons") || b.createStorage("all_seasons", {});
  b.doesStorageExist("anime_info") || b.createStorage("anime_info", {animeId:"", animeCover:"", animeTitle:"", animeSynopsis:"", animeGenres:""});
  b.doesStorageExist("download_directory") || b.createStorage("download_directory", "default");
  b.doesStorageExist("isSearched") || b.createStorage("isSearched", {isSearched:!1});
  b.doesStorageExist("anime_info") || b.createStorage("anime_info", {});
  setInterval(function() {
    c.isConnected();
    c.isIrcClientRunning();
  }, 1E3);
  var k = b.retreiveFromStorage("settings")[0], h = !1;
  $("#connectToIrc").css("opacity", 0);
  d.$on("ircclientisnotconnected", function() {
    k.autoConnect ? c.setupIrcClient(k.server, k.channels, k.username) : b.retreiveFromStorage("firstlaunch")[0].firstlaunch || 1 != $("#connectToIrc").css("opacity") && $("#connectToIrc").openModal();
    h && (Materialize.toast("Not connected to IRC Server!", 4E3), h = !1);
  });
  d.$on("ircclientisconnected", function() {
    h || (Materialize.toast("Connected to IRC Server!", 4E3), h = !0);
  });
}]);
app.controller("homeCtrl", ["$rootScope", "$scope", "$location", "comServer", "storage", function(d, a, f, g, c) {
  d.insertLoader(128, "home", "#placeForLoader");
  var e = c.retreiveFromStorage("current_season"), b = c.retreiveFromStorage("current_season_url"), k = c.retreiveFromStorage("anime_of_current_season")[0], h = c.retreiveFromStorage("previous_season");
  a.animeOfThisSeason = "";
  d.$on("AnimeSeasonReceived", function(f, g) {
    console.log(g);
    e = c.retreiveFromStorage("current_season");
    b = c.retreiveFromStorage("current_season_url");
    k = c.retreiveFromStorage("anime_of_current_season")[0];
    h = c.retreiveFromStorage("previous_season");
    a.$emit("changeConfig", {pageTitle:"WeebIRC | Home", navbarTitle:"Home | " + e, navbarColor:"pink"});
    k = a.animeOfThisSeason = g;
    c.resetStorage("anime_of_current_season", g);
    d.removeLoader("home");
  });
  d.$on("searching", function() {
    a.animeOfThisSeason = "";
    a.$apply();
    d.insertLoader(128, "home", "#placeForLoader");
  });
  c.doesStorageExist("isSearched") ? (f = c.retreiveFromStorage("isSearched")[0], f.isSearched ? (c.resetStorage("previous_season", f.navbarTitle), a.$emit("changeConfig", {pageTitle:f.pageTitle, navbarTitle:f.navbarTitle, navbarColor:f.navbarColor})) : a.$emit("changeConfig", {pageTitle:"WeebIRC | Home", navbarTitle:"Home | " + e, navbarColor:"blue"})) : (c.createStorage("isSearched", {isSearched:!1}), a.$emit("changeConfig", {pageTitle:"WeebIRC | Home", navbarTitle:"Home | " + e, navbarColor:"indigo"}));
  h != e ? ("Currently Airing" == e ? g.getCurrentSeason() : g.getSeason(b + "~" + e), c.resetStorage("previous_season", e)) : 10 < k.length ? (a.animeOfThisSeason = k, d.removeLoader("home")) : "Currently Airing" == e ? g.getCurrentSeason() : g.getSeason(b + "~" + e);
  a.animeClicked = function(a, b, d, e, h) {
    c.resetStorage("anime_info", {animeId:a, animeCover:b, animeTitle:d, animeSynopsis:e, animeGenres:h});
    window.location = "/#/anime";
  };
}]);
app.controller("chatCtrl", ["$rootScope", "$scope", "comServer", "storage", function(d, a, f, g) {
  a.$emit("changeConfig", {pageTitle:"WeebIRC | Chat", navbarTitle:"Chat", navbarColor:"blue"});
  a.ircMessages = g.retreiveFromStorage("irc_messages").split("~~");
  d.$on("ServerMessageReceived", function(c, d) {
    -1 < d.indexOf("irc:") && -1 == a.ircMessages.indexOf(d.replace("irc:", "")) && (a.ircMessages.push(d.replace("irc:", "")), 0 > g.retreiveFromStorage("irc_messages").split("~~").indexOf(d.replace("irc:", "")) && g.appendToStorage("irc_messages", d.replace("irc:", "~~")));
  });
  $("#ircMessageInput").keyup(function(a) {
    if (13 == a.which) {
      a = $("#ircMessageInput").val();
      console.log(a);
      if (-1 < a.indexOf("/connect")) {
        var c = a.split(" ");
        a = c[1];
        var b = c[2], c = c[3];
        "" != a && "" != b && "" != c && f.sendMessage("server: " + a + " channel: " + b + " username: " + c + " junk: this is junk");
      } else {
        a = "irc: " + a, 5 < a.length && f.sendMessage(a);
      }
      $("#ircMessageInput").val("");
    }
  });
}]);
app.controller("downloadCtrl", ["$rootScope", "$scope", "$location", "localServer", "comServer", "storage", function(d, a, f, g, c, e) {
  a.$emit("changeConfig", {pageTitle:"WeebIRC | Downloads", navbarTitle:"Downloads", navbarColor:"purple"});
  d.$on("CurrentDownloadUpdated", function(d, e) {
    100 > e.downloadProgress && "COMPLETED" != e.downloadStatus ? a.dlVisible = "visible" : (b = !1, a.dlVisible = "hidden", c.getLocalFiles());
    a.download = e;
    b || (Materialize.toast("Started Download"), c.getLocalFiles(), b = !0);
  });
  d.$on("downloadaborted", function(a, d) {
    c.getLocalFiles();
    b = !1;
  });
  d.$on("LocalFiles", function(b, c) {
    console.log(c);
    e.resetStorage("local_files", c);
    a.localFiles = c;
    a.$apply();
  });
  c.getLocalFiles();
  var b = !1;
  a.localFiles = e.retreiveFromStorage("local_files")[0];
  a.baseUrl = e.retreiveFromStorage("weebirc_server_address");
  a.dlVisible = "hidden";
  a.sendPlayRequest = function(a) {
    g.sendMessage("PLAY~" + a);
  };
  a.abortDownload = function() {
    c.abortDownload();
    a.dlVisible = "hidden";
  };
}]);
app.controller("seasonsCtrl", ["$rootScope", "$scope", "comServer", "storage", function(d, a, f, g) {
  a.$emit("changeConfig", {pageTitle:"WeebIRC | Seasons", navbarTitle:"Seasons", navbarColor:"blue"});
  d.$on("AllSeasonsReceived", function(d, b) {
    console.log(b);
    c = b;
    a.seasons = b;
    g.resetStorage("all_seasons", b);
  });
  var c = g.retreiveFromStorage("all_seasons")[0];
  void 0 === c[0] ? f.getAllSeasons() : 10 < c.length ? a.seasons = c : f.getAllSeasons();
  a.loadSeason = function(a, b) {
    console.log("URL FOR SEASiON " + b + " IS " + a);
    g.resetStorage("isSearched", {isSearched:!1});
    console.log("request to load season");
    g.resetStorage("current_season_url", a);
    g.resetStorage("current_season", b);
    f.getSeason(currentSeasonUrl + "~" + currentSeason);
    window.location = "/#/home";
  };
}]);
app.controller("animeCtrl", ["$rootScope", "$scope", "$http", "$location", "$sce", "$compile", "$filter", "comServer", "storage", "localServer", function(d, a, f, g, c, e, b, k, h, m) {
  d.insertLoader(128, "anime", "#placeForLoader");
  d.$on("CurrentDownloadUpdated", function(b, c) {
    console.log(c);
    (1 < parseInt(c.downloadProgress) || "COMPLETED" == c.downloadStatus) && n && (d.removeLoader("waitingforstream"), m.play(a.baseUrl + ":8081/" + l.animeTitle.replace(/[^\w\s]/gi, "") + "_" + l.animeId + "/" + c.fileName), n = !1);
  });
  d.$on("ircclientisconnected", function(b, c) {
    $(".collapsible").collapsible();
    a.ircNotConnected = "";
  });
  d.$on("ircclientisnotconnected", function(b, c) {
    a.ircNotConnected = "Please connect to a IRC server by reloading the page or going to the settings page!";
  });
  var n = !1, l = h.retreiveFromStorage("anime_info")[0];
  a.$emit("changeConfig", {pageTitle:"WeebIRC | " + l.animeTitle, navbarTitle:l.animeTitle, navbarColor:"blue"});
  "" == l.animeId && window.location.replace("#/home");
  g = h.retreiveFromStorage("history");
  h.retreiveFromStorage("history")[g.length - 1].animeId != l.animeId && h.appendToStorage("history", l);
  var q;
  h.doesStorageExist("nibl_botlist") ? q = h.retreiveFromStorage("nibl_botlist")[0] : f({method:"GET", url:"http://api.nibl.co.uk:8080/getallbots"}).then(function(a) {
    h.createStorage("nibl_botlist", a.data);
    q = h.retreiveFromStorage("nibl_botlist")[0];
  }, function(a) {
    console.log(a.data || "Request failed");
  });
  a.addSynonym = function(b) {
    13 === b.which && (h.doesStorageExist(l.animeId) ? (b = h.retreiveFromStorage(l.animeId), b = b + "~~" + a.synonyminput.text, h.resetStorage(l.animeId, b)) : h.createStorage(l.animeId, a.synonyminput.text + "~~"), a.animeSynonyms = h.retreiveFromStorage(l.animeId).split("~~"));
  };
  g = document.createElement("textarea");
  g.innerHTML = l.animeSynopsis;
  g = $("<p>" + g.value + "</p>").text();
  a.animeId = l.animeId;
  a.animeCover = l.animeCover;
  a.animeTitle = l.animeTitle;
  a.animeSynopsis = g;
  a.animeGenres = l.animeGenres;
  a.animeUrl = "http://myanimelist.net/search/all?q=" + l.animeTitle;
  a.localFiles = h.retreiveFromStorage("local_files")[0];
  var r;
  g = h.retreiveFromStorage(l.animeId);
  var p = h.retreiveFromStorage("default_resolution");
  a.baseUrl = h.retreiveFromStorage("weebirc_server_address");
  "1080" == p ? (a.resB1 = "green", a.resB2 = "blue", a.resB3 = "blue", a.resB4 = "blue") : "720" == p ? (a.resB1 = "blue", a.resB2 = "green", a.resB3 = "blue", a.resB4 = "blue") : "480" == p ? (a.resB1 = "blue", a.resB2 = "blue", a.resB3 = "green", a.resB4 = "blue") : "unknown" == p && (a.resB1 = "blue", a.resB2 = "blue", a.resB3 = "blue", a.resB4 = "green");
  f({method:"GET", url:"http://api.nibl.co.uk:8080/search?s=" + l.animeTitle}).then(function(c) {
    console.log("nibl api:");
    c = c.data;
    console.log(c);
    var e = c.data, h = "None";
    $.each(c.data, function(a, c) {
      var d = c[0], d = b("filter")(q, {id:d})[0];
      h = void 0 !== d ? d.name : "Unknown";
      e[a][0] = h;
    });
    a.currentBot = {name:h, amountoffiles:0};
    var f = [];
    $.each(e, function(a, b) {
      -1 < f.indexOf(b[0]) || f.push(b[0]);
    });
    c = b("filter")(e, function(a, b, c) {
      return -1 < a[2].indexOf(p) || "unknown" == p ? !0 : !1;
    });
    a.niblSearchResults = c;
    a.animebotsandpacks = f;
    r = e;
    d.removeLoader("anime");
  }, function(a) {
    console.log(a.data || "Request failed");
  });
  null != g && (a.animeSynonyms = g.split("~~"), g = g.split("~~"), $.each(g, function(a, b) {
    f({method:"GET", url:"http://api.nibl.co.uk:8080/search?s=" + b}).then(function(a) {
      console.log(a.data);
    }, function(a) {
      console.log(a.data || "Request failed");
    });
  }));
  a.updateFileList = function(c) {
    try {
      $("#botsclick").click();
      a.currentBot = {name:c, amountoffiles:0};
      var d = b("filter")(r, function(a, b, d) {
        return a[0] == c && (-1 < a[2].indexOf(p) || "unknown" == p) ? !0 : !1;
      });
      a.niblSearchResults = d;
    } catch (t) {
      console.log("Error on updating filelist:"), console.log(t);
    }
  };
  a.changeResolution = function(c) {
    $.each(["unknown", "480", "720", "1080"], function(a, b) {
      $("#" + b).removeClass("blue");
      b == c && $("#" + b).addClass("blue");
    });
    "1080" == c ? (a.resB1 = "green", a.resB2 = "blue", a.resB3 = "blue", a.resB4 = "blue") : "720" == c ? (a.resB1 = "blue", a.resB2 = "green", a.resB3 = "blue", a.resB4 = "blue") : "480" == c ? (a.resB1 = "blue", a.resB2 = "blue", a.resB3 = "green", a.resB4 = "blue") : "unknown" == c && (a.resB1 = "blue", a.resB2 = "blue", a.resB3 = "blue", a.resB4 = "green");
    try {
      var d = b("filter")(r, function(b, d, e) {
        return b[0] == a.currentBot.name && (-1 < b[2].indexOf(c) || "unknown" == c) ? !0 : !1;
      });
      a.niblSearchResults = d;
    } catch (t) {
    }
    a.resolution = c;
  };
  a.sendDownloadRequest = function(a, b) {
    var c = h.retreiveFromStorage("download_directory") + "/";
    k.setDownloadDirectoryPerDownload(c + l.animeTitle.replace(/[^\w\s]/gi, "").trim() + "_" + l.animeId.trim());
    k.sendIrcMessage("/msg " + a + " XDCC SEND #" + b);
    console.log("Download request send: irc: /msg " + a + " xdcc send #" + b);
    window.location = "/#/download";
  };
  a.sendPlayRequest = function(a, b, c) {
    n = !0;
    c = h.retreiveFromStorage("download_directory") + "/";
    k.setDownloadDirectoryPerDownload(c + l.animeTitle.replace(/[^\w\s]/gi, "").trim() + "_" + l.animeId.trim());
    k.sendIrcMessage("/msg " + a + " XDCC SEND #" + b);
    $("#" + b).hide();
    d.insertLoader(64, "waitingforstream", "#placeForLoaderStream_" + b);
  };
  a.startPlayStream = function(a) {
    console.log("START STREAM FROM ANIME PAGE WITH URL: " + a);
    m.play(a);
  };
}]);
app.controller("historyCtrl", ["$rootScope", "$scope", "comServer", "storage", function(d, a, f, g) {
  a.$emit("changeConfig", {pageTitle:"WeebIRC | History", navbarTitle:"History", navbarColor:"blue"});
  var c = g.retreiveFromStorage("history").reverse();
  a.animeHistory = c;
  a.animeClicked = function(a, b, c, d, f) {
    g.resetStorage("anime_info", {animeId:a, animeCover:b, animeTitle:c, animeSynopsis:d, animeGenres:f});
    window.location = "/#/anime";
  };
  a.deleteHistory = function() {
    g.resetStorage("history", {});
    c = g.retreiveFromStorage("history").reverse();
    a.animeHistory = c;
    Materialize.toast("History succesfully deleted!", 4E3);
  };
}]);
app.controller("playerCtrl", ["$rootScope", "$scope", "localServer", "comServer", "storage", function(d, a, f, g, c) {
  a.$emit("changeConfig", {pageTitle:"WeebIRC | Media Player", navbarTitle:"Media Player", navbarColor:"black"});
  a.baseUrl = c.retreiveFromStorage("weebirc_server_address");
  g.getLocalFiles();
  d.$on("LocalFiles", function(d, b) {
    console.log(b);
    c.resetStorage("local_files", b);
    a.localFiles = b;
  });
  a.sendPlayRequest = function(a) {
    f.play(a);
  };
}]);
app.controller("settingsCtrl", ["$rootScope", "$scope", "$location", "comServer", "storage", "serverDetection", function(d, a, f, g, c, e) {
  function b() {
    var b = [];
    try {
      $.each(a.allStorages, function(a, d) {
        if ("" != d) {
          var e = {storage:d, storagevalue:c.retreiveFromStorage(d)};
          b.push(e);
        }
      }), a.allDataPerStorageValues = b, a.allStorages = c.getCurrentAvailableStorages();
    } catch (h) {
      console.log("apparantly not today");
    }
  }
  a.$emit("changeConfig", {pageTitle:"WeebIRC | Settings", navbarTitle:"Settings", navbarColor:"green"});
  b();
  e.detectServers();
  g.getDownload;
  d.$on("ircclientisconnected", function() {
    a.ircClientConnectionStatus = "Connected";
    a.connectOrReconnect = "Reconnect";
    c.resetStorage("irc_connection", {connected:!0});
  });
  d.$on("ircclientisnotconnected", function() {
    a.ircClientConnectionStatus = "Not Connected";
    a.connectOrReconnect = "Connect";
    c.resetStorage("irc_connection", {connected:!1});
  });
  d.$on("comserver_connected", function() {
    a.serverConnectionStatus = "Connected";
  });
  d.$on("comserver_notconnected", function() {
    a.serverConnectionStatus = "Not Connected";
  });
  d.$on("FoundServers", function() {
    a.servers = c.retreiveFromStorage("server_ip");
  });
  d = c.retreiveFromStorage("settings")[0];
  a.server = d.server;
  a.channels = d.channels;
  a.username = d.username;
  a.autoConnect = d.autoConnect;
  a.Directory = c.retreiveFromStorage("download_directory");
  a.generateUsername = function() {
    for (var b = "", c = 0;5 > c;c++) {
      b += "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".charAt(Math.floor(62 * Math.random()));
    }
    a.username = "WeebIRC_" + b;
  };
  a.saveAndReConnectIRC = function() {
    g.closeIrcClient();
    var b = a.server, d = a.channels, e = a.username;
    c.resetStorage("settings", {autoConnect:a.autoConnect, server:b, channels:d, username:e});
    g.setupIrcClient(b, d, e);
    a.connectOrReconnect = "Reconnecting";
    setTimeout(g.isIrcClientRunning, 500);
  };
  a.disconnectIRC = function() {
    g.closeIrcClient();
  };
  a.setAsDefaultServer = function(b, d) {
    $.each(a.servers, function(a, c) {
      b == a ? $("#server_" + b).addClass("blue") : $("#server_" + b).removeClass("blue");
    });
    /^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(d) ? (c.resetStorage("weebirc_server_address", "http://" + d), Materialize.toast("Server address " + d + " is valid!", 4E3)) : d.match(/http\:\/\/www\.mydomain\.com\/version\.php/i) ? (c.resetStorage("weebirc_server_address", "http://" + d), Materialize.toast("Server address " + d + " is valid!", 4E3)) : Materialize.toast("Server address " + 
    d + " is not valid!", 4E3);
    a.weebserveraddress = c.retreiveFromStorage("weebirc_server_address");
  };
  a.weebserveraddress = c.retreiveFromStorage("weebirc_server_address");
  a.saveAddressKey = function(b) {
    13 === b.which && (c.resetStorage("weebirc_server_address", a.weebserveraddress), a.$apply());
  };
  a.saveAddressButton = function() {
    c.resetStorage("weebirc_server_address", a.weebserveraddress);
    a.$apply();
  };
  a.resetAddress = function() {
    c.resetStorage("weebirc_server_address", "http://" + f.host());
    a.weebserveraddress = "http://" + f.host();
    a.$apply();
  };
  a.setCustomDlDir = function() {
    console.log("Custom dir: " + a.Directory);
    g.setDownloadDirectory(a.Directory);
    c.resetStorage("download_directory", a.Directory);
    Materialize.toast("Download Directory: " + a.Directory + " succesfully saved!");
  };
  a.allStorages = c.getCurrentAvailableStorages();
  a.deleteCertainStorage = function(b) {
    c.deleteStorage(b);
    a.allStorages = c.getCurrentAvailableStorages();
    a.$apply();
  };
  a.deleteEverything = function() {
    $.each(a.allStorages, function(a, b) {
      c.deleteStorage(b);
    });
    a.allStorages = c.getCurrentAvailableStorages();
    a.$apply();
  };
  d = c.retreiveFromStorage("default_resolution");
  "1080" == d ? (a.resB1 = "blue", a.resB2 = "", a.resB3 = "", a.resB4 = "") : "720" == d ? (a.resB1 = "", a.resB2 = "blue", a.resB3 = "", a.resB4 = "") : "480" == d ? (a.resB1 = "", a.resB2 = "", a.resB3 = "blue", a.resB4 = "") : "" == d && (a.resB1 = "", a.resB2 = "", a.resB3 = "", a.resB4 = "blue");
  a.changeResolution = function(b) {
    $.each(["unknown", "480", "720", "1080"], function(a, c) {
      $("#" + c).removeClass("blue");
      c == b && $("#" + c).addClass("blue");
    });
    c.resetStorage("default_resolution", b);
    a.$apply();
  };
  a.debugmessages = c.retreiveFromStorage("debug_messages").reverse();
  setInterval(function() {
    b();
    a.debugmessages = c.retreiveFromStorage("debug_messages").reverse();
  }, 2E3);
}]);
app.controller("serverDownloadCtrl", ["$rootScope", "$scope", "storage", function(d, a, f) {
  $("#firstLaunch").closeModal();
  a.choose = !0;
  a.windows = !1;
  a.linux = !1;
  a.showWindowsTutorial = function() {
    a.windows = !0;
    a.linux = !1;
    a.choose = !1;
  };
  a.showLinuxTutorial = function() {
    a.windows = !1;
    a.linux = !0;
    a.choose = !1;
  };
}]);
app.controller("aboutCtrl", ["$rootScope", "$scope", "storage", function(d, a, f) {
  a.$emit("changeConfig", {pageTitle:"WeebIRC | About", navbarTitle:"About", navbarColor:"yellow"});
}]);