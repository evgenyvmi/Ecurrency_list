// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    $('.body-content').on('click', '.sort', function () {
        var dir = $(this).hasClass('asc') ? 'desc' : 'asc';
        $(this).addClass(dir);
        data = {};
        data['sort_by'] = 'name';
        data['sort_dir'] = dir;
        $.get('', data)
            .done(function (res) {
                var filteredContent = $(res).find('tbody');
                $('table').html(filteredContent);
                $('table').find('.sort').addClass(dir);
            })
    })
});