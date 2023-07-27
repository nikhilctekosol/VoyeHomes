

var Model;


$(document).ready(function () {

   
    loadList();


});



function loadList() {

    //$('#body-table-main').find('tr.main-row').remove();
    //showPlaceHolder();
   
    $.getJSON(window.location.origin + "/api/data/get-home-data" , function (response) {


        if (response.actionStatus === "SUCCESS") {

            Model = response.data;           

            populateHomeData();
            
          

        } else {
            // handle error
            console.error("API request failed: " + response.message);
        }
        //hidePlaceHolder();

    });


}



function populateHomeData() {

    if (Model) {

        if (Model.destinationList) {
           // populateDestinationList(Model.destinationList);
        }

        if (Model.tagList) {
            
            populateFeatured(Model.tagList);
            populatePopular(Model.tagList);
        }
      
        

    }
}

function populateDestinationList(destinationList) {
    // Check if the destinationList has data
    if (destinationList.length > 0) {
        // Create the destination section
        var section = $('<section class="destination" id="desitnations">');
        var header = $('<div class="header">Your Favourite Destinations<p>Voye Homes will be your best choice when you need privacy in a cosy home, the ambience of a fabulous vacation and the excellence of a hotel</p></div>');
        var slider = $('<div class="desti-slider">');

        // Append header and slider to the section
        section.append(header);
        section.append(slider);

        // Iterate through the destinationList and create each destination box
        for (var i = 0; i < destinationList.length; i++) {
            var destination = destinationList[i];

            var urlSlug = getURLSlug(destination.title); // assuming getURLSlug is defined somewhere
            var encodedId = encodeString(destination.id.toString());

            // Create each box
            var box = $('<div class="box">');
            var link = $('<a href="destination/' + urlSlug + '-' + encodedId + '">');
            var img = $('<div class="img"><img src="' + destination.thumbnail + '" alt="' + destination.title + '" onerror="this.src=\'/assets/img/default.jpg\'"></div>');
            link.append(img);
            box.append(link);
            box.append('<h4>' + destination.title + '</h4>');
            slider.append(box);
        }

        // Append section to the destinations div
        $('#destinations-div').append(section);

        $('.desti-slider').slick({
            dots: false,
            infinite: true,
            speed: 500,
            slidesToShow: 4,
            slidesToScroll: 1,
            autoplay: true,
            arrows: true,
            responsive: [
                {
                    breakpoint: 1024,
                    settings: {
                        slidesToShow: 3,
                        variableWidth: true,
                        slidesToScroll: 1
                    }
                },
                {
                    breakpoint: 628,
                    settings: {
                        slidesToShow: 2,
                        variableWidth: true,
                        slidesToScroll: 1
                    }
                }
            ]
        });
    }
}

function getThumbnailURL(thumbnail, width) {
    return thumbnail.replace("/image/upload/", "/image/upload/w_" + width + ",c_scale/");
}

function populateFeatured(tagList) {
    var featuredTag = tagList.find(item => item.id === 3);

    if (featuredTag && featuredTag.propertyList && featuredTag.propertyList.length > 0) {
        var randomIndex = Math.floor(Math.random() * featuredTag.propertyList.length);
        var property = featuredTag.propertyList[randomIndex];

        var urlSlug = getURLSlug(property.perma_title);
        var encodedId = encodeString(property.id.toString());

        var col = $('<div class="col-6">');
        var link = $('<a href="' + urlSlug + '-' + encodedId + '">');
        var featured = $('<div class="featured">');

        var featuredhead = $('<div class="featuredhead">');
        featuredhead.append('<div class="featuredLabel">Featured</div>');
        featuredhead.append('<div class="featuredProperty">' + property.title + '</div>');

        var picture = $('<picture>');
        picture.append('<source media="(max-width: 320px)" srcset="' + getThumbnailURL(property.thumbnail, 320) + '">');
        picture.append('<source media="(max-width: 375px)" srcset="' + getThumbnailURL(property.thumbnail, 375) + '">');
        picture.append('<source media="(max-width: 414px)" srcset="' + getThumbnailURL(property.thumbnail, 414) + '">');
        picture.append('<source media="(max-width: 480px)" srcset="' + getThumbnailURL(property.thumbnail, 480) + '">');
        picture.append('<source media="(max-width: 600px)" srcset="' + getThumbnailURL(property.thumbnail, 600) + '">');
        picture.append('<img src="' + getThumbnailURL(property.thumbnail, 600) + '" alt="' + property.title + '" onerror="this.src=\'/assets/img/default.jpg\'">');

        featured.append(featuredhead);
        featured.append(picture);
        link.append(featured);
        col.append(link);

        $('#popu-feature-div').append(col);
    }
}

