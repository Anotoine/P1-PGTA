using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asterix
{
    class DataItem
    {
        private string tag; //Name of the dataItem
        private string description; //Information
        private int length;
        private List<Atom> listValue = new List<Atom>();

        public DataItem(string tag, string desc, Atom atom)
        {
            this.tag = tag;
            this.description = desc;
            this.listValue.Add(atom);
        }

        public DataItem(string tag, string desc)
        {
            this.tag = tag;
            this.description = desc;
        }

        public void addAtom (Atom atom)
        {
            this.listValue.Add(atom);
        }
    }
}
