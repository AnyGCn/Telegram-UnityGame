# Telegram Weg Unity Game
客户端主要只有两个接口，代码里都有注释了

## 用于获取Telegram提供的登录信息
public static TelegramInitData GetTelegramWebAppInitData();

## 发送分享链接
public static void ShareViaTelegram(string gameUrl, string shareId, string shareComment);

## 关于登录鉴权
客户端中有鉴权伪码, HttpsServer/IValidateTokenService.cs 里有经过验证的鉴权C#代码.

## 关于发布 Telegram 测试
这个版本似乎不需要自己专门搭建Bot服务，保证网页顺利发布出来

然后找 [@FatherBot](https://t.me/botfather) 新建 /newapp 附上测试链接即可

后续不建议使用 /newgame 的方式进行游戏，其内生形式与 web app 有所不同

