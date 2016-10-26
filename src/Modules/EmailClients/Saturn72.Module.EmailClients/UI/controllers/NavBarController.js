angular.module("imapAgentApp")
    .controller("NavBarController", function($scope) {
        $scope.menuItems = [{
            name: "Configure IMAP Connection", url:"#/config"}
          ];

        var selectedMenuItem = $scope.menuItems[0];
        $scope.setMenuItem = function(menuItem) {
            selectedMenuItem = menuItem;
        }

        $scope.getMenuItemClass = function(menuItem) {
            return selectedMenuItem == menuItem ? "btn-primary" : "";
        }
    });
