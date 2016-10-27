"use strict";

angular.module("calculatorApp", [])
    .controller("CalculatorController", ["$scope", "$http", function ($scope, $http) {
        $scope.x = 123;
        $scope.y = 12;
        $scope.expressions = {};

        var baseUrl = "http://localhost:3000/api/calc/";

        $scope.add = function () {
            var uri = baseUrl + "add/" + $scope.x + "/" + $scope.y;
            $http.get(uri)
                .then(function (result) {
                    $http.get(baseUrl)
                        .then(function (result) {
                            var exps = result.data;
                            for (var i = 0; i < exps.length; i++) {
                                if (exps[i] === null) {
                                    return;
                                }
                                
                                $scope.expressions[i] = exps[i];
                            }
                        });
                    
                    $scope.result = result.data;
                });
        }
    }]);
