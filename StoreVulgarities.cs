using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using RawBotFYP_GUI_;

namespace RawBotFYP_GUI_.Core.Data
{
    public class StoreVulgarities
    {
        public List<string> StoreVulgar = new List<string>(); // Stores Vulgarities
        
        public StoreVulgarities()
        {
            var relpath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"bin\Debug", @"Core\Data\ListOfVulgarities.txt");

            string[] StoreVulgarList = System.IO.File.ReadAllLines(relpath.ToString()); //Directory for ListOfVulgarities.txt

            try
            {
                foreach (string line in StoreVulgarList)
                {
                    StoreVulgar.Add(line);
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
