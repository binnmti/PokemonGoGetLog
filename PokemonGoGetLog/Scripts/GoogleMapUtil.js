/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="../typings/google.maps.d.ts" />
"use strict";
//ToDo これはモジュールが良い
var GoogleMapUtil = (function () {
    function GoogleMapUtil(canvasId, searchTextId, onClickMap) {
        GoogleMapUtil.canvasId = canvasId;
        GoogleMapUtil.searchTextId = searchTextId;
        GoogleMapUtil.geocoder = new google.maps.Geocoder();
        GoogleMapUtil.onClickMap = onClickMap;
        setStartPosition(new google.maps.LatLng(GoogleMapUtil.firstPositionX, GoogleMapUtil.firstPositionY));
        setAutocomplete();
        setGeolocation();
        function setGeolocation() {
            if (navigator.geolocation) {
                //ToDo フェードインアウトがうまくいっていない
                $("#loading").fadeIn();
                $("#container").fadeOut();
                navigator.geolocation.getCurrentPosition(
                // 成功処理
                function (info) {
                    var center = new google.maps.LatLng(info.coords.latitude, info.coords.longitude);
                    GoogleMapUtil.map.panTo(center);
                    $("#loading").fadeOut();
                    $("#container").fadeIn();
                }, 
                // エラー処理
                function (info) {
                    var latlng = getCookieLatlng();
                    GoogleMapUtil.map.panTo(latlng);
                    console.log('現在地取得エラー: ' + info.code);
                    $("#loading").fadeOut();
                    $("#container").fadeIn();
                    return;
                });
            }
            else {
                //クッキーから現在地を取る。
                var latlng = getCookieLatlng();
                GoogleMapUtil.map.panTo(latlng);
                console.log('本ブラウザではGeolocationが使えません');
            }
        }
        function setAutocomplete() {
            var input = document.getElementById(GoogleMapUtil.searchTextId);
            var options = { componentRestrictions: { country: "jp" } };
            var autocomplete = new google.maps.places.Autocomplete(input, options);
            //Autocompleteで位置が変更された
            google.maps.event.addListener(autocomplete, "place_changed", function () {
                var place = autocomplete.getPlace();
                GoogleMapUtil.map.setCenter(place.geometry.location);
                setMarker(place.geometry.location);
            });
        }
        function getCookieLatlng() {
            //ToDo クッキーから現在地を取る。
            var cookieX = 0;
            var cookieY = 0;
            var center = new google.maps.LatLng(GoogleMapUtil.firstPositionX, GoogleMapUtil.firstPositionY);
            return center;
        }
        function setStartPosition(latlng) {
            var mapOptions = {
                zoom: 15,
                center: latlng // 中心座標 [latlng]
            };
            var canvas = document.getElementById(GoogleMapUtil.canvasId);
            GoogleMapUtil.map = new google.maps.Map(canvas, mapOptions);
            google.maps.event.addListener(GoogleMapUtil.map, "click", setMarkerEvent);
        }
        function setMarkerEvent(event) {
            var ll = new google.maps.LatLng(event.latLng.lat(), event.latLng.lng());
            setMarker(ll);
        }
        function setMarker(ll) {
            if (GoogleMapUtil.marker != null) {
                GoogleMapUtil.marker.setMap(null);
            }
            GoogleMapUtil.marker = new google.maps.Marker({
                map: GoogleMapUtil.map,
                position: ll
            });
            GoogleMapUtil.marker.setMap(GoogleMapUtil.map);
            setOutputAddressFromLatLng(ll);
        }
        function setOutputAddressFromLatLng(ll) {
            if (!navigator.geolocation)
                return;
            GoogleMapUtil.geocoder.geocode({ location: ll, address: "" }, function (results, status) {
                if (status === google.maps.GeocoderStatus.OK) {
                    GoogleMapUtil.onClickMap(ll, results[0].formatted_address);
                }
            });
        }
    }
    //初期位置は六本木ヒルズ
    GoogleMapUtil.firstPositionX = 35.660464;
    GoogleMapUtil.firstPositionY = 139.729281;
    return GoogleMapUtil;
}());
//# sourceMappingURL=GoogleMapUtil.js.map