$(document).ready(function () {
    var firstName = $('#firstName').text();
    var lastName = $('#lastName').text();
    var intials = $('#firstName').text().charAt(0).toUpperCase() + $('#lastName').text().charAt(0).toUpperCase();
    var profileImage = $('#profileImage').text(intials);
});