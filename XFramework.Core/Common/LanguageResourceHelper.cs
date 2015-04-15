using System;
using System.Xml.Linq;



using XFramework.Model;

namespace XFramework.Common
{
    public static class LanguageResourceHelper
    {
        /// <summary>
        /// 根据指定语言获得此语言文本
        /// </summary>
        /// <param name="key"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string GetString(string key,WebLanguage language) {
            string path = string.Format("Language/{0}.xml",language);
            XElement items = XElement.Load(String.Concat(System.AppDomain.CurrentDomain.BaseDirectory, path));
            var list = items.Elements("item");
            string value = string.Empty;
            foreach(var item in list){
                string _key = (string)item.Attribute("key");
                string _value = item.Value;
                if(key.ToLower() == _key.ToLower()){
                    value = _value;
                    break;
                }
            }
            return value;
        }
    }
}
