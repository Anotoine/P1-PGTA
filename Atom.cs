﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asterix
{
    class Atom
    {
        private string name;

        private string str;
        private float value;


        public Atom(string name, float val, string str)
        {
            this.name = name;
            this.str = str;
            this.value = val;
        }
    }
}
