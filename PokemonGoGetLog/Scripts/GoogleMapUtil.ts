/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="../typings/google.maps.d.ts" />
"use strict";

//ToDo これはモジュールが良い
class GoogleMapUtil {
    static map: google.maps.Map;
    static marker: google.maps.Marker;
    static geocoder: google.maps.Geocoder;
    static searchTextId: string;
    static canvasId: string;
    static onClickMap: Function;

    //初期位置は六本木ヒルズ
    static firstPositionX = 35.660464;
    static firstPositionY = 139.729281;


    constructor(canvasId: string, searchTextId: string, onClickMap: Function) {
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
                    info => {
                        var center = new google.maps.LatLng(info.coords.latitude, info.coords.longitude);
                        GoogleMapUtil.map.panTo(center);
                        $("#loading").fadeOut();
                        $("#container").fadeIn();
                    },
                    // エラー処理
                    info => {
                        var latlng = getCookieLatlng();
                        GoogleMapUtil.map.panTo(latlng);
                        console.log('現在地取得エラー: ' + info.code);
                        $("#loading").fadeOut();
                        $("#container").fadeIn();
                        return;
                    }, {
                        enableHighAccuracy: true,
                        timeout: 5000
                    }
                );
            } else {
                //クッキーから現在地を取る。
                var latlng = getCookieLatlng();
                GoogleMapUtil.map.panTo(latlng);
                console.log('本ブラウザではGeolocationが使えません');
            }
        }

        function setAutocomplete() {
            var input = <HTMLInputElement>document.getElementById(GoogleMapUtil.searchTextId);
            var options = <google.maps.places.AutocompleteOptions>{ componentRestrictions: { country: "jp" } };
            var autocomplete = new google.maps.places.Autocomplete(input, options);
            //Autocompleteで位置が変更された

            google.maps.event.addListener(autocomplete, "place_changed", () => {
                var place = autocomplete.getPlace();
                GoogleMapUtil.map.setZoom(17);
                GoogleMapUtil.map.setCenter(place.geometry.location);
                setMarker(place.geometry.location);
            });
        }

        function getCookieLatlng(): google.maps.LatLng {
            //ToDo クッキーから現在地を取る。
            var cookieX = 0;
            var cookieY = 0;
            var center = new google.maps.LatLng(GoogleMapUtil.firstPositionX, GoogleMapUtil.firstPositionY);
            return center;
        }

        function setStartPosition(latlng: google.maps.LatLng) {
            var mapOptions = {
                zoom: 15,			// ズーム値
                center: latlng		// 中心座標 [latlng]
            };
            var canvas = document.getElementById(GoogleMapUtil.canvasId);
            GoogleMapUtil.map = new google.maps.Map(canvas, mapOptions);
            google.maps.event.addListener(GoogleMapUtil.map, "click", setMarkerEvent);

        }

        function setMarkerEvent(event: any) {
            var ll = new google.maps.LatLng(event.latLng.lat(), event.latLng.lng());
            setMarker(ll);
        }

        function setMarker(ll: google.maps.LatLng) {
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

        function setOutputAddressFromLatLng(ll: google.maps.LatLng) {
            if (!navigator.geolocation) return;

            GoogleMapUtil.geocoder.geocode(<google.maps.GeocoderRequest>{ location: ll, address: "" }, (results, status) => {
                if (status === google.maps.GeocoderStatus.OK) {
                    GoogleMapUtil.onClickMap(ll, results[0].formatted_address);
                }
            });
        }
    }
}