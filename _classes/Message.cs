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
        private int Offset; //Donde empieza el siguiente campo
        private CAT20 CAT20;
        private CAT19 CAT19;
        private CAT10 CAT10;
        private CAT21v023 CAT21v023; //p12ed023
        private CAT21v24 CAT21v24; //el otro


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
                    CAT10 = new CAT10(rawList, listFSPEC, Offset).decode();
                    break;
                case 19:
                    decodeCAT19();
                    break;
                case 20:
                    decodeCAT20();
                    break;
                case 21:
                    decodeCAT21v023();
                    break;
            }
        }

        public DateTime getTOD()
        {
            switch (CAT)
            {
                case 10:
                    return CAT10.DI140;
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
                    return CAT10.DI161;
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
                    return CAT10.DI220;
                case 19:
                    return "NONE";
                case 20:
                    return CAT20.DI220;
                case 21:
                    if (CAT21v023 == null)
                        return CAT21v24.DI080;
                    else
                        return CAT21v023.DI080;
                default:
                    return "";
            }
        }
        public string getType() //TODO
        {
            switch (CAT)
            {
                case 10:
                    return CAT10.DI000;
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
                    if (CAT10.DI245 == null)
                        return "NONE";
                    else
                        return CAT10.DI245[1].getStr();
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
                    if (CAT10.DI042 != null)
                        return CAT10.DI042;
                    else if (CAT10.DI041 != null)
                        return CAT10.DI041;
                    else
                        return CAT10.DI040;
                case 19:
                    return new Point();
                case 20:
                    return CAT20.DI042;
                case 21:
                    if (CAT21v023 == null)
                        return CAT21v24.DI130;
                    else
                        return CAT21v023.DI130;
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
                    return Convert.ToInt32(CAT10.DI010[0].getVal());
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
                    return Convert.ToInt32(CAT10.DI010[1].getVal());
                case 19:
                    return Convert.ToInt32(CAT19.DI010[1].getVal());
                case 20:
                    return Convert.ToInt32(CAT20.DI010[1].getVal());
                case 21:
                    if (CAT21v023 == null)
                        return Convert.ToInt32(CAT21v24.DI010[1].getVal());
                    else
                        return Convert.ToInt32(CAT21v023.DI010[1].getVal());
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
        public int getFSPECount()
        {
            return this.listFSPEC.Count(c => c);
        }

        //Decoding CATs
        

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

            if (this.listFSPEC[4])
                CAT20.DI041 = decodeLatLong_CAT20_10();
            
            if (this.listFSPEC[5])
                CAT20.DI042 = decodeXY();

            if (this.listFSPEC[6])
                CAT20.DI161 = decodeTrackNumber();

            if (this.listFSPEC[7])
                CAT20.DI170 = decodeTrackStatus_CAT20();

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

        private void decodeCAT21v023() //p12ed023 -- CAT21v023
        {
            CAT21v023 = new CAT21v023();
            if (this.listFSPEC[1])
                CAT21v023.DI010 = decodeSACSIC();
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
                CAT21v023.DI040 = atoms;
            }
            if (this.listFSPEC[3])
                CAT21v023.DI030 = decodeTOD();
            if (this.listFSPEC[4])
                CAT21v023.DI130 = decodeLatLong_CAT21v023();
            if (this.listFSPEC[5])
            {
                CAT21v023.DI080 = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), 16), 2).PadLeft(16, '0');
                Offset += 3;
            }
            if (this.listFSPEC[6])
            {
                int alt = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
                CAT21v023.DI140 = Convert.ToSingle(alt * 6.25);
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
                code = Int32.Parse(string.Concat(s[4], s[5],s[6] ,s[7]), System.Globalization.NumberStyles.HexNumber);
                a = new Atom("Position Accuracy", code, Convert.ToString(code));
                Offset += 1;
                CAT21v023.DI090 = atoms;
            }
            if(listFSPEC[8])
            {
                if(listFSPEC[9])
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
                    CAT21v023.DI210 = atoms;
                }
                if (listFSPEC[10])
                {
                    float RA = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.01);
                    CAT21v023.DI230 = RA;
                    Offset += 2;
                }
                if (listFSPEC[11])
                {
                    float FL = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 0.25);
                    CAT21v023.DI145 = (int)FL;
                    Offset += 2;
                }
                if (listFSPEC[12])
                {
                    string s = Convert.ToString(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    int code = Convert.ToInt16(s[0]);
                    s = s.Remove(0, 1);
                    float Airspeed;
                    switch(code)
                    {
                        case 0:
                            Airspeed = Convert.ToSingle(Convert.ToInt16(s, 2) * 2*10^(-14));
                            CAT21v023.DI150 = Airspeed;
                            break;
                        case 1:
                            Airspeed = Convert.ToSingle(Convert.ToInt16(s, 2) * 0.001);
                            CAT21v023.DI150 = Airspeed;
                            break;
                    }      
                    Offset += 2;
                }
                if (listFSPEC[13])
                {
                    CAT21v023.DI151 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset += 2;
                }
                if (listFSPEC[14])
                {
                    CAT21v023.DI152 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * (360/(2^16)));
                    Offset += 2;
                }
                if (listFSPEC[15])
                {
                    CAT21v023.DI155 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 6.25);
                    Offset += 2;
                }
                if (listFSPEC[16])
                {
                    if(listFSPEC[17])
                    {
                        CAT21v023.DI157 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16) * 6.25);
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
                        CAT21v023.DI160 = atoms;
                        Offset += 2;
                    }
                    if (listFSPEC[19])
                    {
                        List<Atom> atoms = new List<Atom>();
                        Atom a;
                        string s = Convert.ToString(Convert.ToInt16(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                        int code = Convert.ToInt16(string.Concat(s[0],s[1]));
                        switch(code)
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
                        switch(code)
                        {
                            case 0:
                                CAT21v023.DI165 = atoms;
                                Offset += 1;
                                break;
                            case 1:
                                s = Convert.ToString(Convert.ToInt16(this.rawList[Offset + 1], 2));
                                Offset += 2;
                                s = s.Remove(7, 1);
                                a = new Atom("Rate of Turn", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt16(s, 2) * 0.25)));
                                atoms.Add(a);
                                CAT21v023.DI165 = atoms;
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

                        CAT21v023.DI170 = atoms;
                    }
                    if (listFSPEC[21])
                    {
                        CAT21v023.DI095 = Convert.ToString(Convert.ToInt16(this.rawList[Offset], 16));
                        Offset += 1;
                    }
                    if (listFSPEC[22])
                    {
                        CAT21v023.DI032 = Convert.ToSingle(Convert.ToInt16( this.rawList[Offset], 16) * (2 ^(-8)));
                        Offset += 1;
                    }
                    if (listFSPEC[23])
                    {
                        List<Atom> atoms = new List<Atom>();
                        Atom a;
                        string s = Convert.ToString(Convert.ToInt16(this.rawList[Offset], 16));
                        switch(Convert.ToInt16(s))
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
                        CAT21v023.DI200 = atoms;
                    }
                    if (listFSPEC[24])
                    {
                        if (listFSPEC[25])
                        {
                            int s = Convert.ToInt16(this.rawList[Offset], 16);
                            Offset += 1;
                            switch(s)
                            {
                                case 1:
                                    CAT21v023.DI020 = "light aircraft";
                                    break;
                                case 2:
                                    CAT21v023.DI020 = "reserved";
                                    break;
                                case 3:
                                    CAT21v023.DI020 = "medium aircraft";
                                    break;
                                case 4:
                                    CAT21v023.DI020 = "reserved";
                                    break;
                                case 5:
                                    CAT21v023.DI020 = "heavy aircraft";
                                    break;
                                case 6:
                                    CAT21v023.DI020 = "highly manoeuvrable and high speed";
                                    break;
                                case 7:
                                    CAT21v023.DI020 = "reserved";
                                    break;
                                case 8:
                                    CAT21v023.DI020 = "reserved";
                                    break;
                                case 9:
                                    CAT21v023.DI020 = "reserved";
                                    break;
                                case 10:
                                    CAT21v023.DI020 = "rotocraft";
                                    break;
                                case 11:
                                    CAT21v023.DI020 = "glider/sailplane";
                                    break;
                                case 12:
                                    CAT21v023.DI020 = "lighter-than-air";
                                    break;
                                case 13:
                                    CAT21v023.DI020 = "unmanned aerial vehicle";
                                    break;
                                case 14:
                                    CAT21v023.DI020 = "space/transatmospheric vehicle";
                                    break;
                                case 15:
                                    CAT21v023.DI020 = "ultralight/handglider/paraglider";
                                    break;
                                case 16:
                                    CAT21v023.DI020 = "parachutist/skydiver";
                                    break;
                                case 17:
                                    CAT21v023.DI020 = "reserved";
                                    break;
                                case 18:
                                    CAT21v023.DI020 = "reserved";
                                    break;
                                case 19:
                                    CAT21v023.DI020 = "reserved";
                                    break;
                                case 20:
                                    CAT21v023.DI020 = "surface emergency vehicle";
                                    break;
                                case 21:
                                    CAT21v023.DI020 = "surface service vehicle";
                                    break;
                                case 22:
                                    CAT21v023.DI020 = "fixed ground or tethered obstruction";
                                    break;
                                case 23:
                                    CAT21v023.DI020 = "reserved";
                                    break;
                                case 24:
                                    CAT21v023.DI020 = "reserved";
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
        }

        private void decodeCAT21v24() //(Alex, inserte aqui el pdf) -- CAT21v24
        {
            if (listFSPEC[1])
                CAT21v24.DI010 = decodeSACSIC();
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
                CAT21v24.DI040 = atoms;
                Offset += 1;

            }
            if (listFSPEC[3])
                CAT21v24.DI161 = decodeTrackNumber();
            if (listFSPEC[4])
            {
                CAT21v24.DI015 = Convert.ToInt16(this.rawList[Offset], 16);
                Offset += 1;
            }
            if (listFSPEC[5])
            {
                CAT21v24.DI071 = Convert.ToSingle(Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset +1], this.rawList[Offset+2]),16)*(1/128));
                Offset += 3;
            }
            if (listFSPEC[6])
                CAT21v24.DI130 = decodeLatLong_CAT21v023();
            if (listFSPEC[7])
            {
                int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3], this.rawList[Offset + 4]), System.Globalization.NumberStyles.HexNumber);
                float latreal = Convert.ToSingle(lat * 180 / 2 ^ 30);

                int lon = Int32.Parse(string.Concat(this.rawList[Offset + 5], this.rawList[Offset + 6], this.rawList[Offset + 7], this.rawList[Offset + 8], this.rawList[Offset + 9]), System.Globalization.NumberStyles.HexNumber);
                float lonreal = Convert.ToSingle(lon * 180 / 2 ^ 30);

                Offset += 10;
                CAT21v24.DI131 = new Point().LatLong2XY(latreal, lonreal);
            }
            if (listFSPEC[8])
            {
                if(listFSPEC[9])
                    CAT21v24.DI072 = decodeTOD();

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
                            CAT21v24.DI150 = Airspeed;
                            break;
                        case 1:
                            Airspeed = Convert.ToSingle(Convert.ToInt16(s, 2) * 0.001);
                            CAT21v24.DI150 = Airspeed;
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
                            CAT21v24.DI151 = Airspeed;
                            break;
                        case 1:
                            Airspeed = 32768;
                            CAT21v24.DI151 = Airspeed;
                            break;
                    }
                    Offset += 2;
                }
                if (listFSPEC[12])
                    CAT21v24.DI080 = decodeICAOAddress();
                if (listFSPEC[13])
                    CAT21v24.DI073 = decodeTOD();
                if (listFSPEC[14])
                {
                    List<Atom> atoms = new List<Atom>();
                    Atom a;
                    string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), 16));
                    int code = Convert.ToInt16(string.Concat(s[0], s[1]));
                    switch(code)
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
                    a = new Atom("TOMROP", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s))*(2^(-30))));
                    atoms.Add(a);
                    CAT21v24.DI074 = atoms;
                    Offset += 4;
                        
                }
                if (listFSPEC[15])
                {
                    CAT21v24.DI075 = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), 16)*(1/128));
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
                        a = new Atom("TOMROP", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)*(2^(-30)))));
                        atoms.Add(a);
                        CAT21v24.DI076 = atoms;
                        Offset += 4;
                    }
                }
                if (listFSPEC[18])
                {
                    CAT21v24.DI140 = Convert.ToSingle(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1])) * 6.25);
                    Offset += 2;
                }
                if (listFSPEC[19])
                {
                    List<Atom> atoms = new List<Atom>();
                    Atom a;
                    string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                    ++Offset;
                    atoms.Add(new Atom("NUCr or NACv", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[0], s[1], s[2])))));
                    atoms.Add(new Atom("NUCp or NIC", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[3], s[4], s[5],s[6])))));
                    if(s[7].Equals('1'))
                    {
                        s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                        ++Offset;
                        atoms.Add(new Atom("Navigation Integrity Category for Barometric Altitude", 0, Convert.ToString(Convert.ToInt32(s[0]))));
                        atoms.Add(new Atom("Surveillance (version 1) or Source (version 2) Integrity Level", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[1], s[2])))));
                        atoms.Add(new Atom("Navigation Accuracy Category for Position",0, Convert.ToString(Convert.ToInt32(string.Concat(s[3], s[4], s[5])))));
                        if (s[7].Equals('1'))
                        {
                            s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                            ++Offset;
                            switch(Convert.ToInt32(s[2]))
                            {
                                case 0:
                                    atoms.Add(new Atom("SIL-Supplement", 0, "measured per flight-hour"));
                                    break;
                                case 1: 
                                    atoms.Add(new Atom("SIL-Supplement", 0, "measured per sample"));
                                    break;
                            }
                            atoms.Add(new Atom("Horizontal Position System Design Assurance", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[3], s[4])))));
                            atoms.Add(new Atom("Geometric Altitude Accuracy",0, Convert.ToString(Convert.ToInt32(string.Concat(s[5], s[6])))));
                            if (s[7].Equals('1'))
                            {
                                s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                                ++Offset;
                                atoms.Add(new Atom("Position Integrity Category", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[0], s[1],s[2], s[3])))));
                            }
                            else
                                CAT21v24.DI090 = atoms;
                        }
                        else
                            CAT21v24.DI090 = atoms;
                    }
                    else
                        CAT21v24.DI090 = atoms;
                }
                if(listFSPEC[20])
                {
                    List<Atom> atoms = new List<Atom>();
                    string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                    ++Offset;
                    atoms.Add(new Atom("Version Not Supported", 0, Convert.ToString(Convert.ToInt32(s[1]))));
                    atoms.Add(new Atom("Version Number", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[2], s[3], s[4])))));
                    atoms.Add(new Atom("Link Technology Type", 0, Convert.ToString(Convert.ToInt32(string.Concat(s[5], s[6], s[7])))));
                    CAT21v24.DI210 = atoms;
                }
                if(listFSPEC[21])
                {
                    string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
                    int code = Convert.ToInt32(string.Concat(s[4], s[5], s[6]), 2);

                    code += Convert.ToInt32(string.Concat(s[7], s[8], s[9]), 2);

                    code += Convert.ToInt32(string.Concat(s[10], s[11], s[12]), 2);

                    code += Convert.ToInt32(string.Concat(s[13], s[14], s[15]), 2);
                    CAT21v24.DI070 = code;
                    Offset += 2;
                }
                if(listFSPEC[22])
                {
                    string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset+=2;
                    CAT21v24.DI230 = Convert.ToSingle(Convert.ToInt32(s)*0.01);
                }
                if(listFSPEC[23])
                {
                    string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                    Offset+=2;
                    CAT21v24.DI145 = Convert.ToSingle(Convert.ToInt32(s) * 0.25);
                }
                if(listFSPEC[24])
                {
                    if(listFSPEC[25])
                    {
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                        CAT21v24.DI152 = Convert.ToSingle(Convert.ToInt32(s) * ((360) / (2 ^ 16)));
                        Offset += 2;
                    }
                    if(listFSPEC[26])
                    {
                        string s = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16));
                        ++Offset;
                        List<Atom> atoms = new List<Atom>();
                        int code = Convert.ToInt16(s[0]);
                        switch(code)
                        {
                            case 0:
                                atoms.Add(new Atom("Intent Change Flag",0, "No intent change active"));
                                break;
                            case 1:
                                atoms.Add(new Atom("Intent Change Flag",1, "Intent change flag raised"));
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
                        code = Convert.ToInt16(string.Concat(s[3],s[4],s[5]));
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

                         CAT21v24.DI200 = atoms;
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
                        CAT21v24.DI155 = atoms;
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
                        CAT21v24.DI157 = atoms;
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
                        a = new Atom("Ground Speed", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)) * (2^(-14))));
                        atoms.Add(a);
                        Offset += 2;
                        s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                        Offset += 2;
                        a = new Atom("Geometric Vertical Rate", 0, Convert.ToString(Convert.ToSingle(Convert.ToInt32(s)) * (360/(2^16))));
                        CAT21v24.DI160 = atoms;
                    }
                    if (listFSPEC[30])
                    {
                        string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16));
                        Offset += 2;
                        CAT21v24.DI165 = Convert.ToSingle(Convert.ToInt32(s) * (1 / 32));
                    }
                    if(listFSPEC[31])
                    {
                        int s =Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 1]), 16);
                        Offset += 3;
                        CAT21v24.DI077 = Convert.ToSingle(s * (1 / 128));
                    }
                }
            }
        }

        //Functions
        private List<Atom> decodeSACSIC()
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

        private Point decodeXY_CAT10()
        {
            string s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16), 2).PadLeft(16, '0');
            int x = Convert.ToInt32(s.PadLeft(32, s[0]), 2);

            s = Convert.ToString(Convert.ToInt32(string.Concat(this.rawList[Offset + 2], this.rawList[Offset + 3]), 16), 2).PadLeft(16, '0');
            int y = Convert.ToInt32(s.PadLeft(32, s[0]), 2);
            Offset += 4;
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

        private List<Atom> decodeTrackVelPolar()
        {
            List<Atom> atoms = new List<Atom>();
            List<string> ls = new List<string>() { "Speed", "Track Angle" };
            int speed = Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16);
            atoms.Add(new Atom("Speed", (float)speed * (2 ^ (-14)), Convert.ToString((float)speed * (2^(-14)))));
            Offset += 2;
            int TA = Convert.ToInt16(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), 16);
            atoms.Add(new Atom("Track Angle", (float)TA * (2^16), Convert.ToString((float) TA * (2 ^ 16))));
            Offset += 2;
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

        private Point decodeLatLong_CAT21v023()
        {
            int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            float latreal = Convert.ToSingle(lat * 180 / 2 ^ 23);

            int lon = Int32.Parse(string.Concat(this.rawList[Offset + 3], this.rawList[Offset + 4], this.rawList[Offset + 5]), System.Globalization.NumberStyles.HexNumber);
            float lonreal = Convert.ToSingle(lon * 180 / 2 ^ 23);

            Offset += 6;

            return new Point().LatLong2XY(latreal, lonreal);
        }

        private Point decodeLatLong_CAT20_10()
        {
            int lat = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2], this.rawList[Offset + 3]), System.Globalization.NumberStyles.HexNumber);
            float latreal = Convert.ToSingle(lat * 180 / 2 ^ 25);
            
            int lon = Int32.Parse(string.Concat(this.rawList[Offset + 4], this.rawList[Offset + 5], this.rawList[Offset + 6], this.rawList[Offset + 7]), System.Globalization.NumberStyles.HexNumber);
            float lonreal = Convert.ToSingle(lon * 180 / 2 ^ 25);
            
            Offset += 8;

            return new Point().LatLong2XY(latreal, lonreal);
        }

        private Point decodePolarCoordinates()
        {
            List<Atom> atoms = new List<Atom>();
            int RHO = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1]), System.Globalization.NumberStyles.HexNumber);
            float RHOreal = Convert.ToSingle(RHO);

            int Theta = Int32.Parse(string.Concat(this.rawList[Offset + 2], this.rawList[Offset + 3]), System.Globalization.NumberStyles.HexNumber);
            float Thetareal = Convert.ToSingle(Theta * 360 / 2 ^ 16);

            Offset += 4;

            return new Point().Polar2XY(RHOreal, Thetareal);
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

        private List<Atom> decodeTrackStatus_CAT20()
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

        private List<Atom> decodeTrackStatus_CAT10()
        {
            List<Atom> atoms = new List<Atom>();
            var ls = new List<string> { "CNF", "TRE", "CST", "MAH", "TCC", "STH", "FX", "TOM", "DOU", "MRS", "FX", "GHO", "FX" };
            var ls0 = new List<string> { "Confirmed track", "Default", "Not extrapolation", "Predictable extrapolation due to sensor refresh period",
                "Default", "Tracking performed in 'Sensor Plane'", "Measured position", "End of Data Item"};
            var ls1 = new List<string> { "Track in initiation phase", "Last report for a track", "Predictable extrapolation in masked area", "Extrapolation due to unpredictable absence of detection",
                "Horizontal Manoueuvre", "Slant range correction and a suitable projection technique are used to track in a 2D.reference plane, tangential to the earth model at the Sensor Site co-ordinates.",
                "Smoothed position", "Extension into first extent"};

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
                                atoms.Add(new Atom(ls[cont1], 3, ls1[cont2 + 1]));
                            else
                                atoms.Add(new Atom(ls[cont1], 2, ls1[cont2]));
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
                    else if (i == 1)
                    {
                        int code = Convert.ToInt16(s.Substring(0,2));
                        switch (code)
                        {
                            case 0:
                                atoms.Add(new Atom("TOM", 0, "Unkown type of movement"));
                                break;
                            case 1:
                                atoms.Add(new Atom("TOM", 1, "Taking-off"));
                                break;
                            case 2:
                                atoms.Add(new Atom("TOM", 2, "Landing"));
                                break;
                            case 3:
                                atoms.Add(new Atom("TOM", 3, "Other types of movements"));
                                break;
                        }
                        code = Convert.ToInt16(s.Substring(2, 3));
                        switch (code)
                        {
                            case 0:
                                atoms.Add(new Atom("DOU", 0, "No doubt"));
                                break;
                            case 1:
                                atoms.Add(new Atom("DOU", 1, "Doubtfull correlation (undertemined reaseon)"));
                                break;
                            case 2:
                                atoms.Add(new Atom("DOU", 2, "Doubtfull correlation in clutter"));
                                break;
                            case 3:
                                atoms.Add(new Atom("DOU", 3, "Loss of accuracy"));
                                break;
                            case 4:
                                atoms.Add(new Atom("DOU", 4, "Loss of accuracy in clutter"));
                                break;
                            case 5:
                                atoms.Add(new Atom("DOU", 5, "Untable track"));
                                break;
                            case 6:
                                atoms.Add(new Atom("DOU", 6, "Previously coasted"));
                                break;
                        }
                        code = Convert.ToInt16(s.Substring(5, 2));
                        switch (code)
                        {
                            case 0:
                                atoms.Add(new Atom("MRS", 0, "Merge or split indiacation undetermined"));
                                break;
                            case 1:
                                atoms.Add(new Atom("MRS", 1, "Track merged by association to plot"));
                                break;
                            case 2:
                                atoms.Add(new Atom("MRS", 2, "Track merged by non-association to plot"));
                                break;
                            case 3:
                                atoms.Add(new Atom("MRS", 3, "Split track"));
                                break;
                        }
                    }
                    else if (i == 2)
                    {
                        if (s[0].Equals('0'))
                            atoms.Add(new Atom("GHO", 0, "Default"));
                        else
                            atoms.Add(new Atom("GHO", 0, "Ghost track"));
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
