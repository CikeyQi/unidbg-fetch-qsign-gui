<div align="center">
    <img alt="yuhuo" src="./readme/logo.png"/>

# unidbg-fetch-qsign-gui

[1.1.9](https://github.com/fuqiuluo/unidbg-fetch-qsign/releases/tag/1.1.9)を使用し、組み込みJRE環境で、ワンクリックで起動/停止/バージョン切り替えができます。

Windowsのみ対応（Windows 7はサポートされていません）。Linuxプラットフォームの場合は、[unidbg-fetch-qsign-shell](https://github.com/CikeyQi/unidbg-fetch-qsign-shell) に移動してください。<br>

<img src="https://camo.githubusercontent.com/14b563b6a086f79dab168115f85fb32154367634f07bb3dd07e0c279fc269233/68747470733a2f2f696d672e736869656c64732e696f2f7374617469632f76313f7374796c653d666f722d7468652d6261646765266d6573736167653d57696e646f777326636f6c6f723d303037384434266c6f676f3d57696e646f7773266c6f676f436f6c6f723d464646464646266c6162656c3d"> <img src="https://camo.githubusercontent.com/ff765790707ecba41b57071db549f75fbf0eeffa5ac6996ff077083863b8bea4/68747470733a2f2f696d672e736869656c64732e696f2f7374617469632f76313f7374796c653d666f722d7468652d6261646765266d6573736167653d2e4e455426636f6c6f723d353132424434266c6f676f3d2e4e4554266c6f676f436f6c6f723d464646464646266c6162656c3d"> <img src="https://camo.githubusercontent.com/6301a47e098ea0b84260920a75b5a71f121c5a0b55965dff8ad80bd60db208c7/68747470733a2f2f696d672e736869656c64732e696f2f7374617469632f76313f7374796c653d666f722d7468652d6261646765266d6573736167653d4325324225324226636f6c6f723d303035393943266c6f676f3d43253242253242266c6f676f436f6c6f723d464646464646266c6162656c3d">

</div>

## 起動方法

- [Releases](https://github.com/CikeyQi/unidbg-fetch-qsign-gui/releases) をクリックして最新の.exeファイルをダウンロードし、高性能なWindowsサーバーに配置します。

- .exeファイルをダブルクリックして開き、起動をクリックして実行します。

- 起動が成功すると、ポップアップウィンドウが表示され、OKをクリックしてSignのアドレスをコピーします。

- 注意：このプロジェクトは.NETを必要とします。起動時に以下のウィンドウが表示された場合は、はいをクリックし、自動的にインストールパッケージがダウンロードされます。インストール後、再度.exeファイルを開いて実行できます。

## 設定方法

### Miao-Yunzaiに設定する方法

- config内のbot.yamlファイルを見つけます。

- 末尾に次の設定を追加します：sign_api_addr: さきほどコピーしたアドレス
（一般的には`http://127.0.0.1:ポート/sign?key=キーです`，icqqはバージョン0.5.1以上をお勧めします）

- config内のbot.yamlを見つけて、プロトコルを1または2に変更します（Androidの場合は1、apadの場合は2）。

- 起動に成功します。

- もしこのプロジェクトが役に立った場合は、無料のスターをください。ありがとうございます。

## 謝辞

- unidbg-fetch-qsignプロジェクト：[unidbg-fetch-qsign](https://github.com/fuqiuluo/unidbg-fetch-qsign)
