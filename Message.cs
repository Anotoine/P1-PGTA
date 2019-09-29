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
        private List<string> rawList;
        private int CAT;
        private int Length;
        private List<bool> listFSPEC = new List<bool>();
        private string listFSPECraw = "";
        private int Offset; //Donde empieza el siguiente campo
        private List<DataItem> listDataItem = new List<DataItem>();

        //Constructors needed
        public Message(int ID, List<string> raw, int CAT, int Length)
        {
            this.ID = ID;
            this.rawList = raw;
            this.CAT = CAT;
            this.Length = Length;
            //this.CAT = Int32.Parse(this.rawList[0], System.Globalization.NumberStyles.HexNumber);
            //this.lengthMessage = Int32.Parse(this.rawList[1], System.Globalization.NumberStyles.HexNumber) + Int32.Parse(this.rawList[2], System.Globalization.NumberStyles.HexNumber);
        }

        public Message(List<string> raw)
        {
            this.rawList = raw;
            this.CAT = Int32.Parse(this.rawList[0], System.Globalization.NumberStyles.HexNumber);
            this.Length = Int32.Parse(this.rawList[1], System.Globalization.NumberStyles.HexNumber) + Int32.Parse(this.rawList[2], System.Globalization.NumberStyles.HexNumber);

           
            bool exit = false;
            int i = 0;
            this.listFSPEC.Add(false);
            Offset = 3;
            while (!exit)
            {
                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16),2).PadLeft(8,'0');
                for (int j = 0; j < 8; j++)
                {
                    this.listFSPECraw = string.Concat(this.listFSPECraw, s[j]);
                    if (char.Equals(s[j],'1'))
                        this.listFSPEC.Add(true);
                    else if (char.Equals(s[j], '0'))
                        this.listFSPEC.Add(false);

                    if (j == 7)
                    {
                        Offset++;
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
                decodeSACSIC();             
            }
            if (this.listFSPEC[2])
            {
                var ls = new List<string> { "SSR", "MS", "HF", "VDL4", "UAT", "DME", "OT", "FX", "RAB", "SPI", "CHN", "GBS", "CRT", "SIM", "TST", "FX" };
                var ls1 = new List<string> { "Non-Mode S 1090MHz multilateration", "Mode S 1090MHz multilateration", "HF multilateration",
                    "VDL Mode 4 multilateration", "UAT multilateration", "DME/TACAN multilateration", "Other Technology multilateration",
                    "Extension of Data Item", "Report from field monitor (fixed transponder)", "Special Position Identification", "Chain 2", "Transponder Ground bit set", "Corrupted replies in multilateration",
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
                    string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                    for (int j = 0; j < 8; j++)
                    {
                        if (char.Equals(s[j], '1'))
                            a = new Atom(ls[(i * 8) + j], 1, ls1[(i * 8) + j]);
                        else
                            a = new Atom(ls[(i * 8) + j], 0, ls0[(i * 8) + j]);

                        d.addAtom(a);

                        if (j == 7)
                        {
                            Offset++;
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
                decodeTOD();
            }

            if (this.listFSPEC[4])
            {   //TODO: not finished    DEBE HABER UNA FUNCION, YO NO LA HE ENCONTRADO, HE VISTO Q COGIENDO DATOS EN HEX ASI DEBERIA IR
                int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), System.Globalization.NumberStyles.HexNumber);


                int lon = Int32.Parse(string.Concat(this.rawList[Offset + 4], this.rawList[Offset + 5], this.rawList[Offset + 6], this.rawList[Offset + 7]), System.Globalization.NumberStyles.HexNumber);



                d = new DataItem("I020/041", "Position in WGS-84 Coordinates", new Atom("", 0, ""));
                Offset += 8;
            }

            if (this.listFSPEC[5])
            {
                decodeXY();
            }

            if (this.listFSPEC[6])
            {
                decodeTrackNumber();
            }

            if (this.listFSPEC[7])
            {
                d = new DataItem("I020/170", "Track Status");
                var ls = new List<string> { "CNF", "TRE", "CST", "CDM", "MAH", "STH", "FX","GHO", "", "", "", "", "", "", "FX" };
                var ls0 = new List<string> { "Confirmed track", "Default", "Not extrapolated", "Maintaining", "Climbing", "Default", "Measured position", "End of Data Item",
                    "Default","","","","","","", "End of Data Item" };
                var ls1 = new List<string> { "Track in initiation phase", "Last report for a track", "Extrapolated", "Descending", "Invalid",
                    "Horizontal manoeuvre", "Smoothed position", "Extension into first extent", "Ghost track", "", "", "","","","", "Extension into second extent" };

                int cont1 = 0;
                int cont2 = 0;
                int i = 0;
                bool exit = false;
                while (!exit)
                { 
                    string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                    for (int j = 0; j < 8; j++)
                    {
                        if (j == 3 && i == 0)
                        {
                            if (char.Equals(s[j], '1'))
                            {
                                j++;
                                if (char.Equals(s[j], '1'))
                                    a = new Atom(ls[cont1], 1, ls1[cont2 + 1]);
                                else
                                    a = new Atom(ls[cont1], 0, ls1[cont2]);
                            }
                            else
                            {
                                j++;
                                if (char.Equals(s[j], '1'))
                                    a = new Atom(ls[cont1], 1, ls0[cont2 + 1]);
                                else
                                    a = new Atom(ls[cont1], 0, ls0[cont2]);
                            }
                            cont2++;
                        }
                        else if (char.Equals(s[j], '1'))
                            a = new Atom(ls[cont1], 1, ls1[cont2]);
                        else
                            a = new Atom(ls[cont1], 0, ls0[cont2]);
                        
                        cont1++;
                        cont2++;
                        d.addAtom(a);

                        if (j == 7)
                        {
                            Offset++;
                            if (char.Equals(s[j], '0'))
                                exit = true;
                            if (char.Equals(s[j], '1'))
                                i += 1;
                        }
                    }
                }
                listDataItem.Add(d);
            }
            if (this.listFSPEC[8])
            {
                if (this.listFSPEC[9]) //8 I020/070
                {
                    d = new DataItem("I020/070", "Mode-3/A Code in Octal Representation");

                    string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                    int j = 0;
                    string code = "";
                    while (j < 16)
                    {
                        if (j == 0)
                        {
                            if (char.Equals(s[j], '0'))
                                a = new Atom("V", 0, "Code Validated");
                            else
                                a = new Atom("V", 1, "Code Validated");
                            j++;
                            d.addAtom(a);
                        }
                        else if (j == 1)
                        {
                            if (char.Equals(s[j], '0'))
                                a = new Atom("G", 0, "Default");
                            else
                                a = new Atom("G", 1, "Garbled code");
                            j++; d.addAtom(a);
                        }
                        else if (j == 2)
                        {
                            if (char.Equals(s[j], '0'))
                                a = new Atom("L", 0, "Mode-3/A code derived from the reply of the transpoder");
                            else
                                a = new Atom("L", 1, "Mode-3/A code not extracted during the last update period");
                            j++; d.addAtom(a);
                        }
                        else if (j == 4)
                        {
                            code = string.Concat(code, Convert.ToString(Convert.ToInt32(string.Concat(s[j], s[j + 1], s[j + 2]), 2)));
                            j += 3;
                        }
                        else if (j == 7)
                        {
                            code = string.Concat(code, Convert.ToString(Convert.ToInt32(string.Concat(s[j], s[j + 1], s[j + 2]), 2)));
                            j += 3;
                        }
                        else if (j == 10)
                        {
                            code = string.Concat(code, Convert.ToString(Convert.ToInt32(string.Concat(s[j], s[j + 1], s[j + 2]), 2)));
                            j += 3;
                        }
                        else if (j == 13)
                        {
                            code = string.Concat(code, Convert.ToString(Convert.ToInt32(string.Concat(s[j], s[j + 1], s[j + 2]), 2)));
                            j += 3;
                        }
                        else
                            j++;

                    }
                    d.addAtom(new Atom("Mode-3/A reply", Convert.ToInt32(code), code));
                    Offset += 2;
                    listDataItem.Add(d);

                }
                if (this.listFSPEC[10]) //9 I020/202
                {
                    d = new DataItem("I020/202", "Calculated Track Velocity in Cartesian Coordinates");

                    //string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                    byte[ ] bb = { 0xff, 0xff };
                    int AA = BitConverter.ToInt16(bb,0);
                    d.addAtom(new Atom("V_x",(float) AA/4, AA.ToString()));

                    //string s2 = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                    byte[] cc = { 0xff, 0xff };
                    int BB = BitConverter.ToInt16(cc, 0);
                    d.addAtom(new Atom("V_y", (float)BB / 4, BB.ToString()));

                    listDataItem.Add(d);



                }
                if (this.listFSPEC[11]) //10 I020/090
                {

                }
                if (this.listFSPEC[12]) //11 I020/100
                {

                }
                if (this.listFSPEC[13]) //12 I020/220
                {

                }
                if (this.listFSPEC[14]) //13 I020/245
                {

                }
                if (this.listFSPEC[15]) //14 I020/110
                {

                }
                if (this.listFSPEC[16])
                {
                    if (this.listFSPEC[17]) //15 I020/105
                    {

                    }
                    if (this.listFSPEC[18]) //16 I020/210
                    {

                    }
                    if (this.listFSPEC[19]) //17 I020/300
                    {

                    }
                    if (this.listFSPEC[20]) //18 I020/310
                    {

                    }
                    if (this.listFSPEC[21]) //19 I020/500
                    {

                    }
                    if (this.listFSPEC[22]) //20 I020/400
                    {

                    }
                    if (this.listFSPEC[23]) //21 I020/250
                    {

                    }
                    if (this.listFSPEC[24])
                    {
                        if (this.listFSPEC[25]) //22 I020/230
                        {

                        }
                        if (this.listFSPEC[26]) //23 I020/260
                        {

                        }
                        if (this.listFSPEC[27]) //24 I020/030
                        {

                        }
                        if (this.listFSPEC[28]) //25 I020/055
                        {

                        }
                        if (this.listFSPEC[29]) //26 I020/050
                        {

                        }
                        if (this.listFSPEC[30]) //27 RE
                        {

                        }
                        if (this.listFSPEC[31]) //28 SP
                        {

                        }
                    }
                }
            }



        }
        private void decodeCAT21()
        {

        }



        private void decodeSACSIC()
        {
            DataItem d = new DataItem("I020/010", "Data Source Identifier", new Atom("SAC", Convert.ToInt32(this.rawList[Offset], 16), Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16)).PadLeft(3, '0')));
            Offset++;

            d.addAtom(new Atom("SIC", 1, Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16)).PadLeft(3, '0')));
            Offset++;

            listDataItem.Add(d);
        }
        private void decodeTOD()
        {
            int LSB = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            DataItem d = new DataItem("I020/140", "Time of Day", new Atom("TOD", (float)LSB / 128, new DateTime().AddSeconds((float)LSB / 128).ToString("HH:mm:ss.fff")));
            listDataItem.Add(d);
            Offset += 3;
        }
        private void decodeXY()
        {
            int x = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            int y = Int32.Parse(string.Concat(this.rawList[Offset + 3], this.rawList[Offset + 4], this.rawList[Offset + 5]), System.Globalization.NumberStyles.HexNumber);
            DataItem d = new DataItem("I020/042", "Position in Cartesian Coordinates", new Atom("X", (float)x / 2, Convert.ToString((float)x / 2)));
            d.addAtom(new Atom("Y", (float)y / 2, Convert.ToString((float)y / 2)));
            listDataItem.Add(d);
            Offset += 6;
        }
        private void decodeTrackNumber()
        {
            int trackNumber = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
            DataItem d = new DataItem("I020/161", "Track Number", new Atom("track_num", trackNumber, Convert.ToString(trackNumber).PadLeft(4,'0')));
            listDataItem.Add(d);
            Offset += 2;
        }
    }
}
