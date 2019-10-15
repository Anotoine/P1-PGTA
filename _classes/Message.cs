using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASTERIX
{
    public class Message
    {
        private List<string> rawList;
        public int CAT;
        public int Length;
        private List<bool> listFSPEC;
        public string listFSPECraw;
        private int Offset;//Donde empieza el siguiente campo
        public CAT20 CAT20;
        private CAT19 CAT19;
        //private CAT10 CAT10;
        //private CAT21 CAT21;




        //Constructors needed
        public Message(int ID, List<string> raw, int CAT, int Length)
        {
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


            this.listFSPEC = new List<bool>();
            bool exit = false;
            int i = 0;
            this.listFSPEC.Add(false);
            Offset = 3;
            while (!exit)
            {
                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                for (int j = 0; j < 8; j++)
                {
                    this.listFSPECraw = string.Concat(this.listFSPECraw, s[j]);
                    if (char.Equals(s[j], '1'))
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

            switch(this.CAT)
            {
                case 10:
                    decodeCAT10();
                    break;
                case 19:
                    decodeCAT19();
                    break;
                case 20:
                    decodeCAT20();
                    break;
                case 21:
                    decodeCAT21();
                    break;
            }
        }

        public DateTime getTOD()
        {
            switch (CAT)
            {
                case 10:
                    return new DateTime();
                case 19:
                    return CAT19.DI140;
                case 20:
                    return CAT20.DI140;
                case 21:
                    return new DateTime();
                default:
                    return new DateTime();
            }
        }
        public int getTrackN()
        {
            switch (CAT)
            {
                case 10:
                    return 10;
                case 19:
                    return -1;
                case 20:
                    return CAT20.DI161;
                case 21:
                    return 21;
                default:
                    return -1;
            }
        }
        public string getAddressICAO()
        {
            switch (CAT)
            {
                case 10:
                    return "CAT10";
                case 19:
                    return "NONE";
                case 20:
                    return CAT20.DI220;
                case 21:
                    return "CAT21";
                default:
                    return "";
            }
        }
        public string getType() //TODO
        {
            switch (CAT)
            {
                case 10:
                    return "CAT10";
                case 19:
                    return "MLAT Status Message";
                case 20:
                    return CAT20.DI300;
                case 21:
                    return "CAT21";
                default:
                    return "";
            }
        }
        public string getCallsign()
        {
            switch (CAT)
            {
                case 10:
                    return "CAT10";
                case 19:
                    return "NONE";
                case 20:
                    if (CAT20.DI245 == null)
                        return "NONE";
                    else
                        return CAT20.DI245[1].getStr();
                case 21:
                    return "CAT21";
                default:
                    return "";
            }
        }
        public string getCAT()
        {
            switch (CAT)
            {
                case 10:
                    return "CAT10";
                case 19:
                    return "CAT19";
                case 20:
                    return "CAT20";
                case 21:
                    return "CAT21";
                default:
                    return "";
            }
        }
        public Point getPosition()
        {
            switch (CAT)
            {
                case 10:
                    return new Point();
                case 19:
                    return new Point();
                case 20:
                    return CAT20.DI042;
                case 21:
                    return new Point();
                default:
                    return new Point();
            }
        }
        public int getTotalReceivers()
        {
            switch (CAT)
            {
                case 10:
                    return -1;
                case 19:
                    return -1;
                case 20:
                    return CAT20.DI400.Count;
                case 21:
                    return -1;
                default:
                    return -1;
            }
        }
        public int getSAC()
        {
            switch (CAT)
            {
                case 10:
                    return -1;
                case 19:
                    return Convert.ToInt32(CAT19.DI010[0].getVal());
                case 20:
                    return Convert.ToInt32(CAT20.DI010[0].getVal());
                case 21:
                    return -1;
                default:
                    return -1;
            }
        }
        public int getSIC()
        {
            switch (CAT)
            {
                case 10:
                    return -1;
                case 19:
                    return Convert.ToInt32(CAT19.DI010[1].getVal());
                case 20:
                    return Convert.ToInt32(CAT20.DI010[1].getVal());
                case 21:
                    return -1;
                default:
                    return -1;
            }
        }
        public int getLength()
        {
            return this.Length;
        }
        public string getFSPEC()
        {
            return this.listFSPECraw;
        }

        //Functions
        private void decodeCAT10()
        {

        }

        private void decodeCAT19()
        {
            CAT19 = new CAT19();
            if (this.listFSPEC[1]) //I019/010
                CAT19.DI010 = decodeSACSIC();
            if (this.listFSPEC[2]) //I019/000
                CAT19.DI000 = decodeMessType();
            if (this.listFSPEC[3]) //I019/140
                CAT19.DI140 = decodeTOD();
            if (this.listFSPEC[4]) //I019/550
                CAT19.DI550 = decodeSysStats();
            if (this.listFSPEC[5]) //I019/551
                CAT19.DI551 = decodeTrackProcStats();
            if (this.listFSPEC[6]) //I019/552
                CAT19.DI552 = decodeRemSensorStats();
            if (this.listFSPEC[7]) //I019/553
            {
                //CAT19.DI553 = decodeRefTransStats();
            }

            if (this.listFSPEC[8])
            {
                if (this.listFSPEC[9]) //I019/600
                {
                    //CAT19.DI600 = decodeMLTRefPoint();
                }
                if (this.listFSPEC[10]) //I019/610
                {
                    //CAT19.DI610 = decodeMLTheightPoint();
                }
                if (this.listFSPEC[11]) //I019/620
                {
                    //CAT19.DI620 = decodeUndulation();
                }
            }
        }

        private void decodeCAT20()
        {
            CAT20 = new CAT20();
            if (this.listFSPEC[1])
                CAT20.DI010 = decodeSACSIC();
            
            if (this.listFSPEC[2])
                CAT20.DI020 = decodeTRD();
            
            if (this.listFSPEC[3])
                CAT20.DI140 = decodeTOD();

            if (this.listFSPEC[4])  //TODO: check is never in use in CAT20
                CAT20.DI041 = decodeLatLong();
            
            if (this.listFSPEC[5])
                CAT20.DI042 = decodeXY();

            if (this.listFSPEC[6])
                CAT20.DI161 = decodeTrackNumber();

            if (this.listFSPEC[7])
                CAT20.DI170 = decodeTrackStatus();

            if (this.listFSPEC[8])
            {
                if (this.listFSPEC[9]) //8 I020/070
                    CAT20.DI070 = decodeM3A();

                if (this.listFSPEC[10]) //9 I020/202
                    CAT20.DI202 = decodeTrackVel();

                if (this.listFSPEC[11]) //10 I020/090
                    CAT20.DI090 = decodeFLbin();

                if (this.listFSPEC[12]) //11 I020/100
                {
                    //CAT20.DI100 = decodeModeC(); //TODO
                }

                if (this.listFSPEC[13]) //12 I020/220
                    CAT20.DI220 = decodeICAOAddress();

                if (this.listFSPEC[14]) //13 I020/245
                    CAT20.DI245 = decodeCallSign();

                if (this.listFSPEC[15]) //14 I020/110
                    CAT20.DI110 = decodeMheight(); //Never in use, not checked

                if (this.listFSPEC[16])
                {
                    if (this.listFSPEC[17]) //15 I020/105
                        CAT20.DI105 = decodeGheight();

                    if (this.listFSPEC[18]) //16 I020/210
                        CAT20.DI210 = decodeCalAcc();

                    if (this.listFSPEC[19]) //17 I020/300
                        CAT20.DI300 = decodeVehicleId();

                    if (this.listFSPEC[20]) //18 I020/310
                        CAT20.DI310 = decodePPmes();

                    if (this.listFSPEC[21]) //19 I020/500
                        CAT20.DI500 = decodePosAcu();

                    if (this.listFSPEC[22]) //20 I020/400
                        CAT20.DI400 = decodeReceivers();

                    if (this.listFSPEC[23]) //21 I020/250
                        CAT20.DI250 = decodeMSdata();

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



        private List<Atom> decodeSACSIC() //only checked for CAT20 and CAT19
        {
            List<Atom> atoms = new List<Atom>();
            List<string> ls = new List<string>() { "SAC", "SIC" };
            for (int i = 0; i < 2; i++)
            {
                atoms.Add(new Atom(ls[i], Convert.ToInt32(this.rawList[Offset], 16), Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16)).PadLeft(3, '0')));
                Offset++;
            }
            return atoms;
        }
        private DateTime decodeTOD()
        {
            int LSB = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            Offset += 3;

            return new DateTime().AddSeconds((float)LSB / 128);
        }
        private Point decodeXY()
        {
            string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), 16), 2).PadLeft(24, '0');
            int x = Convert.ToInt32(s.PadLeft(32, s[0]), 2);

            s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset + 3], this.rawList[Offset + 4], this.rawList[Offset + 5]), 16), 2).PadLeft(24, '0');
            int y = Convert.ToInt32(s.PadLeft(32, s[0]), 2);
            Offset += 6;
            return new Point().XY2LatLong((float)x / 2, (float)y / 2);
        }
        private int decodeTrackNumber()
        {
            int trackNumber = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
            Offset += 2;
            return trackNumber;
        }
        private List<Atom> decodeM3A()
        {
            List<Atom> atoms = new List<Atom>();
            Atom a;
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
                        a = new Atom("V", 1, "Code not Validated");
                    j++;
                    atoms.Add(a);
                }
                else if (j == 1)
                {
                    if (char.Equals(s[j], '0'))
                        a = new Atom("G", 0, "Default");
                    else
                        a = new Atom("G", 1, "Garbled code");
                    j++;
                    atoms.Add(a);
                }
                else if (j == 2)
                {
                    if (char.Equals(s[j], '0'))
                        a = new Atom("L", 0, "Mode-3/A code derived from the reply of the transpoder");
                    else
                        a = new Atom("L", 1, "Mode-3/A code not extracted during the last update period");
                    j++;
                    atoms.Add(a);
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
            Offset += 2;
            atoms.Add(new Atom("Mode-3/A reply", Convert.ToInt32(code), code));
            return atoms;
        }
        private List<Atom> decodeTrackVel()
        {
            List<Atom> atoms = new List<Atom>();
            List<string> ls = new List<string>() { "V_x", "V_y" };
            for (int i = 0; i < 2; i++)
            {
                int BB = Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16);
                atoms.Add(new Atom(ls[i], (float)BB / 4, Convert.ToString((float)BB / 4)));
                Offset += 2;
            }
            return atoms;
        }
        private List<Atom> decodeTRD()
        {
            List<Atom> atoms = new List<Atom>();
            var ls = new List<string> { "SSR", "MS", "HF", "VDL4", "UAT", "DME", "OT", "FX", "RAB", "SPI", "CHN", "GBS", "CRT", "SIM", "TST", "FX" };
            var ls1 = new List<string> { "Non-Mode S 1090MHz multilateration", "Mode S 1090MHz multilateration", "HF multilateration",
                    "VDL Mode 4 multilateration", "UAT multilateration", "DME/TACAN multilateration", "Other Technology multilateration",
                    "Extension of Data Item", "Report from field monitor (fixed transponder)", "Special Position Identification", "Chain 2", "Transponder Ground bit set", "Corrupted replies in multilateration",
                    "Simulated target report", "Test Target", "Extension into next extent"};
            var ls0 = new List<string> { "no Non-Mode S 1090MHz multilateration", "no Mode S 1090MHz multilateration", "no HF multilateration",
                    "no VDL Mode 4 multilateration", "no UAT multilateration", "no DME/TACAN multilateration", "No Other Technology multilateration",
                    "End of Data Item", "Report from target transponder", "Absence of SPI", "Chain 1", "Transponder Ground bit not set", "No Corrupted reply in multilateration",
                    "Actual target report","Default","End of Data Item"};

            int i = 0;
            bool exit = false;
            while (!exit)
            {
                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                for (int j = 0; j < 8; j++)
                {
                    if (char.Equals(s[j], '1') && j != 7)
                        atoms.Add(new Atom(ls[(i * 8) + j], 1, ls1[(i * 8) + j]));

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
            return atoms;
        }
        private List<Atom> decodeLatLong()
        {
            List<Atom> atoms = new List<Atom>();
            int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), System.Globalization.NumberStyles.HexNumber);
            float latreal = Convert.ToSingle(lat * 180 / 2 ^ 25);
            
            int lon = Int32.Parse(string.Concat(this.rawList[Offset + 4], this.rawList[Offset + 5], this.rawList[Offset + 6], this.rawList[Offset + 7]), System.Globalization.NumberStyles.HexNumber);
            float lonreal = Convert.ToSingle(lon * 180 / 2 ^ 25);
            
            Offset += 8;

            atoms.Add(new Atom("Latitude", lonreal, Convert.ToString(lonreal)));
            atoms.Add(new Atom("Longitude", latreal, Convert.ToString(latreal)));
            return atoms;
        }
        private List<Atom> decodeCallSign()
        {
            List<Atom> atoms = new List<Atom>();
            //First decoding STI on first Byte
            int STI = Convert.ToInt32(this.rawList[Offset], 16);
            Offset++;

            switch (STI)
            {
                case 0:
                    atoms.Add(new Atom("STI", 00, "Callsign or registration not downlinked from transponder"));
                    break;
                case 64:
                    atoms.Add(new Atom("STI", 01, "Registration downlinked from transponder"));
                    break;
                case 128:
                    atoms.Add(new Atom("STI", 10, "Callsign donwlinked from transponder"));
                    break;
                case 192:
                    atoms.Add(new Atom("STI", 11, "Not defined"));
                    break;
                default:
                    break;
            }

            //Then decoding Callsign on the following 7 Bytes
            string stringCode = "";
            for (int i = Offset; i < Offset + 6; i++)
            {
                stringCode = string.Concat(stringCode, Convert.ToString(Convert.ToInt32(this.rawList[i], 16), 2).PadLeft(8, '0'));
            }
            Offset += 6;

            List<char> code = new List<char>();
            for (int i = 0; i < 8; i++)
            {
                if (stringCode.Substring(6 * i, 6).StartsWith("0"))
                    code.Add((char)Convert.ToInt32(string.Concat("01", stringCode.Substring(6 * i, 6)), 2));
                else
                    code.Add((char)Convert.ToInt32(string.Concat("00", stringCode.Substring(6 * i, 6)), 2));
            }
            atoms.Add(new Atom("Callsign", 0, Regex.Replace(string.Join("", code.ToArray()), @"\s", "")));

            return atoms;
        }
        private string decodeICAOAddress()
        {
            string BB = string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]).ToUpper();
            Offset += 3;

            return BB;
        }
        private List<string> decodeReceivers()
        {
            //Amount of Octets that will extent this camp (REP)
            int REP = Convert.ToInt32(this.rawList[Offset], 16);
            Offset++;

            List<string> dev = new List<string>();
            for (int i = 0; i < (REP - 1); i++)
            {
                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                for (int j = 0; j < 8; j++)
                {
                    if (char.Equals(s[j], '1'))
                        dev.Add(Convert.ToString(8 * (REP - i) - j));
                }
                Offset++;
            }

            return dev;
        }
        private List<Atom> decodeTrackStatus()
        {
            List<Atom> atoms = new List<Atom>();
            var ls = new List<string> { "CNF", "TRE", "CST", "CDM", "MAH", "STH", "FX", "GHO", "", "", "", "", "", "", "FX" };
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
                                atoms.Add(new Atom(ls[cont1], 1, ls1[cont2 + 1]));
                            else
                                atoms.Add(new Atom(ls[cont1], 0, ls1[cont2]));
                        }
                        else
                        {
                            j++;
                            if (char.Equals(s[j], '1'))
                                atoms.Add(new Atom(ls[cont1], 1, ls0[cont2 + 1]));
                            else
                                atoms.Add(new Atom(ls[cont1], 0, ls0[cont2]));
                        }
                        cont2++;
                    }
                    else if (char.Equals(s[j], '1'))
                        atoms.Add(new Atom(ls[cont1], 1, ls1[cont2]));
                    else
                        atoms.Add(new Atom(ls[cont1], 0, ls0[cont2]));

                    cont1++;
                    cont2++;

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
            return atoms;
        }
        private List<Atom> decodeFLbin()
        {
            List<Atom> atoms = new List<Atom>();
            string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
            int VG = Convert.ToInt32(s.Substring(0,2),2);

            switch(VG)
            {
                case 0:
                    atoms.Add(new Atom("V", 0, "Code Validated"));
                    atoms.Add(new Atom("G", 0, "Default"));
                    break;
                case 1:
                    atoms.Add(new Atom("V", 0, "Code Validated"));
                    atoms.Add(new Atom("G", 1, "Garbled code"));
                    break;
                case 2:
                    atoms.Add(new Atom("V", 1, "Code not Validated"));
                    atoms.Add(new Atom("G", 0, "Default"));
                    break;
                case 3:
                    atoms.Add(new Atom("V", 1, "Code not Validated"));
                    atoms.Add(new Atom("G", 1, "Garbled code"));
                    break;
                default:
                    atoms.Add(new Atom());
                    atoms.Add(new Atom());
                    break;
            }

            int BB = Convert.ToInt16(s.Substring(2).PadLeft(16, '0'), 2);
            atoms.Add(new Atom("Fligth Level", (float)BB / 4, Convert.ToString((float)BB / 4)));

            Offset += 2;
            return atoms;
        }
        private float decodeMheight()
        {
            float BB = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 2) * 6.25);
            Offset += 2;

            return BB;
        }
        private float decodeGheight()
        {
            float BB = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 2) * 6.25);
            Offset += 2;

            return BB;
        }
        private List<Atom> decodeCalAcc()
        {
            List<Atom> atoms = new List<Atom>();
            List<string> ls = new List<string>() { "A_x", "A_y" };
            for (int i = 0; i < 2; i++)
            {
                float BB = Convert.ToSingle(Convert.ToInt16(this.rawList[Offset], 2) * 0.25);
                atoms.Add(new Atom(ls[i], BB, Convert.ToString(BB)));
                Offset++;
            }

            return atoms;
        }
        private string decodeVehicleId()
        {
            int BB = Convert.ToInt16(this.rawList[Offset], 2);
            Offset++;
            switch (BB)
            {
                case 0:
                    return "Unknown";
                case 1:
                    return "ATC equipment maintenance";
                case 2:
                    return "Airport maintenance";
                case 3:
                    return "Fire";
                case 4:
                    return "Bird scarer";
                case 5:
                    return "Snow plough";
                case 6:
                    return "Runway sweeper";
                case 7:
                    return "Emergency";
                case 8:
                    return "Police";
                case 9:
                    return "Bus";
                case 10:
                    return "Tug (push/tow)";
                case 11:
                    return "Grass cutter";
                case 12:
                    return "Fuel";
                case 13:
                    return "Baggage";
                case 14:
                    return "Catering";
                case 15:
                    return "Aircraft maintenance";
                case 16:
                    return "Flyco (follow me)";
                default:
                    return "";
            }
        }
        private List<Atom> decodePosAcu()
        {
            List<Atom> atoms = new List<Atom>();
            string BB = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
            Offset++;

            if (char.Equals(BB[0], '0'))
                atoms.Add(new Atom("DOP", 0, "Absence of Subfield 1"));
            else
            {
                atoms.Add(new Atom("DOP", 1, "Pressence of Subfield 1"));

                List<string> ls = new List<string>() { "DOP_x", "DOP_y", "DOP_xy" };
                for (int i = 0; i < 3; i++)
                {
                    float DOP = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.25);
                    atoms.Add(new Atom(ls[i], DOP, Convert.ToString(DOP)));
                    Offset += 2;
                }
            }

            if (char.Equals(BB[1], '0'))
                atoms.Add(new Atom("SDP", 0, "Absence of Subfield 2"));
            else
            {
                atoms.Add(new Atom("SDP", 1, "Presence of Subfield 2"));
                List<string> ls = new List<string>() { "sigma_x", "sigma_y", "sigma_xy" };
                for (int i = 0; i < 3; i++)
                {
                    float DOP = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.25);
                    atoms.Add(new Atom(ls[i], DOP, Convert.ToString(DOP)));
                    Offset += 2;
                }
            }

            if (char.Equals(BB[2], '0'))
                atoms.Add(new Atom("SDH", 0, "Absence of Subfield 3"));
            else
            {
                atoms.Add(new Atom("SDH", 1, "Presence of Subfield 3"));
                float DOP = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.5);
                atoms.Add(new Atom("sigma_GH", DOP, Convert.ToString(DOP)));
                Offset += 2;
            }

            return atoms;
        }
        private string decodePPmes()
        {
            int BB = Convert.ToInt32(this.rawList[Offset], 16);
            Offset++;

            switch(BB)
            {
                case 1:
                    return "Default. Towing Aircraft.";
                case 2:
                    return "Default. Follow me operation.";
                case 3:
                    return "Default. Runway check.";
                case 4:
                    return "Default. Emergency operation.";
                case 5:
                    return "Default. Work in Progress.";
                case 129:
                    return "In Trouble. Towing Aircraft.";
                case 130:
                    return "In Trouble. Follow me operation.";
                case 131:
                    return "In Trouble. Runway check.";
                case 132:
                    return "In Trouble. Emergency operation.";
                case 133:
                    return "In Trouble. Work in Progress.";
                default:
                    return "Invalid information";
            }
        }
        private List<Atom> decodeMSdata()
        {
            List<Atom> atoms = new List<Atom>();
            //Amount of Octets that will extent this camp (REP)
            int REP = Convert.ToInt32(this.rawList[Offset], 16);
            Offset++;

            string s = "";
            for (int i = 0; i < (REP - 1); i++)
            {
                for (int j = 0; i < 8; j++)
                {
                    if (j < 6)
                    {
                        s = string.Concat(s, this.rawList[Offset]);
                    }
                    else
                    {
                        atoms.Add(new Atom("BDS1", 4, this.rawList[Offset].Substring(0, 4)));
                        atoms.Add(new Atom("BDS2", 4, this.rawList[Offset].Substring(4, 4)));
                    }
                    Offset++;
                }
                atoms.Add(new Atom("MB DATA", s.Length, s));
            }
            return atoms;
        }
        private int decodeMessType()
        {
            Offset++;
            return Convert.ToInt32(this.rawList[Offset], 16);
        }
        private List<Atom> decodeSysStats()
        {
            List<Atom> atoms = new List<Atom>();
            
            int BB = Convert.ToInt32(this.rawList[Offset].PadLeft(8, '0').Substring(0, 5));
            Offset++;
            switch(BB)
            {
                case 0:
                    atoms.Add(new Atom("NOGO",00,"Operational"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 1:
                    atoms.Add(new Atom("NOGO", 00, "Operational"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 2:
                    atoms.Add(new Atom("NOGO", 00, "Operational"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 3:
                    atoms.Add(new Atom("NOGO", 00, "Operational"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 4:
                    atoms.Add(new Atom("NOGO", 00, "Operational"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 5:
                    atoms.Add(new Atom("NOGO", 00, "Operational"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 6:
                    atoms.Add(new Atom("NOGO", 00, "Operational"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 7:
                    atoms.Add(new Atom("NOGO", 00, "Operational"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 8:
                    atoms.Add(new Atom("NOGO", 01, "Degraded"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 9:
                    atoms.Add(new Atom("NOGO", 01, "Degraded"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 10:
                    atoms.Add(new Atom("NOGO", 01, "Degraded"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 11:
                    atoms.Add(new Atom("NOGO", 01, "Degraded"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 12:
                    atoms.Add(new Atom("NOGO", 01, "Degraded"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 13:
                    atoms.Add(new Atom("NOGO", 01, "Degraded"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 14:
                    atoms.Add(new Atom("NOGO", 01, "Degraded"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 15:
                    atoms.Add(new Atom("NOGO", 01, "Degraded"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 16:
                    atoms.Add(new Atom("NOGO", 10, "NOGO"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 17:
                    atoms.Add(new Atom("NOGO", 10, "NOGO"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 18:
                    atoms.Add(new Atom("NOGO", 10, "NOGO"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 19:
                    atoms.Add(new Atom("NOGO", 10, "NOGO"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 20:
                    atoms.Add(new Atom("NOGO", 10, "NOGO"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 21:
                    atoms.Add(new Atom("NOGO", 10, "NOGO"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 22:
                    atoms.Add(new Atom("NOGO", 10, "NOGO"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 23:
                    atoms.Add(new Atom("NOGO", 10, "NOGO"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 24:
                    atoms.Add(new Atom("NOGO", 11, "undefined"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 25:
                    atoms.Add(new Atom("NOGO", 11, "undefined"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 26:
                    atoms.Add(new Atom("NOGO", 11, "undefined"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 27:
                    atoms.Add(new Atom("NOGO", 11, "undefined"));
                    atoms.Add(new Atom("OVL", 0, "No overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 28:
                    atoms.Add(new Atom("NOGO", 11, "undefined"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 29:
                    atoms.Add(new Atom("NOGO", 11, "undefined"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 0, "Valid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 30:
                    atoms.Add(new Atom("NOGO", 11, "undefined"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 31:
                    atoms.Add(new Atom("NOGO", 11, "undefined"));
                    atoms.Add(new Atom("OVL", 1, "Overload"));
                    atoms.Add(new Atom("TSV", 1, "Invalid"));
                    atoms.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
            }
            return atoms;
        }
        private List<Atom> decodeTrackProcStats()
        {
            List<Atom> atoms = new List<Atom>();
            string s = Convert.ToString(this.rawList[Offset]);

            for(int i = 1; i < 5; i++)
            {
                int bb = Convert.ToInt32(s.Substring((i - 1) * 2 , 2));
                switch (bb)
                {
                    case 0:
                        atoms.Add(new Atom("TP" + Convert.ToString(i), 00, "Standby - Faulted"));
                        break;
                    case 1:
                        atoms.Add(new Atom("TP" + Convert.ToString(i), 01, "Standby - Good"));
                        break;
                    case 2:
                        atoms.Add(new Atom("TP" + Convert.ToString(i), 10, "Exec - Faulted"));
                        break;
                    case 3:
                        atoms.Add(new Atom("TP" + Convert.ToString(i), 11, "Exec - Good"));
                        break;
                }
            }
            Offset++;
            return atoms;
        }
        private List<Atom> decodeRemSensorStats()
        {
            List<Atom> atoms = new List<Atom>();
            //Amount of Octets that will extent this camp (REP)
            int REP = Convert.ToInt32(this.rawList[Offset], 16);
            Offset++;

            for (int i = 0; i < (REP - 1); i++)
            {
                string RSid = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                Offset++;
                int AA = Convert.ToInt32(this.rawList[Offset].PadLeft(8, '0').Substring(4,2));
                switch(AA)
                {
                    case 0:
                        atoms.Add(new Atom("RS Status", 0, "Faulted"));
                        atoms.Add(new Atom("RS Operational", 0, "Offline"));
                        break;
                    case 1:
                        atoms.Add(new Atom("RS Status", 0, "Faulted"));
                        atoms.Add(new Atom("RS Operational", 1, "Online"));
                        break;
                    case 2:
                        atoms.Add(new Atom("RS Status", 1, "Good"));
                        atoms.Add(new Atom("RS Operational", 0, "Offline"));
                        break;
                    case 3:
                        atoms.Add(new Atom("RS Status", 1, "Good"));
                        atoms.Add(new Atom("RS Operational", 1, "Online"));
                        break;
                }
                AA = Convert.ToInt32(this.rawList[Offset].PadLeft(8, '0').Substring(1, 3));
                switch (AA)
                {
                    case 0:;
                        break;
                    case 1:
                        atoms.Add(new Atom("Transmitter 1090MHz", 1, "Present"));
                        break;
                    case 2:
                        atoms.Add(new Atom("Transmitter 1030MHz", 1, "Present"));
                        break;
                    case 3:
                        atoms.Add(new Atom("Transmitter 1090MHz", 1, "Present"));
                        atoms.Add(new Atom("Transmitter 1030MHz", 1, "Present"));
                        break;
                    case 4:
                        atoms.Add(new Atom("Receiver 1090MHz", 1, "Present"));
                        break;
                    case 5:
                        atoms.Add(new Atom("Transmitter 1090MHz", 1, "Present"));
                        atoms.Add(new Atom("Receiver 1090MHz", 1, "Present"));
                        break;
                    case 6:
                        atoms.Add(new Atom("Transmitter 1090MHz", 1, "Present"));
                        atoms.Add(new Atom("Receiver 1090MHz", 1, "Present"));
                        break;
                    case 7:
                        atoms.Add(new Atom("Transmitter 1090MHz", 1, "Present"));
                        atoms.Add(new Atom("Transmitter 1030MHz", 1, "Present"));
                        atoms.Add(new Atom("Receiver 1090MHz", 1, "Present"));
                        break;
                }
                Offset++;
            }
            return atoms;
        }
    }
}
