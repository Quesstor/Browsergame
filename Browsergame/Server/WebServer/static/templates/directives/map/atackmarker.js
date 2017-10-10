angular.module('app').controller('atackmarker', function ($scope, $rootScope, mapService) {
    $scope.event = function(){
        if(!$rootScope.events || !$rootScope.events["Fight"]) return;        
        for(var e of $rootScope.events["Fight"]){
            if(e.id == $scope.eventid) return e;
        }
    }
    $scope.targetCity = function(){
        return $rootScope.cities[$scope.event().targetCityID];
    }
    $scope.units = function(){
        var event = $scope.event();
        if(!event) return;
        var units = {};
        for(var unitID of event.unitIDs){
            var unit = $rootScope.units[unitID];
            if(!units[unit.type]) units[unit.type]=0;
            units[unit.type] += 1;
        }
        return units;
    }
    $scope.$watch("hover", function(){
        var lineID = "atackmarkerHover"
        if($scope.hover){
            var event = $scope.event();            
            var marker = mapService.layers.events[event.type + event.id];
            mapService.drawPolyLine(lineID, 
                {x:marker._latlng.lat, y:marker._latlng.lng}, $scope.targetCity().location, "red")
        }else mapService.deletePolyLine(lineID);
    })
})