function populatePopular(tagList) {
    var popularTag = tagList.find(item => item.id === 1);

    if (popularTag && popularTag.propertyList && popularTag.propertyList.length > 0) {
        var col = $('<div class="col-6 popular">');
        col.append('<h3>Popular Holiday Homes</h3>');
        col.append('<p>Think of a private holiday home for your next vacation? Think of VOYE HOMES! <a href="category/' + getURLSlug(popularTag.tagName) + '-' + encodeString(popularTag.id.toString()) + '" class="link">View all <i class="fas fa-arrow-right"></i></a></p>');

        var slider = $('<div class="single-slider-auto">');

        popularTag.propertyList.forEach(function (property) {
            var popularItems = $('<div class="popolarItems">');
            var wrapper = $('<div class="wraper">');
            wrapper.append('<div class="brand">VOYE Homes</div>');
            wrapper.append('<div class="name">' + property.title + '</div>');
            wrapper.append('<div class="type">' + property.propertyTypeName + '</div>');
            wrapper.append('<a href="' + getURLSlug(property.perma_title) + '-' + encodeString(property.id.toString()) + '" class="btn btn-primary sm">View Detail</a>');

            popularItems.append(wrapper);
            popularItems.append('<img src="' + getThumbnailURL(property.thumbnail, 600) + '" alt="' + property.title + '" onerror="this.src=\'/assets/img/default.jpg\'">');

            slider.append(popularItems);
        });

        col.append(slider);
        $('#popu-feature-div').append(col);

        // Call the initializeSlick function
        initializeSlick();
    }
}

function initializeSlick() {
    $('.single-slider-auto').slick({
        dots: false,
        infinite: true,
        speed: 500,
        slidesToShow: 4,
        slidesToScroll: 1,
        autoplay: true,
        arrows: true,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    variableWidth: true,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 628,
                settings: {
                    slidesToShow: 2,
                    variableWidth: true,
                    slidesToScroll: 1
                }
            }
        ]
    });
}




function getURLSlug(phrase) {
    // Remove all accents and make the string lower case.
    var output = removeAccents(phrase).toLowerCase();

    // Remove all special characters from the string.
    output = output.replace(/[^a-z0-9\s-]/g, "");

    // Remove all additional spaces in favour of just one.
    output = output.replace(/\s+/g, " ").trim();

    // Replace all spaces with the hyphen.
    output = output.replace(/\s/g, "-");

    // Return the slug.
    return output;
}
function removeAccents(str) {
    return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}

var alphas = "huijkylmacdezfguvnwbj";

function encodeString(inputString) {
    try {
        var encryptedString = "";

        for (var i = 0; i < inputString.length; i++) {
            var alphaPos = parseInt(inputString[i]);
            encryptedString += alphas[alphaPos];
        }

        return encryptedString;
    } catch (error) {
        console.error(error);
        return null;
    }
}

function decodeString(inputString) {
    try {
        var decryptedString = "";

        for (var i = 0; i < inputString.length; i++) {
            var alphaPos = alphas.indexOf(inputString[i]);
            decryptedString += alphaPos.toString();
        }

        return decryptedString;
    } catch (error) {
        console.error(error);
        return null;
    }
}
