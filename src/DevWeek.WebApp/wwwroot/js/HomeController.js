
app.controller('HomeController', ['$scope', '$http', '$interval', '$sce', ($scope, $http, $interval, $sce) => {

    let privList = [];

    $scope.url = "";
    $scope.list = [];

    $scope.enqueue = () => {
        $http({
            method: 'POST',
            url: '/api/Enqueue',
            data: {
                OriginalMediaUrl: $scope.url
            }
        }).then(function successCallback(response) {
            alert(1)
        }, function errorCallback(response) {
            alert(2)
        });
    };

    $interval(() => {
        $http({
            method: 'GET',
            url: '/api/Enqueue',
            data: {}
        }).then(function successCallback(response) {
            Enumerable.From(privList).ForEach(it => { delete it.$$hashKey; });
            if (JSON.stringify(response.data) != JSON.stringify(privList)) {
                privList = response.data;
                $scope.list = response.data;
            }
        }, function errorCallback(response) {
        });

    }, 500)


}]);