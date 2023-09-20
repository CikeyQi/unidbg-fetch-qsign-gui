<div align="center">
    <img alt="yuhuo" src="./readme/logo.png"/>

# unidbg-fetch-qsign-gui

使用[unidbg-fetch-qsign 1.2.1](https://github.com/fuqiuluo/unidbg-fetch-qsign/releases/tag/1.2.1)，内置JRE环境，一键启动/停止/切换版本

仅适用于Windows（不支持Windows 7），Linux平台请移步[官方教程](https://github.com/fuqiuluo/unidbg-fetch-qsign/wiki/%E9%83%A8%E7%BD%B2%E5%9C%A8Linux)<br>

<img src="https://camo.githubusercontent.com/14b563b6a086f79dab168115f85fb32154367634f07bb3dd07e0c279fc269233/68747470733a2f2f696d672e736869656c64732e696f2f7374617469632f76313f7374796c653d666f722d7468652d6261646765266d6573736167653d57696e646f777326636f6c6f723d303037384434266c6f676f3d57696e646f7773266c6f676f436f6c6f723d464646464646266c6162656c3d"> <img src="https://camo.githubusercontent.com/ff765790707ecba41b57071db549f75fbf0eeffa5ac6996ff077083863b8bea4/68747470733a2f2f696d672e736869656c64732e696f2f7374617469632f76313f7374796c653d666f722d7468652d6261646765266d6573736167653d2e4e455426636f6c6f723d353132424434266c6f676f3d2e4e4554266c6f676f436f6c6f723d464646464646266c6162656c3d"> <img src="https://camo.githubusercontent.com/6301a47e098ea0b84260920a75b5a71f121c5a0b55965dff8ad80bd60db208c7/68747470733a2f2f696d672e736869656c64732e696f2f7374617469632f76313f7374796c653d666f722d7468652d6261646765266d6573736167653d4325324225324226636f6c6f723d303035393943266c6f676f3d43253242253242266c6f676f436f6c6f723d464646464646266c6162656c3d">

</div>

## 如何启动

- 点击 [Releases](https://github.com/CikeyQi/unidbg-fetch-qsign-gui/releases) 下载最新版本的.exe文件，放置在一个 `高性能` 的 `Windows` 服务器上

- 双击打开.exe文件，点击 `启动` 即可运行

- 启动成功后会弹出弹窗，点击 `确定` 复制Sign地址备用

- ![启动](/readme/start.jpg)

- PS：本项目需要.NET才可运行，若您启动时弹出以下窗口，请点击 `是` ，将会自动为您下载一个安装包，点开安装后，再次打开.exe文件即可运行

- ![.NET](/readme/.NET.jpg)

## 使用方法

### 如何配置进Miao-Yunzai

- 在config中找到bot.yaml文件

- ![bot配置项路径](/readme/config.jpg)

- 在底部添加：`sign_api_addr: 刚才点击确定复制的地址`
（一般是`http://127.0.0.1:端口/sign?key=密钥`，icqq建议升级到0.5.1版本以上才能自动匹配版本）

- ![增加配置项](/readme/bot.jpg)

- 在config中找到bot.yaml更改协议为1或2（安卓手机或apad）

- ![修改协议](/readme/qq.jpg)

- 启动成功

- ![启动成功](/readme/login.jpg)

- 如果该项目对你有帮助，请给我一个免费的Star，谢谢

## 致谢

- unidbg-fetch-qsign项目：[unidbg-fetch-qsign](https://github.com/fuqiuluo/unidbg-fetch-qsign)