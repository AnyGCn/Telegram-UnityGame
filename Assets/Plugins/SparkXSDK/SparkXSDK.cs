using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Web;

namespace SparkX
{
    public static class SparkXSDK
    {
        /// <summary>
        /// 获取Telegram Web App的初始化数据
        /// </summary>
        /// <returns>返回获得的初始数据</returns>
        public static TelegramInitData GetTelegramWebAppInitData()
        {
            string serializedData = SparkXJSFunction.GetTelegramWebAppInitData();
            if (String.IsNullOrEmpty(serializedData))
            {
                Debug.LogWarning("Failed to get Telegram Web App Init Data.");
                return new TelegramInitData(serializedData);
            }
            
            return new TelegramInitData(serializedData);
        }

        /// <summary>
        /// 在Telegram中分享游戏
        /// </summary>
        /// <param name="gameUrl"> 游戏地址 </param>
        /// <param name="shareId"> 用于表示用户身份的信息 </param>
        /// <param name="shareComment"> 需要附带的分享描述 </param>
        public static void ShareViaTelegram(string gameUrl, string shareId, string shareComment)
        {
            SparkXJSFunction.ShareViaTelegram($"{gameUrl}?startapp={shareId}", shareComment);
        }
    }
}
