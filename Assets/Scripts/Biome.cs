using UnityEngine;
using System.Collections;

public class Biome : Object
{
    public BiomeType biomeType { get; private set; }
    public AmountOfWater amountOfWater { get; private set; }
    public BaseTemperature baseTemperature { get; private set; }
    public Temperature temperature;
    public TemperatureVariation temperatureVariation { get; private set; }
    public BaseHumidity baseHumidity { get; private set; }
    public Humidity humidity;
    public HumidityVariation humidityVariation { get; private set; }
    public Color color;

    public Biome(BiomeType biomeType)
    {
        this.biomeType = biomeType;

        switch (biomeType)
        {
            case (BiomeType.DESERT):
                amountOfWater = AmountOfWater.NONE;
                baseTemperature = BaseTemperature.HIGH;
                temperature = Temperature.HIGH;
                temperatureVariation = TemperatureVariation.HIGH;
                baseHumidity = BaseHumidity.LOW;
                humidity = Humidity.LOW;
                humidityVariation = HumidityVariation.LOW;
                color = new Color(255, 255, 0);
                break;
            case (BiomeType.FOREST):
                amountOfWater = AmountOfWater.HIGH;
                baseTemperature = BaseTemperature.MEDIUM;
                temperature = Temperature.MEDIUM;
                temperatureVariation = TemperatureVariation.HIGH;
                baseHumidity = BaseHumidity.HIGH;
                humidity = Humidity.HIGH;
                humidityVariation = HumidityVariation.LOW;
                color = Color.green;
                break;
            case (BiomeType.MOUNTAIN):
                amountOfWater = AmountOfWater.NONE;
                baseTemperature = BaseTemperature.LOW;
                temperature = Temperature.LOW;
                temperatureVariation = TemperatureVariation.LOW;
                baseHumidity = BaseHumidity.LOW;
                humidity = Humidity.LOW;
                humidityVariation = HumidityVariation.LOW;
                color = Color.grey;
                break;
            case (BiomeType.OCEAN):
                amountOfWater = AmountOfWater.HIGH;
                baseTemperature = BaseTemperature.MEDIUM;
                temperature = Temperature.MEDIUM;
                temperatureVariation = TemperatureVariation.HIGH;
                baseHumidity = BaseHumidity.HIGH;
                humidity = Humidity.HIGH;
                humidityVariation = HumidityVariation.HIGH;
                color = Color.blue;
                break;
            case (BiomeType.PLAIN):
                amountOfWater = AmountOfWater.MEDIUM;
                baseTemperature = BaseTemperature.MEDIUM;
                temperature = Temperature.MEDIUM;
                temperatureVariation = TemperatureVariation.MEDIUM;
                baseHumidity = BaseHumidity.LOW;
                humidity = Humidity.LOW;
                humidityVariation = HumidityVariation.MEDIUM;
                color = Color.magenta;
                break;
            case (BiomeType.RIVER):
                amountOfWater = AmountOfWater.HIGH;
                baseTemperature = BaseTemperature.MEDIUM;
                temperature = Temperature.MEDIUM;
                temperatureVariation = TemperatureVariation.MEDIUM;
                baseHumidity = BaseHumidity.MEDIUM;
                humidity = Humidity.MEDIUM;
                humidityVariation = HumidityVariation.HIGH;
                color = Color.cyan;
                break;
        }
        color.a = 0.25f;
    }
}
