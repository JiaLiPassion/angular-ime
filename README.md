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
