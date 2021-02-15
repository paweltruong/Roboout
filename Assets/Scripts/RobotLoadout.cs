using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using TMPro;
using System;

/// <summary>
/// main class used in robot prefab, handles presentation of roboInstanceData
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(AudioSource))]
public class RobotLoadout : MonoBehaviour, IPaintableProvider
{
    [SerializeField] internal RobotIdentity identity;
    [SerializeField] internal int identityIndex;
    [SerializeField] internal System.Guid Id;


    [Tooltip("Żeby dodawane moduły z tyły przesuwało w sorting order")]
    public int ModulesBehindBodySortingLayerModifier = -100;
    public int ModulesInsideBodySortingLayerModifier = -50;
    public int ModuleInFrontBodySortingLayerModifier = 100;


    [Header("Mainframe Setups")]
    [SerializeField] MainframeSetup_Pantium Pantium_Setup;
    [SerializeField]
    MainframeType CurrentMainframe;

    public Mainframe Mainframe;
    [Header("Modules")]
    public Module[] Reactors;
    public Module[] Heads;
    public Module[] Arms;
    public Module[] Legs;
    public Module[] Utilities;
    public Module[] Storage;


    [Header("Modules Visualization (Supported modules)")]
    [HideInInspector]
    public ModuleVisualState[] Modules;


