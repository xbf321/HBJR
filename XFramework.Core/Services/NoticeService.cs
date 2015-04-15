using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XFramework.Services
{
    public static class NoticeService
    {
        /// <summary>
        /// 获得XMLPath
        /// </summary>
        /// <returns></returns>
        private static string GetXMLPath() {
            return string.Concat(AppDomain.CurrentDomain.BaseDirectory,"App_Data\\Notice.xml");
        }
        /// <summary>
        /// 创建XML文件，如果存在，返回
        /// </summary>
        private static void CreateXMLFile() {
            string path = GetXMLPath();
            if(!System.IO.File.Exists(path)){
                XDocument doc = new XDocument(new XElement("items"));
                doc.Declaration = new XDeclaration("1.0","utf-8","true");
                doc.Save(path);
            }
        }
        /// <summary>
        /// 获取所有条目
        /// </summary>
        /// <returns></returns>
        public static IList<Tuple<string, string, int, string>> List() {
            IList<Tuple<string, string, int, string>> list = new List<Tuple<string, string, int, string>>();
            XElement doc = XElement.Load(GetXMLPath());
            var items = doc.Elements("item");
            foreach(var item in items){
                string title = item.Value;
                string url = (string)item.Attribute("url");
                int sort = (int)item.Attribute("sort");
                string guid = (string)item.Attribute("guid");
                list.Add(Tuple.Create(title,url,sort,guid));
            }
            return list.OrderBy(p=>p.Item3).ToList();
        }
        /// <summary>
        /// 获得某条目详细信息
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static Tuple<string, string, int> Get(string guid) {
            XElement doc = XElement.Load(GetXMLPath());
            var item = doc.Elements("item").Where(p => (string)p.Attribute("guid") == guid).FirstOrDefault();
            if(item == null){
                return Tuple.Create(string.Empty,string.Empty,0);
            }
            return Tuple.Create(item.Value,(string)item.Attribute("url"),(int)item.Attribute("sort"));
        }
        /// <summary>
        /// 删除某条目
        /// </summary>
        /// <param name="guid"></param>
        public static void Delete(string guid) {
            string xmlPath = GetXMLPath();
            XElement doc = XElement.Load(xmlPath);
            doc.Elements("item").Where(p => (string)p.Attribute("guid") == guid).DescendantsAndSelf().Remove();
            doc.Save(xmlPath);
        }
        /// <summary>
        /// 添加或编辑某条目
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="sort"></param>
        /// <param name="guid"></param>
        public static void Update(string title,string url,int sort,string guid) {
            if (string.IsNullOrEmpty(guid))
            {
                Add(title, url, sort);
            }
            else {
                Edit(title,url,sort,guid);
            }
        }
        /// <summary>
        /// 添加某条目
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="sort"></param>
        private static void Add(string title,string url,int sort) {
            CreateXMLFile();

            string path = GetXMLPath();
            XElement element = XElement.Load(path);
            element.Add(new XElement("item",
                new XAttribute("url",url),
                new XAttribute("sort",sort),
                new XAttribute("guid", Guid.NewGuid()), new XCData(title)));
            element.Save(path);
        }
        /// <summary>
        /// 编辑某条目
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="sort"></param>
        /// <param name="guid"></param>
        private static void Edit(string title,string url,int sort,string guid) {
            CreateXMLFile();
            string path = GetXMLPath();
            //编辑就是先删除，在添加
            XElement doc = XElement.Load(path);
            //删除
            doc.Elements("item").Where(p => (string)p.Attribute("guid") == guid).DescendantsAndSelf().Remove();
            doc.Save(path);
            //添加
            Add(title,url,sort);
        }

    }
}
