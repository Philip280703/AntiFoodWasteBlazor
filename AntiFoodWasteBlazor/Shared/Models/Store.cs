using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiFoodWasteBlazor.Shared.Models
{
    public class Store
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }

        public override string ToString() => $"{Name}, {Street}, {Zip} {City}";
    }
}
