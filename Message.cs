using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P1_PGTA;

namespace P1_PGTA
{
    class Message
    {
        private int ID;
        private List<String> rawList;
        private int CAT;
        private int Length;
        private List<String> listFSPEC;

        //Constructors needed
        public Message(int ID, List<String> raw, int CAT, int Length)
        {
            this.ID = ID;
            this.rawList = raw;
            this.CAT = CAT;
            this.Length = Length;
            //this.CAT = Int32.Parse(this.rawList[0], System.Globalization.NumberStyles.HexNumber);
            //this.lengthMessage = Int32.Parse(this.rawList[1], System.Globalization.NumberStyles.HexNumber) + Int32.Parse(this.rawList[2], System.Globalization.NumberStyles.HexNumber);
        }

        //GETs needed
        public int getID()
        {
            return ID;
        }
        public int getCAT()
        {
            return CAT;
        }
        public int getLength()
        {
            return Length;
        }
        public List<String> getList()
        {
            return rawList;
        }

        //SETs needed
        public void setID(int ID)
        {
            this.ID = ID;
        }
        public void setRaw(List<String> raw)
        {
            this.rawList = raw;
        }
        public void setCAT(int CAT)
        {
            this.CAT = CAT;
        }
        public void setLengthMessage(int Length)
        {
            this.Length = Length;
        }
    }
}
