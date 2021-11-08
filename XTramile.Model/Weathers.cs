namespace XTramile.Model
{
    public class Weathers
    {
        public int? Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string IconUrl  { 
            get { 
                if(string.IsNullOrEmpty(Icon)) return $"https://openweathermap.org/img/wn/50d@2x.png"; 
                else return $"https://openweathermap.org/img/wn/{Icon}@2x.png"; 
            } 
        }
    }

}
