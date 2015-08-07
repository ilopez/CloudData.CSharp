using CloudDataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudData cd = new CloudData("http://opencart.israellopezconsulting.com/fishbowl/", "DLVJvVSnjuOHresulfD4");

            var rs = cd.Query("select * from oc_country limit 10");
            Console.WriteLine(rs.RawContent);

            var obj = cd.Query<List<TestObject>>("select * from oc_country limit 10");
            Console.WriteLine("Records: " + obj.Count); 

            bool test = cd.Execute("update oc_country set address_format = 'TEST' where country_id = 1");
            Console.WriteLine(test);

            Console.ReadLine();
        }
    }

    public class TestObject
    {
        public string country_id { get; set; }
        public string name { get; set; }
        public string iso_code_2 { get; set; }
        public string iso_code_3 { get; set; }
        public string address_format { get; set; }
        public string postcode_required { get; set; }
        public string status { get; set; }
    }
}
