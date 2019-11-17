namespace Ideafix
{
    public class AircraftDB
    {
        public string ICAOAddress { get; set; }
        public string RegID { get; set; }
        public string Country { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string Airline { get; set; }
        public string ImageUrl { get; set; }

        public AircraftDB(string[] vs)
        {
            this.ICAOAddress = vs[0].ToUpper();
            this.RegID = vs[1].ToUpper();
            this.Country = vs[2];
            this.Type = vs[3];
            this.Model = vs[4];
            this.Airline = vs[5];
            this.ImageUrl = vs[6];
        }
    }
}
