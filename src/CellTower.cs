namespace CellTower
{
    public class CellTower
    {
        private string id { get; }
        private int easting { get; }
        private int northing { get; }
        private double longitude { get; }
        private double latitude { get; }

        public CellTower(string id, int easting, int northing, double longitude, double latitude)
        {
            this.id = id;
            this.easting = easting;
            this.northing = northing;
            this.longitude = longitude;  // negative values refer to the west, positive values refer to the east
            this.latitude = latitude;    // negative values refer to the south, positive values refer to the north
        }

        // GETTERS
        public string getID()
        {
            return this.id;
        }

        public int getEasting()
        {
            return this.easting;
        }

        public int getNorthing()
        {
            return this.northing;
        }

        public double getLongitude()
        {
            return this.longitude;
        }

        public double getLatitude()
        {
            return this.latitude;
        }
    }
}