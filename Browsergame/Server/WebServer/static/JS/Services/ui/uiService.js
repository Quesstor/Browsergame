angular.module('app').service('uiService', function ($rootScope, $http, $compile, $timeout) {
    this.Math = window.Math;
    var uiService = this;
    this.isFighter = function (unittype) {
        return $rootScope.settings.units[unittype].atack > 0;
    }
    this.playerHasUnitOnPlanet = function (planet, select) {
        if (!planet) return false;
        for (var unittype in planet.unitcounts) {
            if (!select) return true;
            else if (select == "Händler" && !uiService.isFighter(unittype)) return true;
            else if (select == "Fighter" && uiService.isFighter(unittype)) return true;
        }
        return false;
    }
    this.planetHasOffers = function(planet){
        if(!planet) planet=$rootScope.selectedPlanet;
        if(!planet) return false;
        for(var k in planet.items){
            if(planet.items[k].offer!=0) return true;
        }
        return false;
    }
    this.canProduce = function (building, menge) {
        if (!$rootScope.selectedPlanet) return false;
        for (var educt in building.educts) {
            if ($rootScope.selectedPlanet.items[educt].quant < educt.quant * menge) return false;
        }
        return true;
    }
    this.canUpgradeBuilding = function (building) {
        if (!$rootScope.selectedPlanet) return false;
        if ($rootScope.player.gold < building.buildPrice * (building.level + 1)) return false;
        for (type in building.buildCosts) {
            if ($rootScope.selectedPlanet.items[type].quant < building.buildCosts[type].quant * (building.level + 1)) return false;
        }
        for (type in building.buildRequirements) {
            if ($rootScope.selectedPlanet.buildings[type].level < building.buildRequirements[type]) return false;
        };
        return true;
    }

});