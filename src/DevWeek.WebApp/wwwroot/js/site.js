// Write your JavaScript code.
var app = angular.module('app', []).config(function ($sceDelegateProvider) {
    $sceDelegateProvider.resourceUrlWhitelist([
        // Allow same origin resource loads.
        'self',
        // Allow loading from our assets domain.  Notice the difference between * and **.
        'http://*:*/**'
    ]);
   
});



