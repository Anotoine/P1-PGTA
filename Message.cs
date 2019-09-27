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
        private int LengthFSPEC;
        private List<DataItem> listDataItem = new List<DataItem>();

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
            LengthFSPEC = 3 - 1;
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
                        LengthFSPEC++;
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
        private void decodeCAT20()                     //DataItem(string tag, string desc, int leng, List<atom> atoms)
        {
            DataItem d;
            Atom a;
            if (this.listFSPEC[1])
            {
                 d = new DataItem("I020/010", "Data Source Identifier", new Atom("SAC", 1, Convert.ToString(Convert.ToInt32(this.rawList[LengthFSPEC + 1], 16)).PadLeft(3, '0')));
                 d.addAtom(new Atom("SIC", 1, Convert.ToString(Convert.ToInt32(this.rawList[LengthFSPEC + 2], 16)).PadLeft(3, '0')));
                 listDataItem.Add(d);
            }
            if (this.listFSPEC[2])
            {
                var ls = new List<string> { "SSR", "MS", "HF", "VDL4", "UAT", "DME", "OT", "FX", "RAB", "SPI", "CHN", "GBS", "CRT", "SIM", "TST", "FX" };
                var ls1 = new List<string> { "Non-Mode S 1090MHz multilateration", "Mode S 1090MHz multilateration", "HF multilateration",
                    "VDL Mode 4 multilateration", "UAT multilateration", "DME/TACAN multilateration", "Other Technology multilateration",
                    "Extension of Data Item", "Report from field monitor (fixed transponder)", "Special Position Identification", "Chan 2", "Transponder Ground bit set", "Corrupted replies in multilateration",
                    "Simulated target report", "Test Target", "Extension into next extent"};
                var ls0 = new List<string> { "no Non-Mode S 1090MHz multilateration", "no Mode S 1090MHz multilateration", "no HF multilateration",
                    "no VDL Mode 4 multilateration", "no UAT multilateration", "no DME/TACAN multilateration", "No Other Technology multilateration",
                    "End of Data Item", "Report from target transponder", "Absence of SPI", "Chain 1", "Transponder Ground bit not set", "No Corrupted reply in multilateration",
                    "Actual target report","Default","End of Data Item"};

                d = new DataItem("I020/020", "Target Report Descriptor");

                int i = 0;
                bool exit = false;
                while (!exit)
                {
                    string s = Convert.ToString(Convert.ToInt32(this.rawList[LengthFSPEC + 3 + i], 16), 2).PadLeft(8, '0');
                    for (int j = 0; j < 8; j++)
                    {
                        if (char.Equals(s[j], '1'))
                            a = new Atom(ls[(i * 8) + j], 1, ls1[(i * 8) + j]);
                        else
                            a = new Atom(ls[(i * 8) + j], 1, ls0[(i * 8) + j]);

                        d.addAtom(a);

                        if (j == 7)
                        {
                            if (char.Equals(s[j], '0'))
                                exit = true;
                            if (char.Equals(s[j], '1'))
                                i += 1;
                        }
                    }
                }
                listDataItem.Add(d);
            }
            if (this.listFSPEC[3])
            {

            }
























            if (this.listFSPEC[4])
            {

            }
            if (this.listFSPEC[5])
            {

            }
            if (this.listFSPEC[6])
            {

            }
            if (this.listFSPEC[7])
            {

            }
            if (this.listFSPEC[8])
            {

            }
            if (this.listFSPEC[9])
            {

            }
            if (this.listFSPEC[10])
            {

            }
            if (this.listFSPEC[11])
            {

            }
            if (this.listFSPEC[12])
            {

            }
            if (this.listFSPEC[13])
            {

            }

        }
        private void decodeCAT21()
        {

        }
    }
}
