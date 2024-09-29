using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using UnityEngine;

namespace SparkX
{
    /// <summary>
    /// Telegram 用户信息
    /// </summary>
    public struct TelegramUserInfo
    {
        /// <summary>
        /// 用户ID (唯一标识)
        /// </summary>
        public ulong id;
        
        /// <summary>
        /// 用户姓氏, 平时聊天显示
        /// </summary>
        public string first_name;
        
        /// <summary>
        /// 用户名称, 平时聊天显示
        /// </summary>
        public string last_name;
        
        /// <summary>
        /// 用户名 (用于查找用户,用户自取,唯一) 名片中显示 @username
        /// </summary>
        public string username;
        
        /// <summary>
        /// 用户使用语言
        /// </summary>
        public string language_code;
        
        /// <summary>
        /// Bot是否允许向该用户发送消息
        /// </summary>
        public bool allowMessages;
        
        public override string ToString()
        {
            return $"id: {id}\nfirst_name: {first_name}\nlast_name: {last_name}\nusername: {username}\nlanguage_code: {language_code}\nallowMessages: {allowMessages}";
        }
    }
    
    /// <summary>
    /// Telegram 初始化数据
    /// </summary>
    public class TelegramInitData
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public readonly TelegramUserInfo user;
        
        /// <summary>
        /// 登录UNIX时间戳,可用于验证登录是否过期
        /// </summary>
        public readonly ulong auth_date;
        
        /// <summary>
        /// 分享链接后面附带的参数,用于分享识别
        /// </summary>
        public readonly string start_param;
        
        /// <summary>
        /// 由 Bot Token 作为密钥使用 HMAC_SHA256 方法加密生成的哈希值, 用于验证数据是否被篡改
        /// <see href="https://core.telegram.org/bots/webapps#validating-data-received-via-the-mini-app"/>
        /// </summary>
        public readonly string hash;
        
        internal TelegramInitData(string query)
        {
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            foreach (string key in queryParams.AllKeys)
            {
                switch (key)
                {
                    case "user":
                        user = JsonUtility.FromJson<TelegramUserInfo>(queryParams[key]);
                        break;
                    case "start_param":
                        start_param = queryParams[key];
                        break;
                    case "hash":
                        hash = queryParams[key];
                        break;
                    case "auth_date":
                        auth_date = ulong.Parse(queryParams[key]);
                        break;
                }
            }
        }
        
        public override string ToString()
        {
            return $"{user.ToString()}\nstart_param: {start_param??String.Empty}\nhash: {hash??String.Empty}\nauth_date: {auth_date}";
        }

        /// <summary>
        /// 这里提供验证的伪代码, 请根据实际情况修改
        /// </summary>
        /// <returns></returns>
        public bool Validate(string query)
        {
            NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
            Dictionary<string, string> data_check_array = new Dictionary<string, string>();
            foreach (string key in queryParams.AllKeys)
            {
                if (key != "hash")
                {
                    data_check_array.Add(key, queryParams[key]);
                }
            }
            
            // 请根据实际情况修改
            string data_check_string = string.Join("\n", data_check_array);
            // string secret_key = HMAC_SHA256(<bot_token>, "WebAppData");
            // string computedHash = hex(HMAC_SHA256(data_check_string, secret_key)) == hash;
            string computedHash = ((data_check_string));
            return computedHash == hash;
        }
    }
}
