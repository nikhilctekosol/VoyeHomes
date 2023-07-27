$(document).ready(function () {
    $(".see-more").click(function (event) {
        $(".see-more").hide();
        $(".see-less").show();
        $(".about-property").addClass("about-property-full");
    });
    $(".see-less").click(function (event) {
        $(".see-more").show();
        $(".see-less").hide();
        $(".about-property").removeClass("about-property-full");
    });

    $(".share-btn").click(function (event) {
       
        if (navigator.share) {
            navigator.share({
                title: document.title,
                text: "Hey, did you see this on Voye Homes?",
                url: window.location.href
            }).then(() => console.log('Successful share'))
                .catch(error => console.log('Error sharing:', error));
        }
        else {
            console.log('Advanced share is not possible in this browser');
        }
    });

    $(".goBack").click(function () {
       
        if (history.length <=1) {
          
            window.location.href = window.location.origin;
        }
        
    });

    $("#roomId").change(function () {
        /*     alert('Room change');*/

        $('#roomName').val($("#roomId option:selected").text());
        // loadPriceList();
    });

    //$('#roomType').on('change', function () {
    //    loadPriceList();
    //});
   
});


function initMap(lat, lng) {

   
    // The location of pos
    var pos = { lat: lat, lng: lng };
    // The map, centered at Uluru
    var map = new google.maps.Map(
        document.getElementById('map'), { zoom: 13, center: pos, disableDefaultUI: true, fullscreenControl: true });
    // The marker, positioned at Uluru
    var marker = new google.maps.Marker({ position: pos, map: map });
}

function loadCountryList() {
    
    $.get("api/data/get-country-list", function (result) {

        alert(result.status);
        console.log(JSON.stringify(result));
      
        result.data.forEach(function (e, i) {

          
            $('#countryCode').append($('<option/>').val(e.phonecode).text('+'+e.phonecode+'('+e.nicename+')'));
        }); 
    });
}

function loadPriceList() {

    $('#priceList').html('');
    $('#priceListJson').val('');
    $('#totalPricePerRoom').val('');

    var roomId = $('#roomId').val();
    var propertyId = $('#propertyId').val();

    var fromDate = selectedStartDate.format('YYYY-MM-DD');
    var toDate = selectedEndDate.format('YYYY-MM-DD');

    var base_url = window.location.origin;
    var url = base_url+"/api/data/get-price-list?roomId=" + roomId + "&propertyId=" + propertyId
        + "&fromDate=" + fromDate + "&toDate=" + toDate;
    //console.log(url);
    var total = 0;
    $.get(url, function (result) {
       // console.log(JSON.stringify(result.data));

        $('#priceListJson').val(JSON.stringify(result.data));
      
        result.data.forEach(function (e, i) {
            total += e.price;
            $('#priceList').append('<li>' + moment(e.invDate).format('MMM D, YYYY') + ' : RS ' + e.price.toFixed(2) +'</li>');
        });

        if (total <= 0) {
            $('#price_block').hide();
        } else {
            $('#price_block').show();
        }
        $('#priceList').append('<li><b >Total RS ' + total.toFixed(2) + '</b> </li>');

        $('#total').text(total.toFixed(2));

        $('#totalPricePerRoom').val(total);
        
    });
}



