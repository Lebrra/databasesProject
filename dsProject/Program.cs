using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace dsProject
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<TestItem> items;
            TestItem item;

            using (StreamReader r = new StreamReader("test.json"))
            {
                string json = r.ReadToEnd();
                //items = JsonConvert.DeserializeObject<List<TestItem>>(json);
                item = JsonConvert.DeserializeObject<TestItem>(json);
            }

            if(item != null)
            {
                Console.WriteLine("wow I read it");
                Console.WriteLine(item.name);
                Console.WriteLine(item.data);
            }
            else Console.WriteLine("sadness");

            Console.ReadKey();
        }
    }

    public class TestItem
    {
        public string name;
        public string data;
    }
}
