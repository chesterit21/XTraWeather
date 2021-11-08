$(document).ready(function(){
    $('#ddl-country').select2();
    $('#ddl-city').select2();
    bindcountry();
});

window.bindcountry = function() {
    var baseurl = window.location.origin +"/weather/country";
    $.ajax({
        type: 'GET',
        url: baseurl ,
        success: function (data) 
        {
            let ddl = document.getElementById('ddl-country');
            let option = document.createElement('option');
            option.value = "";
            option.selected = true;
            option.innerText = "select country";
            ddl.appendChild(option);
            for (let a = 0; a < data.length; a++) {
                const country = data[a];
                option = document.createElement('option');
                option.value = country.countryCode;
                option.innerText = country.countryName;
                ddl.appendChild(option);
            }
            $('#ddl-country').select2();
        }
    });    
}

window.bindcity = function(country){
    if(country === '' || country === null || country.length <= 1) return;
    var baseurl = window.location.origin +"/weather/city/" + country;
    $('.ajax-loading').css("display","inline-block");
    $.ajax({
        type: 'GET',
        url: baseurl ,
        success: function (data) 
        {
            let ddl = document.getElementById('ddl-city');
            let option = document.createElement('option');
            option.value = "";
            option.selected = true;
            option.innerText = "select city";
            ddl.appendChild(option);
            for (let a = 0; a < data.length; a++) {
                const city = data[a];
                option = document.createElement('option');
                option.value = city.name;
                option.innerText = city.name;
                ddl.appendChild(option);
            }
            $('#ddl-city').select2();
            $('.ajax-loading').css("display","none");
        }
    });    

};

window.getweather = function(country,city){
    var baseurl = window.location.origin +"/weather/get/" + country +"/" + city;
    $.ajax({
        type: 'GET',
        url: baseurl ,
        success: function (data) 
        {
            console.log(data);
            $('.title-city').html(data.name);
            $('.title-country').html($('#ddl-country').select2('data')[0].text);
            $('.icon-weather').attr('src',data.weather[0].iconUrl);
            $('.weather-description').html(data.weather[0].description);
            $('.temp-cel').html(data.main.tempCelcius);
            $('.temp-dew').html(data.main.dewPoint);
            $('.temp-fah').html(data.main.temp + " F");
            $('.temp-pres').html(data.main.pressure);
            $('.temp-humidity').html(data.main.humidity);
            $('.temp-vis').html((data.visibility/1000) +" km" );
            $('.temp-time').html(data.timeWeather);
            $('.temp-lat').html(data.coord.lat);
            $('.temp-lon').html(data.coord.lon);
            $('.weather-speed').html(data.wind.speed);
            $('.weather-deg').html(data.wind.deg);
        }
    });    
}

$('#ddl-country').on('select2:select', function (e) { 
    var data = e.params.data;
    $('#ddl-city').empty();
    bindcity(data.id);
});
$('#ddl-city').on('select2:select', function (e) { 
    var data = e.params.data;
    getweather($('#ddl-country').val(),data.id);
});