    [Header("PaintJob")]
    [SerializeField()]
    PaintJobManager paintJobManager;
    [SerializeField()] public List<IPaintJobPaintable> Paintables;
    public PaintJobManager PaintJobManager => paintJobManager;
    UnityEvent OnPaintableChanged;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] HpBarUI hpBar;
    [SerializeField] Transform floatingTextSpawner;
    [SerializeField] GameObject DamageKineticFloatingTextPrefab;
    [SerializeField] GameObject DamageThermalFloatingTextPrefab;
    [SerializeField] GameObject HealFloatingTextPrefab;
    /// <summary>
    /// for blocking
    /// </summary>
    [SerializeField] GameObject DamageBlockedFloatingTextPrefab;
    /// <summary>
    /// for armor
    /// </summary>
    [SerializeField] GameObject DamageReducedFloatingTextPrefab;
    /// <summary>
    /// for shield
    /// </summary>
    [SerializeField] GameObject DamageAbsorbedFloatingTextPrefab;
    [SerializeField] ExplosionAnimationController explosion;

    [Header("SFX")]
    [SerializeField] AudioClip[] LightDamageSFX;
    [SerializeField] AudioClip[] MediumDamageSFX;
    [SerializeField] AudioClip[] HeavyDamageSFX;
    [SerializeField] AudioClip[] DeathSFX;
    AudioSource audioSource;

    bool flipX;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        explosion.onExplosionMaxEffectFrame.AddListener(() =>
        {
            var mainframe = GetMainframeSetup();
            mainframe.gameObject.SetActive(false);
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        if (paintJobManager != null)
            paintJobManager.onSelectedPaintJobChanged.AddListener(OnSelectedPaintJobChanged);
        ValidateLoadout();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        //LoadOnce in Editor if empty
        if (Modules == null || Modules.Length == 0)
            ReloadModuleVisualStates();
#endif

        BuildRobot();
    }

    public void ReloadModuleVisualStates()
    {

        var moduleStates = new List<ModuleVisualState>();
        var modules = Resources.LoadAll<Module>("Modules");
        if (modules != null)
        {
            //reaktory
            var knownReactors = modules.Where(m => m.Slot == SlotType.Reactor);
            foreach (var m in knownReactors)
            {
                var moduleState = new ModuleVisualState { key = m.Key, installed = new bool[Mainframe.ReactorSlots] };
                moduleStates.Add(moduleState);
            }
            //head
            var knownHeads = modules.Where(m => m.Slot == SlotType.Head);
            foreach (var m in knownHeads)
            {
                var moduleState = new ModuleVisualState { key = m.Key, installed = new bool[Mainframe.HeadSlots] };
                moduleStates.Add(moduleState);
            }
            //arms
            var knownArms = modules.Where(m => m.Slot == SlotType.Arm);
            foreach (var m in knownArms)
            {
                var moduleState = new ModuleVisualState { key = m.Key, installed = new bool[Mainframe.ArmSlots] };
                moduleStates.Add(moduleState);
            }
            //legs
            var knownLegs = modules.Where(m => m.Slot == SlotType.Leg);
            foreach (var m in knownLegs)
            {
                var moduleState = new ModuleVisualState { key = m.Key, installed = new bool[Mainframe.LegSlots] };
                moduleStates.Add(moduleState);
            }
            //utilities
            var knownUtilities = modules.Where(m => m.Slot == SlotType.Utility);
            foreach (var m in knownUtilities)
            {
                var moduleState = new ModuleVisualState { key = m.Key, installed = new bool[Mainframe.UtilitySlots] };
                moduleStates.Add(moduleState);
            }
        }
        Modules = moduleStates.ToArray();
    }

    void ValidateLoadout()
    {
        if (Mainframe == null
            || (Reactors != null && Reactors.Any(r => r.Slot != SlotType.Reactor))
            || (Arms != null && Arms.Any(a => a.Slot != SlotType.Arm))
            || (Legs != null && Legs.Any(l => l.Slot != SlotType.Leg))
            || (Heads != null && Heads.Any(h => h.Slot != SlotType.Head))
            || (Utilities != null && Utilities.Any(u => u.Slot != SlotType.Utility))
            || (Storage != null && Storage.Any(s => s.Slot != SlotType.Utility))
            )
            Debug.LogError("Loadout invalid");
    }

    internal void Set(RoboInstanceData roboInstanceData, RobotIdentity identity)
    {
        this.Id = roboInstanceData.Id;
        this.identity = identity;
        if (identity == RobotIdentity.Enemy)
            flipX = true;
        Mainframe = roboInstanceData.mainframe;
        var modulesToEquip = roboInstanceData.AllEquippedModules;

        foreach (var module in modulesToEquip)
        {
            var visualStateForKey = Modules.FirstOrDefault(mvs => mvs.key == module.Key);
            visualStateForKey.installed[module.SlotIndex] = true;
        }

        textName.text = roboInstanceData.RoboName;
        hpBar.Bind(roboInstanceData);
        Bind(roboInstanceData);
    }

    void Bind(RoboInstanceData roboInstanceData)
    {
        roboInstanceData.onDamageKinetic += RoboInstanceData_onDamageKinetic;
        roboInstanceData.onHeal += RoboInstanceData_onHeal;
        roboInstanceData.onKilled += RoboInstanceData_onKilled;
    }

    private void RoboInstanceData_onKilled(RoboInstanceData sender)
    {
        Explode();
    }

    private void RoboInstanceData_onHeal(RoboInstanceData sender, RoboInstanceDataEventArgs<float> e)
    {
        SpawnFloatingText(HealFloatingTextPrefab, ((int)e.Value).ToString());
    }

    private void RoboInstanceData_onDamageKinetic(RoboInstanceData sender, RoboInstanceDataEventArgs<float> e)
    {
        SpawnFloatingText(DamageKineticFloatingTextPrefab, ((int)e.Value).ToString());
    }

    void SpawnFloatingText(GameObject prefab, string value)
    {
        var obj = Instantiate(prefab, floatingTextSpawner);
        var floatingText = obj.GetComponent<FloatingTextPopup>();
        floatingText.Set(value);
    }

    void BuildRobot()
    {
        var mainframeSetup = GetMainframeSetup();
        if (mainframeSetup != null)
        {
            if (flipX)
                mainframeSetup.transform.localScale = new Vector3(-1, mainframeSetup.transform.localScale.y, mainframeSetup.transform.localScale.z);
            EnableModules(mainframeSetup);

            PaintJobManager.ApplyPaint(GetPaintables());
        }
        else
            Debug.LogError("Could not find MainframeSetup");
    }

    MainframeSetup_Base GetMainframeSetup()
    {
        if (Mainframe != null)
        {
            MainframeSetup_Base mainframeSetup = null;
            switch (Mainframe.Type)
            {
                default:
                    mainframeSetup = Pantium_Setup;
                    break;
            }
            return mainframeSetup;
        }
        return null;
    }

    void EnableModules(MainframeSetup_Base mainframeSetup)
    {
        if (mainframeSetup != null)
        {
            foreach (var moduleVS in Modules)
            {
                for (int i = 0; i < moduleVS.installed.Length; ++i)
                {
                    var foundVisualization = mainframeSetup.ModuleVisualisations.FirstOrDefault(m => m.Key == moduleVS.key && m.slotPlacementIndex == i);
                    if (foundVisualization != null)
                    {
                        if (foundVisualization.slotPlacementIndex < moduleVS.installed.Length)
                            foundVisualization.gameObject.SetActive(moduleVS.installed[foundVisualization.slotPlacementIndex]);
                        //else
                        //    Debug.LogWarning($"Found module '{foundVisualization.name}' does not fit mainframe config try fixing 'slotPlacementIndex' or mainframe slot counts for {Mainframe.name}");
                    }
                    else
                    {
                        //Debug.LogWarning($"Mainframe '{mainframeSetup.name}' does not have module visualization for '{moduleVS.key}' on slot '{i}'");
                    }
                }
            }
        }
    }

    public ModuleVisualization GetModuleVisualization(ModuleKey key, int slotIndex)
    {
        var mainframeSetup = GetMainframeSetup();
        if (mainframeSetup != null)
        {
            var foundVisualization = mainframeSetup.ModuleVisualisations.FirstOrDefault(m => m.Key == key && m.slotPlacementIndex == slotIndex);
            if (foundVisualization != null)
            {
                return foundVisualization;
            }
            else
            {
                Debug.LogError($"Mainframe '{mainframeSetup.name}' does not have module visualization for '{key}' on slot '{slotIndex}'");
            }
        }
        else
            Debug.LogError("MainframeSetup not found");
        return null;
    }

    void OnSelectedPaintJobChanged()
    {
        PaintJobManager.ApplyPaint(GetPaintables());
    }

    public IEnumerable<IPaintJobPaintable> GetPaintables()
    {
        var list = new List<IPaintJobPaintable>();

        //current Mainframe
        var mainframeVisualization = GetMainframeVisualizationObject();
        if (mainframeVisualization == null)
            Debug.LogError("Cannot find mainframe visualization for painting");
        list.Add(mainframeVisualization);

        //modules
        if (mainframeVisualization.ModuleVisualisations != null)
            list.AddRange(mainframeVisualization.ModuleVisualisations);


        return list;
    }

    MainframeSetup_Base GetMainframeVisualizationObject()
    {
        switch (CurrentMainframe)
        {
            //TODO:add support for more mainframes
            default:
                return Pantium_Setup;
        }
    }


    /// <summary>
    /// for testing purpose TODO:move to MenuItem Roboout/AnimationTools
    /// </summary>
    public void Attack1()
    {
        var mainframeSetup = GetMainframeSetup();
        mainframeSetup.TriggerAnimationOnMainframe(AnimationParameters.Trigger_Attack1);
        mainframeSetup.TriggerAnimationOnAllModules(AnimationParameters.Trigger_Attack1);
    }

    public void Attack1(ModuleKey key, int slotIndex)
    {
        var mainframeSetup = GetMainframeSetup();
        //besides specific modulekey only body(mainframe) triggers animation
        mainframeSetup.TriggerAnimationOnMainframe(AnimationParameters.Trigger_Attack1);
        mainframeSetup.TriggerAnimationOnModule(AnimationParameters.Trigger_Attack1, key, slotIndex);
    }

    public void TakeHit1(int appliedDamaged)
    {
        var mainframeSetup = GetMainframeSetup();
        mainframeSetup.TriggerAnimationOnMainframe(AnimationParameters.Trigger_TakeHit1);
        mainframeSetup.TriggerAnimationOnAllModules(AnimationParameters.Trigger_TakeHit1);

        PlayTakeHitSound(appliedDamaged);
        
    }

    void PlayTakeHitSound(int appliedDamaged)
    {
        AudioClip randomClip = null;
        if (appliedDamaged < 2)
        {
            randomClip = LightDamageSFX.GetRandom();
        }
        else if (appliedDamaged < 4)
        {
            randomClip = MediumDamageSFX.GetRandom();
        }
        else
        {
            randomClip = HeavyDamageSFX.GetRandom();
        }

        Debug.Log($"TakeHIt:play {randomClip.name}");
        if (audioSource != null && randomClip != null)
            audioSource.PlayOneShot(randomClip);
    }

    void PlayDeathSound()
    {
        AudioClip randomClip = DeathSFX.GetRandom();        

        if (audioSource != null && randomClip != null)
            audioSource.PlayOneShot(randomClip);
    }

    public void Idle()
    {
        var mainframeSetup = GetMainframeSetup();
        mainframeSetup.TriggerAnimationOnMainframe(AnimationParameters.Trigger_Idle);
        mainframeSetup.TriggerAnimationOnAllModules(AnimationParameters.Trigger_Idle);
    }
    /// <summary>
    /// for testing purpose TODO:move to MenuItem Roboout/AnimationTools
    /// </summary>
    public void Kick1()
    {
        var mainframeSetup = GetMainframeSetup();
        mainframeSetup.TriggerAnimationOnMainframe(AnimationParameters.Trigger_Kick1);
        mainframeSetup.TriggerAnimationOnAllModules(AnimationParameters.Trigger_Kick1);
    }
    public void Kick1(ModuleKey key, int slotIndex)
    {
        var mainframeSetup = GetMainframeSetup();
        //head is moved by neck bone in mainframe animation so we dont trigger head for animation
        mainframeSetup.TriggerAnimationOnMainframe(AnimationParameters.Trigger_Kick1);
        mainframeSetup.TriggerAnimationOnModule(AnimationParameters.Trigger_Kick1, key, slotIndex);
        //arms move when kicking
        mainframeSetup.TriggerAnimationOnModules(AnimationParameters.Trigger_Kick1, SlotType.Arm);
        
    }
    /// <summary>
    /// for testing purpose TODO:move to MenuItem Roboout/AnimationTools
    /// </summary>
    public void ShootRecoil1()
    {
        var mainframeSetup = GetMainframeSetup();
        mainframeSetup.TriggerAnimationOnMainframe(AnimationParameters.Trigger_ShootRecoil1);
        mainframeSetup.TriggerAnimationOnAllModules(AnimationParameters.Trigger_ShootRecoil1);
    }
    public void ShootRecoil1(ModuleKey key, int slotIndex)
    {
        var mainframeSetup = GetMainframeSetup();
        mainframeSetup.TriggerAnimationOnMainframe(AnimationParameters.Trigger_ShootRecoil1);
        mainframeSetup.TriggerAnimationOnModule(AnimationParameters.Trigger_ShootRecoil1, key, slotIndex);
    }
    public void ToggleBlock(bool toggle)
    {
        var mainframeSetup = GetMainframeSetup();
        mainframeSetup.SetAnimationOnMainframe(AnimationParameters.Param_Block1, toggle);
        mainframeSetup.SetAnimationOnAllModules(AnimationParameters.Param_Block1, toggle);
    }
    public void BlockHit()
    {
        var mainframeSetup = GetMainframeSetup();
        mainframeSetup.TriggerAnimationOnMainframe(AnimationParameters.Trigger_BlockHit);
        mainframeSetup.TriggerAnimationOnAllModules(AnimationParameters.Trigger_BlockHit);
    }

    public void Explode()
    {
        explosion.PlayAnimation();
        //hiding mainframe object handled in Awake methos explosion.onExplosionMaxEffectFrame
        PlayDeathSound();
    }
}
