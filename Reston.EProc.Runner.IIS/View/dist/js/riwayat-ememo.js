var id = $("#EMemoId").val();
var id_pengadaan = window.location.hash.replace("#", "");
if (id == null || id == "") {
    id = id_pengadaan;
}
app.controller('history', ['$scope', '$http', function ($scope, $http) {
    $scope.history = {};
    $http.get("Api/ememo/riwayatDokumen?Id=" + id)
        .then(function (response) {
            $scope.history = response.data;
        });
}]);