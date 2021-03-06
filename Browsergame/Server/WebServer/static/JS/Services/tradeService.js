﻿angular.module('app').service('tradeService', function ($http, $rootScope, $compile, syncService) {
    var tradeService = this;
    this.moveitems = function (item, quant) {
        var tradeData = { token: $rootScope.token, cityid: $rootScope.selectedCity.id, unitid: $rootScope.selectedUnit.id, itemtype: item.type, quant: quant, price: 0 };
        $http.post("action/action/trade", tradeData)
        .success(successFunction)
        .error(function (data) { alert("Der Händler konnte seine Waren nicht handeln."); });
    }
    this.trade = function (item, quant) {
        var price = item.price * quant;
        quant = Math.sign(item.offer) * quant;
        var tradeData = { token: $rootScope.token, cityid: $rootScope.selectedCity.id, unitid: $rootScope.selectedUnit.id, itemtype: item.type, quant: quant, price: price };
        $http.post("action/action/trade", tradeData)
        .success(successFunction)
        .error(function (data) { alert("Der Händler konnte seine Waren nicht handeln."); });
    };
    this.tradeOk = function (item, menge) {
        if (!$rootScope.selectedUnit) return false;
        if (menge == 0 || item.offer == 0) return false;
        if (Math.abs(menge) > Math.abs(item.offer)) return false;
        if(item.offer < 0){
            if (!$rootScope.selectedUnit.items[item.type] || $rootScope.selectedUnit.items[item.type].quant < menge) return false;
        }else{
            if ($rootScope.player.money < item.price * menge || $rootScope.selectedUnit.itemquantsum + menge > $rootScope.selectedUnit.storage) return false;
        }
        return true;
    }
    this.saveTrade = function (item, quant) {
        if(tradeService.tradeOk(item, quant)) tradeService.trade(item, quant);
    }
});