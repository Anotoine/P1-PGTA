using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        }

        private void decodeTRD()
        {

        }

        private void decodeTOD()
        {

        }

        private void decodeLatLong()
        {

        }
        private void decodeXY()
        {

        }
        private void decodeTrackNumber()
        {

        }
        private void decodeTrackStatus()
        {

        }
        private void decodeM3A()
        {

        }
        private void decodeTrackVel()
        {

        }
        private void decodeFL()
        {

        }
        private void decodeModeC()
        {

        }
        private void decodeICAOAddress()
        {

        }
        private void decodeCallSign()
        {

        }
        private void decodeMheight()
        {

        }
        private void decodeGheight()
        {

        }
        private void decodeCalAcc()
        {

        }
        private void decodeVehicleId()
        {

        }
        private void decodePPmes()
        {

        }
        private void decodePosAcu()
        {

        }
        private void decodeReceivers()
        {

        }
        private void decodeMSdata()
        {

        }
    }
}
