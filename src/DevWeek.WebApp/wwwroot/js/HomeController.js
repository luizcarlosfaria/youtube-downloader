

app.controller('HomeController', ['$scope', '$http', '$interval', '$timeout', '$sce', ($scope, $http, $interval, $timeout, $sce) => {

    let privList = [];

    $scope.searchType = "many";

    $scope.url = "";
    $scope.list = [];
    $scope.notificationQueue = [];

    $scope.enqueue = () => {
        let urls = $scope.url.split(/\r\n|\r|\n/);

        Enumerable.From(urls).ForEach(url => {

            $http({
                method: 'POST',
                url: '/api/Enqueue',
                data: {
                    OriginalMediaUrl: url
                }
            }).then(function successCallback(response) {
                $scope.notificationQueue.push("O vídeo '" + url + "' foi enviado para validação e processamento...");
                $timeout(() => {
                    $scope.notificationQueue.shift();
                }, 3000);
                $scope.url = "";
            }, function errorCallback(response) {
                alert("Algo não deu muito certo!");
            });


        });

    };

    $interval(() => {
        $http({
            method: 'GET',
            url: '/api/downloads',
            data: {}
        }).then(function successCallback(response) {
            Enumerable.From(privList).ForEach(it => { delete it.$$hashKey; });
            if (JSON.stringify(response.data) != JSON.stringify(privList)) {
                privList = response.data;
                $scope.list = response.data;
            }
        }, function errorCallback(response) {
        });

    }, 1500)


}]);