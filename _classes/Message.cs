using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public Message(List<string> raw, int CAT, int Length)
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
                    CAT10 = new CAT10(rawList, listFSPEC, Offset).Decode();
                    break;
                case 19:
                    CAT19 = new CAT19(rawList, listFSPEC, Offset).decode();
                    break;
                case 20:
                    CAT20 = new CAT20(rawList, listFSPEC, Offset).Decode();
                    break;
                case 21:
                    try
                    {
                        CAT21v023 = new CAT21v023(rawList, listFSPEC, Offset).decode();
                    }
                    catch
                    {
                        CAT21v24 = new CAT21v24(rawList, listFSPEC, Offset).decode();
                    }
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
                    if (CAT10.DI040 != null)
                        return CAT10.DI040;
                    else if (CAT10.DI041 != null)
                        return CAT10.DI041;
                    else
                        return CAT10.DI042;
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
    }
}
