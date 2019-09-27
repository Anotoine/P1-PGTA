using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P1_PGTA;

namespace P1_PGTA
{
    class Message
    {
        private int ID;
        private List<String> rawList;
        private int CAT;
        private int Length;
        private List<bool> listFSPEC = new List<bool>();
        private string listFSPECraw = "";

        //Constructors needed
        public Message(int ID, List<String> raw, int CAT, int Length)
        {
            this.ID = ID;
            this.rawList = raw;
            this.CAT = CAT;
            this.Length = Length;
            //this.CAT = Int32.Parse(this.rawList[0], System.Globalization.NumberStyles.HexNumber);
            //this.lengthMessage = Int32.Parse(this.rawList[1], System.Globalization.NumberStyles.HexNumber) + Int32.Parse(this.rawList[2], System.Globalization.NumberStyles.HexNumber);
        }

        public Message(List<String> raw)
        {
            this.rawList = raw;
            this.CAT = Int32.Parse(this.rawList[0], System.Globalization.NumberStyles.HexNumber);
            this.Length = Int32.Parse(this.rawList[1], System.Globalization.NumberStyles.HexNumber) + Int32.Parse(this.rawList[2], System.Globalization.NumberStyles.HexNumber);

           
            bool exit = false;
            int i = 0;
            this.listFSPEC.Add(false);
            while (!exit)
            {
                string s = Convert.ToString(Convert.ToInt32(this.rawList[3 + i], 16),2).PadLeft(8,'0');
                for (int j = 0; j < 8; j++)
                {
                    this.listFSPECraw = string.Concat(this.listFSPECraw, s[j]);
                    if (char.Equals(s[j],'1'))
                        this.listFSPEC.Add(true);
                    else if (char.Equals(s[j], '0'))
                        this.listFSPEC.Add(false);

                    if (j == 7)
                    {
                        if (char.Equals(s[j], '0'))
                            exit = true;
                        if (char.Equals(s[j], '1'))
                            i += 1;
                    }
                        
                }
            }

            if (this.CAT == 10)
            {
                decodeCAT10();
            }
            else if (this.CAT == 19)
            {
                decodeCAT19();
            }
            else if (this.CAT == 20)
            {
                decodeCAT20();
            }
            else if (this.CAT == 21)
            {
                decodeCAT21();
            }
        }

        //GETs needed
        public int getID()
        {
            return ID;
        }
        public int getCAT()
        {
            return CAT;
        }
        public int getLength()
        {
            return Length;
        }
        public List<String> getList()
        {
            return rawList;
        }
        public string getlistFSPEC()
        {
            return this.listFSPECraw;
        }

        //SETs needed
        public void setID(int ID)
        {
            this.ID = ID;
        }
        public void setRaw(List<String> raw)
        {
            this.rawList = raw;
        }
        public void setCAT(int CAT)
        {
            this.CAT = CAT;
        }
        public void setLengthMessage(int Length)
        {
            this.Length = Length;
        }

        //Functions
        private void decodeCAT10()
        {

        }
        private void decodeCAT19()
        {

        }
        private void decodeCAT20()
        {

        }
        private void decodeCAT21()
        {

        }
    }
}
