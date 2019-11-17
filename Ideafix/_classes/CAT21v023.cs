using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ideafix
{
    class CAT21v023
    {
        //Message stuff
        private List<bool> listFSPEC;
        private List<string> rawList;
        private int Offset;

        //DataItem
        internal List<Atom> DI010 { get; set; }
        internal string DI020 { get; set; }
        internal DateTime DI030 { get; set; }
        internal float DI032 { get; set; }
        internal List<Atom> DI040 { get; set; }
        internal string DI080 { get; set; }
        internal List<Atom> DI090 { get; set; }
        internal float DI091 { get; set; }
        internal string DI095 { get; set; }
        internal List<Atom> DI110 { get; set; }
        internal Point DI130 { get; set; }
        internal float DI140 { get; set; }
        internal int DI145 { get; set; }
        internal List<Atom> DI146 { get; set; }
        internal List<Atom> DI148 { get; set; }
        internal float DI150 { get; set; }
        internal float DI151 { get; set; }
        internal float DI152 { get; set; }
        internal float DI155 { get; set; }
        internal float DI157 { get; set; }
        internal List<Atom> DI160 { get; set; }
        internal List<Atom> DI165 { get; set; }
        internal string DI170 { get; set; }
        internal List<Atom> DI200 { get; set; }
        internal List<Atom> DI210 { get; set; }
        internal List<Atom> DI220 { get; set; }
        internal float DI230 { get; set; }


        public CAT21v023(List<string> rawList, List<bool> listFSPEC, int Offset)
        {
            this.listFSPEC = listFSPEC;
            this.rawList = rawList;
            this.Offset = Offset;
        }

        public CAT21v023 decode()
        {
            if (this.listFSPEC[1]) //I021/010
                decodeSACSIC();
            if (this.listFSPEC[2]) //I021/040
                decodeTRD();
            if (this.listFSPEC[3]) //I021/030
                decodeTOD();
            if (this.listFSPEC[4]) //I021/130
                decodeLatLong();
            if (this.listFSPEC[5]) //I021/080
                decodeICAOAddress();
            if (this.listFSPEC[6]) //I021/140
                decodeGAlt();
            if (this.listFSPEC[7]) //I021/090
                decodeFoM();
            if (this.listFSPEC[8])
            {
                if (this.listFSPEC[9]) //I021/210
                    decodeLinkTech();
                if (this.listFSPEC[10]) //I021/230
                    decodeRollAngle();
                if (this.listFSPEC[11]) //I021/145
                    decodeFL();
                if (this.listFSPEC[12]) //I021/150
                    decodeAirSpeed();
                if (this.listFSPEC[13]) //I021/151
                    decodeTrueAirSpeed();
                if (this.listFSPEC[14]) //I021/152
                    decodeMagneticHeading();
                if (this.listFSPEC[15]) //I021/155
                    decodeBarometricVerticalRate();
                if (this.listFSPEC[16])
                {
                    if (this.listFSPEC[17]) //I021/157
                        decodeGVerticalRate();
                    if (this.listFSPEC[18]) //I021/160
                        decodeGroundVector();
                    if (this.listFSPEC[19]) //I021/165
                        decodeRateTurn();
                    if (this.listFSPEC[20]) //I021/170
                        decodeCallsign();
                    if (this.listFSPEC[21]) //I021/095
                        decodeVelAcc();
                    if (this.listFSPEC[22]) //I021/032
                        decodeTODAcc();
                    if (this.listFSPEC[23]) //I021/200
                        decodeTargetStats();
                    if (this.listFSPEC[24])
                    {
                        if (this.listFSPEC[25]) //I021/020
                            decodeEmmitterCat();
                        if (this.listFSPEC[26]) //I021/220
                            decodeMetReport();
                        if (this.listFSPEC[27]) //I021/146
                            decodeIntermediateStateSelAlt();
                        if (this.listFSPEC[28]) //I021/148
                            decodeFinalStateSelAlt();
                        if (this.listFSPEC[29]) //I021/110
                            decodeTrajectoryIntent();
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

        private void decodeTOD()
        {
            int LSB = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            Offset += 3;

            this.DI030 = new DateTime().AddSeconds((float)LSB / 128);
        }

        private void decodeLatLong()
        {
            int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            float latreal = Convert.ToSingle(lat * 180 / Math.Pow(2,23));

            int lon = Int32.Parse(string.Concat(this.rawList[Offset + 3], this.rawList[Offset + 4], this.rawList[Offset + 5]), System.Globalization.NumberStyles.HexNumber);
            float lonreal = Convert.ToSingle(lon * 180 / Math.Pow(2, 23));

            Offset += 6;

            this.DI130 = new Point().FromLatLong(latreal, lonreal);
        }

        private void decodeTRD()
        {
            var ls = new List<string> { "DCR", "GBS", "SIM", "TST", "RAB", "SAA", "SPI", "FX", "ATP", "ARC" };

            var ls0 = new List<string> { "No differential correction(ADS-B)","Ground bit not set","Actual target report","Default", "Report from target transponder",
                                              "Equipment not capable to provide Selected Altitude", "Absence of SPI", "Non unique address", "24-Bit ICAO address", "Unknown", "25 ft"};

            var ls1 = new List<string> { "Differential correction (ADS-B)", "Ground bit set", "Simulated target report","Test Target","Report from field monitor(fixed transponder)",
                                              "Equipment capable to provide Selected Altitude","Special Position Identification", "Surface vehicle address", "Anonymous address", "100 ft"};

            List<Atom> atoms = new List<Atom>();
            Atom a;
            int cont1 = 0;
            int cont2 = 0;

            string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
            for (int j = 0; j < 7; j++)
            {

                if (char.Equals(s[j], '1'))
                    a = new Atom(ls[cont1], 1, ls1[cont2]);
                else
                    a = new Atom(ls[cont1], 0, ls0[cont2]);

                cont1++;
                cont2++;
                atoms.Add(a);
            }
            Offset += 1;
            s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
            if (char.Equals(s[1], '1'))
            {
                if (char.Equals(s[2], '1'))
                    a = new Atom(ls[cont1], 3, ls1[cont2 + 1]);
                else
                    a = new Atom(ls[cont1], 2, ls1[cont2]);
                atoms.Add(a);
            }
            else if (char.Equals(s[1], '0'))
            {
                if (char.Equals(s[2], '1'))
                    a = new Atom(ls[cont1], 1, ls0[cont2 + 1]);
                else
                    a = new Atom(ls[cont1], 0, ls0[cont2]);
                atoms.Add(a);
            }
            cont1 += 2;
            cont2 += 2;
            if (char.Equals(s[3], '0'))
            {
                if (char.Equals(s[4], '1'))
                    a = new Atom(ls[cont1], 1, ls0[cont2 + 1]);
                else
                    a = new Atom(ls[cont1], 0, ls0[cont2]);
                atoms.Add(a);
            }
            else
            {
                a = new Atom(ls[cont1], 2, ls1[cont2]);
                atoms.Add(a);
            }
            Offset++;
            this.DI040 = atoms;
        }

        private void decodeICAOAddress()
        {
            this.DI080 = string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]).ToUpper();
            Offset += 3;
        }

        private void decodeGAlt()
        {
            int alt = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
            this.DI140 = Convert.ToSingle(alt * 6.25);
            Offset += 2;
        }

        private void decodeFoM()
        {
            string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
            int code = Int32.Parse(string.Concat(s[0], s[1]), System.Globalization.NumberStyles.HexNumber);
            List<Atom> atoms = new List<Atom>();
            Atom a;
            switch (code)
            {
                case 0:
                    a = new Atom("AC", 0, "unknown");
                    atoms.Add(a);
                    break;
                case 1:
                    a = new Atom("AC", 1, "ACAS not operational");
                    atoms.Add(a);
                    break;
                case 2:
                    a = new Atom("AC", 2, "ACAS operational");
                    atoms.Add(a);
                    break;
                case 3:
                    a = new Atom("AC", 3, "invalid");
                    atoms.Add(a);
                    break;
            }
            code = Int32.Parse(string.Concat(s[2], s[3]), System.Globalization.NumberStyles.HexNumber);
            switch (code)
            {
                case 0:
                    a = new Atom("MN", 0, "unknown");
                    atoms.Add(a);
                    break;
                case 1:
                    a = new Atom("MN", 1, "Multiple navigational aids not operating");
                    atoms.Add(a);
                    break;
                case 2:
                    a = new Atom("MN", 2, "Multiple navigational aids operating");
                    atoms.Add(a);
                    break;
                case 3:
                    a = new Atom("MN", 3, "invalid");
                    atoms.Add(a);
                    break;
            }
            code = Int32.Parse(string.Concat(s[4], s[5]), System.Globalization.NumberStyles.HexNumber);
            switch (code)
            {
                case 0:
                    a = new Atom("DC", 0, "unknown");
                    atoms.Add(a);
                    break;
                case 1:
                    a = new Atom("DC", 1, "Differential correction");
                    atoms.Add(a);
                    break;
                case 2:
                    a = new Atom("DC", 2, "No differential correction");
                    atoms.Add(a);
                    break;
                case 3:
                    a = new Atom("DC", 3, "invalid");
                    atoms.Add(a);
                    break;
            }
            Offset += 1;
            s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
            code = Int32.Parse(string.Concat(s[4], s[5], s[6], s[7]), System.Globalization.NumberStyles.HexNumber);
            a = new Atom("Position Accuracy", code, Convert.ToString(code));
            atoms.Add(a);
            Offset += 1;
            this.DI090 = atoms;
        }

        private void decodeLinkTech()
        {
            string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
            List<Atom> atoms = new List<Atom>();
            Atom a;
            int code = Convert.ToInt16(string.Concat(s[3]));
            switch (code)
            {
                case 0:
                    a = new Atom("Cockpit Display of Traffic Information", 0, "Unknown");
                    atoms.Add(a);
                    break;
                case 1:
                    a = new Atom("Cockpit Display of Traffic Information", 1, "Aircraft equiped with CDTI");
                    atoms.Add(a);
                    break;
            }
            code = Convert.ToInt16(string.Concat(s[4]));
            switch (code)
            {
                case 0:
                    a = new Atom("Mode-S Extended Squitter", 0, "Not used");
                    atoms.Add(a);
                    break;
                case 1:
                    a = new Atom("Mode-S Extended Squitter", 1, "Used");
                    atoms.Add(a);
                    break;
            }
            code = Convert.ToInt16(string.Concat(s[5]));
            switch (code)
            {
                case 0:
                    a = new Atom("UAT", 0, "Not used");
                    atoms.Add(a);
                    break;
                case 1:
                    a = new Atom("UAT", 1, "Used");
                    atoms.Add(a);
                    break;
            }
            code = Convert.ToInt16(string.Concat(s[6]));
            switch (code)
            {
                case 0:
                    a = new Atom("VDL Mode 4", 0, "Not used");
                    atoms.Add(a);
                    break;
                case 1:
                    a = new Atom("VDL Mode 4", 1, "Used");
                    atoms.Add(a);
                    break;
            }
            code = Convert.ToInt16(string.Concat(s[7]));
            switch (code)
            {
                case 0:
                    a = new Atom("Other Technology", 0, "Not used");
                    atoms.Add(a);
                    break;
                case 1:
                    a = new Atom("Other Technology", 1, "Used");
                    atoms.Add(a);
                    break;
            }
            Offset += 1;
            this.DI210 = atoms;
        }

        private void decodeRollAngle()
        {
            this.DI230 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.01);
            Offset += 2;
        }

        private void decodeFL()
        {
            this.DI145 = (int) Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.25);
            Offset += 2;
        }

        private void decodeAirSpeed()
        {
            string s = Convert.ToString(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
            int code = Convert.ToInt16(string.Concat(s[0]));
            s = s.Remove(0, 1);
            float Airspeed;
            switch (code)
            {
                case 0:
                    this.DI150 = Convert.ToSingle(Convert.ToInt16(s, 2) * 2 * Math.Pow(2, -14));
                    break;
                case 1:
                    this.DI150 = Convert.ToSingle(Convert.ToInt16(s, 2) * 0.001);
                    break;
            }
            Offset += 2;
        }

        private void decodeTrueAirSpeed()
        {
            this.DI151 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
            Offset += 2;
        }

        private void decodeMagneticHeading()
        {
            this.DI152 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * (360 / Math.Pow(2, 16)));
            Offset += 2;
        }

        private void decodeBarometricVerticalRate()
        {
            this.DI155 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 6.25);
            Offset += 2;
        }

        private void decodeGVerticalRate()
        {
            this.DI157 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 6.25);
            Offset += 2;
        }

        private void decodeGroundVector()
        {
            this.DI160 = new List<Atom>();
            this.DI160.Add(new Atom("Ground Speed", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * Math.Pow(2, -14)))));
            Offset += 2;
            this.DI160.Add(new Atom("Track Angle", 1, Convert.ToString(Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * (360 / Math.Pow(2, 16))))));
            Offset += 2;
        }

        private void decodeRateTurn()
        {
            this.DI165 = new List<Atom>();
            List<Atom> atoms = new List<Atom>();
            Atom a;
            string s = Convert.ToString(Convert.ToInt16(this.rawList[Offset], 16), 2).PadLeft(8, '0');
            switch (Convert.ToInt16(string.Concat(s[0], s[1])))
            {
                case 0:
                    a = new Atom("Turn Indicator", 0, "Not available");
                    this.DI165.Add(a);
                    break;
                case 1:
                    a = new Atom("Turn Indicator", 1, "Left");
                    this.DI165.Add(a);
                    break;
                case 2:
                    a = new Atom("Turn Indicator", 2, "Right");
                    this.DI165.Add(a);
                    break;
                case 3:
                    a = new Atom("Turn Indicator", 3, "Straight");
                    this.DI165.Add(a);
                    break;
            }
            switch (Convert.ToInt16(string.Concat(s[7])))
            {
                case 0:
                    Offset += 1;
                    break;
                case 1:
                    s = Convert.ToString(Convert.ToInt16(this.rawList[Offset + 1], 2));
                    Offset += 2;
                    s = s.Remove(7, 1);
                    this.DI165.Add(new Atom("Rate of Turn", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt16(s, 2) * 0.25))));
                    break;
            }

        }

        private void decodeCallsign()
        {
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
            this.DI170 = code;
        }

        private void decodeVelAcc()
        {
            this.DI095 = Convert.ToString(Convert.ToInt16(this.rawList[Offset], 16));
            Offset += 1;
        }
        
        private void decodeTODAcc()
        {
            this.DI032 = Convert.ToSingle(Convert.ToInt16(this.rawList[Offset], 16) * Math.Pow(2, -8));
            Offset += 1;
        }

        private void decodeTargetStats()
        {
            this.DI200 = new List<Atom>();
            string s = Convert.ToString(Convert.ToInt16(this.rawList[Offset], 16));
            switch (Convert.ToInt16(s))
            {
                case 0:
                    this.DI200.Add(new Atom("Target Status", 0, "No emergency / not reported"));
                    break;
                case 1:
                    this.DI200.Add(new Atom("Target Status", 1, "General emergency"));
                    break;
                case 2:
                    this.DI200.Add(new Atom("Target Status", 2, "Lifeguard / medical"));
                    break;
                case 3:
                    this.DI200.Add(new Atom("Target Status", 3, "Minimum fuel"));
                    break;
                case 4:
                    this.DI200.Add(new Atom("Target Status", 4, "No communication"));
                    break;
                case 5:
                    this.DI200.Add(new Atom("Target Status", 5, "Unlawful interference"));
                    break;
            }
            Offset += 1;
        }

        private void decodeEmmitterCat()
        {
            switch (Convert.ToInt16(this.rawList[Offset], 16))
            {
                case 1:
                    this.DI020 = "light aircraft";
                    break;
                case 2:
                    this.DI020 = "reserved";
                    break;
                case 3:
                    this.DI020 = "medium aircraft";
                    break;
                case 4:
                    this.DI020 = "reserved";
                    break;
                case 5:
                    this.DI020 = "heavy aircraft";
                    break;
                case 6:
                    this.DI020 = "highly manoeuvrable and high speed";
                    break;
                case 7:
                    this.DI020 = "reserved";
                    break;
                case 8:
                    this.DI020 = "reserved";
                    break;
                case 9:
                    this.DI020 = "reserved";
                    break;
                case 10:
                    this.DI020 = "rotocraft";
                    break;
                case 11:
                    this.DI020 = "glider/sailplane";
                    break;
                case 12:
                    this.DI020 = "lighter-than-air";
                    break;
                case 13:
                    this.DI020 = "unmanned aerial vehicle";
                    break;
                case 14:
                    this.DI020 = "space/transatmospheric vehicle";
                    break;
                case 15:
                    this.DI020 = "ultralight/handglider/paraglider";
                    break;
                case 16:
                    this.DI020 = "parachutist/skydiver";
                    break;
                case 17:
                    this.DI020 = "reserved";
                    break;
                case 18:
                    this.DI020 = "reserved";
                    break;
                case 19:
                    this.DI020 = "reserved";
                    break;
                case 20:
                    this.DI020 = "surface emergency vehicle";
                    break;
                case 21:
                    this.DI020 = "surface service vehicle";
                    break;
                case 22:
                    this.DI020 = "fixed ground or tethered obstruction";
                    break;
                case 23:
                    this.DI020 = "reserved";
                    break;
                case 24:
                    this.DI020 = "reserved";
                    break;
            }
            Offset++;
        }

        private void decodeMetReport()
        {
            string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
            ++Offset;
            switch (Convert.ToInt16(string.Concat(s[0])))
            {
                case 0:
                    this.DI220.Add(new Atom("Wind Speed", 0, "Absence of Subfield #1"));
                    break;
                case 1:
                    this.DI220.Add(new Atom("Wind Speed", 1, "Presence of Subfield #1"));
                    break;
            }
            switch (Convert.ToInt16(string.Concat(s[1])))
            {
                case 0:
                    this.DI220.Add(new Atom("Wind Direction", 0, "Absence of Subfield #2"));
                    break;
                case 1:
                    this.DI220.Add(new Atom("Wind Direction", 1, "Presence of Subfield #2"));
                    break;
            }
            switch (Convert.ToInt16(string.Concat(s[2])))
            {
                case 0:
                    this.DI220.Add(new Atom("Temperature", 0, "Absence of Subfield #3"));
                    break;
                case 1:
                    this.DI220.Add(new Atom("Temperature", 1, "Presence of Subfield #3"));
                    break;
            }
            switch (Convert.ToInt16(string.Concat(s[3])))
            {
                case 0:
                    this.DI220.Add(new Atom("Turbulence", 0, "Absence of Subfield #4"));
                    break;
                case 1:
                    this.DI220.Add(new Atom("Turbulence", 1, "Presence of Subfield #4"));
                    break;
            }
            switch (Convert.ToInt16(string.Concat(s[7])))
            {
                case 0:
                    break;
                case 1:
                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset += 2;
                    this.DI220.Add(new Atom("Wind Speed", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 1))));

                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset += 2;
                    this.DI220.Add(new Atom("Wind Direction", 1, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 1))));

                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset += 2;
                    this.DI220.Add(new Atom("Temperature", 2, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.25))));

                    s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                    ++Offset;
                    this.DI220.Add(new Atom("Turbulence", 3, Convert.ToString(Convert.ToInt32(s))));
                    break;
            }
        }

        private void decodeIntermediateStateSelAlt()
        {
            string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
            Offset += 2;
            switch (Convert.ToInt16(string.Concat(s[0])))
            {
                case 0:
                    this.DI146.Add(new Atom("Source Availability", 0, "No source information provided"));
                    break;
                case 1:
                    this.DI146.Add(new Atom("Source Availability", 1, "Source Information provided"));
                    break;
            }
            switch (Convert.ToInt16(string.Concat(s[1], s[2])))
            {
                case 0:
                    this.DI146.Add(new Atom("Source", 0, "Unknown"));
                    break;
                case 1:
                    this.DI146.Add(new Atom("Source", 1, "Aircraft Altitude (Holding Altitude)"));
                    break;
                case 2:
                    this.DI146.Add(new Atom("Source", 2, "MCP/FCU Selected Altitude"));
                    break;
                case 3:
                    this.DI146.Add(new Atom("Source", 3, "FMS Selected Altitude"));
                    break;
            }
            s = s.Remove(0, 3);
            this.DI146.Add(new Atom("Altitude", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 25))));
        }

        private void decodeFinalStateSelAlt()
        {
            string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
            Offset += 2;
            switch (Convert.ToInt16(string.Concat(s[0])))
            {
                case 0:
                    this.DI148.Add(new Atom("Manage Vertical Mode", 0, "Not active"));
                    break;
                case 1:
                    this.DI148.Add(new Atom("Manage Vertical Mode", 1, "Active"));
                    break;
            }
            switch (Convert.ToInt16(string.Concat(s[1])))
            {
                case 0:
                    this.DI148.Add(new Atom("Altitude Hold Mode", 0, "Not active"));
                    break;
                case 1:
                    this.DI148.Add(new Atom("Altitude Hold Mode", 1, "Active"));
                    break;
            }
            switch (Convert.ToInt16(string.Concat(s[2])))
            {
                case 0:
                    this.DI148.Add(new Atom("Approach Mode", 0, "Not active"));
                    break;
                case 1:
                    this.DI148.Add(new Atom("Approach Mode", 1, "Active"));
                    break;
            }
            s = s.Remove(0, 3);
            this.DI148.Add(new Atom("Altitude", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 25))));
        }

        private void decodeTrajectoryIntent()
        {
            string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
            Offset++;
            switch (Convert.ToInt16(string.Concat(s[0])))
            {
                case 0:
                    this.DI110.Add(new Atom("NAV", 0, "Trajectory Intent Data is available for this aircraft"));
                    break;
                case 1:
                    this.DI110.Add(new Atom("NAV", 1, "Trajectory Intent Data is not available for this aircraft"));
                    break;
            }
            switch (Convert.ToInt16(string.Concat(s[1])))
            {
                case 0:
                    this.DI110.Add(new Atom("NVB", 0, "Trajectory Intent Data is valid"));
                    break;
                case 1:
                    this.DI110.Add(new Atom("NVB", 1, "Trajectory Intent Data is not valid"));
                    break;
            }
            switch (Convert.ToInt16(string.Concat(s[7])))
            {
                case 0:
                    break;
                case 1:
                    this.DI110.Add(new Atom("REP", 0, Convert.ToString(Convert.ToInt32(Convert.ToInt32(this.rawList[Offset], 16)))));
                    ++Offset;
                    s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                    Offset++;
                    switch (Convert.ToInt16(string.Concat(s[0])))
                    {
                        case 0:
                            this.DI110.Add(new Atom("TCA", 0, "TCP number available"));
                            break;
                        case 1:
                            this.DI110.Add(new Atom("TCA", 1, "TCP number not available"));
                            break;
                    }
                    switch (Convert.ToInt16(string.Concat(s[1])))
                    {
                        case 0:
                            this.DI110.Add(new Atom("NC", 0, "TCP compliance"));
                            break;
                        case 1:
                            this.DI110.Add(new Atom("NC", 1, "TCP non-compliance"));
                            break;
                    }
                    s = s.Remove(0, 2);
                    this.DI110.Add(new Atom("TCP number", 0, Convert.ToString(Convert.ToInt32(s))));

                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset += 2;
                    this.DI110.Add(new Atom("Altitude", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 10))));

                    int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
                    float latreal = Convert.ToSingle(lat * 180 / Math.Pow(2, 23));

                    int lon = Int32.Parse(string.Concat(this.rawList[Offset + 3], this.rawList[Offset + 4], this.rawList[Offset + 5]), System.Globalization.NumberStyles.HexNumber);
                    float lonreal = Convert.ToSingle(lon * 180 / Math.Pow(2, 23));

                    this.DI110.Add(new Atom("Latitude", latreal, Convert.ToString(latreal)));
                    this.DI110.Add(new Atom("Longitude", lonreal, Convert.ToString(lonreal)));

                    Offset += 6;

                    s = Convert.ToString(Convert.ToInt32((this.rawList[Offset], 16)));
                    Offset += 1;

                    switch (Convert.ToInt16(string.Concat(s[0], s[1], s[2], s[3])))
                    {
                        case 0:
                            this.DI110.Add(new Atom("Point Type", 0, "Unknown"));
                            break;
                        case 1:
                            this.DI110.Add(new Atom("Point Type", 1, "Fly by waypoint (LT)"));
                            break;
                        case 2:
                            this.DI110.Add(new Atom("Point Type", 2, "Fly over waypoint (LT)"));
                            break;
                        case 3:
                            this.DI110.Add(new Atom("Point Type", 3, "Hold pattern (LT)"));
                            break;
                        case 4:
                            this.DI110.Add(new Atom("Point Type", 4, "Procedure hold (LT)"));
                            break;
                        case 5:
                            this.DI110.Add(new Atom("Point Type", 5, "Procedure turn (LT)"));
                            break;
                        case 6:
                            this.DI110.Add(new Atom("Point Type", 6, "RF leg (LT)"));
                            break;
                        case 7:
                            this.DI110.Add(new Atom("Point Type", 7, "Top of climb (VT)"));
                            break;
                        case 8:
                            this.DI110.Add(new Atom("Point Type", 8, "Top of descent (VT)"));
                            break;
                        case 9:
                            this.DI110.Add(new Atom("Point Type", 9, "Start of level (VT)"));
                            break;
                        case 10:
                            this.DI110.Add(new Atom("Point Type", 10, "Cross-over altitude (VT)"));
                            break;
                        case 11:
                            this.DI110.Add(new Atom("Point Type", 11, "Transition altitude (VT)"));
                            break;
                    }
                    switch (Convert.ToInt16(string.Concat(s[4], s[5])))
                    {
                        case 0:
                            this.DI110.Add(new Atom("TD", 0, "N/A"));
                            break;
                        case 1:
                            this.DI110.Add(new Atom("TD", 1, "Turn right"));
                            break;
                        case 2:
                            this.DI110.Add(new Atom("TD", 2, "Turn left"));
                            break;
                        case 3:
                            this.DI110.Add(new Atom("TD", 3, "No turn"));
                            break;
                    }
                    switch (Convert.ToInt16(string.Concat(s[6])))
                    {
                        case 0:
                            this.DI110.Add(new Atom("Turn Radius Availabilty", 0, "TTR not available"));
                            break;
                        case 1:
                            this.DI110.Add(new Atom("Turn Radius Availabilty", 1, "TTR available"));
                            break;
                    }
                    switch (Convert.ToInt16(string.Concat(s[7])))
                    {
                        case 0:
                            this.DI110.Add(new Atom("TOA", 0, "TOV available"));
                            break;
                        case 1:
                            this.DI110.Add(new Atom("TOA", 1, "TOV not available"));
                            break;
                    }
                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), 16));
                    Offset += 3;
                    this.DI110.Add(new Atom("Time Over Point", Convert.ToInt32(s), s));


                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset += 2;
                    this.DI110.Add(new Atom("TCP Turn radius", Convert.ToSingle(Convert.ToInt32(s) * 0.01), Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.01))));

                    break;
            }
        }

    }
}
