using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTERIX
{
    public class ShowRow
    {
        public string ImageUrl { get; set; }
        public string TOD { get; set; }
        public string ICAOAddress { get; set; }
        public string Length { get; set; }
        public string Country { get; set; }
        public string Callsign { get; set; }
        public string CAT { get; set; }
        public string SAC { get; set; }
        public string SIC { get; set; }
        public string FSPEC { get; set; }
        public int FSPECount { get; set; }
        public string Type { get; set; }
        public string TrackN { get; set; }
        public string RegID { get; set; }

        public ShowRow(Message m)
        {
            this.TOD = m.getTOD().ToString("HH:mm:ss.fff");
            this.ICAOAddress = m.getAddressICAO();
            this.Length = Convert.ToString(m.getLength());
            this.FSPEC = m.getFSPEC();
            this.FSPECount = m.getFSPECount();
            this.Type = m.getType();
            if (m.getTrackN() != -1)
                this.TrackN = Convert.ToString(m.getTrackN());
            this.SAC = Convert.ToString(m.getSAC());
            this.SIC = Convert.ToString(m.getSIC());
            this.CAT = m.getCAT();
            this.Callsign = m.getCallsign();
        }

        public void AddDBData(List<AircraftDB> listPlanes)
        {
            if (listPlanes != null)
            {
                bool exit = false;
                int i = 0;
                if (this.ICAOAddress != "NONE")
                {
                    while (!(exit || i >= listPlanes.Count))
                    {
                        if (string.Compare(this.ICAOAddress, listPlanes[i].ICAOAddress) == 0)
                        {
                            this.RegID = listPlanes[i].RegID.ToUpper();
                            this.Country = listPlanes[i].Country.ToUpper();
                            this.Type = "Aircraft";
                            this.ImageUrl = listPlanes[i].ImageUrl;
                            exit = true;
                        }
                    i++;
                    }
                }
            }
        }
    }
}
