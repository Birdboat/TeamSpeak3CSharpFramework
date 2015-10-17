using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace TeamSpeak3FrameworkPlugin
{
    public class Exports
    {
        [DllExport]
        public static string ts3plugin_name()
        {
            return MyFirstPlugin.Instance.PluginName;
        }

        [DllExport]
        public static string ts3plugin_version()
        {
            return MyFirstPlugin.Instance.PluginVersion;
        }

        [DllExport]
        public static int ts3plugin_apiVersion()
        {
            return MyFirstPlugin.Instance.ApiVersion;
        }

        [DllExport]
        public static string ts3plugin_author()
        {
            return MyFirstPlugin.Instance.Author;
        }

        [DllExport]
        public static string ts3plugin_description()
        {
            return MyFirstPlugin.Instance.Description;
        }

        [DllExport]
        public static void ts3plugin_setFunctionPointers(TS3Functions funcs)
        {
            MyFirstPlugin.Instance.Functions = funcs;
        }

        [DllExport]
        public static int ts3plugin_init()
        {
            return MyFirstPlugin.Instance.Init();

        }

        [DllExport]
        public static void ts3plugin_shutdown()
        {
            MyFirstPlugin.Instance.Shutdown();
        }

        [DllExport]
        public static void ts3plugin_registerPluginID(string id)
        {
            var functs = MyFirstPlugin.Instance.Functions;
            MyFirstPlugin.Instance.PluginID = id;
        }

        [DllExport]
        public static void ts3plugin_freeMemory(System.IntPtr data)
        {
            Marshal.FreeHGlobal(data);
        }

        [DllExport]
        public static void ts3plugin_currentServerConnectionChanged(ulong serverConnectionHandlerID)
        {
            var functs = MyFirstPlugin.Instance.Functions;
            functs.printMessageToCurrentTab(serverConnectionHandlerID.ToString());
        }

        [DllExport]
        public static unsafe void ts3plugin_initMenus(PluginMenuItem*** menuItems, char** menuIcon)
        {

        }

        [DllExport]
        public static unsafe void ts3plugin_onMenuItemEvent(ulong serverConnectionHandlerID, PluginMenuType type,
            int menuItemID, ulong selectedItemID)
        {
            
        }
    }

    public class UsefulFuncs
    {
        static Boolean Is64Bit()
        {
            return Marshal.SizeOf(typeof(IntPtr)) == 8;
        }

        public static IntPtr EnumIntPtr(object generic)
        {
            if (!generic.GetType().IsEnum) return IntPtr.Zero;
            return new IntPtr((int)generic);
        }

        public unsafe static char* my_strcpy(char* destination, int buffer, char* source)
        {
            char* p = destination;
            int x = 0;
            while (*source != '\0' && x < buffer)
            {
                *p++ = *source++;
                x++;
            }
            *p = '\0';
            return destination;
        }

        private static byte[] convertLPSTR(string _string)
        {
            List<byte> lpstr = new List<byte>();
            foreach (char c in _string.ToCharArray())
            {
                lpstr.Add(Convert.ToByte(c));
            }
            lpstr.Add(Convert.ToByte('\0'));

            return lpstr.ToArray();
        }

        #region Menu Helpers
        public static unsafe PluginMenuItem* createMenuItem(PluginMenuType type, int id, string text, string icon)
        {
            PluginMenuItem* menuItem = (PluginMenuItem*)Marshal.AllocHGlobal(sizeof(PluginMenuItem)).ToPointer();
            menuItem->type = type;
            menuItem->id = id;

            //char* i_ptr = (char *) System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(icon).ToPointer();
            my_strcpy(menuItem->icon, NativeConstants.PLUGIN_MENU_BUFSZ, (char*)Marshal.StringToHGlobalAnsi(icon).ToPointer());

            //IntPtr t_ptr = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(text);
            //char* t_cptr = (char*)t_ptr.ToPointer();
            my_strcpy(menuItem->text, NativeConstants.PLUGIN_MENU_BUFSZ, (char*)Marshal.StringToHGlobalAnsi(text).ToPointer());
            return menuItem;
        }

        public static unsafe void InitMenusHelper(PluginMenuItem*** menuItems, char** MenuIcon, string iconname, params NonNative.PluginMenuHelper[] helpers)
        {
            // x represents the number of elements we want to create
            var x = helpers.Length;
            // size represents how many elements we should send back, because the last elements in menuItems should be null
            var size = x + 1;
            // our 'indexer'
            int n = 0;

            // This is allocating for all our elements
            *menuItems = (PluginMenuItem**) Marshal.AllocHGlobal(sizeof (PluginMenuItem*)*size);

            // for each element in our array, put in our menu
            foreach (var e in helpers)
            {
                (*menuItems)[n++] = UsefulFuncs.createMenuItem(e.type, e.index, e.name, e.icon);
            }

            // Top it off with null
            (*menuItems)[n++] = null;

            // Make sure we did top it of with null
            Debug.Assert(n == size);
            
            // Allocate for the menuicon
            *MenuIcon = (char*) Marshal.AllocHGlobal(NativeConstants.PLUGIN_MENU_BUFSZ * sizeof (char));
            my_strcpy(*MenuIcon, NativeConstants.PLUGIN_MENU_BUFSZ,
                (char*) Marshal.StringToHGlobalAnsi(iconname).ToPointer());

        }

        public static unsafe void InitMenusHelper(PluginMenuItem*** menuItems, char** MenuIcon, string iconname, List<NonNative.PluginMenuHelper> helpers)
        {
            // x represents the number of elements we want to create
            var x = helpers.Count;
            // size represents how many elements we should send back, because the last elements in menuItems should be null
            var size = x+1;
            // our 'indexer'
            int n = 0;

            // This is allocating for all our elements
            *menuItems = (PluginMenuItem**)Marshal.AllocHGlobal(sizeof(PluginMenuItem*) * size);

            // for each element in our array, put in our menu
            foreach (var e in helpers)
            {
                (*menuItems)[n++] = UsefulFuncs.createMenuItem(e.type, e.index, e.name, e.icon);
            }
            // Top it off with null
            (*menuItems)[n++] = null;
            Debug.Assert(n == size);

            *MenuIcon = (char*)Marshal.AllocHGlobal(NativeConstants.PLUGIN_MENU_BUFSZ * sizeof(char));
            UsefulFuncs.my_strcpy(*MenuIcon, NativeConstants.PLUGIN_MENU_BUFSZ,
                (char*)Marshal.StringToHGlobalAnsi(iconname).ToPointer());

        }
#endregion

        public static uint GetChannelVariableAsStr(ulong serverConnectionHandlerID, ulong channelID, ChannelProperties property, ref string ptr)
        {
            var funcs = MyFirstPlugin.Instance.Functions;
            IntPtr refs = IntPtr.Zero;
            uint reter =
                (funcs.getChannelVariableAsString(serverConnectionHandlerID, channelID, EnumIntPtr(property),
                    ref refs));
            ptr = Marshal.PtrToStringAnsi(refs);
            return reter;
        }

        public static unsafe List<string> GetAllChannelNamesAsDict(ulong serverConnectionHandlerID)
        {
            var funcs = MyFirstPlugin.Instance.Functions;
            IntPtr channellistPtr = IntPtr.Zero;
            var dict = new List<string>();
            if (funcs.getChannelList(serverConnectionHandlerID, ref channellistPtr) != Errors.ERROR_ok)
            {
                funcs.logMessage("Failed", LogLevel.LogLevel_ERROR, "Plugin", serverConnectionHandlerID);
                return null;
            }

            ulong* ptr = (ulong*)channellistPtr.ToPointer();
            for (ulong t = 0; ptr[t] != 0; t++)
            {
                string result = string.Empty;
                IntPtr resPtr = IntPtr.Zero;
                if (funcs.getChannelVariableAsString(serverConnectionHandlerID, ptr[t], new IntPtr(0), ref resPtr) !=
                    Errors.ERROR_ok)
                {
                    funcs.logMessage("Plugin", LogLevel.LogLevel_ERROR, "Plugin", serverConnectionHandlerID);
                    return null;
                }

                if ((result = Marshal.PtrToStringAnsi(resPtr)) == null) return null;

                dict.Add(result);
            }
            return dict;
        }
    }
}
