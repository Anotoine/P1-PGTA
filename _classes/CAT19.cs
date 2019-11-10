using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTERIX
{
    class CAT19
    {
        //Message stuff
        private readonly List<bool> listFSPEC;
        private readonly List<string> rawList;
        private int Offset;

        //DataItem
        internal string DI000 { get; set; }
        internal List<Atom> DI010 { get; set; }
        internal DateTime DI140 { get; set; }
        internal List<Atom> DI550 { get; set; }
        internal List<Atom> DI551 { get; set; }
        internal List<Atom> DI552 { get; set; }
        internal List<Atom> DI553 { get; set; }
        internal List<Atom> DI600 { get; set; }
        internal List<Atom> DI610 { get; set; }
        internal float DI620 { get; set; }

        public CAT19(List<string> rawList, List<bool> listFSPEC, int Offset)
        {
            this.listFSPEC = listFSPEC;
            this.rawList = rawList;
            this.Offset = Offset;
        }

        public CAT19 decode()
        {
            if (this.listFSPEC[1]) //I019/010
                decodeSACSIC();
            if (this.listFSPEC[2]) //I019/000
                decodeMessageType();
            if (this.listFSPEC[3]) //I019/140
                decodeTOD();
            if (this.listFSPEC[4]) //I019/550
                decodeSysStats();
            if (this.listFSPEC[5]) //I019/551
                decodeTrackProcStats();
            if (this.listFSPEC[6]) //I019/552
                decodeRemSensorStats();
            if (this.listFSPEC[7]) //I019/553
                decodeRefTransStats();

            if (this.listFSPEC[8])
            {
                if (this.listFSPEC[9]) //I019/600
                    decodeMLTRefPoint();
                if (this.listFSPEC[10]) //I019/610
                    decodeMLTheightPoint();
                if (this.listFSPEC[11]) //I019/620
                    decodeUndulation();
            }
            return this;
        }

        private void decodeSACSIC()
        {
            this.DI010 = new List<Atom>();
            List<string> ls = new List<string>() { "SAC", "SIC" };
            for (int i = 0; i < 2; i++)
            {
                this.DI010.Add(new Atom(ls[i], Convert.ToInt32(this.rawList[Offset], 16), Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 10).PadLeft(3, '0')));
                Offset++;
            }
        }

        private void decodeMessageType()
        {
            int code = Convert.ToInt32(this.rawList[Offset], 16);
            switch (code)
            {
                case 0:
                    this.DI000 = "";
                    break;
                case 1:
                    this.DI000 = "Start of Update Cycle";
                    break;
                case 2:
                    this.DI000 = "Periodic Status Message";
                    break;
                case 3:
                    this.DI000 = "Event-triggered Status Message";
                    break;
            }
            ++Offset;
        }

        private void decodeTOD()
        {
            int LSB = Int32.Parse(string.Concat(this.rawList[Offset], this.rawList[Offset + 1], this.rawList[Offset + 2]), System.Globalization.NumberStyles.HexNumber);
            Offset += 3;

            this.DI140 = new DateTime().AddSeconds((float)LSB / 128);
        }

        private void decodeSysStats()
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

        private void decodeTrackProcStats()
        {
            string s = Convert.ToString(this.rawList[Offset]);

            for (int i = 1; i < 5; i++)
            {
                int bb = Convert.ToInt32(s.Substring((i - 1) * 2, 2));
                switch (bb)
                {
                    case 0:
                        this.DI551.Add(new Atom("TP" + Convert.ToString(i), 00, "Standby - Faulted"));
                        break;
                    case 1:
                        this.DI551.Add(new Atom("TP" + Convert.ToString(i), 01, "Standby - Good"));
                        break;
                    case 2:
                        this.DI551.Add(new Atom("TP" + Convert.ToString(i), 10, "Exec - Faulted"));
                        break;
                    case 3:
                        this.DI551.Add(new Atom("TP" + Convert.ToString(i), 11, "Exec - Good"));
                        break;
                }
            }
            Offset++;
        }

        private void decodeRemSensorStats()
        {
            this.DI552 = new List<Atom>();
            //Amount of Octets that will extent this camp (REP)
            int REP = Convert.ToInt32(this.rawList[Offset], 16);
            Offset++;

            for (int i = 0; i < (REP - 1); i++)
            {
                string RSid = Convert.ToString(Convert.ToInt32(this.rawList[Offset], 16), 2).PadLeft(8, '0');
                Offset++;
                int AA = Convert.ToInt32(this.rawList[Offset].PadLeft(8, '0').Substring(4, 2));
                switch (AA)
                {
                    case 0:
                        this.DI552.Add(new Atom("RS Status", 0, "Faulted"));
                        this.DI552.Add(new Atom("RS Operational", 0, "Offline"));
                        break;
                    case 1:
                        this.DI552.Add(new Atom("RS Status", 0, "Faulted"));
                        this.DI552.Add(new Atom("RS Operational", 1, "Online"));
                        break;
                    case 2:
                        this.DI552.Add(new Atom("RS Status", 1, "Good"));
                        this.DI552.Add(new Atom("RS Operational", 0, "Offline"));
                        break;
                    case 3:
                        this.DI552.Add(new Atom("RS Status", 1, "Good"));
                        this.DI552.Add(new Atom("RS Operational", 1, "Online"));
                        break;
                }
                AA = Convert.ToInt32(this.rawList[Offset].PadLeft(8, '0').Substring(1, 3));
                switch (AA)
                {
                    case 0:
                        ;
                        break;
                    case 1:
                        this.DI552.Add(new Atom("Transmitter 1090MHz", 1, "Present"));
                        break;
                    case 2:
                        this.DI552.Add(new Atom("Transmitter 1030MHz", 1, "Present"));
                        break;
                    case 3:
                        this.DI552.Add(new Atom("Transmitter 1090MHz", 1, "Present"));
                        this.DI552.Add(new Atom("Transmitter 1030MHz", 1, "Present"));
                        break;
                    case 4:
                        this.DI552.Add(new Atom("Receiver 1090MHz", 1, "Present"));
                        break;
                    case 5:
                        this.DI552.Add(new Atom("Transmitter 1090MHz", 1, "Present"));
                        this.DI552.Add(new Atom("Receiver 1090MHz", 1, "Present"));
                        break;
                    case 6:
                        this.DI552.Add(new Atom("Transmitter 1090MHz", 1, "Present"));
                        this.DI552.Add(new Atom("Receiver 1090MHz", 1, "Present"));
                        break;
                    case 7:
                        this.DI552.Add(new Atom("Transmitter 1090MHz", 1, "Present"));
                        this.DI552.Add(new Atom("Transmitter 1030MHz", 1, "Present"));
                        this.DI552.Add(new Atom("Receiver 1090MHz", 1, "Present"));
                        break;
                }
                Offset++;
            }
        }

        private void decodeRefTransStats()
        {

        }

        private void decodeMLTRefPoint()
        {

        }

        private void decodeMLTheightPoint()
        {

        }

        private void decodeUndulation()
        {

        }
    }
}
