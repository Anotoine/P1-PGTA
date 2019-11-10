using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTERIX
{
    class CAT21v24
    {
        //Message stuff
        private List<bool> listFSPEC;
        private List<string> rawList;
        private int Offset;

        //DataItem
        internal string DI008 { get; set; }
        internal List<Atom> DI010 { get; set; }
        internal int DI015 { get; set; }
        internal List<Atom> DI016 { get; set; }
        internal string DI020 { get; set; }
        internal List<Atom> DI040 { get; set; }
        internal int DI070 { get; set; }
        internal float DI071 { get; set; }
        internal DateTime DI072 { get; set; }
        internal DateTime DI073 { get; set; }
        internal List<Atom> DI074 { get; set; }
        internal float DI075 { get; set; }
        internal List<Atom> DI076 { get; set; }
        internal float DI077 { get; set; }
        internal string DI080 { get; set; } //ICAO Address
        internal List<Atom> DI090 { get; set; }
        internal float DI110 { get; set; }
        internal Point DI130 { get; set; } //Position 
        internal Point DI131 { get; set; } //Position high-res
        internal float DI132 { get; set; }
        internal float DI140 { get; set; }
        internal float DI145 { get; set; }
        internal float DI146 { get; set; }
        internal float DI148 { get; set; }
        internal float DI150 { get; set; }
        internal float DI151 { get; set; }
        internal float DI152 { get; set; }
        internal List<Atom> DI155 { get; set; }
        internal List<Atom> DI157 { get; set; }
        internal List<Atom> DI160 { get; set; }
        internal int DI161 { get; set; }
        internal float DI165 { get; set; }
        internal List<Atom> DI170 { get; set; }
        internal List<Atom> DI200 { get; set; }
        internal List<Atom> DI210 { get; set; }
        internal string DI220 { get; set; }
        internal float DI230 { get; set; }
        internal List<Atom> DI250 { get; set; }
        internal List<Atom> DI260 { get; set; }
        internal List<Atom> DI271 { get; set; }
        internal string DI295 { get; set; }
        internal string DI400 { get; set; }

        public CAT21v24(List<string> rawList, List<bool> listFSPEC, int Offset)
        {
            this.listFSPEC = listFSPEC;
            this.rawList = rawList;
            this.Offset = Offset;
        }

        public CAT21v24 decode()
        {
            if (listFSPEC[1])
                decodeSACSIC();
            if (listFSPEC[2])
            {
                List<Atom> atoms = new List<Atom>();
                Atom a;
                string s = Convert.ToString(Convert.ToInt16(this.rawList[Offset], 16));
                int code = Convert.ToInt16(string.Concat(s[0], s[1], s[2]), 2);
                switch (code)
                {
                    case 0:
                        a = new Atom("Address Type", 0, "24-Bit ICAO address");
                        atoms.Add(a);
                        break;
                    case 1:
                        a = new Atom("Address Type", 1, "Duplicate address");
                        atoms.Add(a);
                        break;
                    case 2:
                        a = new Atom("Address Type", 2, "Surface vehicle address");
                        atoms.Add(a);
                        break;
                    case 3:
                        a = new Atom("Address Type", 3, "Anonymous address");
                        atoms.Add(a);
                        break;
                }
                code = Convert.ToInt16(string.Concat(s[3], s[4]), 2);
                switch (code)
                {
                    case 0:
                        a = new Atom("Altitude Reporting Capability", 0, "25 ft");
                        atoms.Add(a);
                        break;
                    case 1:
                        a = new Atom("Altitude Reporting Capability", 1, "100 ft");
                        atoms.Add(a);
                        break;
                    case 2:
                        a = new Atom("Altitude Reporting Capability", 2, "Unknown");
                        atoms.Add(a);
                        break;
                    case 3:
                        a = new Atom("Altitude Reporting Capability", 3, "Invalid");
                        atoms.Add(a);
                        break;
                }
                code = Convert.ToInt16(s[5]);
                switch (code)
                {
                    case 0:
                        a = new Atom("Range Check", 0, "Default");
                        atoms.Add(a);
                        break;
                    case 1:
                        a = new Atom("Range Check", 1, "Range Check passed, CPR Validation pending");
                        atoms.Add(a);
                        break;
                }
                code = Convert.ToInt16(s[6]);
                switch (code)
                {
                    case 0:
                        a = new Atom("Report Type", 0, "Report from target transponder");
                        atoms.Add(a);
                        break;
                    case 1:
                        a = new Atom("Report Type", 1, "Report from field monitor(fixed transponder)");
                        atoms.Add(a);
                        break;
                }
                code = Convert.ToInt16(s[7]);
                switch (code)
                {
                    case 0:
                        a = new Atom("Report Type", 0, "Report from target transponder");
                        atoms.Add(a);
                        break;
                    case 1:
                        a = new Atom("Report Type", 1, "Report from field monitor(fixed transponder)");
                        atoms.Add(a);
                        break;
                }
                this.DI040 = atoms;
                Offset += 1;

            }
            if (listFSPEC[3])
                decodeTrackNumber();
            if (listFSPEC[4])
            {
                this.DI015 = Convert.ToInt16(this.rawList[Offset], 16);
                Offset += 1;
            }
            if (listFSPEC[5])
            {
                this.DI071 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), 16) * (1 / 128));
                Offset += 3;
            }
            if (listFSPEC[6])
                decodeLatLong();
            if (listFSPEC[7])
            {
                int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3], this.rawList[Offset + 4]), System.Globalization.NumberStyles.HexNumber);
                float latreal = Convert.ToSingle(lat * 180 / 2 ^ 30);

                int lon = Int32.Parse(string.Concat(this.rawList[Offset + 5], this.rawList[Offset + 6], this.rawList[Offset + 7], this.rawList[Offset + 8], this.rawList[Offset + 9]), System.Globalization.NumberStyles.HexNumber);
                float lonreal = Convert.ToSingle(lon * 180 / 2 ^ 30);

                Offset += 10;
                this.DI131 = new Point().LatLong2XY(latreal, lonreal);
            }
            if (listFSPEC[8])
            {
                if (listFSPEC[9])
                    decodeTOD();

                if (listFSPEC[10])
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
                if (listFSPEC[11])
                {
                    string s = Convert.ToString(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    int code = Convert.ToInt16(s[0]);
                    s = s.Remove(0, 1);
                    float Airspeed;
                    switch (code)
                    {
                        case 0:
                            Airspeed = Convert.ToSingle(Convert.ToInt16(s, 2));
                            this.DI151 = Airspeed;
                            break;
                        case 1:
                            Airspeed = 32768;
                            this.DI151 = Airspeed;
                            break;
                    }
                    Offset += 2;
                }
                if (listFSPEC[12])
                   decodeICAOAddress();
                if (listFSPEC[13])
                    decodeTOD();
                if (listFSPEC[14])
                {
                    List<Atom> atoms = new List<Atom>();
                    Atom a;
                    string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), 16));
                    int code = Convert.ToInt16(string.Concat(s[0], s[1]));
                    switch (code)
                    {
                        case 0:
                            a = new Atom("FSI", 0, "+0");
                            atoms.Add(a);
                            break;
                        case 1:
                            a = new Atom("FSI", 1, "+1");
                            atoms.Add(a);
                            break;
                        case 2:
                            a = new Atom("FSI", 2, "-1");
                            atoms.Add(a);
                            break;
                        case 3:
                            a = new Atom("FSI", 3, "Reserved");
                            atoms.Add(a);
                            break;
                    }
                    s = s.Remove(0, 2);
                    a = new Atom("TOMROP", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)) * (2 ^ (-30))));
                    atoms.Add(a);
                    this.DI074 = atoms;
                    Offset += 4;

                }
                if (listFSPEC[15])
                {
                    this.DI075 = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), 16) * (1 / 128));
                    Offset += 3;
                }
                if (listFSPEC[16])
                {
                    if (listFSPEC[17])
                    {
                        List<Atom> atoms = new List<Atom>();
                        Atom a;
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), 16));
                        int code = Convert.ToInt16(string.Concat(s[0], s[1]));
                        switch (code)
                        {
                            case 0:
                                a = new Atom("FSI", 0, "+0");
                                atoms.Add(a);
                                break;
                            case 1:
                                a = new Atom("FSI", 1, "+1");
                                atoms.Add(a);
                                break;
                            case 2:
                                a = new Atom("FSI", 2, "-1");
                                atoms.Add(a);
                                break;
                            case 3:
                                a = new Atom("FSI", 3, "Reserved");
                                atoms.Add(a);
                                break;
                        }
                        s = s.Remove(0, 2);
                        a = new Atom("TOMROP", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * (2 ^ (-30)))));
                        atoms.Add(a);
                        this.DI076 = atoms;
                        Offset += 4;
                    }
                }
                if (listFSPEC[18])
                {
                    this.DI140 = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1])) * 6.25);
                    Offset += 2;
                }
                if (listFSPEC[19])
                {
                    List<Atom> atoms = new List<Atom>();
                    Atom a;
                    string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                    ++Offset;
                    atoms.Add(new Atom("NUCr or NACv", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[0], s[1], s[2])))));
                    atoms.Add(new Atom("NUCp or NIC", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[3], s[4], s[5], s[6])))));
                    if (s[7].Equals('1'))
                    {
                        s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                        ++Offset;
                        atoms.Add(new Atom("Navigation Integrity Category for Barometric Altitude", 0, Convert.ToString(Convert.ToInt32(s[0]))));
                        atoms.Add(new Atom("Surveillance (version 1) or Source (version 2) Integrity Level", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[1], s[2])))));
                        atoms.Add(new Atom("Navigation Accuracy Category for Position", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[3], s[4], s[5])))));
                        if (s[7].Equals('1'))
                        {
                            s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                            ++Offset;
                            switch (Convert.ToInt32(s[2]))
                            {
                                case 0:
                                    atoms.Add(new Atom("SIL-Supplement", 0, "measured per flight-hour"));
                                    break;
                                case 1:
                                    atoms.Add(new Atom("SIL-Supplement", 0, "measured per sample"));
                                    break;
                            }
                            atoms.Add(new Atom("Horizontal Position System Design Assurance", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[3], s[4])))));
                            atoms.Add(new Atom("Geometric Altitude Accuracy", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[5], s[6])))));
                            if (s[7].Equals('1'))
                            {
                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                ++Offset;
                                atoms.Add(new Atom("Position Integrity Category", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[0], s[1], s[2], s[3])))));
                            }
                            else
                                this.DI090 = atoms;
                        }
                        else
                            this.DI090 = atoms;
                    }
                    else
                        this.DI090 = atoms;
                }
                if (listFSPEC[20])
                {
                    List<Atom> atoms = new List<Atom>();
                    string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                    ++Offset;
                    atoms.Add(new Atom("Version Not Supported", 0, Convert.ToString(Convert.ToInt32(s[1]))));
                    atoms.Add(new Atom("Version Number", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[2], s[3], s[4])))));
                    atoms.Add(new Atom("Link Technology Type", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[5], s[6], s[7])))));
                    this.DI210 = atoms;
                }
                if (listFSPEC[21])
                {
                    string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                    int code = Convert.ToInt32(string.Concat(s[4], s[5], s[6]), 2);

                    code += Convert.ToInt32(string.Concat(s[7], s[8], s[9]), 2);

                    code += Convert.ToInt32(string.Concat(s[10], s[11], s[12]), 2);

                    code += Convert.ToInt32(string.Concat(s[13], s[14], s[15]), 2);
                    this.DI070 = code;
                    Offset += 2;
                }
                if (listFSPEC[22])
                {
                    string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset += 2;
                    this.DI230 = Convert.ToSingle(Convert.ToInt32(s) * 0.01);
                }
                if (listFSPEC[23])
                {
                    string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset += 2;
                    this.DI145 = Convert.ToSingle(Convert.ToInt32(s) * 0.25);
                }
                if (listFSPEC[24])
                {
                    if (listFSPEC[25])
                    {
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                        this.DI152 = Convert.ToSingle(Convert.ToInt32(s) * ((360) / (2 ^ 16)));
                        Offset += 2;
                    }
                    if (listFSPEC[26])
                    {
                        string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                        ++Offset;
                        List<Atom> atoms = new List<Atom>();
                        int code = Convert.ToInt16(s[0]);
                        switch (code)
                        {
                            case 0:
                                atoms.Add(new Atom("Intent Change Flag", 0, "No intent change active"));
                                break;
                            case 1:
                                atoms.Add(new Atom("Intent Change Flag", 1, "Intent change flag raised"));
                                break;
                        }
                        code = Convert.ToInt16(s[1]);
                        switch (code)
                        {
                            case 0:
                                atoms.Add(new Atom("LNAV Mode", 0, "LNAV Mode engaged"));
                                break;
                            case 1:
                                atoms.Add(new Atom("LNAV Mode", 1, "LNAV Mode not engaged"));
                                break;
                        }
                        code = Convert.ToInt16(s[2]);
                        switch (code)
                        {
                            case 0:
                                atoms.Add(new Atom("ME", 0, "No military emergency"));
                                break;
                            case 1:
                                atoms.Add(new Atom("ME", 1, "Military emergency"));
                                break;
                        }
                        code = Convert.ToInt16(string.Concat(s[3], s[4], s[5]));
                        switch (code)
                        {
                            case 0:
                                atoms.Add(new Atom("Priority Status", 0, "No emergency / not reported"));
                                break;
                            case 1:
                                atoms.Add(new Atom("Priority Status", 1, "General emergency"));
                                break;
                            case 2:
                                atoms.Add(new Atom("Priority Status", 2, "Lifeguard / medical emergency"));
                                break;
                            case 3:
                                atoms.Add(new Atom("Priority Status", 3, "Minimum fuel"));
                                break;
                            case 4:
                                atoms.Add(new Atom("Priority Status", 4, "No communications"));
                                break;
                            case 5:
                                atoms.Add(new Atom("Priority Status", 5, "Unlawful interference"));
                                break;
                            case 6:
                                atoms.Add(new Atom("Priority Status", 6, "Downed” Aircraft"));
                                break;
                        }
                        code = Convert.ToInt16(string.Concat(s[6], s[7]));
                        switch (code)
                        {
                            case 0:
                                atoms.Add(new Atom("Surveillance Status", 0, "No condition reported"));
                                break;
                            case 1:
                                atoms.Add(new Atom("Surveillance Status", 1, "Permanent Alert (Emergency condition"));
                                break;
                            case 2:
                                atoms.Add(new Atom("Surveillance Status", 2, "Temporary Alert (change in Mode 3/A Code other than emergency)"));
                                break;
                            case 3:
                                atoms.Add(new Atom("Surveillance Status", 3, "SPI set"));
                                break;
                        }

                        this.DI200 = atoms;
                    }
                    if (listFSPEC[27])
                    {
                        List<Atom> atoms = new List<Atom>();
                        Atom a;
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                        int code = Convert.ToInt16(s[0]);
                        switch (code)
                        {
                            case 0:
                                a = new Atom("Range Exceeded Indicator", 0, "Value in defined range");
                                atoms.Add(a);
                                break;
                            case 1:
                                a = new Atom("Range Exceeded Indicator", 1, "Value exceeds defined range");
                                atoms.Add(a);
                                break;
                        }
                        s = s.Remove(0, 1);
                        a = new Atom("Barometric Vertical Rate", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)) * (6.25)));
                        atoms.Add(a);
                        Offset += 2;
                        this.DI155 = atoms;
                    }
                    if (listFSPEC[28])
                    {
                        List<Atom> atoms = new List<Atom>();
                        Atom a;
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                        int code = Convert.ToInt16(s[0]);
                        switch (code)
                        {
                            case 0:
                                a = new Atom("Range Exceeded Indicator", 0, "Value in defined range");
                                atoms.Add(a);
                                break;
                            case 1:
                                a = new Atom("Range Exceeded Indicator", 1, "Value exceeds defined range");
                                atoms.Add(a);
                                break;
                        }
                        s = s.Remove(0, 1);
                        a = new Atom("Geometric Vertical Rate", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)) * (6.25)));
                        atoms.Add(a);
                        Offset += 2;
                        this.DI157 = atoms;
                    }
                    if (listFSPEC[29])
                    {
                        List<Atom> atoms = new List<Atom>();
                        Atom a;
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                        int code = Convert.ToInt16(s[0]);
                        switch (code)
                        {
                            case 0:
                                a = new Atom("Range Exceeded Indicator", 0, "Value in defined range");
                                atoms.Add(a);
                                break;
                            case 1:
                                a = new Atom("Range Exceeded Indicator", 1, "Value exceeds defined range");
                                atoms.Add(a);
                                break;
                        }
                        s = s.Remove(0, 1);
                        a = new Atom("Ground Speed", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)) * (2 ^ (-14))));
                        atoms.Add(a);
                        Offset += 2;
                        s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                        Offset += 2;
                        a = new Atom("Geometric Vertical Rate", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)) * (360 / (2 ^ 16))));
                        this.DI160 = atoms;
                    }
                    if (listFSPEC[30])
                    {
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                        Offset += 2;
                        this.DI165 = Convert.ToSingle(Convert.ToInt32(s) * (1 / 32));
                    }
                    if (listFSPEC[31])
                    {
                        int s = Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 1]), 16);
                        Offset += 3;
                        this.DI077 = Convert.ToSingle(s * (1 / 128));
                    }
                }
            }
            return this;
        }
        private void decodeSACSIC()
        {
            List<string> ls = new List<string>() { "SAC", "SIC" };
            for (int i = 0; i < 2; i++)
            {
                this.DI010.Add(new Atom(ls[i], Convert.ToInt32(this.rawList[Offset], 16), Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16)).PadLeft(3, '0')));
                Offset++;
            }
        }

        private void decodeTrackNumber()
        {
            this.DI161 = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
            Offset += 2;
        }

        private void decodeLatLong()
        {
            int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), System.Globalization.NumberStyles.HexNumber);
            float latreal = Convert.ToSingle(lat * 180 / 2 ^ 25);

            int lon = Int32.Parse(string.Concat(this.rawList[Offset + 4], this.rawList[Offset + 5], this.rawList[Offset + 6], this.rawList[Offset + 7]), System.Globalization.NumberStyles.HexNumber);
            float lonreal = Convert.ToSingle(lon * 180 / 2 ^ 25);

            Offset += 8;

            this.DI130 = new Point().LatLong2XY(latreal, lonreal);
        }

        private void decodeTOD()
        {
            int LSB = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            Offset += 3;

            this.DI073 = new DateTime().AddSeconds((float)LSB / 128);
        }

        private void decodeICAOAddress()
        {
            this.DI080 = string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]).ToUpper();
            Offset += 3;
        }
    }
}
