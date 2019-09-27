using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P1_PGTA
{
    class Atom
    {
        private string name;
        private int status;
        private string value1;
        private int value2;
        private double value3;


        public Atom(string name, int sta, string val)
        {
            this.name = name;
            this.status = sta;
            this.value1 = val;
        }
        public Atom(string name, int sta, int val)
        {
            this.name = name;
            this.status = sta;
            this.value2 = val;
        }
        public Atom(string name, int sta, double val)
        {
            this.name = name;
            this.status = sta;
            this.value3 = val;
        }
    }
}
