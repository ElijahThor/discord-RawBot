using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RawBotFYP_GUI_.Core.Data
{
    public class StoreBlackList
    {
        public List<string> blackList = new List<string>(); // Stores Vulgarities

        public StoreBlackList()
        {
            var relpath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"bin\Debug", @"Core\Data\BlackList.txt");
            string[] blacklist = System.IO.File.ReadAllLines(relpath.ToString()); //Directory for blacklist.txt

            try
            {
                foreach (string line in blacklist)
                {
                    blackList.Add(line);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}

