angular.module('app').service('tradeService', function ($http, $rootScope, $compile) {
    var tradeService = this;
    var successFunction = function (data) {
        if (data) {
            if (data.error) console.warn(data.error);
            else playSound("noti");
            $rootScope.updateData(data);
        }
    };
    this.movegoods = function (good, quant) {
        var tradeData = { token: $rootScope.token, planetid: $rootScope.selectedPlanet.id, unitid: $rootScope.selectedUnit.id, goodtype: good.type, quant: quant, price: 0 };
        $http.post("action/action/trade", tradeData)
        .success(successFunction)
        .error(function (data) { alert("Der Händler konnte seine Waren nicht handeln."); });
    }
    this.trade = function (good, quant) {
        var price = good.price * quant;
        quant = Math.sign(good.offer) * quant;
        var tradeData = { token: $rootScope.token, planetid: $rootScope.selectedPlanet.id, unitid: $rootScope.selectedUnit.id, goodtype: good.type, quant: quant, price: price };
        $http.post("action/action/trade", tradeData)
        .success(successFunction)
        .error(function (data) { alert("Der Händler konnte seine Waren nicht handeln."); });
    };
    this.setPrice = function (good, sell) {
        if (sell) var offer = Math.abs(good.setOffer);
        else offer = -Math.abs(good.setOffer);
        $http.post("action/action/setPrice", { token: $rootScope.token, planetid: $rootScope.selectedPlanet.id, goodType: good.type, price: good.price, offer: offer })
        .success(successFunction)
        .error(function () { alert("Fehler beim Einstellen der Waren."); });
    }
    this.tradeOk = function (good, menge) {
        if (!$rootScope.selectedUnit) return false;
        if (menge == 0 || good.offer == 0) return false;
        if (Math.abs(menge) > Math.abs(good.offer)) return false;
        if(good.offer < 0){
            if (!$rootScope.selectedUnit.goods[good.type] || $rootScope.selectedUnit.goods[good.type].quant < menge) return false;
        }else{
            if ($rootScope.player.gold < good.price * menge || $rootScope.selectedUnit.itemquantsum + menge > $rootScope.selectedUnit.storage) return false;
        }
        return true;
    }
    this.saveTrade = function (good, quant) {
        if(tradeService.tradeOk(good, quant)) tradeService.trade(good, quant);
    }
});