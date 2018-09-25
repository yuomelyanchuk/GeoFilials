namespace GeoFilials.Scripts
{
    public struct NewCityToDB
    {
        public string Region { get; set; }
        public string City { get; set; }

        public NewCityToDB(string Region, string City)
        {
            this.Region = Region;
            this.City = City;
        }

        public override string ToString()
        {
            return string.Format("Область - {0} \t| Город - {1}", this.Region, this.City);
        }
    }
}
