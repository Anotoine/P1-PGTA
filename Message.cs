using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            {   //TODO: check is never in use in CAT20
                int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), System.Globalization.NumberStyles.HexNumber);
                float latreal = Convert.ToSingle(lat * 180 / 2 ^ 25);

                int lon = Int32.Parse(string.Concat(this.rawList[Offset + 4], this.rawList[Offset + 5], this.rawList[Offset + 6], this.rawList[Offset + 7]), System.Globalization.NumberStyles.HexNumber);
                float lonreal = Convert.ToSingle(lon * 180 / 2 ^ 25);

                Offset += 8;

                d = new DataItem("I020/041", "Position in WGS-84 Coordinates", new Atom("Latitude", lonreal, Convert.ToString(lonreal)));
                d.addAtom(new Atom("Longitude", latreal, Convert.ToString(latreal)));
                listDataItem.Add(d);
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
                    List<string> ls = new List<string>() { "V_x", "V_y" };
                    for (int i = 0; i < 2; i++)
                    {
                        int BB = Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset+1]),16);
                        d.addAtom(new Atom(ls[i], (float)BB/4, Convert.ToString((float)BB / 4)));
                        Offset += 2;
                    }
                    listDataItem.Add(d);
                }
                if (this.listFSPEC[11]) //10 I020/090
                {
                    d = new DataItem("I020 / 090", "Flight Level in Binary Representation");

                    string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                    int j = 0;
                    while (j < 3)
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
                            int BB = Convert.ToInt16(s.Substring(2).PadLeft(16, '0'), 2);
                            d.addAtom(new Atom("Fligth Level", (float)BB / 4, Convert.ToString((float)BB / 4)));
                            j++;
                        }

                    }
                    Offset += 2;
                    listDataItem.Add(d);

                }
                if (this.listFSPEC[12]) //11 I020/100
                {
                    //TODO
                }
                if (this.listFSPEC[13]) //12 I020/220
                {
                    d = new DataItem("I020/220", "Target Address");
                    string BB = string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]);
                    d.addAtom(new Atom("ICAO Address", Convert.ToInt32(BB, 16) , BB));
                    Offset += 3;
                    listDataItem.Add(d);
                }
                if (this.listFSPEC[14]) //13 I020/245
                {
                    d = new DataItem("I020/245", "Target Identification");

                    //First decoding STI on first Byte
                    string STI = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16),2).PadLeft(8,'0');
                    Offset++;

                    if (char.Equals(STI[0], '0'))
                    {
                        if (char.Equals(STI[1], '0'))
                            a = new Atom("STI", 00, "Callsign or registration not downlinked from transponder");
                        else
                            a = new Atom("STI", 01, "Registration downlinked from transponder");
                    }
                    else
                    {
                        if (char.Equals(STI[1], '0'))
                            a = new Atom("STI", 10, "Callsign donwlinked from transponder");
                        else
                            a = new Atom("STI", 11, "Not defined");
                    }
                    d.addAtom(a);

                    //Then decoding Callsign on the following 7 Bytes
                    string stringCode = "";
                    for (int i = Offset; i < Offset+6; i++)
                    {
                        stringCode = string.Concat(stringCode, Convert.ToString(Convert.ToInt32(this.rawList[i], 16), 2).PadLeft(8,'0'));
                    }
                    Offset += 6;
                    
                    List<char> code = new List<char>();
                    for (int i = 0; i < 8; i++)
                    {
                        if (stringCode.Substring(6 * i, 6).StartsWith("0"))
                            code.Add((char)Convert.ToInt32(string.Concat("01",stringCode.Substring(6 * i, 6)), 2));
                        else
                            code.Add((char)Convert.ToInt32(string.Concat("00", stringCode.Substring(6 * i, 6)), 2));
                    }

                    d.addAtom(new Atom("Callsign", 0, Regex.Replace(string.Join("",code.ToArray()), @"\s", "")));
                    //Offset += 2;
                    listDataItem.Add(d);
                }
                if (this.listFSPEC[15]) //14 I020/110
                { //could NOT be checked because is never used.
                    d = new DataItem("I020/110", "Measured Height (Local Cartesian Coordinates");
                    float BB = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 2) * 6.25);

                    Offset += 2;
                    d.addAtom(new Atom("Measured Height", BB, Convert.ToString(BB)));
                    listDataItem.Add(d);
                }
                if (this.listFSPEC[16])
                {
                    if (this.listFSPEC[17]) //15 I020/105
                    {
                        d = new DataItem("I020/105", "Geometric Height (WGS-84)");
                        float BB = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 2) * 6.25);

                        Offset += 2;
                        d.addAtom(new Atom("Geometric Height", BB, Convert.ToString(BB)));
                        listDataItem.Add(d);
                    }
                    if (this.listFSPEC[18]) //16 I020/210
                    {
                        d = new DataItem("I020/210", "Calculated Acceleration");
                        List<string> ls = new List<string>() { "A_x", "A_y" };
                        for (int i = 0; i < 2; i++)
                        {
                            float BB = Convert.ToSingle(Convert.ToInt16(this.rawList[Offset], 2) * 0.25);
                            d.addAtom(new Atom(ls[i], BB , Convert.ToString(BB)));
                            Offset++;
                        }
                        listDataItem.Add(d);
                    }
                    if (this.listFSPEC[19]) //17 I020/300
                    {
                        d = new DataItem("I020/300", "Vehicle Fleet Identification");
                        int BB = Convert.ToInt16(this.rawList[Offset], 2);
                        switch (BB)
                        {
                            case 0:
                                d.addAtom(new Atom("VFI", BB, "Unknown"));
                                break;
                            case 1:
                                d.addAtom(new Atom("VFI", BB, "ATC equipment maintenance"));
                                break;
                            case 2:
                                d.addAtom(new Atom("VFI", BB, "Airport maintenance"));
                                break;
                            case 3:
                                d.addAtom(new Atom("VFI", BB, "Fire"));
                                break;
                            case 4:
                                d.addAtom(new Atom("VFI", BB, "Bird scarer"));
                                break;
                            case 5:
                                d.addAtom(new Atom("VFI", BB, "Snow plough"));
                                break;
                            case 6:
                                d.addAtom(new Atom("VFI", BB, "Runway sweeper"));
                                break;
                            case 7:
                                d.addAtom(new Atom("VFI", BB, "Emergency"));
                                break;
                            case 8:
                                d.addAtom(new Atom("VFI", BB, "Police"));
                                break;
                            case 9:
                                d.addAtom(new Atom("VFI", BB, "Bus"));
                                break;
                            case 10:
                                d.addAtom(new Atom("VFI", BB, "Tug (push/tow)"));
                                break;
                            case 11:
                                d.addAtom(new Atom("VFI", BB, "Grass cutter"));
                                break;
                            case 12:
                                d.addAtom(new Atom("VFI", BB, "Fuel"));
                                break;
                            case 13:
                                d.addAtom(new Atom("VFI", BB, "Baggage"));
                                break;
                            case 14:
                                d.addAtom(new Atom("VFI", BB, "Catering"));
                                break;
                            case 15:
                                d.addAtom(new Atom("VFI", BB, "Aircraft maintenance"));
                                break;
                            case 16:
                                d.addAtom(new Atom("VFI", BB, "Flyco (follow me)"));
                                break;
                            default:
                                break;
                        }
                        Offset++;
                        listDataItem.Add(d);
                    }
                    if (this.listFSPEC[20]) //18 I020/310
                    {
                        d = new DataItem("I020/310", "Pre-programmed Message");
                        string BB = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2);

                        if (char.Equals(BB[0], '0'))
                            a = new Atom("TRB", 0, "Default");
                        else
                            a = new Atom("TRB", 1, "In Trouble");
                        d.addAtom(a);

                        int val = Convert.ToInt32(BB.Remove(1,7).PadLeft(8,'0'));


                        switch (val)
                        {
                            case 1:
                                d.addAtom(new Atom("VFI", val, "Towing Aircraft"));
                                break;
                            case 2:
                                d.addAtom(new Atom("VFI", val, "'Follow me' operation"));
                                break;
                            case 3:
                                d.addAtom(new Atom("VFI", val, "Runway check"));
                                break;
                            case 4:
                                d.addAtom(new Atom("VFI", val, "Emergency operation"));
                                break;
                            case 5:
                                d.addAtom(new Atom("VFI", val, "Work in Progress"));
                                break;
                            case 0:
                                d.addAtom(new Atom("VFI", val, "No info"));
                                break;
                            default:
                                break;
                        }

                        Offset++;
                        listDataItem.Add(d);
                    }
                    if (this.listFSPEC[21]) //19 I020/500
                    {
                        d = new DataItem("I020/500", "Position Accuracy");
                        string BB = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8,'0');
                        Offset++;

                        if (char.Equals(BB[0], '0'))
                            d.addAtom(new Atom("DOP", 0, "Absence of Subfield 1"));
                        else
                        {
                            d.addAtom(new Atom("DOP", 1, "Pressence of Subfield 1"));

                            List<string> ls = new List<string>() { "DOP_x", "DOP_y", "DOP_xy" };
                            for (int i = 0; i < 3; i++)
                            {
                                float DOP = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset+1]), 16) * 0.25);
                                d.addAtom(new Atom(ls[i], DOP, Convert.ToString(DOP)));
                                Offset += 2;
                            }
                        }

                        if (char.Equals(BB[1], '0'))
                            d.addAtom(new Atom("SDP", 0, "Absence of Subfield 2"));
                        else
                        {
                            d.addAtom(new Atom("SDP", 1, "Presence of Subfield 2"));
                            List<string> ls = new List<string>() { "sigma_x", "sigma_y", "sigma_xy" };
                            for (int i = 0; i < 3; i++)
                            {
                                float DOP = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.25);
                                d.addAtom(new Atom(ls[i], DOP, Convert.ToString(DOP)));
                                Offset += 2;
                            }
                        }

                        if (char.Equals(BB[2], '0'))
                            d.addAtom(new Atom("SDH", 0, "Absence of Subfield 3"));
                        else
                        {
                            d.addAtom(new Atom("SDH", 1, "Presence of Subfield 3"));
                            float DOP = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.5);
                            d.addAtom(new Atom("sigma_GH", DOP, Convert.ToString(DOP)));
                            Offset += 2;
                        }

                        listDataItem.Add(d);
                    }
                    if (this.listFSPEC[22]) //20 I020/400
                    {
                        d = new DataItem("I020/400", "Contributing Devices");
                        //Amount of Octets that will extent this camp (REP)
                        int REP = Convert.ToInt32(this.rawList[Offset], 16);
                        Offset++;

                        List<string> dev = new List<string>();
                        for (int i = 0; i < (REP - 1); i++)
                        {
                            string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                            for(int j = 0; j < 8; j++)
                            {
                                if (char.Equals(s[j], '1'))
                                    dev.Add(Convert.ToString(8*(REP-i)-j));
                            }
                            Offset++;
                        }
                        listDataItem.Add(d);
                    }
                    if (this.listFSPEC[23]) //21 I020/250
                    {
                        d = new DataItem("I020/250", "Mode S MB Data");
                        //Amount of Octets that will extent this camp (REP)
                        int REP = Convert.ToInt32(this.rawList[Offset], 16);
                        Offset++;

                        listDataItem.Add(d);
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
            DataItem d = new DataItem("I020/010", "Data Source Identifier");
            List<string> ls = new List<string>() {"SAC", "SIC" };
            for (int i = 0; i < 2; i++)
            {
                d.addAtom(new Atom(ls[i], Convert.ToInt32(this.rawList[Offset], 16), Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16)).PadLeft(3, '0')));
                Offset++;
            }
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
