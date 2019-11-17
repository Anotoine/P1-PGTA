using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ideafix
{
    class CAT10
    {
        //Message stuff
        private readonly List<bool> listFSPEC;
        private readonly List<string> rawList;
        private int Offset;

        //DataItem
        internal string DI000 { get; set; }
        internal List<Atom> DI010 { get; set; }
        internal List<Atom> DI020 { get; set; }
        internal Point DI040 { get; set; }
        internal Point DI041 { get; set; }
        internal Point DI042 { get; set; }
        internal List<Atom> DI060 { get; set; }
        internal List<Atom> DI090 { get; set; }
        internal float DI091 { get; set; }
        internal float DI131 { get; set; }
        internal DateTime DI140 { get; set; }
        internal int DI161 { get; set; }
        internal List<Atom> DI170 { get; set; }
        internal List<Atom> DI200 { get; set; }
        internal List<Atom> DI202 { get; set; }
        internal List<Atom> DI210 { get; set; }
        internal string DI220 { get; set; }
        internal List<Atom> DI245 { get; set; }
        internal List<Atom> DI250 { get; set; }
        internal List<Atom> DI270 { get; set; }
        internal List<Atom> DI280 { get; set; }
        internal string DI300 { get; set; }
        internal string DI310 { get; set; }
        internal List<Atom> DI500 { get; set; }
        internal List<Atom> DI550 { get; set; }

        //increment de SNR a ARP
        double incX, incY;

        public CAT10(List<string> rawList, List<bool> listFSPEC, int Offset)
        {
            this.listFSPEC = listFSPEC;
            this.rawList = rawList;
            this.Offset = Offset;

            Point SMR = new Point().FromLatLong(41.29562, 2.095114); //SMR point
            incX = SMR.X;
            incY = SMR.Y;
        }

        public CAT10 Decode()
        {
            if (this.listFSPEC[1]) //I010/010
                DecodeSACSIC();
            if (this.listFSPEC[2]) //I010/000
                DecodeMessageType();
            if (this.listFSPEC[3]) //I010/020          
                DecodeTargetDescriptor();
            if (this.listFSPEC[4]) //I010/140
                DecodeTOD();
            if (this.listFSPEC[5]) //I010/041
               DecodeLatLong();
            if (this.listFSPEC[6]) //I010/040
               DecodePolarCoordinates();
            if (this.listFSPEC[7]) //I010/042
               DecodeXY();
            if (this.listFSPEC[8]) //FX
            {
                if (this.listFSPEC[9])  //I010/200
                    DecodeTrackVelPolar();
                if (this.listFSPEC[10])  //I010/202
                    DecodeTrackVel();
                if (this.listFSPEC[11])  //I010/161
                    DecodeTrackNumber();
                if (this.listFSPEC[12])  //I010/170
                    DecodeTrackStatus();
                if (this.listFSPEC[13]) //I010/060
                    DecodeM3A();
                if (this.listFSPEC[14]) //I010/220
                    DecodeICAOAddress();
                if (this.listFSPEC[15]) //I010/245
                    DecodeCallSign();
                if (this.listFSPEC[16]) //FX
                {
                    if (this.listFSPEC[17])  //I010/250
                        DecodeMSdata();
                    if (this.listFSPEC[18])  //I010/300
                        DecodeVehicleId();
                    if (this.listFSPEC[19])  //I010/090
                        DecodeFL();
                    if (this.listFSPEC[20])  //I010/091
                        DecodeMheight();
                    if (this.listFSPEC[21])  //I010/270
                        DecodeTargetSize();
                    if (this.listFSPEC[22])  //I010/550
                        DecodeSysStats();
                    if (this.listFSPEC[23])  //I010/310
                        DecodePPmes();
                    if (this.listFSPEC[24]) //FX
                    {
                        if (this.listFSPEC[25])  //I010/500
                            DecodeStandardDeviationPosition();
                        if (this.listFSPEC[25])  //I010/280
                            DecodePresence();
                        if (this.listFSPEC[26])  //I010/131
                            DecodeAmplitudePrimaryPlot();
                        if (this.listFSPEC[27])  //I010/210
                            DecodeCalculatedAcceleration();
                    }
                }
            }
            return this;
        }
        private void DecodeSACSIC()
        {
            this.DI010 = new List<Atom>();
            List<string> ls = new List<string>() { "SAC", "SIC" };
            for (int i = 0; i < 2; i++)
            {
                this.DI010.Add(new Atom(ls[i], Convert.ToInt32(this.rawList[Offset], 16), Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16)).PadLeft(3, '0')));
                Offset++;
            }
        }

        private void DecodeMessageType()
        {
            int code = Convert.ToInt32(this.rawList[Offset], 16);
            switch (code)
            {
                case 1:
                    this.DI000 = "Target Report";
                    break;
                case 2:
                    this.DI000 = "Start of Update Cycle";
                    break;
                case 3:
                    this.DI000 = "Periodic Status Message";
                    break;
                case 4:
                    this.DI000 = "Event-triggered Status Message";
                    break;
            }
            ++Offset;
        }

        private void DecodeTargetDescriptor()
        {
            var ls = new List<string> { "TYP", "DCR", "CHN", "GBS", "CRT", "FX", "SIM", "TST", "RAB", "LOP", "TOT", "FX", "SPI", "FX" };

            var ls0 = new List<string> { "SSR multilateration", "Mode S multilateration","ADS-B", "PSR", "No differential correction(ADS-B)","Chain 1","Transponder Ground bit not set","No Corrupted reply in multilateration",
                                               "End of Data Item","Actual target report","Default","Report from target transponder","Undetermined","Loop start","Undetermined","Aircraft","End of Data Item","Absence of SPI","End of Data Item" };

            var ls1 = new List<string> { "Magnetic Loop System","HF multilateration", "Not defined","Other types","Differential correction (ADS-B)", "Chain 2","Transponder Ground bit set","Corrupted replies in multilateration",
                                             "Extension into first extent","Simulated target report","Test Target","Report from field monitor(fixed transponder)","Loop finish","","Ground vehicle","Helicopter","Extension into next extent",
                                             "Special Position Identification","Extension into next extent"};

            List<Atom> atoms = new List<Atom>();
            Atom a;
            int cont1 = 0;
            int cont2 = 0;
            int i = 1;
            bool exit = false;
            while (!exit)
            {
                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                for (int j = 0; j < 8; j++)
                {
                    if (i == 1)
                    {
                        if (char.Equals(s[j], '1'))
                        {
                            j++;
                            if (char.Equals(s[j], '1'))
                            {
                                j++;
                                if (char.Equals(s[j], '1'))
                                    a = new Atom(ls[cont1], 7, ls1[cont2 + 3]);
                                else
                                    a = new Atom(ls[cont1], 6, ls1[cont2 + 2]);
                            }
                            else
                            {
                                j++;
                                if (char.Equals(s[j], '1'))
                                    a = new Atom(ls[cont1], 5, ls1[cont2 + 1]);
                                else
                                    a = new Atom(ls[cont1], 4, ls1[cont2]);
                            }
                        }
                        else
                        {
                            j++;
                            if (char.Equals(s[j], '1'))
                            {
                                j++;
                                if (char.Equals(s[j], '1'))
                                    a = new Atom(ls[cont1], 3, ls0[cont2 + 3]);
                                else
                                    a = new Atom(ls[cont1], 2, ls0[cont2 + 2]);
                            }
                            else
                            {
                                j++;
                                if (char.Equals(s[j], '1'))
                                    a = new Atom(ls[cont1], 1, ls0[cont2 + 1]);
                                else
                                    a = new Atom(ls[cont1], 0, ls0[cont2]);
                            }
                        }
                        cont2 = 3;
                    }
                    else if (i == 10)
                    {
                        if (char.Equals(s[j], '0'))
                        {
                            j++;
                            if (char.Equals(s[j], '0'))
                                a = new Atom(ls[cont1], 0, ls0[cont2]);
                            else
                                a = new Atom(ls[cont1], 1, ls0[cont2 + 1]);

                        }
                        else
                        {
                            a = new Atom(ls[cont1], 2, ls1[cont2]);
                            j++;
                        }
                        cont2++;
                    }
                    else if (i == 11)
                    {
                        if (char.Equals(s[j], '0'))
                        {
                            j++;
                            if (char.Equals(s[j], '0'))
                                a = new Atom(ls[cont1], 0, ls0[cont2]);
                            else
                                a = new Atom(ls[cont1], 1, ls0[cont2 + 1]);
                        }
                        else
                        {
                            j++;
                            if (char.Equals(s[j], '0'))
                                a = new Atom(ls[cont1], 2, ls1[cont2]);
                            else
                                a = new Atom(ls[cont1], 3, ls1[cont2 + 1]);
                        }
                        j++;
                        cont2++;
                    }
                    else if (char.Equals(s[j], '1'))
                        a = new Atom(ls[cont1], 1, ls1[cont2]);
                    else
                        a = new Atom(ls[cont1], 0, ls0[cont2]);

                    cont1++;
                    cont2++;
                    i++;
                    atoms.Add(a);

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
            this.DI020 = atoms;
        }

        private void DecodeTOD()
        {
            int LSB = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            Offset += 3;

            this.DI140 = new DateTime().AddSeconds((float)LSB / 128);
        }

        private void DecodeLatLong()
        {
            int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), System.Globalization.NumberStyles.HexNumber);
            float latreal = Convert.ToSingle(lat * 180 / Math.Pow(2, 31));

            int lon = Int32.Parse(string.Concat(this.rawList[Offset + 4], this.rawList[Offset + 5], this.rawList[Offset + 6], this.rawList[Offset + 7]), System.Globalization.NumberStyles.HexNumber);
            float lonreal = Convert.ToSingle(lon * 180 / Math.Pow(2, 31));

            Offset += 8;

            this.DI041 = new Point().FromLatLong(latreal, lonreal);
        }

        private void DecodePolarCoordinates()
        {
            int RHO = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
            float RHOreal = Convert.ToSingle(RHO);

            int Theta = Int32.Parse(string.Concat(this.rawList[Offset + 2], this.rawList[Offset + 3]), System.Globalization.NumberStyles.HexNumber);
            float Thetareal = Convert.ToSingle(Theta * 360 / Math.Pow(2,16));
            

            Offset += 4;

            this.DI040 = new Point().FromPolar(RHOreal, Thetareal,incX, incY);
        }
        
        private void DecodeXY()
        {
            int x = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
            int y = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
            Offset += 4;
            this.DI042 = new Point().FromCartesian(x, y);
        }

        private void DecodeTrackVelPolar()
        {
            this.DI200 = new List<Atom>();
            int speed = Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16);
            this.DI200.Add(new Atom("Speed", Convert.ToSingle(speed * Math.Pow(2, -14)), Convert.ToString((float)speed * Math.Pow(2, -14))));
            Offset += 2;
            int TA = Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16);
            this.DI200.Add(new Atom("Track Angle", Convert.ToSingle(TA * 360/Math.Pow(2, 16)), Convert.ToString((float)TA * 360/Math.Pow(2, 16))));
            Offset += 2;
        }
        
        private void DecodeTrackVel()
        {
            this.DI202 = new List<Atom>();
            List<string> ls = new List<string>() { "V_x", "V_y" };
            for (int i = 0; i < 2; i++)
            {
                int BB = Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16);
                this.DI202.Add(new Atom(ls[i], (float)BB / 4, Convert.ToString((float)BB / 4)));
                Offset += 2;
            }
        }

        private void DecodeTrackNumber()
        {
            this.DI161 = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
            Offset += 2;
        }

        private void DecodeTrackStatus()
        {
            var ls = new List<string> { "CNF", "TRE", "CST", "MAH", "TCC", "STH", "FX", "TOM", "DOU", "MRS", "FX", "GHO", "FX" };
            var ls0 = new List<string> { "Confirmed track", "Default", "Not extrapolation", "Predictable extrapolation due to sensor refresh period",
                "Default", "Tracking performed in 'Sensor Plane'", "Measured position", "End of Data Item"};
            var ls1 = new List<string> { "Track in initiation phase", "Last report for a track", "Predictable extrapolation in masked area", "Extrapolation due to unpredictable absence of detection",
                "Horizontal Manoueuvre", "Slant range correction and a suitable projection technique are used to track in a 2D.reference plane, tangential to the earth model at the Sensor Site co-ordinates.",
                "Smoothed position", "Extension into first extent"};

            this.DI170 = new List<Atom>();
            int cont1 = 0;
            int cont2 = 0;
            int i = 0;
            bool exit = false;
            while (!exit)
            {
                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                for (int j = 0; j < 8; j++)
                {
                    if (j == 2 && i == 0)
                    {
                        if (char.Equals(s[j], '1'))
                        {
                            j++;
                            if (char.Equals(s[j], '1'))
                                this.DI170.Add(new Atom(ls[cont1], 3, ls1[cont2 + 1]));
                            else
                                this.DI170.Add(new Atom(ls[cont1], 2, ls1[cont2]));
                        }
                        else
                        {
                            j++;
                            if (char.Equals(s[j], '1'))
                                this.DI170.Add(new Atom(ls[cont1], 1, ls0[cont2 + 1]));
                            else
                                this.DI170.Add(new Atom(ls[cont1], 0, ls0[cont2]));
                        }
                        cont2++;

                    }
                    else if (i == 1)
                    {
                        int code = Convert.ToInt16(s.Substring(0, 2));
                        switch (code)
                        {
                            case 0:
                                this.DI170.Add(new Atom("TOM", 0, "Unkown type of movement"));
                                break;
                            case 1:
                                this.DI170.Add(new Atom("TOM", 1, "Taking-off"));
                                break;
                            case 2:
                                this.DI170.Add(new Atom("TOM", 2, "Landing"));
                                break;
                            case 3:
                                this.DI170.Add(new Atom("TOM", 3, "Other types of movements"));
                                break;
                        }
                        code = Convert.ToInt16(s.Substring(2, 3));
                        switch (code)
                        {
                            case 0:
                                this.DI170.Add(new Atom("DOU", 0, "No doubt"));
                                break;
                            case 1:
                                this.DI170.Add(new Atom("DOU", 1, "Doubtfull correlation (undertemined reaseon)"));
                                break;
                            case 2:
                                this.DI170.Add(new Atom("DOU", 2, "Doubtfull correlation in clutter"));
                                break;
                            case 3:
                                this.DI170.Add(new Atom("DOU", 3, "Loss of accuracy"));
                                break;
                            case 4:
                                this.DI170.Add(new Atom("DOU", 4, "Loss of accuracy in clutter"));
                                break;
                            case 5:
                                this.DI170.Add(new Atom("DOU", 5, "Untable track"));
                                break;
                            case 6:
                                this.DI170.Add(new Atom("DOU", 6, "Previously coasted"));
                                break;
                        }
                        code = Convert.ToInt16(s.Substring(5, 2));
                        switch (code)
                        {
                            case 0:
                                this.DI170.Add(new Atom("MRS", 0, "Merge or split indiacation undetermined"));
                                break;
                            case 1:
                                this.DI170.Add(new Atom("MRS", 1, "Track merged by association to plot"));
                                break;
                            case 2:
                                this.DI170.Add(new Atom("MRS", 2, "Track merged by non-association to plot"));
                                break;
                            case 3:
                                this.DI170.Add(new Atom("MRS", 3, "Split track"));
                                break;
                        }
                    }
                    else if (i == 2)
                    {
                        if (s[0].Equals('0'))
                            this.DI170.Add(new Atom("GHO", 0, "Default"));
                        else
                            this.DI170.Add(new Atom("GHO", 0, "Ghost track"));
                    }
                    else if (char.Equals(s[j], '1'))
                        this.DI170.Add(new Atom(ls[cont1], 1, ls1[cont2]));
                    else
                        this.DI170.Add(new Atom(ls[cont1], 0, ls0[cont2]));

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
        }

        private void DecodeM3A()
        {
            Atom a;
            this.DI060 = new List<Atom>();
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
                    this.DI060.Add(a);
                }
                else if (j == 1)
                {
                    if (char.Equals(s[j], '0'))
                        a = new Atom("G", 0, "Default");
                    else
                        a = new Atom("G", 1, "Garbled code");
                    j++;
                    this.DI060.Add(a);
                }
                else if (j == 2)
                {
                    if (char.Equals(s[j], '0'))
                        a = new Atom("L", 0, "Mode-3/A code derived from the reply of the transpoder");
                    else
                        a = new Atom("L", 1, "Mode-3/A code not extracted during the last update period");
                    j++;
                    this.DI060.Add(a);
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
            this.DI060.Add(new Atom("Mode-3/A reply", Convert.ToInt32(code), code));
        }

        private void DecodeICAOAddress()
        {
            this.DI220 = string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]).ToUpper();
            Offset += 3;
        } 

        private void DecodeCallSign()
        {
            this.DI245 = new List<Atom>();
            //First decoding STI on first Byte
            int STI = Convert.ToInt32(this.rawList[Offset], 16);
            Offset++;

            switch (STI)
            {
                case 0:
                    this.DI245.Add(new Atom("STI", 00, "Callsign or registration not downlinked from transponder"));
                    break;
                case 64:
                    this.DI245.Add(new Atom("STI", 01, "Registration downlinked from transponder"));
                    break;
                case 128:
                    this.DI245.Add(new Atom("STI", 10, "Callsign donwlinked from transponder"));
                    break;
                case 192:
                    this.DI245.Add(new Atom("STI", 11, "Not defined"));
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

            string code = "";
            for (int i = 0; i < 8; i++)
            {
                if (!stringCode.Substring(6 * i, 6).Equals("000000"))
                {
                    if (stringCode.Substring(6 * i, 6).StartsWith("0"))
                    {
                        byte[] BB = new byte[] { Convert.ToByte(string.Concat("01", stringCode.Substring(6 * i, 6)), 2) };
                        code += Encoding.UTF8.GetString(BB);
                    }
                    else
                    {
                        byte[] BB = new byte[] { Convert.ToByte(string.Concat("00", stringCode.Substring(6 * i, 6)), 2) };
                        code += Encoding.UTF8.GetString(BB);
                    }
                }
            }
            this.DI245.Add(new Atom("Callsign", 0, code));
        }

        private void DecodeMSdata()
        {
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
                        this.DI250.Add(new Atom("BDS1", 4, this.rawList[Offset].Substring(0, 4)));
                        this.DI250.Add(new Atom("BDS2", 4, this.rawList[Offset].Substring(4, 4)));
                    }
                    Offset++;
                }
                this.DI250.Add(new Atom("MB DATA", s.Length, s));
            }
        }

        private void DecodeVehicleId()
        {
            switch (Convert.ToInt16(this.rawList[Offset], 2))
            {
                case 0:
                    this.DI300 = "Unknown";
                    break;
                case 1:
                    this.DI300 = "ATC equipment maintenance";
                    break;
                case 2:
                    this.DI300 = "Airport maintenance";
                    break;
                case 3:
                    this.DI300 = "Fire";
                    break;
                case 4:
                    this.DI300 = "Bird scarer";
                    break;
                case 5:
                    this.DI300 = "Snow plough";
                    break;
                case 6:
                    this.DI300 = "Runway sweeper";
                    break;
                case 7:
                    this.DI300 = "Emergency";
                    break;
                case 8:
                    this.DI300 = "Police";
                    break;
                case 9:
                    this.DI300 = "Bus";
                    break;
                case 10:
                    this.DI300 = "Tug (push/tow)";
                    break;
                case 11:
                    this.DI300 = "Grass cutter";
                    break;
                case 12:
                    this.DI300 = "Fuel";
                    break;
                case 13:
                    this.DI300 = "Baggage";
                    break;
                case 14:
                    this.DI300 = "Catering";
                    break;
                case 15:
                    this.DI300 = "Aircraft maintenance";
                    break;
                case 16:
                    this.DI300 = "Flyco (follow me)";
                    break;
                default:
                    break;
            }
            Offset++;
        }

        private void DecodeFL()
        {
            string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
            int VG = Convert.ToInt32(s.Substring(0, 2), 2);
            this.DI090 = new List<Atom>();

            switch (VG)
            {
                case 0:
                    this.DI090.Add(new Atom("V", 0, "Code Validated"));
                    this.DI090.Add(new Atom("G", 0, "Default"));
                    break;
                case 1:
                    this.DI090.Add(new Atom("V", 0, "Code Validated"));
                    this.DI090.Add(new Atom("G", 1, "Garbled code"));
                    break;
                case 2:
                    this.DI090.Add(new Atom("V", 1, "Code not Validated"));
                    this.DI090.Add(new Atom("G", 0, "Default"));
                    break;
                case 3:
                    this.DI090.Add(new Atom("V", 1, "Code not Validated"));
                    this.DI090.Add(new Atom("G", 1, "Garbled code"));
                    break;
                default:
                    this.DI090.Add(new Atom());
                    this.DI090.Add(new Atom());
                    break;
            }

            int BB = Convert.ToInt16(s.Substring(2).PadLeft(16, '0'), 2);
            this.DI090.Add(new Atom("Fligth Level", (float)BB / 4, Convert.ToString((float)BB / 4)));

            Offset += 2;
        }

        private void DecodeMheight()
        {
            this.DI091 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 2) * 6.25);
            Offset += 2;
        }

        private void DecodeTargetSize()
        {
            this.DI270 = new List<Atom>();
            string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
            int Length = Convert.ToInt32(string.Concat(s[0], s[1], s[2], s[3], s[4], s[5], s[6]), 2);
            this.DI270.Add(new Atom("Length", Length, Convert.ToString((float)Length * 1)));
            Offset++;
            if (s[7] == '1')
            {
                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                int Orientation = Convert.ToInt32(string.Concat(s[0], s[1], s[2], s[3], s[4], s[5], s[6]), 2);
                this.DI270.Add(new Atom("Orientation", (float)Orientation * (360 / 128), Convert.ToString((float)Orientation * (360 / 128))));
                Offset++;
                if (s[7] == '1')
                {
                    s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                    int Width = Convert.ToInt32(string.Concat(s[0], s[1], s[2], s[3], s[4], s[5], s[6]), 2);
                    this.DI270.Add(new Atom("Width", Width, Convert.ToString((float)Width * 1)));
                    Offset++;
                }
            }
        }

        private void DecodeSysStats()
        {
            this.DI550 = new List<Atom>();
            switch (Convert.ToInt32(this.rawList[Offset].PadLeft(8, '0').Substring(0, 5)))
            {
                case 0:
                    this.DI550.Add(new Atom("NOGO", 00, "Operational"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 1:
                    this.DI550.Add(new Atom("NOGO", 00, "Operational"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 2:
                    this.DI550.Add(new Atom("NOGO", 00, "Operational"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 3:
                    this.DI550.Add(new Atom("NOGO", 00, "Operational"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 4:
                    this.DI550.Add(new Atom("NOGO", 00, "Operational"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 5:
                    this.DI550.Add(new Atom("NOGO", 00, "Operational"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 6:
                    this.DI550.Add(new Atom("NOGO", 00, "Operational"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 7:
                    this.DI550.Add(new Atom("NOGO", 00, "Operational"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 8:
                    this.DI550.Add(new Atom("NOGO", 01, "Degraded"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 9:
                    this.DI550.Add(new Atom("NOGO", 01, "Degraded"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 10:
                    this.DI550.Add(new Atom("NOGO", 01, "Degraded"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 11:
                    this.DI550.Add(new Atom("NOGO", 01, "Degraded"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 12:
                    this.DI550.Add(new Atom("NOGO", 01, "Degraded"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 13:
                    this.DI550.Add(new Atom("NOGO", 01, "Degraded"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 14:
                    this.DI550.Add(new Atom("NOGO", 01, "Degraded"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 15:
                    this.DI550.Add(new Atom("NOGO", 01, "Degraded"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 16:
                    this.DI550.Add(new Atom("NOGO", 10, "NOGO"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 17:
                    this.DI550.Add(new Atom("NOGO", 10, "NOGO"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 18:
                    this.DI550.Add(new Atom("NOGO", 10, "NOGO"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 19:
                    this.DI550.Add(new Atom("NOGO", 10, "NOGO"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 20:
                    this.DI550.Add(new Atom("NOGO", 10, "NOGO"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 21:
                    this.DI550.Add(new Atom("NOGO", 10, "NOGO"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 22:
                    this.DI550.Add(new Atom("NOGO", 10, "NOGO"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 23:
                    this.DI550.Add(new Atom("NOGO", 10, "NOGO"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 24:
                    this.DI550.Add(new Atom("NOGO", 11, "undefined"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 25:
                    this.DI550.Add(new Atom("NOGO", 11, "undefined"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 26:
                    this.DI550.Add(new Atom("NOGO", 11, "undefined"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 27:
                    this.DI550.Add(new Atom("NOGO", 11, "undefined"));
                    this.DI550.Add(new Atom("OVL", 0, "No overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 28:
                    this.DI550.Add(new Atom("NOGO", 11, "undefined"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 29:
                    this.DI550.Add(new Atom("NOGO", 11, "undefined"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 0, "Valid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
                case 30:
                    this.DI550.Add(new Atom("NOGO", 11, "undefined"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 0, "Test Target Operative"));
                    break;
                case 31:
                    this.DI550.Add(new Atom("NOGO", 11, "undefined"));
                    this.DI550.Add(new Atom("OVL", 1, "Overload"));
                    this.DI550.Add(new Atom("TSV", 1, "Invalid"));
                    this.DI550.Add(new Atom("TTF", 1, "Test Target Failure"));
                    break;
            }
            Offset++;
        }

        private void DecodePPmes()
        {
            switch (Convert.ToInt32(this.rawList[Offset], 16))
            {
                case 1:
                    this.DI310 = "Default. Towing Aircraft.";
                    break;
                case 2:
                    this.DI310 = "Default. Follow me operation.";
                    break;
                case 3:
                    this.DI310 = "Default. Runway check.";
                    break;
                case 4:
                    this.DI310 = "Default. Emergency operation.";
                    break;
                case 5:
                    this.DI310 = "Default. Work in Progress.";
                    break;
                case 129:
                    this.DI310 = "In Trouble. Towing Aircraft.";
                    break;
                case 130:
                    this.DI310 = "In Trouble. Follow me operation.";
                    break;
                case 131:
                    this.DI310 = "In Trouble. Runway check.";
                    break;
                case 132:
                    this.DI310 = "In Trouble. Emergency operation.";
                    break;
                case 133:
                    this.DI310 = "In Trouble. Work in Progress.";
                    break;
                default:
                    this.DI310 = "Invalid information";
                    break;
            }
            Offset++;
        }

        private void DecodeStandardDeviationPosition()
        {
            float Ox = Convert.ToSingle(Convert.ToInt16(this.rawList[Offset], 2) * 0.25);
            Offset += 1;
            this.DI500.Add(new Atom("Ox", Ox, Convert.ToString(Ox)));
            float Oy = Convert.ToSingle(Convert.ToInt16(this.rawList[Offset], 2) * 0.25);
            Offset += 1;
            this.DI500.Add(new Atom("Oy", Oy, Convert.ToString(Oy)));
            float Oxy = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 2) * 0.25);
            Offset += 2;
            this.DI500.Add(new Atom("Oxy", Oxy, Convert.ToString(Oxy)));
        }

        private void DecodePresence()
        {
            int REP = Convert.ToInt16(this.rawList[Offset], 2);
            Offset += 1;
            this.DI280.Add(new Atom("REP", 0, Convert.ToString(REP)));
            int DRHO = Convert.ToInt16(this.rawList[Offset], 2);
            Offset += 1;
            this.DI280.Add(new Atom("DRHO", 0, Convert.ToString(DRHO)));
            float DTHETA = Convert.ToSingle(Convert.ToInt16(this.rawList[Offset], 2) * 0.15);
            Offset += 1;
            this.DI280.Add(new Atom("DTHETA", 0, Convert.ToString(DTHETA)));
        }

        private void DecodeAmplitudePrimaryPlot()
        {
            this.DI131 = Convert.ToInt16(this.rawList[Offset], 2);
            Offset++;
        }

        private void DecodeCalculatedAcceleration()
        {
            float ax = Convert.ToSingle(Convert.ToInt16(this.rawList[Offset], 2) * 0.25);
            Offset += 1;
            this.DI210.Add(new Atom("ax", 0, Convert.ToString(ax)));
            float ay = Convert.ToSingle(Convert.ToInt16(this.rawList[Offset], 2) * 0.25);
            Offset += 1;
            this.DI210.Add(new Atom("ay", 0, Convert.ToString(ay)));
        }
    }
}
