# angular-ime
Angular directive to control Japanese IME mode through a global Activex Control

## Background
CSS ime-mode can only support enable or disable Japanese IME, but can't control 
more detailed conversion mode, such as 
- Hiragana ひらがな
- KatakanaHalf　半角カタカナ
- Alpha 半角英数
- AlphaFull 全角英数
- Katakana 全角カタカナ

So we have to use activex to implement such features, but we don't want to embed
a native text box into html because the look&feel is totally different, so we
just let activex to control ime through Win32 API and leave the textbox as 
html input.

## Usage

Just put the following activex object into your index.html

    <object id="IMEManager" onerror="window.objectLoadFailure = true" classid="clsid:7A5D58C7-1C27-4DFF-8C8F-F5876FF94C64"></object>
    
And initialize your angular app with ngIme module

    angular.module("testApp", ["ngIme"]).controller("testController", function() {});
            
Then you can use the ime directive.

    半角カナ:<input type="text" ime-conversion="KatakanaHalf"/><br>
    ひらがな:<input type="text" ime-conversion="Hiragana"/><br>
    英数:<input type="text" ime-conversion="Alpha"/><br>
    全角英数:<input type="text" ime-conversion="AlphaFull"/><br>
    カタカナ:<input type="text" ime-conversion="Katakana"/><br>
