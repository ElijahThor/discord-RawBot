using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace RawBotFYP_GUI_.Core.Data
{
    public class StoreJokes
    {
        public List<string> JokeRNG = new List<string>(); // Store Jokes from ListOfJokes.txt
        
        public StoreJokes()
        {
            var relpath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"bin\Debug", @"Core\Data\ListOfJokes.txt");

            string[] JokeRNGs = System.IO.File.ReadAllLines(relpath.ToString()); //Directory for ListOfVulgarities.txt

            try
            {
                foreach (string line in JokeRNGs)
                {
                    JokeRNG.Add(line); // Adds Jokes from ListOfJokes.txt into JokeRNG Dynamic Array
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
