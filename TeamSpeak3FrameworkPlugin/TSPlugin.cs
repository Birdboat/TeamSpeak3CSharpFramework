using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace TeamSpeak3FrameworkPlugin
{
    public class TSPlugin<T> where T : class, new()
    {
        /// <summary>
        /// Local Instance of the Plugin
        /// </summary>
        private readonly static Lazy<T> _instance = new Lazy<T>(() => new T());

        protected TSPlugin()
        {
        }

        public static T Instance
        {
            get { return _instance.Value; }
        }

        //public MenuMaker MenuMaker { get; set; }
        
        public TS3Functions Functions { get; set; }

        public string Author { get; set; }
        public string PluginName { get; set; }
        public string PluginVersion { get; set; }
        public int ApiVersion { get { return 20; } }
        public string Description { get; set; }
        public string PluginID { get; set; }
        public virtual unsafe int Init()
        {
            return 0;
        }
        public virtual unsafe void Shutdown()
        {

        }

        ~TSPlugin()
        {
           
        }
    }

    /*
    public class MenuMaker
    {
        private List<NonNative.PluginMenuHelper> list;
        public List<NonNative.PluginMenuHelper> List { get { return list; } set { list = value; } }
        public MenuMaker()
        {
            list = new List<NonNative.PluginMenuHelper>();
        }

        public void Add(NonNative.PluginMenuHelper onj)
        {
               
        }
    }
    */
}
