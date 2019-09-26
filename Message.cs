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
        private List<String> rawList;
        private int CAT;
        private int lengthMessage;
        private List<String> listFSPEC;

        //Constructors needed
        public Message(List<String> raw)
        {
            this.rawList = raw;
            this.CAT = Int32.Parse(this.rawList[0], System.Globalization.NumberStyles.HexNumber);
            this.lengthMessage = Int32.Parse(this.rawList[1], System.Globalization.NumberStyles.HexNumber) + Int32.Parse(this.rawList[2], System.Globalization.NumberStyles.HexNumber);
        }

        //GETs needed
        public int getCAT()
        {
            return CAT;
        }
        public int getLengthMessage()
        {
            return lengthMessage;
        }


        //SETs needed
        public void setRaw (List<String> raw)
        {
            this.rawList = raw;
        }
        public void setCAT (int CAT)
        {
            this.CAT = CAT;
        }
        public void setLengthMessage(int Length)
        {
            this.lengthMessage = Length;
        }
    }
}
