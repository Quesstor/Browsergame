﻿angular.module('app').service('uiService', function ($rootScope, $http, $compile, $timeout) {
    this.Math = window.Math;
    var uiService = this;
    this.playerHasUnitOnCity = function (city, select) {
        if (!city) return false;
        for (var unittype in city.unitcounts) {
            if (!select) return true;
            else if (select == "Händler" && !uiService.isFighter(unittype)) return true;
            else if (select == "Fighter" && uiService.isFighter(unittype)) return true;
        }
        return false;
    }
});