var selectedStartDate;
var selectedEndDate;
$(function () {

   

    var start = moment();
    var end = moment().add(2, 'days');

    function cb(start, end) {

        selectedStartDate = start;
        selectedEndDate = end;

        // loadPriceList();

        $('#startDate').val(start.format('YYYY-MM-DD'));
        $('#endDate').val(end.format('YYYY-MM-DD'));

        $('#daterange').val(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
    }

    $('#daterange').daterangepicker({
        startDate: start,
        endDate: end, 
        minDate: start,
        locale: {
            format: 'MMMM D, YYYY'
        }
    }, cb);

    cb(start, end);

    var checkInDate;
    var checkOutDate;
    var adultsCount = 0;
    var childrenCount = 0;
    var propertyId = 0;
    var custId = 0;

    $("#enquiry-form1").submit(function (e) {

        e.preventDefault();
        var form = document.getElementById('enquiry-form');
        if (form.checkValidity() === true) {
            $('.submit-btn').attr("disabled", true);
            $('.submit-spinner').show();
            custId = 0;
            try {
              

                checkInDate = selectedStartDate.format('YYYY-MM-DD HH:mm:00');
              
                checkOutDate = selectedEndDate.format('YYYY-MM-DD HH:mm:00');
               
                adultsCount = $("#adultsCount").val().trim();
                childrenCount = $("#childrenCount").val().trim();
                propertyId = $("#propertyId").val().trim();

              
                var apiurl = "api/data/register-customer";
                var data = {
                    custName: $("#custName").val().trim(),
                    custEmail: $("#custEmail").val().trim(),
                    custPhone: $("#countryCode").val().trim() + parseInt($("#phone").val()).toString(),//todo delete starting zeros
                    referralCode: $("#referralCode").val().trim(),
                }


                $.ajax({
                    url: apiurl,
                    type: 'POST',                   
                    data: data,
                    dataType: 'json',
                    success: function (result) {

                        if (result.actionStatus === 'SUCCESS') {

                            
                            custId = result.data.custId;

                            $('#otp-section').show();
                            $('#enquiry-section').hide();
                        }
                        
                        $('.submit-btn').attr("disabled", false);
                        $('.submit-spinner').hide();
                        
                    },
                    error: function () {
                        $('.submit-btn').attr("disabled", false);
                        $('.submit-spinner').hide();
                    }
                });
            } catch (e) {
                console.log(e);
                $('.submit-btn').attr("disabled", false);
                $('.submit-spinner').hide();

            }
        }
        
        
        

        return false;
    });
   
    $("#otp-form").submit(function (e) {

        e.preventDefault();
        var form = document.getElementById('otp-form');
        if (form.checkValidity() === true) {
            $('.submit-btn').attr("disabled", true);
            $('.submit-spinner').show();
            
            try {

                var apiurl = "api/data/send-enquiry";
                var data = {
                    checkInDate : selectedStartDate.format('YYYY-MM-DD HH:mm:00'),
                    checkOutDate : selectedEndDate.format('YYYY-MM-DD HH:mm:00'),
                    adultsCount : $("#adultsCount").val().trim(),
                    childrenCount : $("#childrenCount").val().trim(),
                    propertyId : $("#propertyId").val().trim(),
                    custId: custId,
                    otp: $("#smscode").val().trim()
                }

                $('#errormsg').text("Could not send enquiry, please try again.");

                $.ajax({
                    url: apiurl,
                    type: 'POST',
                    data: data,
                    dataType: 'json',
                    success: function (result) {


                        if (result.actionStatus === 'SUCCESS') {
                           
                            $('#DialogEnquirySuccess').modal('show');
                        }  
                        else {
                            $('#DialogEnquiryDanger').modal('show');
                        }

                        if (result.actionStatus === 'OTPERROR') {

                            $('#errormsg').text("OTP entered is wrong. Please try again.");

                        } else {

                            $('#enquiry-section').show();
                            $('#otp-section').hide();
                            $('#ModalReserve').modal('hide');
                        }

                        $('.submit-btn').attr("disabled", false);
                        $('.submit-spinner').hide();

                    },
                    error: function () {
                        $('#DialogEnquiryDanger').modal('show');
                        $('.submit-btn').attr("disabled", false);
                        $('.submit-spinner').hide();
                    }
                });
            } catch (e) {
                console.log(e);
                $('.submit-btn').attr("disabled", false);
                $('.submit-spinner').hide();

            }
        }




        return false;
    });
   

    //$(".btn-reserve, .btn-alert-reserve").click(function (e) {

        
        
    //});

   


});

function openReserve(reserveAllowed, reserveAlert) {
    if (reserveAllowed === "ALLOWED") {
        if (reserveAlert.length > 10) {
            $('#DialogEnquiryAlert').modal('show');
        }
        else {
            $('#enquiry-section').show();
            $('#otp-section').hide();
            $('#ModalReserve').modal('show'); 
        }
       
    } else{
        $('#DialogEnquiryAlert').modal('show'); 
    }
    
}

(function () {
    'use strict';
    window.addEventListener('load', function () {
        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.getElementsByClassName('needs-validation');
        // Loop over them and prevent submission
        var validation = Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                //alert(form.checkValidity()) ;
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    }, false);
})();

(function () {
   
})();

//google map autocomplete

function initAutocomplete() {

    try {
       
        var sessionToken = new google.maps.places.AutocompleteSessionToken();

        // Create the search box and link it to the UI element.
        var input = document.getElementById('pac-input');
        var autocomplete = new google.maps.places.Autocomplete(input, sessionToken);
       
        autocomplete.addListener("place_changed", () => {
           
            const place = autocomplete.getPlace();

            if (!place.geometry) {
                // User entered the name of a Place that was not suggested and
                // pressed the Enter key, or the Place Details request failed.
                //window.alert("No details available for input: '" + place.name + "'");
                return;
            }

            let latitude = place.geometry.location.lat();
            let longitude = place.geometry.location.lng();

            let address = "";

            if (place.address_components) {
                address = [
                    (place.address_components[0] &&
                        place.address_components[0].short_name) ||
                    "",
                    (place.address_components[1] &&
                        place.address_components[1].short_name) ||
                    "",
                    (place.address_components[2] &&
                        place.address_components[2].short_name) ||
                    ""
                ].join(" ");
            }

            window.location.href = '/search/' + latitude + '-' + longitude + '-' + address; 
          

        });

       
       

    }
    catch (e) {
        console.log(e);
    }
}

// {Pre Loader}
(function ($) {
    'use strict';
    $(window).on('load', function () {
        if ($(".pre-loader").length > 0) {
            $(".pre-loader").fadeOut("slow");
        }
    });
})(jQuery)

