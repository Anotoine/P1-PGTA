using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

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
        public string Type { get; set; }
        public string RegID { get; set; }

        public ShowRow(Message m)
        {
            this.ImageUrl = "https://cdn.jetphotos.com/full/6/69088_1542915518.jpg";
            this.TOD = m.getTOD().ToString("HH:mm:ss.fff");
            this.ICAOAddress = m.getAddressICAO();
            this.Length = Convert.ToString(m.getLength());
            this.FSPEC = m.getFSPEC();
            this.Type = m.getType();
            this.SAC = Convert.ToString(m.getSAC());
            this.SIC = Convert.ToString(m.getSIC());
            this.Country = "NONE";
            this.CAT = m.getCAT();
            this.Callsign = m.getCallsign();
            //string matricula = "";
            //try
            //{
            //    if (m.getAddressICAO() != "" && m.getAddressICAO() != "NONE")
            //    {
            //        string URL = "https://junzis.com/adb/?q=" + m.getAddressICAO();
            //        HtmlWeb web = new HtmlWeb();
            //        var htmlDoc = web.Load(@URL);

            //        if (htmlDoc.DocumentNode.SelectNodes("//table/tr/td") != null)
            //            matricula = htmlDoc.DocumentNode.SelectNodes("//table/tr/td")[1].InnerHtml;
            //    }
            //}
            //catch (Exception e)
            //{
            //    this.RegID = e.ToString();
            //}
            //finally
            //{
            //    this.RegID = matricula;
            //}
            //this.RegID = matricula;

        }
    }
}
