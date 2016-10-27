    "use strict";

    angular.module("calculatorApp", [])
        .controller("CalculatorController", ["$scope", "$http", function ($scope, $http) {
            $scope.x = 123;
            $scope.y = 12;
         
            var baseUrl = "http://localhost:3000/api/calc/";

            $scope.add = function () {
                var uri = baseUrl + "add/" + $scope.x + "/" + $scope.y;
                $http.get(uri)
                    .then(function (result) {
                        $scope.result = result.data;
                    });
            }
        }]);
