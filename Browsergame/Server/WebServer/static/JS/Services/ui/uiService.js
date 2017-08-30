angular.module('app').service('uiService', function ($rootScope, $http, $compile, $timeout) {
    this.Math = window.Math;
    var uiService = this;
    this.isFighter = function (unittype) {
        return $rootScope.settings.units[unittype].atack > 0;
    }
    this.playerHasUnitOnCity = function (city, select) {
        if (!city) return false;
        for (var unittype in city.unitcounts) {
            if (!select) return true;
            else if (select == "Händler" && !uiService.isFighter(unittype)) return true;
            else if (select == "Fighter" && uiService.isFighter(unittype)) return true;
        }
        return false;
    }
    this.cityHasOffers = function(city){
        if(!city) city=$rootScope.selectedCity;
        if(!city) return false;
        for(var k in city.items){
            if(city.items[k].offer!=0) return true;
        }
        return false;
    }



});