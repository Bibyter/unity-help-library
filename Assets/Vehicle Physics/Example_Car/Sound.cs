using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Example_Car
{
    /// <summary>
    /// Main class that manages all the sound aspects of the vehicle.
    /// </summary>
    [System.Serializable]
    public class Sound
    {
        /// <summary>
        /// Spatial blend of all audio sources. Can not be changed at runtime.
        /// </summary>
        [Tooltip("Spatial blend of all audio sources. Can not be changed at runtime.")]
        [Range(0, 1)]
        public float spatialBlend = 1f;

        /// <summary>
        /// Master volume of a vehicle. To adjust volume of all vehicles or their components check audio mixer.
        /// </summary>
        [Tooltip("Master volume of a vehicle. To adjust volume of all vehicles or their components check audio mixer.")]
        [Range(0, 2)]
        public float masterVolume = 1f;

        [Header("Engine")]

        /// <summary>
        /// Sound of engine idling.
        /// </summary>
        [Tooltip("Sound of engine idling.")]
        public EngineIdleSoundComponent engineIdleComponent = new EngineIdleSoundComponent();

        /// <summary>
        /// Engine start / stop component. First clip is for starting and second one is for stopping.
        /// </summary>
        [Tooltip("Engine start / stop component. First clip is for starting and second one is for stopping.")]
        public EngineStartStopComponent engineStartStopComponent = new EngineStartStopComponent();

        /// <summary>
        /// Forced induction whistle component. Can be used for air intake noise or supercharger if spool up time is set to 0 under engine settings.
        /// </summary>
        //[Tooltip("Forced induction whistle component. Can be used for air intake noise or supercharger if spool up time is set to 0 under engine settings.")]
        //public TurboWhistleComponent turboWhistleComponent = new TurboWhistleComponent();

        /// <summary>
        /// Sound of turbo's wastegate. Supports multiple clips.
        /// </summary>
        //[Tooltip("Sound of turbo's wastegate. Supports multiple clips.")]
        //public TurboFlutterComponent turboFlutterComponent = new TurboFlutterComponent();

        /// <summary>
        /// Exhaust popping sound on deceleration / rev limiter.
        /// </summary>
        //[Tooltip("Sound of turbo's wastegate. Supports multiple clips.")]
        //public BackfireComponent exhaustPopComponent = new BackfireComponent();

        //[Header("Transmission")]

        /// <summary>
        /// Transmission whine from straight cut gears or just a noisy gearbox.
        /// </summary>
        //[Tooltip("Transmission whine from straight cut gears or just a noisy gearbox.")]
        //public TransmissionWhineComponent transmissionWhineComponent = new TransmissionWhineComponent();

        /// <summary>
        /// Sound from changing gears. Supports multiple clips.
        /// </summary>
        //[Tooltip("Sound from changing gears. Supports multiple clips.")]
        //public GearChangeComponent gearChangeComponent = new GearChangeComponent();

        //[Header("Suspension")]

        /// <summary>
        /// Sound from wheels hitting ground and/or obstracles. Supports multiple clips.
        /// </summary>
        //[Tooltip("Sound from wheels hitting ground and/or obstracles. Supports multiple clips.")]
        //public SuspensionComponent suspensionComponent = new SuspensionComponent();

        //[Header("Surface Noise")]

        /// <summary>
        /// Sound produced by wheel rolling over a surface. Tire hum.
        /// </summary>
        //[Tooltip("Sound produced by wheel rolling over a surface. Tire hum.")]
        //public SurfaceComponent surfaceComponent = new SurfaceComponent();

        /// <summary>
        /// Sound produced by wheel skidding over a surface. Tire squeal.
        /// </summary>
        //[Tooltip("Sound produced by wheel skidding over a surface. Tire squeal.")]
        //public SkidComponent skidComponent = new SkidComponent();

        //[Header("Crash")]

        /// <summary>
        /// Sound of vehicle hitting other objects. Supports multiple clips.
        /// </summary>
        //[Tooltip("Sound of vehicle hitting other objects. Supports multiple clips.")]
        //public CrashComponent crashComponent = new CrashComponent();

        //[Header("Other")]

        /// <summary>
        /// Sound of air brakes releasing air. Supports multiple clips.
        /// </summary>
        //[Tooltip("Sound of air brakes releasing air. Supports multiple clips.")]
        //public AirBrakeComponent airBrakeComponent = new AirBrakeComponent();

        /// <summary>
        /// Tick-tock sound of a working blinker. First clip is played when blinker is turning on and second clip is played when blinker is turning off.
        /// </summary>
        //[Tooltip("Tick-tock sound of a working blinker. First clip is played when blinker is turning on and second clip is played when blinker is turning off.")]
        //public BlinkerComponent blinkerComponent = new BlinkerComponent();

        //[Tooltip("Horn sound.")]
        //public HornComponent hornComponent = new HornComponent();

        [Header("Interior Parameters")]

        /// <summary>
        /// Set to true if listener inside vehicle. Mixer must be set up.
        /// </summary>
        [Tooltip("Set to true if listener is inside the vehicle. Mixer must be set up.")]
        public bool insideVehicle = false;
        private bool wasInsideVehickle = false;

        /// <summary>
        /// Sound attenuation inside vehicle.
        /// </summary>
        [Tooltip("Sound attenuation inside vehicle.")]
        public float interiorAttenuation = -7f;

        public float lowPassFrequency = 6000f;

        [Range(1f, 10f)]
        public float lowPassQ = 1f;


        [HideInInspector]
        public AudioMixerGroup masterGroup;

        [HideInInspector]
        public AudioMixerGroup engineMixerGroup;

        [HideInInspector]
        public AudioMixerGroup transmissionMixerGroup;

        [HideInInspector]
        public AudioMixerGroup surfaceNoiseMixerGroup;

        [HideInInspector]
        public AudioMixerGroup turboMixerGroup;

        [HideInInspector]
        public AudioMixerGroup suspensionMixerGroup;

        [HideInInspector]
        public AudioMixerGroup crashMixerGroup;

        [HideInInspector]
        public AudioMixerGroup otherMixerGroup;

        private float originalAttenuation;

        public List<SoundComponent> components = new List<SoundComponent>();
        private AudioMixer audioMixer;

        private VehicleController vc;

        public void Initialize(VehicleController vc)
        {
            this.vc = vc;

            audioMixer = Resources.Load("VehicleAudioMixer") as AudioMixer;
            masterGroup = audioMixer.FindMatchingGroups("Master")[0];
            engineMixerGroup = audioMixer.FindMatchingGroups("Engine")[0];
            transmissionMixerGroup = audioMixer.FindMatchingGroups("Transmission")[0];
            surfaceNoiseMixerGroup = audioMixer.FindMatchingGroups("SurfaceNoise")[0];
            turboMixerGroup = audioMixer.FindMatchingGroups("Turbo")[0];
            suspensionMixerGroup = audioMixer.FindMatchingGroups("Suspension")[0];
            crashMixerGroup = audioMixer.FindMatchingGroups("Crash")[0];
            otherMixerGroup = audioMixer.FindMatchingGroups("Other")[0];

            // Remember initial states
            audioMixer.GetFloat("attenuation", out originalAttenuation);

            /*
            * IMPORTANT
            * When adding a new sound component also add it to the list below so it can be enabled / disabled when vehicle is activated or suspended.
            */
            components = new List<SoundComponent>
            {
                engineIdleComponent,
                engineStartStopComponent,
                //skidComponent,
                //surfaceComponent,
                //turboFlutterComponent,
                //turboWhistleComponent,
                //transmissionWhineComponent,
                //gearChangeComponent,
                //airBrakeComponent,
                //blinkerComponent,
                //hornComponent,
                //exhaustPopComponent
            };

            // Do not use following components if vehicle is trailer.
            if (/*!vc.trailer.isTrailer*/true)
            {
                engineStartStopComponent.Initialize(vc, engineMixerGroup);
                engineIdleComponent.Initialize(vc, engineMixerGroup);
                //exhaustPopComponent.Initialize(vc, engineMixerGroup);
                //turboWhistleComponent.Initialize(vc, turboMixerGroup);
                //turboFlutterComponent.Initialize(vc, turboMixerGroup);
                //transmissionWhineComponent.Initialize(vc, transmissionMixerGroup);
                //gearChangeComponent.Initialize(vc, transmissionMixerGroup);
                //airBrakeComponent.Initialize(vc, otherMixerGroup);
                //blinkerComponent.Initialize(vc, otherMixerGroup);
                //hornComponent.Initialize(vc, otherMixerGroup);
            }

            //skidComponent.Initialize(vc, surfaceNoiseMixerGroup);
            //surfaceComponent.Initialize(vc, surfaceNoiseMixerGroup);
            //crashComponent.Initialize(vc, crashMixerGroup);
            //suspensionComponent.Initialize(vc, suspensionMixerGroup);
        }


        public void Update()
        {
            // Adjust sound if inside vehicle.
            if (!wasInsideVehickle && insideVehicle)
            {
                audioMixer.SetFloat("attenuation", interiorAttenuation);
                audioMixer.SetFloat("lowPassFrequency", lowPassFrequency);
                audioMixer.SetFloat("lowPassQ", lowPassQ);
            }
            else if (wasInsideVehickle && !insideVehicle)
            {
                audioMixer.SetFloat("attenuation", originalAttenuation);
                audioMixer.SetFloat("lowPassFrequency", 22000f);
                audioMixer.SetFloat("lowPassQ", 1f);
            }
            wasInsideVehickle = insideVehicle;

            // Update individual components.
            if (/*!vc.trailer.isTrailer*/true)
            {
                engineIdleComponent.Update();
                engineStartStopComponent.Update();
                //turboWhistleComponent.Update();
                //turboFlutterComponent.Update();
                //gearChangeComponent.Update();
                //transmissionWhineComponent.Update();
                //airBrakeComponent.Update();
                //blinkerComponent.Update();
                //hornComponent.Update();
                //exhaustPopComponent.Update();
            }

            //skidComponent.Update();
            //surfaceComponent.Update();
            //suspensionComponent.Update();
        }

        /// <summary>
        /// Initializes audio source to it's starting values.
        /// </summary>
        /// <param name="audioSource">AudioSource in question.</param>
        /// <param name="play">Play on awake?</param>
        /// <param name="loop">Should clip be looped?</param>
        /// <param name="volume">Volume of the audio source.</param>
        /// <param name="clip">Clip that will be set at the start.</param>
        public void SetAudioSourceDefaults(AudioSource audioSource, bool play = false, bool loop = false, float volume = 0f, AudioClip clip = null)
        {
            if (audioSource != null)
            {
                audioSource.spatialBlend = spatialBlend;
                audioSource.playOnAwake = play;
                audioSource.loop = loop;
                audioSource.volume = volume * vc.sound.masterVolume;
                audioSource.clip = clip;
                audioSource.priority = 200;

                if (play)
                    audioSource.Play();
                else
                    audioSource.Stop();
            }
            else
            {
                Debug.LogWarning("AudioSource is null. Defaults cannot be set.");
            }
        }

        /// <summary>
        /// Enable sound.
        /// </summary>
        public void Enable()
        {
            foreach (SoundComponent sc in components)
            {
                if (sc != null) sc.Enable();
            }
        }

        /// <summary>
        /// Disable all sound components.
        /// </summary>
        public void Disable()
        {
            foreach (SoundComponent sc in components)
            {
                if (sc != null && sc != engineStartStopComponent)
                {
                    sc.Disable();
                }
            }
        }

        /// <summary>
        /// Sets defaults to all the basic sound components when script is first added or reset is called.
        /// </summary>
        public void SetDefaults()
        {
            // Set defaults to each sound component as they all inherit from the same base class and are set to same default values by default.

            engineIdleComponent.volume = 0.3f;
            engineIdleComponent.pitch = 0.9f;

            engineStartStopComponent.volume = 0.4f;

            //transmissionWhineComponent.volume = 0.05f;
            //transmissionWhineComponent.pitch = 0.1f;

            //gearChangeComponent.volume = 0.1f;

            //suspensionComponent.volume = 0.1f;

            //surfaceComponent.volume = 0.25f;

            //skidComponent.volume = 0.45f;

            //crashComponent.volume = 0.32f;

            //turboWhistleComponent.volume = 0.04f;
            //turboWhistleComponent.pitch = 0.8f;

            //turboFlutterComponent.volume = 0.04f;

            try
            {
                if (this != null)
                {
                    if (engineStartStopComponent.Clips.Count < 2)
                    {
                        engineStartStopComponent.Clips.Add(Resources.Load("Defaults/EngineStart") as AudioClip);
                        engineStartStopComponent.Clips.Add(Resources.Load("Defaults/EngineStop") as AudioClip);
                    }

                    if (engineIdleComponent.Clip == null)
                    {
                        engineIdleComponent.Clip = Resources.Load("Defaults/EngineIdle") as AudioClip;
                    }

                    //if (gearChangeComponent.Clips.Count == 0)
                    //    gearChangeComponent.Clips.Add(Resources.Load("Defaults/GearShift") as AudioClip);

                    //if (turboWhistleComponent.Clip == null)
                    //    turboWhistleComponent.Clip = Resources.Load("Defaults/TurboWhistle") as AudioClip;

                    //if (turboFlutterComponent.Clip == null)
                    //    turboFlutterComponent.Clip = Resources.Load("Defaults/TurboFlutter") as AudioClip;

                    //if (suspensionComponent.Clip == null)
                    //    suspensionComponent.Clip = Resources.Load("Defaults/SuspensionThump") as AudioClip;

                    //if (crashComponent.Clips.Count == 0)
                    //    crashComponent.Clips.Add(Resources.Load("Defaults/Crash") as AudioClip);

                    //if (blinkerComponent.Clips.Count == 0)
                    //{
                    //    blinkerComponent.clips.Add(Resources.Load("Defaults/BlinkerOn") as AudioClip);
                    //    blinkerComponent.clips.Add(Resources.Load("Defaults/BlinkerOff") as AudioClip);
                    //}

                    //if (hornComponent.Clip == null)
                    //    hornComponent.Clip = Resources.Load("Defaults/Horn") as AudioClip;
                }
            }
            catch
            {
                Debug.LogWarning("One or more of the default sound resources could not be found. Default sounds will not be assigned.");
            }
        }
    }

    #region
    /// <summary>
    /// Base abstract class from which all vehicle sound components inherit.
    /// </summary>
    [System.Serializable]
    public abstract class SoundComponent
    {
        /// <summary>
        /// Base volume of the sound component.
        /// </summary>
        [Tooltip("Base volume of the sound component.")]
        [Range(0f, 1f)]
        public float volume = 0.1f;

        /// <summary>
        /// Base pitch of the sound component.
        /// </summary>
        [Tooltip("Base pitch of the sound component.")]
        [Range(0f, 2f)]
        public float pitch = 1f;

        /// <summary>
        /// List of audio clips this component can use. Some components can use multiple clips in which case they will be chosen at random, and some components can use only one 
        /// in which case only the first clip will be selected. Check manual for more details.
        /// </summary>
        [Tooltip("List of audio clips this component can use. Some components can use multiple clips in which case they will be chosen at random, and some components can use only one " +
            "in which case only the first clip will be selected. Check manual for more details.")]
        public List<AudioClip> clips = new List<AudioClip>();

        protected List<AudioSource> sources = new List<AudioSource>();
        protected VehicleController vc;
        protected AudioMixerGroup audioMixerGroup;

        /// <summary>
        /// Adds outputs of sources to the mixer.
        /// </summary>
        public void RegisterSources()
        {
            foreach (AudioSource source in sources)
            {
                source.outputAudioMixerGroup = audioMixerGroup;
            }
        }

        /// <summary>
        /// Gets or sets the first clip in the clips list.
        /// </summary>
        public AudioClip Clip
        {
            get
            {
                if (clips.Count > 0)
                {
                    return clips[0];
                }
                return null;
            }
            set
            {
                if (clips.Count > 0)
                {
                    clips[0] = value;
                }
                clips.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the whole clip list.
        /// </summary>
        public List<AudioClip> Clips
        {
            get
            {
                return clips;
            }
            set
            {
                clips = value;
            }
        }

        /// <summary>
        /// Gets or sets the first audio source in the sources list.
        /// </summary>
        public AudioSource Source
        {
            get
            {
                if (sources.Count > 0)
                {
                    return sources[0];
                }
                return null;
            }
            set
            {
                if (sources.Count > 0)
                {
                    sources[0] = value;
                }
                sources.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the whole sources list.
        /// </summary>
        public List<AudioSource> Sources
        {
            get
            {
                return sources;
            }
            set
            {
                sources = value;
            }
        }


        /// <summary>
        /// Gets a random clip from clips list.
        /// </summary>
        public AudioClip RandomClip
        {
            get
            {
                return clips[Random.Range(0, clips.Count)];
            }
        }


        /// <summary>
        /// Sets volume for the [id]th source in sources list. Use instead of directly changing source volume as this takes master volume into account.
        /// </summary>
        public void SetVolume(float volume, int id)
        {
            if (!sources[id]) return;
            sources[id].volume = volume * vc.sound.masterVolume;
        }

        /// <summary>
        /// Sets volume for the first source in sources list. Use instead of directly changing source volume as this takes master volume into account.
        /// </summary>
        public void SetVolume(float volume)
        {
            if (!Source) return;
            Source.volume = volume * vc.sound.masterVolume;
        }


        /// <summary>
        /// Sets pitch for the [id]th source in sources list. 
        /// </summary>
        public void SetPitch(float pitch, int id)
        {
            if (!sources[id]) return;
            sources[id].pitch = pitch;
        }


        /// <summary>
        /// Sets pitch for the first source in sources list.
        /// </summary>
        public void SetPitch(float pitch)
        {
            if (!Source) return;
            Source.pitch = pitch;
        }

        /// <summary>
        /// Gets volume of the Source. Equal to Source.volume.
        /// </summary>
        public float GetVolume()
        {
            if (!Source) return 0;
            return Source.volume;
        }

        /// <summary>
        /// Gets pitch of the Source. Equal to Source.volume.
        /// </summary>
        public float GetPitch()
        {
            if (!Source) return 0;
            return Source.pitch;
        }


        public void Enable()
        {
            foreach (AudioSource source in sources)
            {
                if (!source.enabled) source.enabled = true;
            }
        }

        public void Disable()
        {
            foreach (AudioSource source in sources)
            {
                if (source.enabled) source.enabled = false;
            }
        }

        public abstract void Initialize(VehicleController vc, AudioMixerGroup amg);
        public abstract void Update();
    }

    /// <summary>
    /// Sound of an engine idling.
    /// </summary>
    [System.Serializable]
    public class EngineIdleSoundComponent : SoundComponent
    {
        /// <summary>
        /// Volume added to the base engine volume depending on engine state.
        /// </summary>
        [Tooltip("Volume added to the base engine volume depending on engine state.")]
        [Range(0, 1)]
        public float volumeRange = 0.5f;

        /// <summary>
        /// Pitch added to the base engine pitch depending on engine RPM.
        /// </summary>
        [Tooltip("Pitch added to the base engine pitch depending on engine RPM.")]
        [Range(0, 4)]
        public float pitchRange = 1.5f;

        /// <summary>
        /// Smoothing of engine sound.
        /// </summary>
        [Tooltip("Smoothing of engine sound.")]
        [Range(0, 1)]
        public float smoothing = 0.1f;

        /// <summary>
        /// Distortion that will be added to the engine sound through mixer when under heavy load / high RPM.
        /// </summary>
        [Tooltip("Distortion that will be added to the engine sound through mixer when under heavy load / high RPM.")]
        [Range(0, 1)]
        public float maxDistortion = 0.4f;

        public override void Initialize(VehicleController vc, AudioMixerGroup amg)
        {
            this.vc = vc;
            this.audioMixerGroup = amg;

            // Initialize engine sound
            if (Clip != null)
            {
                Source = vc.gameObject.AddComponent<AudioSource>();
                vc.sound.SetAudioSourceDefaults(Source, true, true, 0f, Clip);
                RegisterSources();
                Source.Stop();
                SetVolume(0);
            }
        }

        public override void Update()
        {
            // Engine sound
            if (Source != null && Clip != null)
            {
                if (vc.engine.IsRunning || vc.engine.Starting || vc.engine.Stopping)
                {
                    if (!Source.isPlaying && Source.enabled) Source.Play();

                    float rpmModifier = Mathf.Clamp01((vc.engine.RPM - vc.engine.minRPM) / vc.engine.maxRPM);
                    float newPitch = pitch + rpmModifier * pitchRange;
                    Source.pitch = Mathf.Lerp(Source.pitch, newPitch, 1f - smoothing);

                    float volumeModifier = 0;
                    if (vc.transmission.Gear == 0)
                    {
                        volumeModifier = rpmModifier;
                    }
                    else
                    {
                        if (vc.transmission.transmissionType == Transmission.TransmissionType.Manual)
                        {
                            volumeModifier = rpmModifier * 0.65f + Mathf.Clamp01(vc.input.Vertical) * 0.3f;
                        }
                        else
                        {
                            volumeModifier = rpmModifier * 0.65f + Mathf.Abs(vc.input.Vertical) * 0.3f;
                        }
                    }

                    float newVolume = (volume + Mathf.Clamp01(volumeModifier) * volumeRange);

                    // Set distortion
                    audioMixerGroup.audioMixer.SetFloat("engineDistortion", Mathf.Lerp(0f, maxDistortion, volumeModifier));

                    if (vc.engine.Starting)
                        newVolume = vc.engine.StartingPercent * volume;

                    if (vc.engine.Stopping)
                        newVolume = (1f - vc.engine.StoppingPercent) * volume;

                    // Add random offsets if vehicle damaged
                    if (vc.damage.enabled && vc.damage.performanceDegradation)
                    {
                        float damageRandomRange = 0.2f * vc.damage.DamagePercent;
                        float damageOffset = Random.Range(-damageRandomRange, damageRandomRange) * volumeRange;
                        newVolume *= (1f + damageOffset);
                        Source.pitch += damageOffset * Source.pitch;
                    }

                    SetVolume(newVolume);
                }
                else
                {
                    if (Source.isPlaying) Source.Stop();
                    Source.volume = 0;
                    Source.pitch = 0;
                }
            }
        }
    }


    /// <summary>
    /// Sound of an engine starting / stopping.
    /// First audio clip is for engine starting, and second one is for engine stopping.
    /// </summary>
    [System.Serializable]
    public class EngineStartStopComponent : SoundComponent
    {
        public override void Initialize(VehicleController vc, AudioMixerGroup amg)
        {
            this.vc = vc;
            this.audioMixerGroup = amg;

            // Initilize start/stop source
            if (Clips.Count > 1)
            {
                Source = vc.gameObject.AddComponent<AudioSource>();
                vc.sound.SetAudioSourceDefaults(Source, false, false);
                RegisterSources();
                Source.enabled = false;
            }
        }

        public override void Update()
        {
            // Starting and stopping engine sound
            if (Source != null && Clips.Count > 1)
            {
                if ((vc.engine.Starting || vc.engine.Stopping))
                {
                    if (!Source.enabled) Source.enabled = true;
                    if (vc.engine.Starting) Source.clip = Clips[0];
                    if (vc.engine.Stopping) Source.clip = Clips[1];

                    float newVolume = volume;

                    if (vc.engine.Starting)
                        newVolume = (1f - vc.engine.StartingPercent) * volume;

                    if (vc.engine.Stopping)
                        newVolume = (1f - vc.engine.StoppingPercent) * volume;

                    SetVolume(newVolume);

                    if (!Source.isPlaying && Source.enabled)
                        Source.Play();
                }
                else
                {
                    Source.volume = 0;
                    Source.Stop();
                    Source.enabled = false;
                }
            }
        }
    }
    #endregion
}