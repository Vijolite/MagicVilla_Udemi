
using System.Diagnostics;

namespace MagicVilla_VillaAPI.Logging
{
    public class Logging : ILogging
    {
        public void Log(string message, string type)
        {
            if (type == "error")
            {
                //Console.WriteLine("ERROR = " + message);
                Debug.WriteLine("ERROR = " + message);
            }
            else
            {
                //Console.WriteLine(message);
                Debug.WriteLine(message);
            }
        }

    }
}
