using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RGiesecke.DllExport;

namespace TeamSpeak3FrameworkPlugin
{
    public class MyFirstPlugin : TSPlugin<MyFirstPlugin>
    {
        
        public MyFirstPlugin()
        {
            Description = "My First Test Plugin!";
            PluginName = "Test Plugin";
            PluginVersion = "0.1";
            Author = "Birdboat";
        }
        
        public sealed unsafe override int Init()
        {
            return 0;
        }

        public sealed unsafe override void Shutdown()
        {

        }
    }
    
}
