﻿$(document).ready(function () {

    $(document).on("click","#product-Detail", function (e) {
        e.preventDefault();
        let url = $(this).attr("href");


        fetch(url).then(function (response) {
            return response.text();
        })
            .then(function (data) {
                //console.log(data)

                $("#quickModal .product-details-modal").html(data)
            });

        $("#quickModal").modal("show");







//        $.ajax({
//            url: url,
//            type: "GET",
//            dataType:"json",
//            success: function (response) {
//                console.log(response.title)


//                //let htmldata =  `<div class="row">
//    <div class="col-lg-5">
//        <!-- Product Details Slider Big Image-->
//        <div class="product-details-slider sb-slick-slider arrow-type-two" data-slick-setting='{
//              "slidesToShow": 1,
//              "arrows": false,
//              "fade": true,
//              "draggable": false,
//              "swipe": false,
//              "asNavFor": ".product-slider-nav"
//              }'>
//            @foreach (ProductImage productImage in Model.ProductImages)
//            {
//                <div class="single-slide">
//                    <img src="~/image/products/@productImage.Name" alt="">
//                </div>
//            }
//        </div>
//        <!-- Product Details Slider Nav -->
//        <div class="mt--30 product-slider-nav sb-slick-slider arrow-type-two"
//             data-slick-setting='{
//            "infinite":true,
//              "autoplay": true,
//              "autoplaySpeed": 8000,
//              "slidesToShow": 4,
//              "arrows": true,
//              "prevArrow":{"buttonClass": "slick-prev","iconClass":"fa fa-chevron-left"},
//              "nextArrow":{"buttonClass": "slick-next","iconClass":"fa fa-chevron-right"},
//              "asNavFor": ".product-details-slider",
//              "focusOnSelect": true
//              }'>
//            @foreach (ProductImage productImage in Model.ProductImages)
//            {
//                <div class="single-slide">
//                    <img src="~/image/products/@productImage.Name" alt="">
//                </div>
//            }
//        </div>
//    </div>
//    <div class="col-lg-7 mt--30 mt-lg--30">
//        <div class="product-details-info pl-lg--30 ">
//            <p class="tag-block">Tags: <a href="#">Movado</a>, <a href="#">Omega</a></p>
//            <h3 class="product-title">Beats EP Wired On-Ear Headphone-Black</h3>
//            <ul class="list-unstyled">
//                <li>Ex Tax: <span class="list-value"> £@Model.ExTax?.ToString("F2")</span></li>
//                <li>Brands: <a href="#" class="list-value font-weight-bold"> @Model.Genre.Name</a></li>
//                <li>Product Code: <span class="list-value"> @Model.Code</span></li>
//                <li>Reward Points: <span class="list-value"> @Model.Point</span></li>
//                <li>Availability: <span class="list-value"> @(Model.IsAvailability ? "In Stock":"Out Stock")</span></li>
//            </ul>
//            <div class="price-block">
//                @if (Model.DiscountPrice != null)
//                {
//                    <span class="price-new">£@Model.DiscountPrice?.ToString("F2")</span>
//                    <del class="price-old">£@Model.Price.ToString("F2")</del>
//                }
//                else
//                {
//                    <span class="price-new">£@Model.Price.ToString("F2")</span>
//                }
//            </div>
//            <div class="rating-widget">
//                <div class="rating-block">
//                    @for (int i = 1; i <= 5; i++)
//                    {
//                        if (i <= Model.Star)
//                        {
//                            <span class="fas fa-star star_on"></span>
//                            continue;
//                        }
//                        <span class="fas fa-star "></span>
//                    }
//                </div>
//                <div class="review-widget">
//                    <a href="#">(1 Reviews)</a> <span>|</span>
//                    <a href="#">Write a review</a>
//                </div>
//            </div>
//            <article class="product-details-article">
//                <h4 class="sr-only">Product Summery</h4>
//                <p>
//                    @Model.Description
//                </p>
//            </article>
//            <div class="add-to-cart-row">
//                <div class="count-input-block">
//                    <span class="widget-label">Qty</span>
//                    <input type="number" class="form-control text-center" value="1">
//                </div>
//                <div class="add-cart-btn">
//                    <a href="#" class="btn btn-outlined--primary">
//                        <span class="plus-icon">+</span>Add to Cart
//                    </a>
//                </div>
//            </div>
//            <div class="compare-wishlist-row">
//                <a href="#" class="add-link"><i class="fas fa-heart"></i>Add to Wish List</a>
//                <a href="#" class="add-link"><i class="fas fa-random"></i>Add to Compare</a>
//            </div>
//        </div>
//    </div>
//</div>`
//            }
//        })
    })

    $(documet).on("keyup", "#searchProduct", function (e) {
        e.preventDefault();

        let search = $(this).val();

        $("#searchedList").empty();

        if (search.length>0) {
            let url = "/product/searchsearch" + seacrh
            fetch(url).then(response => response.text())
                .then(data =>
                {
                    $("#searchedList").html(data);
                }
                );
        }
    })
})