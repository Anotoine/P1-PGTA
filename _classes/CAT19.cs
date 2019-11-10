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
        private List<bool> listFSPEC;
        private List<string> rawList;
        private int Offset;

        //DataItem
        internal int DI000 { get; set; }
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
               decodeMessType();
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

        }

        private void decodeMessType()
        {

        }

        private void decodeTOD()
        {

        }

        private void decodeSysStats()
        {

        }

        private void decodeTrackProcStats()
        {

        }

        private void decodeRemSensorStats()
        {

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
