// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    var table;
    var page = 2;
    var seed = $('#seedvalue').val();
    var region = $('#regions :selected').val();
    var numberOferrors = $('#errors').val();

    init(seed, region);

    function init(seed, region) {
        $.ajax({
            url: "/Home/GetData",
            type: "GET",
            dataType: "json",
            data: { region: region, seed: seed, numErrors: numberOferrors },
            success: function (data) {
                table = initTable(data);
                $('.dataTables_scrollBody').on('scroll', function () {
                    if ($(this).scrollTop() +
                        $(this).innerHeight() >=
                        $(this)[0].scrollHeight) {
                        $.ajax({
                            url: "/Home/GetData",
                            type: "GET",
                            data: { region: region, seed: seed, numErrors: numberOferrors, page:page },
                            dataType: "json",
                            success: function (data) {
                                table.rows.add(data).draw(false);
                                page++;
                            }
                        })
                    }
                });
            }
        });
    }
    function initTable(data) {
        return $('#users').DataTable({
            deferRender: true,
            scrollY: 300,
            scrollCollapse: true,
            scroller: true,
            "ordering": false,
            data: data,
            "paging": false,
            searching: false,
            columns: [
                { data: 'index' },
                { data: 'userID' },
                { data: 'name' },
                { data: 'address' },
                { data: 'phone' },
            ],
            "preDrawCallback": function (settings) {
                pageScrollPos = $('div.dataTables_scrollBody').scrollTop();
            },
            "drawCallback": function (settings) {
                $('div.dataTables_scrollBody').scrollTop(pageScrollPos);
            }
        });
    }



    $("#regions").on('change',function () {
        region = $('#regions :selected').val();
        table.destroy();
        init(seed, region);
    });

    $('#errorsSlider').on('change', function () {
        $('#errors').val($('#errorsSlider').val());
        numberOferrors = $('#errors').val();
        table.destroy();
        init(seed, region);
    });

    $('#errors').on('change', function () {
        $('#errorsSlider').val($('#errors').val());
        numberOferrors = $('#errors').val();
        table.destroy();
        init(seed, region);
    });

    $('#seedvalue').on('change', function () {
        seed = $(this).val();
        table.destroy();
        init(seed, region);
    });

    $('#seedvalueRND').on('click', function () {
        var characters = 'abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789';
        var rndSeed = characters.charAt(Math.floor(Math.random() * characters.length));
        $('#seedvalue').val(rndSeed);
        
        table.destroy();
        init(rndSeed, region);
    });

   
});








