angular.module("imapAgentApp")
    .controller("ConfigController", function($scope) {

        $scope.activeConfigurationCount = function() {
            var count = 0;
            angular.forEach($scope.data.configs, function(c) {
                if (c.active) {
                    count++;
                }
            });
            return count;
        }

        $scope.warningLevel = function() {
            return $scope.activeConfigurationCount() > 0 ? "label-info" : "label-warning";
        }

        $scope.data = {};
        $scope.data = {
            configs: [{
              name:"name1",
                username: "Buy Flowers",
                active: true
            }, {
              name:"name2",
                username: "Get Shoes",
                active: false
            }, {
              name:"name3",
                username: "Collect Tickets",
                active: true
            }, {
              name:"name4",
                username: "Call Joe",
                active: false
            }]
        };


    });
