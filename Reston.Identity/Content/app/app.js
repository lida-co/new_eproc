/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license
 */

/// <reference path="../libs/angular/angular.1.2.13.js" />

window.identityServer = (function () {
    "use strict";

    var identityServer = {
        getModel: function () {
            var modelJson = document.getElementById("modelJson");
            var encodedJson = '';
            if (typeof (modelJson.textContent) !== undefined) {
                encodedJson = modelJson.textContent;
            } else {
                encodedJson = modelJson.innerHTML;
            }
            var json = Encoder.htmlDecode(encodedJson);
            var model = JSON.parse(json);
            model.usernamex = "";
            return model;
        }
    };

    return identityServer;
})();

(function () {
    "use strict";

    (function () {
        var app = angular.module("app", []);
        //app.controller('LayoutCtrl', ['$scope', '$http', function ($scope, $http, Model) {
        //    $scope.model = Model;
        //}]);

        app.controller("LayoutCtrl", function ($scope, Model) {
          
            
            $scope.model = Model;
            $scope.submit = function () {

                // Set the 'submitted' flag to true
                // return false;
                $scope.model.username = $scope.model.usernamex + "#" + $scope.model.captca;
               $scope.submitted = true;
                // Send the form to server
                // $http.post ...
            }
        });

        app.directive("antiForgeryToken", function () {
            return {
                restrict: 'E',
                replace: true,
                scope: {
                    token: "="
                },
                template: "<input type='hidden' name='{{token.name}}' value='{{token.value}}'>"
            };
        });

        app.directive("focusIf", function ($timeout) {
            return {
                restrict: 'A',
                scope: {
                    focusIf:'='
                },
                link: function (scope, elem, attrs) {
                    if (scope.focusIf) {
                        $timeout(function () {
                            elem.focus();
                        }, 100);
                    }
                }
            };
        });
    })();

    (function () {
        var model = identityServer.getModel();
        

        angular.module("app").constant("Model", model);
        

        if (model.autoRedirect && model.redirectUrl) {
            if (model.autoRedirectDelay < 0) {
                model.autoRedirectDelay = 0;
            }
            window.setTimeout(function () {
                window.location = model.redirectUrl;
            }, model.autoRedirectDelay * 1000);
        }

        // Custom UI Events
        var pwd = document.getElementById("pwd");
        var toggleIcon = document.querySelector(".icon-right");
        if(pwd && toggleIcon) {
            toggleIcon.addEventListener("click", function() {
                pwd.type = pwd.type === "password" ? "text" : "password";
            });
        }

        var btnRefresh = document.querySelector(".btn-refresh");
        var captchaImage = document.getElementById("captchaImage");
        if(btnRefresh && captchaImage) {
            btnRefresh.addEventListener("click", function() {
                var currentSrc = captchaImage.src; 
                var match = currentSrc.match(/^(.*\/Home\/)GetCaptchaImage/i);
                var baseUrl = match ? match[1] : "/Home/";
                
                var xhr = new XMLHttpRequest();
                xhr.open("GET", baseUrl + "createCaptca", true);
                xhr.onreadystatechange = function () {
                    if (xhr.readyState === 4 && xhr.status === 200) {
                        var newKey = xhr.responseText.replace(/"/g, '');
                        if(newKey) {
                            var scope = angular.element(document.getElementById('form-login')).scope();
                            if(scope && scope.model) {
                                scope.$apply(function() {
                                    scope.model.username = newKey;
                                });
                            } else {
                                captchaImage.src = baseUrl + "GetCaptchaImage?k=" + newKey + "&t=" + new Date().getTime();
                            }
                        }
                    }
                };
                xhr.send();
            });
        }

        var btnBack = document.querySelector(".btn-back");
        if(btnBack) {
            btnBack.addEventListener("click", function(e) {
                e.preventDefault();
                history.back();
            });
        }
    })();

})();
