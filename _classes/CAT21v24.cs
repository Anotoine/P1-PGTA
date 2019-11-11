using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ASTERIX
{
    class CAT21v24
    {
        //Message stuff
        private List<bool> listFSPEC;
        private List<string> rawList;
        private int Offset;

        //DataItem
        internal List<Atom> DI008 { get; set; }
        internal List<Atom> DI010 { get; set; }
        internal int DI015 { get; set; }
        internal float DI016 { get; set; }
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
        internal List<Atom> DI110 { get; set; }
        internal Point DI130 { get; set; } //Position 
        internal Point DI131 { get; set; } //Position high-res
        internal int DI132 { get; set; }
        internal float DI140 { get; set; }
        internal float DI145 { get; set; }
        internal List<Atom> DI146 { get; set; }
        internal List<Atom> DI148 { get; set; }
        internal float DI150 { get; set; }
        internal float DI151 { get; set; }
        internal float DI152 { get; set; }
        internal List<Atom> DI155 { get; set; }
        internal List<Atom> DI157 { get; set; }
        internal List<Atom> DI160 { get; set; }
        internal int DI161 { get; set; }
        internal float DI165 { get; set; }
        internal string DI170 { get; set; }
        internal List<Atom> DI200 { get; set; }
        internal List<Atom> DI210 { get; set; }
        internal List<Atom> DI220 { get; set; }
        internal float DI230 { get; set; }
        internal List<Atom> DI250 { get; set; }
        internal List<Atom> DI260 { get; set; }
        internal List<Atom> DI271 { get; set; }
        internal List<Atom> DI295 { get; set; }
        internal int DI400 { get; set; }

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
                string s = Convert.ToString(Convert.ToInt16(this.rawList[Offset], 16), 2).PadLeft(8,'0');
                int code = Convert.ToInt16(string.Concat(s[0], s[1], s[2]));
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
                code = Convert.ToInt16(string.Concat(s[3], s[4]));
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
                switch (Convert.ToInt16(string.Concat(s[5])))
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
                switch (Convert.ToInt16(string.Concat(s[6])))
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
                switch (Convert.ToInt16(string.Concat(s[7])))
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
                this.DI071 = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), 16) * (1 / 128));
                Offset += 3;
            }
            if (listFSPEC[6])
                decodeLatLong();
            if (listFSPEC[7])
                decodeLatLong_HighRes();
            if (listFSPEC[8])
            {
                if (listFSPEC[9])
                    decodeTODVel();
                if (listFSPEC[10])
                {
                    string s = Convert.ToString(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                    int code = Convert.ToInt16(string.Concat(s[0]));
                    s = s.Remove(0, 1);
                    float Airspeed;
                    switch (code)
                    {
                        case 0:
                            Airspeed = Convert.ToSingle(Convert.ToInt16(s, 2) * Math.Pow(2,-14));
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
                    string s = Convert.ToString(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16,'0');
                    int code = Convert.ToInt16(string.Concat(s[0]));
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
                    string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), 16), 2).PadLeft(8*4, '0');
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
                    a = new Atom("TOMROP", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)) * Math.Pow(2,-30)));
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
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), 16), 2).PadLeft(8*4, '0');
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
                        a = new Atom("TOMROP", 0, Convert.ToString(Convert.ToInt32(s,2) * Math.Pow(2,-30)));
                        atoms.Add(a);
                        this.DI076 = atoms;
                        Offset += 4;
                    }
                }
                if (listFSPEC[18])
                {
                    this.DI140 = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 6.25);
                    Offset += 2;
                }
                if (listFSPEC[19])
                {
                    List<Atom> atoms = new List<Atom>();
                    Atom a;
                    string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8,'0');
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
                    string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8,'0');
                    ++Offset;
                    atoms.Add(new Atom("Version Not Supported", Convert.ToInt32(s[1]), Convert.ToString(Convert.ToInt32(s[1]))));
                    atoms.Add(new Atom("Version Number", Convert.ToInt32(string.Concat(s[2], s[3], s[4])), Convert.ToString(Convert.ToInt32(string.Concat(s[2], s[3], s[4])))));
                    atoms.Add(new Atom("Link Technology Type", Convert.ToInt32(string.Concat(s[5], s[6], s[7])), Convert.ToString(Convert.ToInt32(string.Concat(s[5], s[6], s[7])))));
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
                        this.DI152 = Convert.ToSingle(Convert.ToInt32(s) * ((360) / Math.Pow(2,16)));
                        Offset += 2;
                    }
                    if (listFSPEC[26])
                    {
                        string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                        ++Offset;
                        List<Atom> atoms = new List<Atom>();
                        int code = Convert.ToInt16(string.Concat(s[0]));
                        switch (code)
                        {
                            case 0:
                                atoms.Add(new Atom("Intent Change Flag", 0, "No intent change active"));
                                break;
                            case 1:
                                atoms.Add(new Atom("Intent Change Flag", 1, "Intent change flag raised"));
                                break;
                        }
                        code = Convert.ToInt16(string.Concat(s[1]));
                        switch (code)
                        {
                            case 0:
                                atoms.Add(new Atom("LNAV Mode", 0, "LNAV Mode engaged"));
                                break;
                            case 1:
                                atoms.Add(new Atom("LNAV Mode", 1, "LNAV Mode not engaged"));
                                break;
                        }
                        code = Convert.ToInt16(string.Concat(s[2]));
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
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                        int code = Convert.ToInt16(string.Concat(s[0]));
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
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                        int code = Convert.ToInt16(string.Concat(s[0]));
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
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                        int code = Convert.ToInt16(string.Concat(s[0]));
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
                        a = new Atom("Ground Speed", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)) * Math.Pow(2,-14)));
                        atoms.Add(a);
                        Offset += 2;
                        s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                        Offset += 2;
                        a = new Atom("Geometric Vertical Rate", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)) * (360 / Math.Pow(2,16))));
                        atoms.Add(a);
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
                        int s = Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), 16);
                        Offset += 3;
                        this.DI077 = Convert.ToSingle(s * (1 / 128));
                    }
                    if (listFSPEC[32])
                    {
                        if (listFSPEC[33])
                            decodeCallsign();
                        if (listFSPEC[34])
                        {
                            List<Atom> atoms = new List<Atom>();
                            string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                            ++Offset;
                            int code = Convert.ToInt32(string.Concat(s[0]));
                            switch (code)
                            {
                                case 0:
                                    atoms.Add(new Atom("Wind Speed", 0, "Absence of Subfield #1"));
                                    break;
                                case 1:
                                    atoms.Add(new Atom("Wind Speed", 1, "Presence of Subfield #1"));
                                    break;
                            }
                            code = Convert.ToInt32(string.Concat(s[1]));
                            switch (code)
                            {
                                case 0:
                                    atoms.Add(new Atom("Wind Direction", 0, "Absence of Subfield #2"));
                                    break;
                                case 1:
                                    atoms.Add(new Atom("Wind Direction", 1, "Presence of Subfield #2"));
                                    break;
                            }
                            code = Convert.ToInt32(string.Concat(s[2]));
                            switch (code)
                            {
                                case 0:
                                    atoms.Add(new Atom("Temperature", 0, "Absence of Subfield #3"));
                                    break;
                                case 1:
                                    atoms.Add(new Atom("Temperature", 1, "Presence of Subfield #3"));
                                    break;
                            }
                            code = Convert.ToInt32(string.Concat(s[3]));
                            switch (code)
                            {
                                case 0:
                                    atoms.Add(new Atom("Turbulence", 0, "Absence of Subfield #4"));
                                    break;
                                case 1:
                                    atoms.Add(new Atom("Turbulence", 1, "Presence of Subfield #4"));
                                    break;
                            }
                            code = Convert.ToInt32(string.Concat(s[7]));
                            switch (code)
                            {
                                case 0:
                                    this.DI220 = atoms;
                                    break;
                                case 1:
                                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                                    Offset += 2;
                                    atoms.Add(new Atom("Wind Speed", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 1))));
                                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                                    Offset += 2;
                                    atoms.Add(new Atom("Wind Direction", 1, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 1))));
                                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                                    Offset += 2;
                                    atoms.Add(new Atom("Temperature", 2, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.25))));
                                    s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                    ++Offset;
                                    atoms.Add(new Atom("Turbulence", 3, Convert.ToString(Convert.ToInt32(s))));
                                    this.DI220 = atoms;
                                    break;
                            }
                        }
                        if (listFSPEC[35])
                        {
                            List<Atom> atoms = new List<Atom>();
                            string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                            Offset += 2;
                            int code = Convert.ToInt16(string.Concat(s[0]));
                            switch (code)
                            {
                                case 0:
                                    atoms.Add(new Atom("Source Availability", 0, "No source information provided"));
                                    break;
                                case 1:
                                    atoms.Add(new Atom("Source Availability", 1, "Source Information provided"));
                                    break;
                            }
                            code = Convert.ToInt16(string.Concat(s[1], s[2]));
                            switch (code)
                            {
                                case 0:
                                    atoms.Add(new Atom("Source", 0, "Unknown"));
                                    break;
                                case 1:
                                    atoms.Add(new Atom("Source", 1, "Aircraft Altitude (Holding Altitude)"));
                                    break;
                                case 2:
                                    atoms.Add(new Atom("Source", 2, "MCP/FCU Selected Altitude"));
                                    break;
                                case 3:
                                    atoms.Add(new Atom("Source", 3, "FMS Selected Altitude"));
                                    break;
                            }
                            s = s.Remove(0, 3);
                            atoms.Add(new Atom("Altitude", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 25))));
                            this.DI146 = atoms;
                        }
                        if (listFSPEC[36])
                        {
                            List<Atom> atoms = new List<Atom>();
                            string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                            Offset += 2;
                            int code = Convert.ToInt16(string.Concat(s[0]));
                            switch (code)
                            {
                                case 0:
                                    atoms.Add(new Atom("Manage Vertical Mode", 0, "Not active or unknown"));
                                    break;
                                case 1:
                                    atoms.Add(new Atom("Manage Vertical Mode", 1, "Active"));
                                    break;
                            }
                            code = Convert.ToInt16(string.Concat(s[1]));
                            switch (code)
                            {
                                case 0:
                                    atoms.Add(new Atom("Altitude Hold Mode", 0, "Not active or unknown"));
                                    break;
                                case 1:
                                    atoms.Add(new Atom("Altitude Hold Mode", 1, "Active"));
                                    break;
                            }
                            code = Convert.ToInt16(string.Concat(s[2]));
                            switch (code)
                            {
                                case 0:
                                    atoms.Add(new Atom("Approach Mode", 0, "Not active or unknown"));
                                    break;
                                case 1:
                                    atoms.Add(new Atom("Approach Mode", 1, "Active"));
                                    break;
                            }
                            s = s.Remove(0, 3);
                            atoms.Add(new Atom("Altitude", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 25))));

                            this.DI148 = atoms;
                        }
                        if (listFSPEC[37])
                            decode110();
                        if(listFSPEC[38])
                        {
                            string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                            ++Offset;
                            this.DI016 = Convert.ToSingle(Convert.ToInt16(s)*0.5);
                        }
                        if(listFSPEC[39])
                        {
                            if(listFSPEC[40])
                            {
                                List<Atom> atoms = new List<Atom>();
                                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(16, '0');
                                Offset += 1;
                                int code = Convert.ToInt16(string.Concat(s[0]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("TCAS Resolution Advisory active", 0, "TCAS II or ACAS RA not active"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("TCAS Resolution Advisory active", 1, "TCAS RA active"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[1],s[2]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Target Trajectory Change Report Capability", 0, "no capability for Trajectory Change Reports"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Target Trajectory Change Report Capability", 1, "support for TC+0 reports only"));
                                        break;
                                    case 2:
                                        atoms.Add(new Atom("Target Trajectory Change Report Capability", 2, "support for multiple TC reports"));
                                        break;
                                    case 3:
                                        atoms.Add(new Atom("Target Trajectory Change Report Capability", 3, "reserved"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[3]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Target State Report Capability", 0, "no capability to support Target State Reports"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Target State Report Capability", 1, "capable of supporting target State Reports"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[4]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Air-Referenced Velocity Report Capability", 0, "no capability to generate ARV-reports"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Air-Referenced Velocity Report Capability", 1, "capable of generate ARV-reports"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[5]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Cockpit Display of Traffic Information airborne", 0, "CDTI not operational"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Cockpit Display of Traffic Information airborne", 1, "CDTI operational"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[6]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("TCAS System Status", 0, "TCAS operational"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("TCAS System Status", 1, "TCAS not operational"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[7]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Single Antenna", 0, "Antenna Diversity"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Single Antenna", 1, "Single Antenna only"));
                                        break;
                                }

                                this.DI008 = atoms;
                            }
                            if(listFSPEC[41])
                            {
                                List<Atom> atoms = new List<Atom>();
                                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(16, '0');
                                Offset += 1;
                                int code = Convert.ToInt16(string.Concat(s[2]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Position Offset Applied", 0, "Position transmitted is not ADS-B position reference point"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Position Offset Applied", 1, "Position transmitted is the ADS-B position reference point"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[3]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Cockpit Display of Traffic Information Surface", 0, "CDTI not operational"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Cockpit Display of Traffic Information Surface", 1, "CDTI operational"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[4]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Class B2 transmit power less than 70 Watts", 0, "≥ 70 Watts"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Class B2 transmit power less than 70 Watts", 1, "< 70 Watts"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[5]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Receiving ATC Services", 0, "Aircraft not receiving ATC-services"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Receiving ATC Services", 1, "Aircraft receiving ATC services"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[6]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Setting of “IDENT”-switch", 0, "IDENT switch not active"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Setting of “IDENT”-switch", 1, "IDENT switch active"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[7]));
                                switch (code)
                                {
                                    case 0:
                                        this.DI271 = atoms;
                                        break;
                                    case 1:
                                        s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                        Offset += 1;
                                        atoms.Add(new Atom("Length and width of the aircraft", 0, Convert.ToString(Convert.ToInt16(s))));
                                        this.DI271 = atoms;
                                        break;
                                }
                                
                            }
                            if (listFSPEC[42])
                            {
                                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                Offset += 1;
                                this.DI132 = Convert.ToInt32(s);
                            }
                            if (listFSPEC[43])                            
                                DecodeMSdata();
                            
                            if (listFSPEC[44])
                            {
                                List<Atom> atoms = new List<Atom>();
                                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                                Offset += 1;
                                atoms.Add(new Atom("TYP", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[0],s[1],s[2],s[3],s[4])))));
                                atoms.Add(new Atom("STYP", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[5], s[6], s[7])))));
                                s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset],this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                                Offset += 1;
                                s = s.Remove(14, 2);
                                atoms.Add(new Atom("ARA", 0, Convert.ToString(Convert.ToInt32(s))));
                                s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                                s = s.Remove(0, 5);
                                atoms.Add(new Atom("RAC", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[0],s[1],s[2],s[3])))));
                                atoms.Add(new Atom("RAT", 0, Convert.ToString(Convert.ToInt32(s[4]))));
                                atoms.Add(new Atom("MTE", 0, Convert.ToString(Convert.ToInt32(s[5]))));
                                atoms.Add(new Atom("TTI", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[6], s[7])))));
                                s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), 16), 2).PadLeft(8*4, '0');
                                Offset += 4;
                                s = s.Remove(0, 5);
                                atoms.Add(new Atom("TID", 0, Convert.ToString(Convert.ToInt32(s))));
                                this.DI260 = atoms;
                            }
                            if (listFSPEC[45])
                            {
                                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                Offset += 1;
                                this.DI400 = Convert.ToInt32(s);
                            }
                            if (listFSPEC[46])
                            {
                                List<Atom> atoms = new List<Atom>();
                                string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                                Offset += 1;
                                int code = Convert.ToInt16(string.Concat(s[0]));
                                switch(code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Subfield #1: Aircraft Operational Status age", 0, "Absence of Subfield #1"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Subfield #1: Aircraft Operational Status age", 0, "Presence of Subfield #1"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[1]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Subfield #2: Target Report Descriptor age", 0, "Absence of Subfield #2"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Subfield #2: Target Report Descriptor age", 0, "Presence of Subfield #2"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[2]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Subfield #3: Mode 3/A Code age", 0, "Absence of Subfield #3"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Subfield #3: Mode 3/A Code age", 0, "Presence of Subfield #3"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[3]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Subfield #4: Quality Indicators age", 0, "Absence of Subfield #4"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Subfield #4: Quality Indicators age", 0, "Presence of Subfield #4"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[4]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Subfield #5: Trajectory Intent age", 0, "Absence of Subfield #5"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Subfield #5: Trajectory Intent age", 0, "Presence of Subfield #5"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[5]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Subfield #6: Message Amplitude age", 0, "Absence of Subfield #6"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Subfield #6: Message Amplitude age", 0, "Presence of Subfield #6"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[6]));
                                switch (code)
                                {
                                    case 0:
                                        atoms.Add(new Atom("Subfield #7: Geometric Height age", 0, "Absence of Subfield #7"));
                                        break;
                                    case 1:
                                        atoms.Add(new Atom("Subfield #7: Geometric Height age", 0, "Presence of Subfield #7"));
                                        break;
                                }
                                code = Convert.ToInt16(string.Concat(s[7]));
                                switch (code)
                                {
                                    case 0:
                                        this.DI295 = atoms;
                                        break;
                                    case 1:
                                        s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                                        Offset += 1;
                                        code = Convert.ToInt16(string.Concat(s[0]));
                                        switch (code)
                                        {
                                            case 0:
                                                atoms.Add(new Atom("Subfield #8: Flight Level age", 0, "Absence of Subfield #8"));
                                                break;
                                            case 1:
                                                atoms.Add(new Atom("Subfield #8: Flight Level age", 0, "Presence of Subfield #8"));
                                                break;
                                        }
                                        code = Convert.ToInt16(string.Concat(s[1]));
                                        switch (code)
                                        {
                                            case 0:
                                                atoms.Add(new Atom("Subfield #9: Intermediate State Selected Altitude age", 0, "Absence of Subfield #9"));
                                                break;
                                            case 1:
                                                atoms.Add(new Atom("Subfield #9: Intermediate State Selected Altitude age", 0, "Presence of Subfield #9"));
                                                break;
                                        }
                                        code = Convert.ToInt16(string.Concat(s[2]));
                                        switch (code)
                                        {
                                            case 0:
                                                atoms.Add(new Atom("Subfield #10: Final State Selected Altitude age", 0, "Absence of Subfield #10"));
                                                break;
                                            case 1:
                                                atoms.Add(new Atom("Subfield #10: Final State Selected Altitude age", 0, "Presence of Subfield #10"));
                                                break;
                                        }
                                        code = Convert.ToInt16(string.Concat(s[3]));
                                        switch (code)
                                        {
                                            case 0:
                                                atoms.Add(new Atom("Subfield #11: Air Speed age", 0, "Absence of Subfield #11"));
                                                break;
                                            case 1:
                                                atoms.Add(new Atom("Subfield #11: Air Speed age", 0, "Presence of Subfield #11"));
                                                break;
                                        }
                                        code = Convert.ToInt16(string.Concat(s[4]));
                                        switch (code)
                                        {
                                            case 0:
                                                atoms.Add(new Atom("Subfield #12: True Air Speed age", 0, "Absence of Subfield #12"));
                                                break;
                                            case 1:
                                                atoms.Add(new Atom("Subfield #12: True Air Speed age", 0, "Presence of Subfield #12"));
                                                break;
                                        }
                                        code = Convert.ToInt16(string.Concat(s[5]));
                                        switch (code)
                                        {
                                            case 0:
                                                atoms.Add(new Atom("Subfield #13: Magnetic Heading age", 0, "Absence of Subfield #13"));
                                                break;
                                            case 1:
                                                atoms.Add(new Atom("Subfield #13: Magnetic Heading age", 0, "Presence of Subfield #13"));
                                                break;
                                        }
                                        code = Convert.ToInt16(string.Concat(s[6]));
                                        switch (code)
                                        {
                                            case 0:
                                                atoms.Add(new Atom("Subfield #14: Barometric Vertical Rate age", 0, "Absence of Subfield #14"));
                                                break;
                                            case 1:
                                                atoms.Add(new Atom("Subfield #14: Barometric Vertical Rate age", 0, "Presence of Subfield #14"));
                                                break;
                                        }
                                        code = Convert.ToInt16(string.Concat(s[7]));
                                        switch (code)
                                        {
                                            case 0:
                                                this.DI295 = atoms;
                                                break;
                                            case 1:
                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                                                Offset += 1;
                                                code = Convert.ToInt16(string.Concat(s[0]));
                                                switch (code)
                                                {
                                                    case 0:
                                                        atoms.Add(new Atom("Subfield #15: Geometric Vertical Rate age", 0, "Absence of Subfield #15"));
                                                        break;
                                                    case 1:
                                                        atoms.Add(new Atom("Subfield #15: Geometric Vertical Rate age", 0, "Presence of Subfield #15"));
                                                        break;
                                                }
                                                code = Convert.ToInt16(string.Concat(s[1]));
                                                switch (code)
                                                {
                                                    case 0:
                                                        atoms.Add(new Atom("Subfield #16: Ground Vector age", 0, "Absence of Subfield #16"));
                                                        break;
                                                    case 1:
                                                        atoms.Add(new Atom("Subfield #16: Ground Vector age", 0, "Presence of Subfield #16"));
                                                        break;
                                                }
                                                code = Convert.ToInt16(string.Concat(s[2]));
                                                switch (code)
                                                {
                                                    case 0:
                                                        atoms.Add(new Atom("Subfield #17: Track Angle Rate age", 0, "Absence of Subfield #17"));
                                                        break;
                                                    case 1:
                                                        atoms.Add(new Atom("Subfield #17: Track Angle Rate age", 0, "Presence of Subfield #17"));
                                                        break;
                                                }
                                                code = Convert.ToInt16(string.Concat(s[3]));
                                                switch (code)
                                                {
                                                    case 0:
                                                        atoms.Add(new Atom("Subfield #18: Target Identification age", 0, "Absence of Subfield #18"));
                                                        break;
                                                    case 1:
                                                        atoms.Add(new Atom("Subfield #18: Target Identification age", 0, "Presence of Subfield #18"));
                                                        break;
                                                }
                                                code = Convert.ToInt16(string.Concat(s[4]));
                                                switch (code)
                                                {
                                                    case 0:
                                                        atoms.Add(new Atom("Subfield #19: Target Status age", 0, "Absence of Subfield #19"));
                                                        break;
                                                    case 1:
                                                        atoms.Add(new Atom("Subfield #19: Target Status age", 0, "Presence of Subfield #19"));
                                                        break;
                                                }
                                                code = Convert.ToInt16(string.Concat(s[5]));
                                                switch (code)
                                                {
                                                    case 0:
                                                        atoms.Add(new Atom("Subfield #20: Met Information age", 0, "Absence of Subfield #20"));
                                                        break;
                                                    case 1:
                                                        atoms.Add(new Atom("Subfield #20: Met Information age", 0, "Presence of Subfield #20"));
                                                        break;
                                                }
                                                code = Convert.ToInt16(string.Concat(s[6]));
                                                switch (code)
                                                {
                                                    case 0:
                                                        atoms.Add(new Atom("Subfield #21: Roll Angle age", 0, "Absence of Subfield #21"));
                                                        break;
                                                    case 1:
                                                        atoms.Add(new Atom("Subfield #21: Roll Angle age", 0, "Presence of Subfield #21"));
                                                        break;
                                                }
                                                code = Convert.ToInt16(string.Concat(s[7]));
                                                switch (code)
                                                {
                                                    case 0:
                                                        this.DI295 = atoms; 
                                                        break;
                                                    case 1:
                                                        s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                                                        Offset += 1;
                                                        code = Convert.ToInt16(string.Concat(s[0]));
                                                        switch (code)
                                                        {
                                                            case 0:
                                                                atoms.Add(new Atom("Subfield #22: ACAS Resolution Advisory age", 0, "Absence of Subfield #22"));
                                                                break;
                                                            case 1:
                                                                atoms.Add(new Atom("Subfield #22: ACAS Resolution Advisory age", 0, "Presence of Subfield #22"));
                                                                break;
                                                        }
                                                        code = Convert.ToInt16(string.Concat(s[1]));
                                                        switch (code)
                                                        {
                                                            case 0:
                                                                atoms.Add(new Atom("Subfield #23: Surface Capabilities and Characteristics age", 0, "Absence of Subfield #23"));
                                                                break;
                                                            case 1:
                                                                atoms.Add(new Atom("Subfield #23: Surface Capabilities and Characteristics age", 0, "Presence of Subfield #23"));
                                                                break;
                                                        }
                                                        code = Convert.ToInt16(string.Concat(s[7]));
                                                        switch (code)
                                                        {
                                                            case 0:
                                                                this.DI295 = atoms;
                                                                break;
                                                            case 1:
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("AOS", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)*0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("TRD", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("M3A", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("QI", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("TI", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("MAM", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("GH", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("FL", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("ISA", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("FSA", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("AS", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("TAS", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("MH", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("BVR", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("GVR", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("GV", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("TAR", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("TI", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("TS", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("MET", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("ROA", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("ARA", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                                                Offset += 1;
                                                                atoms.Add(new Atom("SCC", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.1))));
                                                                this.DI295 = atoms;
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                        break;
                                }
                            }
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

        private void decodeTrackNumber()
        {
            this.DI161 = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
            Offset += 2;
        }

        private void decodeLatLong()
        {
            int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            float latreal = Convert.ToSingle(lat * 180 / Math.Pow(2, 23));

            int lon = Int32.Parse(string.Concat(this.rawList[Offset + 3], this.rawList[Offset + 4], this.rawList[Offset + 5]), System.Globalization.NumberStyles.HexNumber);
            float lonreal = Convert.ToSingle(lon * 180 / Math.Pow(2, 23));

            Offset += 6;

            this.DI130 = new Point().LatLong2XY(latreal, lonreal);
        }

        private void decodeLatLong_HighRes()
        {
            int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), System.Globalization.NumberStyles.HexNumber);
            float latreal = Convert.ToSingle(lat * 180 / Math.Pow(2, 30));

            int lon = Int32.Parse(string.Concat(this.rawList[Offset + 4], this.rawList[Offset + 5], this.rawList[Offset + 6], this.rawList[Offset + 7]), System.Globalization.NumberStyles.HexNumber);
            float lonreal = Convert.ToSingle(lon * 180 / Math.Pow(2, 30));

            Offset += 8;

            this.DI131 = new Point().LatLong2XY(latreal, lonreal);
        }

        private void decodeTOD()
        {
            int LSB = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            Offset += 3;

            this.DI073 = new DateTime().AddSeconds((float)LSB / 128);
        }

        private void decodeTODVel()
        {
            int LSB = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            Offset += 3;

            this.DI072 = new DateTime().AddSeconds((float)LSB / 128);
        }

        private void decodeICAOAddress()
        {
            this.DI080 = string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]).ToUpper();
            Offset += 3;
        }

        private void DecodeMSdata()
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

        private void decode110()
        {
            List<Atom> atoms = new List<Atom>();
            string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
            Offset++;
            int code = Convert.ToInt16(string.Concat(s[0]));
            switch (code)
            {
                case 0:
                    atoms.Add(new Atom("NAV", 0, "Trajectory Intent Data is available for this aircraft"));
                    break;
                case 1:
                    atoms.Add(new Atom("NAV", 1, "Trajectory Intent Data is not available for this aircraft"));
                    break;
            }
            code = Convert.ToInt16(string.Concat(s[1]));
            switch (code)
            {
                case 0:
                    atoms.Add(new Atom("NVB", 0, "Trajectory Intent Data is valid"));
                    break;
                case 1:
                    atoms.Add(new Atom("NVB", 1, "Trajectory Intent Data is not valid"));
                    break;
            }
            code = Convert.ToInt16(string.Concat(s[7]));
            switch(code)
            {
                case 0:
                    this.DI110 = atoms;
                    break;
                case 1:
                    atoms.Add(new Atom("REP",0,Convert.ToString(Convert.ToInt32(Convert.ToInt32(this.rawList[Offset], 16)))));
                    ++Offset;
                    s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                    Offset++;
                    switch (Convert.ToInt16(string.Concat(s[0])))
                    {
                        case 0:
                            atoms.Add(new Atom("TCA", 0, "TCP number available"));
                            break;
                        case 1:
                            atoms.Add(new Atom("TCA", 1, "TCP number not available"));
                            break;
                    }
                    switch (Convert.ToInt16(string.Concat(s[1])))
                    {
                        case 0:
                            atoms.Add(new Atom("NC", 0, "TCP compliance"));
                            break;
                        case 1:
                            atoms.Add(new Atom("NC", 1, "TCP non-compliance"));
                            break;
                    }
                    s = s.Remove(0, 2);
                    atoms.Add(new Atom("TCP number", 0, Convert.ToString(Convert.ToInt32(s))));
                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset += 2;
                    atoms.Add(new Atom("Altitude", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)*10))));
                    int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
                    float latreal = Convert.ToSingle(lat * 180 / Math.Pow(2, 23));

                    int lon = Int32.Parse(string.Concat(this.rawList[Offset + 3], this.rawList[Offset + 4], this.rawList[Offset + 5]), System.Globalization.NumberStyles.HexNumber);
                    float lonreal = Convert.ToSingle(lon * 180 / Math.Pow(2, 23));

                    atoms.Add(new Atom("Latitude", 0, Convert.ToString(latreal)));
                    atoms.Add(new Atom("Longitude", 0, Convert.ToString(lonreal)));

                    Offset += 6;

                    s = Convert.ToString(Convert.ToInt32((this.rawList[Offset], 16)), 2).PadLeft(8, '0');
                    Offset += 1;

                    code = Convert.ToInt32(string.Concat(s[0], s[1], s[2], s[3]));
                    switch(code)
                    {
                        case 0:
                            atoms.Add(new Atom("Point Type", 0, "Unknown"));
                            break;
                        case 1:
                            atoms.Add(new Atom("Point Type", 1, "Fly by waypoint (LT)"));
                            break;
                        case 2:
                            atoms.Add(new Atom("Point Type", 2, "Fly over waypoint (LT)"));
                            break;
                        case 3:
                            atoms.Add(new Atom("Point Type", 3, "Hold pattern (LT)"));
                            break;
                        case 4:
                            atoms.Add(new Atom("Point Type", 4, "Procedure hold (LT)"));
                            break;
                        case 5:
                            atoms.Add(new Atom("Point Type", 5, "Procedure turn (LT)"));
                            break;
                        case 6:
                            atoms.Add(new Atom("Point Type", 6, "RF leg (LT)"));
                            break;
                        case 7:
                            atoms.Add(new Atom("Point Type", 7, "Top of climb (VT)"));
                            break;
                        case 8:
                            atoms.Add(new Atom("Point Type", 8, "Top of descent (VT)"));
                            break;
                        case 9:
                            atoms.Add(new Atom("Point Type", 9, "Start of level (VT)"));
                            break;
                        case 10:
                            atoms.Add(new Atom("Point Type", 10, "Cross-over altitude (VT)"));
                            break;
                        case 11:
                            atoms.Add(new Atom("Point Type", 11, "Transition altitude (VT)"));
                            break;
                    }
                    code = Convert.ToInt32(string.Concat(s[4], s[5]));
                    switch (code)
                    {
                        case 0:
                            atoms.Add(new Atom("TD", 0, "N/A"));
                            break;
                        case 1:
                            atoms.Add(new Atom("TD", 1, "Turn right"));
                            break;
                        case 2:
                            atoms.Add(new Atom("TD", 2, "Turn left"));
                            break;
                        case 3:
                            atoms.Add(new Atom("TD", 3, "No turn"));
                            break;
                    }
                    
                    code = Convert.ToInt16(string.Concat(s[6]));
                    switch(code)
                    {
                        case 0:
                            atoms.Add(new Atom("Turn Radius Availabilty", 0, "TTR not available"));
                            break;
                        case 1:
                            atoms.Add(new Atom("Turn Radius Availabilty", 1, "TTR available"));
                            break;
                    }
                    code = Convert.ToInt16(string.Concat(s[7]));
                    switch (code)
                    {
                        case 0:
                            atoms.Add(new Atom("TOA", 0, "TOV available"));
                            break;
                        case 1:
                            atoms.Add(new Atom("TOA", 1, "TOV not available"));
                            break;
                    }
                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1],this.rawList[Offset + 2]), 16));
                    Offset += 3;
                    atoms.Add(new Atom("Time Over Point", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 1))));
                    
                    
                    s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset += 2;
                    atoms.Add(new Atom("TCP Turn radius", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s) * 0.01))));

                    this.DI110 = atoms;
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

            List<char> code = new List<char>();
            for (int i = 0; i < 8; i++)
            {
                if (stringCode.Substring(6 * i, 6).StartsWith("0"))
                    code.Add((char)Convert.ToInt32(string.Concat("01", stringCode.Substring(6 * i, 6)), 2));
                else
                    code.Add((char)Convert.ToInt32(string.Concat("00", stringCode.Substring(6 * i, 6)), 2));
            }
            this.DI170 = Regex.Replace(string.Join("", code.ToArray()), @"\s", "");
        }
    }
}
