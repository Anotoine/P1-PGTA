using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASTERIX
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
        internal List<Atom> DI170 { get; set; }
        internal List<Atom> DI200 { get; set; }
        internal List<Atom> DI210 { get; set; }
        internal string DI220 { get; set; }
        internal float DI230 { get; set; }


        public CAT21v023(List<string> rawList, List<bool> listFSPEC, int Offset)
        {
            this.listFSPEC = listFSPEC;
            this.rawList = rawList;
            this.Offset = Offset;
        }

        public CAT21v023 decode()
        {
            if (this.listFSPEC[1])
                decodeSACSIC();
            if (this.listFSPEC[2])
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
                Offset = +1;
                this.DI040 = atoms;
            }
            if (this.listFSPEC[3])
                decodeTOD();
            if (this.listFSPEC[4])
                decodeLatLong();
            if (this.listFSPEC[5])
            {
                this.DI080 = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), 16), 2).PadLeft(16, '0');
                Offset += 3;
            }
            if (this.listFSPEC[6])
            {
                int alt = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
                this.DI140 = Convert.ToSingle(alt * 6.25);
                Offset += 3;
            }
            if (this.listFSPEC[7])
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
                Offset += 1;
                this.DI090 = atoms;
            }
            if (listFSPEC[8])
            {
                if (listFSPEC[9])
                {
                    string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                    List<Atom> atoms = new List<Atom>();
                    Atom a;
                    int code = Convert.ToInt32(s[3]);
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
                    code = Convert.ToInt32(s[4]);
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
                    code = Convert.ToInt32(s[5]);
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
                    code = Convert.ToInt32(s[6]);
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
                    code = Convert.ToInt32(s[7]);
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
                if (listFSPEC[10])
                {
                    float RA = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.01);
                    this.DI230 = RA;
                    Offset += 2;
                }
                if (listFSPEC[11])
                {
                    float FL = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.25);
                    this.DI145 = (int)FL;
                    Offset += 2;
                }
                if (listFSPEC[12])
                {
                    string s = Convert.ToString(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    int code = Convert.ToInt16(s[0]);
                    s = s.Remove(0, 1);
                    float Airspeed;
                    switch (code)
                    {
                        case 0:
                            Airspeed = Convert.ToSingle(Convert.ToInt16(s, 2) * 2 * 10 ^ (-14));
                            this.DI150 = Airspeed;
                            break;
                        case 1:
                            Airspeed = Convert.ToSingle(Convert.ToInt16(s, 2) * 0.001);
                            this.DI150 = Airspeed;
                            break;
                    }
                    Offset += 2;
                }
                if (listFSPEC[13])
                {
                    this.DI151 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset += 2;
                }
                if (listFSPEC[14])
                {
                    this.DI152 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * (360 / (2 ^ 16)));
                    Offset += 2;
                }
                if (listFSPEC[15])
                {
                    this.DI155 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 6.25);
                    Offset += 2;
                }
                if (listFSPEC[16])
                {
                    if (listFSPEC[17])
                    {
                        this.DI157 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 6.25);
                        Offset += 2;
                    }
                    if (listFSPEC[18])
                    {
                        List<Atom> atoms = new List<Atom>();
                        Atom a;
                        a = new Atom("Ground Speed", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 2 ^ (-14))));
                        atoms.Add(a);
                        Offset += 2;
                        a = new Atom("Track Angle", 1, Convert.ToString(Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * (360 / (2 ^ 16)))));
                        atoms.Add(a);
                        this.DI160 = atoms;
                        Offset += 2;
                    }
                    if (listFSPEC[19])
                    {
                        List<Atom> atoms = new List<Atom>();
                        Atom a;
                        string s = Convert.ToString(Convert.ToInt16(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                        int code = Convert.ToInt16(string.Concat(s[0], s[1]));
                        switch (code)
                        {
                            case 0:
                                a = new Atom("Turn Indicator", 0, "Not available");
                                atoms.Add(a);
                                break;
                            case 1:
                                a = new Atom("Turn Indicator", 1, "Left");
                                atoms.Add(a);
                                break;
                            case 2:
                                a = new Atom("Turn Indicator", 2, "Right");
                                atoms.Add(a);
                                break;
                            case 3:
                                a = new Atom("Turn Indicator", 3, "Straight");
                                atoms.Add(a);
                                break;
                        }
                        code = Convert.ToInt16(s[7]);
                        switch (code)
                        {
                            case 0:
                                this.DI165 = atoms;
                                Offset += 1;
                                break;
                            case 1:
                                s = Convert.ToString(Convert.ToInt16(this.rawList[Offset + 1], 2));
                                Offset += 2;
                                s = s.Remove(7, 1);
                                a = new Atom("Rate of Turn", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt16(s, 2) * 0.25)));
                                atoms.Add(a);
                                this.DI165 = atoms;
                                break;
                        }

                    }
                    if (listFSPEC[20])
                    {
                        List<Atom> atoms = new List<Atom>();
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
                                code.Add((char)Convert.ToInt32(string.Concat("01", stringCode.Substring(6 * i, 6)), 16));
                            else
                                code.Add((char)Convert.ToInt32(string.Concat("00", stringCode.Substring(6 * i, 6)), 16));
                        }
                        atoms.Add(new Atom("Callsign", 0, Regex.Replace(string.Join("", code.ToArray()), @"\s", "")));

                        this.DI170 = atoms;
                    }
                    if (listFSPEC[21])
                    {
                        this.DI095 = Convert.ToString(Convert.ToInt16(this.rawList[Offset], 16));
                        Offset += 1;
                    }
                    if (listFSPEC[22])
                    {
                        this.DI032 = Convert.ToSingle(Convert.ToInt16(this.rawList[Offset], 16) * (2 ^ (-8)));
                        Offset += 1;
                    }
                    if (listFSPEC[23])
                    {
                        List<Atom> atoms = new List<Atom>();
                        Atom a;
                        string s = Convert.ToString(Convert.ToInt16(this.rawList[Offset], 16));
                        switch (Convert.ToInt16(s))
                        {
                            case 0:
                                a = new Atom("Target Status", 0, "No emergency / not reported");
                                atoms.Add(a);
                                break;
                            case 1:
                                a = new Atom("Target Status", 1, "General emergency");
                                atoms.Add(a);
                                break;
                            case 2:
                                a = new Atom("Target Status", 2, "Lifeguard / medical");
                                atoms.Add(a);
                                break;
                            case 3:
                                a = new Atom("Target Status", 3, "Minimum fuel");
                                atoms.Add(a);
                                break;
                            case 4:
                                a = new Atom("Target Status", 4, "No communication");
                                atoms.Add(a);
                                break;
                            case 5:
                                a = new Atom("Target Status", 5, "Unlawful interference");
                                atoms.Add(a);
                                break;
                        }
                        Offset += 1;
                        this.DI200 = atoms;
                    }
                    if (listFSPEC[24])
                    {
                        if (listFSPEC[25])
                        {
                            int s = Convert.ToInt16(this.rawList[Offset], 16);
                            Offset += 1;
                            switch (s)
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
                        }

                        if (listFSPEC[26])
                        {

                        }

                        if (listFSPEC[27])
                        {

                        }

                        if (listFSPEC[28])
                        {

                        }

                        if (listFSPEC[29])
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

        private void decodeTOD()
        {
            int LSB = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            Offset += 3;

            this.DI030 = new DateTime().AddSeconds((float)LSB / 128);
        }

        private void decodeLatLong()
        {
            int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            float latreal = Convert.ToSingle(lat * 180 / 2 ^ 23);

            int lon = Int32.Parse(string.Concat(this.rawList[Offset + 3], this.rawList[Offset + 4], this.rawList[Offset + 5]), System.Globalization.NumberStyles.HexNumber);
            float lonreal = Convert.ToSingle(lon * 180 / 2 ^ 23);

            Offset += 6;

            this.DI130 = new Point().LatLong2XY(latreal, lonreal);
        }
    }
}
