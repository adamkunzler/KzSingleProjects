using Kz.Engine.DataStructures;
using Kz.Engine.Trigonometry;
using Newtonsoft.Json;
using System.Numerics;
using System.Text;

namespace Kz.IdlePlanetMiner
{
    public static class PlanetManager
    {
        /// <summary>
        /// represents the data in the planet_data.json file
        /// </summary>
        private class PlanetDto
        {
            public string Planet { get; set; }
            public string UnlockPrice { get; set; }
            public string Group { get; set; }
            public string Resources { get; set; }
            public string Yield { get; set; }
        }

        /// <summary>
        /// Load and return all the planets
        /// </summary>        
        public static List<Planet> GetAllPlanets()
        {
            var planets = LoadPlanetsData();
            InitPlanets(planets);
            return planets;
        }

        /// <summary>
        /// Takes the base planet information from LoadPlanetsData() and adds additional information
        /// via a call to planet.Init()
        /// </summary>        
        private static void InitPlanets(List<Planet> planets)
        {
            var random = new Random(117);
            var theta = TrigUtil.DegreesToRadians(45.0f);
            var magnitude = 400.0f;

            uint planetId = 1;
            foreach (var planet in planets)
            {
                var max = 90.0f;
                var min = max / 2.0f;
                var randTheta = (float)random.NextDouble() * TrigUtil.DegreesToRadians(max) - TrigUtil.DegreesToRadians(min);
                var randMagnitude = (float)random.NextDouble() * 20.0f - 10.0f;

                theta += TrigUtil.DegreesToRadians(67.5f) + randTheta;
                magnitude += 50.0f + randMagnitude;

                var polar = new Vector2f(magnitude, theta);
                var coord = polar.ToCartesian();

                planet.Init(planetId++, coord, (float)random.Next(-360, 360));
            }
        }

        /// <summary>
        /// Load planet data from planet_data.json
        /// </summary>
        /// <returns></returns>
        private static List<Planet> LoadPlanetsData()
        {
            var json = File.ReadAllText("Resources/planet_data.json");

            var rawPlanets = JsonConvert.DeserializeObject<List<PlanetDto>>(json);
            if (rawPlanets == null) return [];

            var planets = rawPlanets.Select(x => new Planet
            {
                Name = x.Planet,
                UnlockPrice = ParseUnlockPrice(x.UnlockPrice),
                Group = uint.Parse(x.Group),
                Resources = ProcessPlanetResources(x.Resources, x.Yield),
            }).ToList();

            return planets;
        }

        /// <summary>
        /// Creates a list of PlanetResource from comma delimited inputs
        /// </summary>        
        private static List<PlanetResource> ProcessPlanetResources(string resources, string yield)
        {
            var planetResources = new List<PlanetResource>();

            var eachResource = resources.Split(',');
            var eachYield = yield.Split(',');

            for (var i = 0; i < eachResource.Length; i++)
            {
                planetResources.Add(new PlanetResource(eachResource[i], uint.Parse(eachYield[i])));
            }

            return planetResources;
        }

        private static BigInteger ParseUnlockPrice(string unlockPrice)
        {
            var digits = new List<char> { 'K', 'M', 'B', 'T', 'q', 'Q', 's', 'S', 'O' };

            var lastDigit = unlockPrice[unlockPrice.Length - 1];
            var value = lastDigit == '0' 
                ? float.Parse(unlockPrice) 
                : float.Parse(unlockPrice.Substring(0, unlockPrice.Length - 1));

            // remove decimals since BigInteger.Parse can't handle them
            // e.g. 1.5k => 1.5000 => 1500
            var numDigits = 0;
            var intValue = 0;
            if (unlockPrice.Contains('.'))
            {                
                numDigits = unlockPrice.Split('.')[1][..^1].Length;

                if (numDigits == 1) intValue = (int)(value * 10);
                else if (numDigits == 2) intValue = (int)(value * 100);
                else if (numDigits == 3) intValue = (int)(value * 1000);
            }
            else
            {
                intValue = (int)value;
            }

            var sb = new StringBuilder();
            sb.Append(intValue);
            if (lastDigit != '0')
            { 
                var numZeroes = digits.IndexOf(lastDigit) * 3 + 3 - numDigits;
                var zeroes = string.Join("", Enumerable.Repeat("0", numZeroes));
                sb.Append(zeroes);
            }

            var strValue = sb.ToString();
            //Console.WriteLine($"{unlockPrice} => {strValue}");
                       
            var bi = BigInteger.Parse(strValue);            
            return bi;
        }
    }
}