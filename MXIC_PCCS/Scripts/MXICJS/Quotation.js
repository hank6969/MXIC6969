﻿$(document).ready(function () {
    $.ajax({
        async: false,
        cache: false,
        type: "post",
        datatype: "json",
        url: "/Webpage/QuotationSelect",
    
        success: function (data) {
            dataList = [];
            dataList = JSON.parse(data);
            console.log(dataList);
           
          
        }})

})
