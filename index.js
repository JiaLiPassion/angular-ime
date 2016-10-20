/**
 * Created by lijia on 2016/10/21.
 */
angular.module('ngIme', [])
    .directive('imeConversion', [function() {
        return {
            restrict: 'A',
            link: function(scope, element, attrs, model) {
                var ime = attrs["imeConversion"];
                element.bind('focus', function() {
                    scope.currentIme = IMEManager.GetCurrentInputConversion();
                    IMEManager.SetInputConversion(ime);
                });

                element.bind('blur', function() {
                    if (scope.currentIme) {
                        IMEManager.RestoreInputConversion(scope.currentIme);
                    }
                });
            }
        }
    }])