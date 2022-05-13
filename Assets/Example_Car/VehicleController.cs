using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example_Car
{
    public class VehicleController : MonoBehaviour
    {
        [SerializeField] Engine _engine;
        public Engine engine => _engine;

        public Transmission transmission { private set; get; } = new Transmission();

        public Input input { private set; get; } = new Input();

        [SerializeField] Sound _sound;
        public Sound sound => _sound;

        public DamageHandler damage { private set; get; } = new DamageHandler();

        [SerializeField] float _rpm;
        [SerializeField] float _rpmPercent;
        [SerializeField] float _torque;
        [SerializeField] float _power;

        void Start()
        {
            _engine.Initialize(this);
            _sound.Initialize(this);

            _sound.Enable();
        }

        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                _engine.Toggle();
            }

            _engine.Update();

            _sound.Update();

            _rpm = _engine.RPM;
            _rpmPercent = _engine.RPMPercent;
            _torque = _engine.Torque;
            _power = _engine.Power;
        }
    }

    public sealed class Transmission
    {
        public enum TransmissionType { Manual, Automatic, AutomaticSequential }
        public TransmissionType transmissionType => TransmissionType.Manual;

        public int Gear => 0;

        public bool Shifting => false;

        public void UpdateClutch()
        {

        }
    }

    public sealed class Input
    {
        public float Vertical => UnityEngine.Input.GetAxis("Vertical");
    }

    public sealed class DamageHandler
    {
        public bool enabled => false;
        public bool performanceDegradation => false;
        public float DamagePercent => 0f;
    }

}