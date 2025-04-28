namespace CellTower
{
    public class CellTower
    {
        private string id { get; }
        private int easting { get; }
        private int northing { get; }
        private double longitude { get; }
        private double latitude { get; }
        private int frequency { get; set; }
        private bool cannotAssignFrequency { get; set; }
        private List<CellTower> closeTowers { get; set; }
        private List<CellTower> farTowers { get; set; }
        private CellTower? furthestTower { get; set; }

        public CellTower(string id, int easting, int northing, double longitude, double latitude)
        {
            this.id = id;
            this.easting = easting;
            this.northing = northing;
            this.longitude = longitude;  // negative values refer to the west, positive values refer to the east
            this.latitude = latitude;    // negative values refer to the south, positive values refer to the north
            this.frequency = -1;
            this.cannotAssignFrequency = false;
            this.closeTowers = new List<CellTower>();
            this.farTowers = new List<CellTower>();
            this.furthestTower = null;
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

        public int getFrequency()
        {
            return this.frequency;
        }

        public bool getCannotAssignFrequency()
        {
            return this.cannotAssignFrequency;
        }

        public List<CellTower> getCloseTowers()
        {
            return this.closeTowers;
        }

        public List<CellTower> getFarTowers()
        {
            return this.farTowers;
        }

        public CellTower? getFurthestTower()
        {
            return this.furthestTower;
        }

        // SETTERS
        public void setFrequency(int frequency)
        {
            this.frequency = frequency;
        }
        public void setCannotAssignFrequency(bool cannotAssignFrequency)
        {
            this.cannotAssignFrequency = cannotAssignFrequency;
        }
        public void setCloseTowers(List<CellTower> closeTowers)
        {
            this.closeTowers = closeTowers;
        }

        public void setFarTowers(List<CellTower> farTowers)
        {
            this.farTowers = farTowers;
        }

        public void setFurthestTower(CellTower furthestTower)
        {
            this.furthestTower = furthestTower;
        }
    }
}