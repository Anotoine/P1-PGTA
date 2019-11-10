using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASTERIX
{
    public class CAT20
    {
        //Message stuff
        private List<bool> listFSPEC;
        private List<string> rawList;
        private int Offset;

        //DataItem
        internal List<Atom> DI010 { get; set; }
        internal List<Atom> DI020 { get; set; }
        internal List<Atom> DI030 { get; set; }
        internal Point DI041 { get; set; }
        internal Point DI042 { get; set; }
        internal List<Atom> DI050 { get; set; }
        internal List<Atom> DI070 { get; set; }
        internal List<Atom> DI090 { get; set; }
        internal List<Atom> DI100 { get; set; }
        internal float DI110 { get; set; }
        internal float DI105 { get; set; }
        internal DateTime DI140 { get; set; }
        internal int DI161 { get; set; }
        internal List<Atom> DI170 { get; set; }
        internal List<Atom> DI202 { get; set; }
        internal List<Atom> DI210 { get; set; }
        internal string DI220 { get; set; }
        internal List<Atom> DI230 { get; set; }
        internal List<Atom> DI245 { get; set; }
        internal List<Atom> DI250 { get; set; }
        internal List<Atom> DI260 { get; set; }
        internal string DI300 { get; set; }
        internal string DI310 { get; set; }
        internal List<string> DI400 { get; set; }
        internal List<Atom> DI500 { get; set; }

        public CAT20(List<string> rawList, List<bool> listFSPEC, int Offset)
        {
            this.listFSPEC = listFSPEC;
            this.rawList = rawList;
            this.Offset = Offset;
        }

        public CAT20 decode()
        {
            if (this.listFSPEC[1]) //I020/010
                decodeSACSIC();
            if (this.listFSPEC[2]) //I020/020
                decodeTRD();
            if (this.listFSPEC[3]) //I020/140
                decodeTOD();
            if (this.listFSPEC[4]) //I020/041
                decodeLatLong();
            if (this.listFSPEC[5]) //I020/042
                decodeXY();
            if (this.listFSPEC[6]) //I020/161
                decodeTrackNumber();
            if (this.listFSPEC[7]) //I020/170
                decodeTrackStatus();
            if (this.listFSPEC[8])
            {
                if (this.listFSPEC[9]) //8 I020/070
                    decodeM3A();
                if (this.listFSPEC[10]) //9 I020/202
                    decodeTrackVel();
                if (this.listFSPEC[11]) //10 I020/090
                    decodeFL();
                if (this.listFSPEC[12]) //11 I020/100
                    decodeModeC(); //TODO
                if (this.listFSPEC[13]) //12 I020/220
                   decodeICAOAddress();
                if (this.listFSPEC[14]) //13 I020/245
                    decodeCallSign();
                if (this.listFSPEC[15]) //14 I020/110
                    decodeMheight();
                if (this.listFSPEC[16])
                {
                    if (this.listFSPEC[17]) //15 I020/105
                        decodeGheight();
                    if (this.listFSPEC[18]) //16 I020/210
                        decodeCalAcc();
                    if (this.listFSPEC[19]) //17 I020/300
                        decodeVehicleId();
                    if (this.listFSPEC[20]) //18 I020/310
                        decodePPmes();
                    if (this.listFSPEC[21]) //19 I020/500
                        decodePosAcu();
                    if (this.listFSPEC[22]) //20 I020/400
                        decodeReceivers();
                    if (this.listFSPEC[23]) //21 I020/250
                        decodeMSdata();
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

            return this;
        }
        private void decodeSACSIC()
        {
            this.DI010 = new List<Atom>();
            List<string> ls = new List<string>() { "SAC", "SIC" };
            for (int i = 0; i < 2; i++)
            {
                this.DI010.Add(new Atom(ls[i], Convert.ToInt32(this.rawList[Offset], 16), Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16)).PadLeft(3, '0')));
                Offset++;
            }
        }

        private void decodeTRD()
        {
            this.DI020 = new List<Atom>();
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
                        this.DI020.Add(new Atom(ls[(i * 8) + j], 1, ls1[(i * 8) + j]));

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

        private void decodeTOD()
        {
            int LSB = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            Offset += 3;

            this.DI140 = new DateTime().AddSeconds((float)LSB / 128);
        }

        private void decodeLatLong()
        {
            int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), System.Globalization.NumberStyles.HexNumber);
            float latreal = Convert.ToSingle(lat * 180 / Math.Pow(2, 25));

            int lon = Int32.Parse(string.Concat(this.rawList[Offset + 4], this.rawList[Offset + 5], this.rawList[Offset + 6], this.rawList[Offset + 7]), System.Globalization.NumberStyles.HexNumber);
            float lonreal = Convert.ToSingle(lon * 180 / Math.Pow(2, 25));

            Offset += 8;

            this.DI041 = new Point().LatLong2XY(latreal, lonreal);
        }

        private void decodeXY()
        {
            string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), 16), 2).PadLeft(24, '0');
            int x = Convert.ToInt32(s.PadLeft(32, s[0]), 2);

            s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset + 3], this.rawList[Offset + 4], this.rawList[Offset + 5]), 16), 2).PadLeft(24, '0');
            int y = Convert.ToInt32(s.PadLeft(32, s[0]), 2);
            Offset += 6;
            this.DI042 = new Point().XY2LatLong((float)x / 2, (float)y / 2);
        }

        private void decodeTrackNumber()
        {
            this.DI161 = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
            Offset += 2;
        }

        private void decodeTrackStatus()
        {
            this.DI170 = new List<Atom>();
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
                                this.DI170.Add(new Atom(ls[cont1], 1, ls1[cont2 + 1]));
                            else
                                this.DI170.Add(new Atom(ls[cont1], 0, ls1[cont2]));
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

        private void decodeM3A()
        {
            this.DI070 = new List<Atom>();
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
                    this.DI070.Add(a);
                }
                else if (j == 1)
                {
                    if (char.Equals(s[j], '0'))
                        a = new Atom("G", 0, "Default");
                    else
                        a = new Atom("G", 1, "Garbled code");
                    j++;
                    this.DI070.Add(a);
                }
                else if (j == 2)
                {
                    if (char.Equals(s[j], '0'))
                        a = new Atom("L", 0, "Mode-3/A code derived from the reply of the transpoder");
                    else
                        a = new Atom("L", 1, "Mode-3/A code not extracted during the last update period");
                    j++;
                    this.DI070.Add(a);
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
            this.DI070.Add(new Atom("Mode-3/A reply", Convert.ToInt32(code), code));
        }

        private void decodeTrackVel()
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

        private void decodeFL()
        {
            this.DI090 = new List<Atom>();
            string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
            int VG = Convert.ToInt32(s.Substring(0, 2), 2);

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

        private void decodeModeC()
        {

        }

        private void decodeICAOAddress()
        {
            this.DI220 = string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]).ToUpper();
            Offset += 3;
        }

        private void decodeCallSign()
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

            List<char> code = new List<char>();
            for (int i = 0; i < 8; i++)
            {
                if (stringCode.Substring(6 * i, 6).StartsWith("0"))
                    code.Add((char)Convert.ToInt32(string.Concat("01", stringCode.Substring(6 * i, 6)), 2));
                else
                    code.Add((char)Convert.ToInt32(string.Concat("00", stringCode.Substring(6 * i, 6)), 2));
            }
            this.DI245.Add(new Atom("Callsign", 0, Regex.Replace(string.Join("", code.ToArray()), @"\s", "")));

        }

        private void decodeMheight()
        {
            this.DI110 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 2) * 6.25);
            Offset += 2;
        }

        private void decodeGheight()
        {
            this.DI105 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 2) * 6.25);
            Offset += 2;
        }

        private void decodeCalAcc()
        {
            this.DI210 = new List<Atom>();
            List<string> ls = new List<string>() { "A_x", "A_y" };
            for (int i = 0; i < 2; i++)
            {
                float BB = Convert.ToSingle(Convert.ToInt16(this.rawList[Offset], 2) * 0.25);
                this.DI210.Add(new Atom(ls[i], BB, Convert.ToString(BB)));
                Offset++;
            }
        }

        private void decodeVehicleId()
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

        private void decodePPmes()
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
                    break;
            }
            Offset++;
        }

        private void decodePosAcu()
        {
            this.DI500 = new List<Atom>();
            string BB = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
            Offset++;

            if (char.Equals(BB[0], '0'))
                this.DI500.Add(new Atom("DOP", 0, "Absence of Subfield 1"));
            else
            {
                this.DI500.Add(new Atom("DOP", 1, "Pressence of Subfield 1"));

                List<string> ls = new List<string>() { "DOP_x", "DOP_y", "DOP_xy" };
                for (int i = 0; i < 3; i++)
                {
                    float DOP = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.25);
                    this.DI500.Add(new Atom(ls[i], DOP, Convert.ToString(DOP)));
                    Offset += 2;
                }
            }

            if (char.Equals(BB[1], '0'))
                this.DI500.Add(new Atom("SDP", 0, "Absence of Subfield 2"));
            else
            {
                this.DI500.Add(new Atom("SDP", 1, "Presence of Subfield 2"));
                List<string> ls = new List<string>() { "sigma_x", "sigma_y", "sigma_xy" };
                for (int i = 0; i < 3; i++)
                {
                    float DOP = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.25);
                    this.DI500.Add(new Atom(ls[i], DOP, Convert.ToString(DOP)));
                    Offset += 2;
                }
            }

            if (char.Equals(BB[2], '0'))
                this.DI500.Add(new Atom("SDH", 0, "Absence of Subfield 3"));
            else
            {
                this.DI500.Add(new Atom("SDH", 1, "Presence of Subfield 3"));
                float DOP = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.5);
                this.DI500.Add(new Atom("sigma_GH", DOP, Convert.ToString(DOP)));
                Offset += 2;
            }
        }

        private void decodeReceivers()
        {
            this.DI400 = new List<string>();
            //Amount of Octets that will extent this camp (REP)
            int REP = Convert.ToInt32(this.rawList[Offset], 16);
            Offset++;

            for (int i = 0; i < (REP - 1); i++)
            {
                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                for (int j = 0; j < 8; j++)
                {
                    if (char.Equals(s[j], '1'))
                        this.DI400.Add(Convert.ToString(8 * (REP - i) - j));
                }
                Offset++;
            }
        }

        private void decodeMSdata()
        {
            this.DI250 = new List<Atom>();
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
    }
}
