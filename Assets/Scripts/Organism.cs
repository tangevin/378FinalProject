using UnityEngine;
using System.Collections;

public abstract class Organism : MonoBehaviour
{
    public HumidityTolerance humidityTol { get; private set; }
    public TemperatureTolerance tempTol { get; private set; }
    public Lifespan lifespan { get; private set; }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    protected void initialize(HumidityTolerance humidityTolerance, TemperatureTolerance tempTol, Lifespan lifespan) {
        this.humidityTol = humidityTolerance;
        this.tempTol = tempTol;
        this.lifespan = lifespan;
    }
}
