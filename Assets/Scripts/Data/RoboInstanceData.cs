using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RoboInstanceData
{
    GameState gameState;
    public System.Guid Id;
    public RobotIdentity Identity;
    public event RoboInstanceDataEventHandler<float> onPowerChanged;

    public event RoboInstanceDataEventHandler<float> onHeal;
    public event RoboInstanceDataEventHandler<float> onDamageKinetic;
    /// <summary>
    /// got hit with pure power cost
    /// </summary>
    public event RoboInstanceDataEventHandler<float> onPowerCost;
    /// <summary>
    /// when kinetic damage is blocked
    /// </summary>
    public event RoboInstanceDataEventHandler<float> onDamageBlocked;
    public event RoboInstanceDataEventHandler<float> onDamageReducedByArmor;
    public event RoboInstanceDataEventHandler<float> onDamageThermal;
    /// <summary>
    /// when shield absorbs thermal damage
    /// </summary>
    public event RoboInstanceDataEventHandler<float> onDamageAbsorbed;
    public event RoboInstanceDataEventHandler onKilled;
    public event RoboInstanceDataEventHandler<float> onBoltsChanged;

    public string RoboName;

    internal Mainframe mainframe;

    internal List<ModuleInstance> reactors = new List<ModuleInstance>();
    internal List<ModuleInstance> heads = new List<ModuleInstance>();
    internal List<ModuleInstance> arms = new List<ModuleInstance>();
    internal List<ModuleInstance> legs = new List<ModuleInstance>();
    internal List<ModuleInstance> utilities = new List<ModuleInstance>();
    internal List<ModuleInstance> storage = new List<ModuleInstance>();

    public IEnumerable<ModuleInstance> AllEquippedModules => reactors.Union(heads).Union(arms).Union(legs).Union(utilities);
    public bool IsKilled => CurrentPower <= 0;

    public bool AreThereFreeReactorSlots => reactors.Count < mainframe.ReactorSlots;
    public bool AreThereFreeHeadSlots => heads.Count < mainframe.HeadSlots;
    public bool AreThereFreeArmSlots => arms.Count < mainframe.ArmSlots;
    public bool AreThereFreeLegSlots => legs.Count < mainframe.LegSlots;
    public bool AreThereFreeUtilitySlots => utilities.Count < mainframe.UtilitySlots;
    public bool AreThereFreeStorageSlots => storage.Count < mainframe.StorageSlots;

    //Stats
    public float CurrentPower { get; private set; }
    internal bool hasScout;
    public float MaxPower => mainframe.BaseHp + GetBonusHP();
    float bolts;
    public float Bolts => bolts;
    /// <summary>
    /// % of bolts/gold that drops  f.e 1f => 100%
    /// </summary>
    public float TotalSalvage => mainframe.BaseSalvaging + GetBonusSalvaging();
    /// <summary>
    /// kinetic armor block
    /// </summary>
    public float BlockPoints { get; private set; }
    /// <summary>
    /// thermal shield block
    /// </summary>
    public float ShieldPoints { get; private set; }
    public bool IsPlayer => Identity == RobotIdentity.Player;
    public bool IsEnemy => Identity == RobotIdentity.Enemy;
    public bool IsAlly => Identity == RobotIdentity.PlayerAlly;

    /// <summary>
    /// 0-1f,     1 -> 100%
    /// </summary>
    public float HealthPercent => MaxPower != 0 ? CurrentPower / MaxPower : 0;

    /// <summary>
    /// kinetic damage reduction
    /// </summary>
    int armor;


    public RoboInstanceData()
    {
        if (gameState == null)
            gameState = GameState.instance;

        this.Id = System.Guid.NewGuid();
    }

    public void AddBolts(float amount)
    {
        bolts += amount;
        onBoltsChanged?.Invoke(this, new RoboInstanceDataEventArgs<float>(amount));
    }
    
    public void SetCompleteLoadout(Mainframe mainframe, Module[] reactors = null, Module[] heads = null, Module[] arms = null, Module[] legs = null, Module[] utilities = null, Module[] storage = null)
    {
        this.mainframe = mainframe;
        armor = mainframe.BaseArmor;
        CurrentPower = Mathf.FloorToInt(mainframe.BaseHp);
        reactors?.ToList().ForEach(m => AddModule(new ModuleInstance(m, this.reactors.Count), AreThereFreeReactorSlots, ref this.reactors));
        heads?.ToList().ForEach(m => AddModule(new ModuleInstance(m, this.heads.Count), AreThereFreeHeadSlots, ref this.heads));
        arms?.ToList().ForEach(m => AddModule(new ModuleInstance(m, this.arms.Count), AreThereFreeArmSlots, ref this.arms));
        legs?.ToList().ForEach(m => AddModule(new ModuleInstance(m, this.legs.Count), AreThereFreeLegSlots, ref this.legs));
        utilities?.ToList().ForEach(m => AddModule(new ModuleInstance(m, this.utilities.Count), AreThereFreeUtilitySlots, ref this.utilities));
        storage?.ToList().ForEach(m => AddModule(new ModuleInstance(m, this.storage.Count), AreThereFreeStorageSlots, ref this.storage));

        //set full hp
        CurrentPower = MaxPower;
    }

    void AddModule(ModuleInstance module, bool areThereFreeSlots, ref List<ModuleInstance> specificSlotList)
    {
        if (mainframe == null)
        {
            Debug.LogError($"Mainframe not set up");
            return;
        }
        if (areThereFreeSlots)
        {
            specificSlotList.Add(module);
        }
        else
            throw new Exception($"There are no '{module.Slot}' slots for {module.Key}");
    }

    /// <summary>
    /// Drop random amount of bolts/gold from this robot's modules
    /// </summary>
    internal float DropBolts()
    {
        var totalDrop = 0f;
        foreach(var module in AllEquippedModules)
        {
            totalDrop+= UnityEngine.Random.Range(0f, module.MaxBoltsDrop);
        }
        return totalDrop;
    }

    internal int GetBonusKineticDMG()
    {
        int bonusDmg = 0;
        bonusDmg += reactors.Where(m => m.BonusDamageKinetic > 0 && m.IsEnabled).Sum(m => m.BonusDamageKinetic);
        bonusDmg += heads.Where(m => m.BonusDamageKinetic > 0 && m.IsEnabled).Sum(m => m.BonusDamageKinetic);
        bonusDmg += arms.Where(m => m.BonusDamageKinetic > 0 && m.IsEnabled).Sum(m => m.BonusDamageKinetic);
        bonusDmg += legs.Where(m => m.BonusDamageKinetic > 0 && m.IsEnabled).Sum(m => m.BonusDamageKinetic);
        bonusDmg += utilities.Where(m => m.BonusDamageKinetic > 0 && m.IsEnabled).Sum(m => m.BonusDamageKinetic);
        return bonusDmg;
    }
    internal int GetBonusThermalDMG()
    {
        int bonusDmg = 0;
        bonusDmg += reactors.Where(m => m.BonusDamageThermal > 0 && m.IsEnabled).Sum(m => m.BonusDamageThermal);
        bonusDmg += heads.Where(m => m.BonusDamageThermal > 0 && m.IsEnabled).Sum(m => m.BonusDamageThermal);
        bonusDmg += arms.Where(m => m.BonusDamageThermal > 0 && m.IsEnabled).Sum(m => m.BonusDamageThermal);
        bonusDmg += legs.Where(m => m.BonusDamageThermal > 0 && m.IsEnabled).Sum(m => m.BonusDamageThermal);
        bonusDmg += utilities.Where(m => m.BonusDamageThermal > 0 && m.IsEnabled).Sum(m => m.BonusDamageThermal);
        return bonusDmg;
    }

    internal string PrintLoadout()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(mainframe.ToString());
        sb.AppendLine($"Reactors: {String.Join(", ", reactors.Select(r => r.ModuleName).ToArray())}");
        sb.AppendLine($"Heads: {String.Join(", ", heads.Select(r => r.ModuleName).ToArray())}");
        sb.AppendLine($"Arms: {String.Join(", ", arms.Select(r => r.ModuleName).ToArray())}");
        sb.AppendLine($"Legs: {String.Join(", ", legs.Select(r => r.ModuleName).ToArray())}");
        sb.AppendLine($"Utilities: {String.Join(", ", utilities.Select(r => r.ModuleName).ToArray())}");
        sb.AppendLine($"Storage: {String.Join(", ", storage.Select(r => r.ModuleName).ToArray())}");

        return sb.ToString();

    }

    internal int GetBonusHP()
    {
        int bonusMaxHp = 0;
        bonusMaxHp += reactors.Where(m => m.BonusMaxHp > 0 && m.IsEnabled).Sum(m => m.BonusMaxHp);
        bonusMaxHp += heads.Where(m => m.BonusMaxHp > 0 && m.IsEnabled).Sum(m => m.BonusMaxHp);
        bonusMaxHp += arms.Where(m => m.BonusMaxHp > 0 && m.IsEnabled).Sum(m => m.BonusMaxHp);
        bonusMaxHp += legs.Where(m => m.BonusMaxHp > 0 && m.IsEnabled).Sum(m => m.BonusMaxHp);
        bonusMaxHp += utilities.Where(m => m.BonusMaxHp > 0 && m.IsEnabled).Sum(m => m.BonusMaxHp);
        return bonusMaxHp;
    }

    
    internal float GetBonusSalvaging()
    {
        float bonusSalvaging = 0;
        bonusSalvaging += reactors.Where(m => m.BonusSalvaging > 0 && m.IsEnabled).Sum(m => m.BonusSalvaging);
        bonusSalvaging += heads.Where(m => m.BonusSalvaging > 0 && m.IsEnabled).Sum(m => m.BonusSalvaging);
        bonusSalvaging += arms.Where(m => m.BonusSalvaging > 0 && m.IsEnabled).Sum(m => m.BonusSalvaging);
        bonusSalvaging += legs.Where(m => m.BonusSalvaging > 0 && m.IsEnabled).Sum(m => m.BonusSalvaging);
        bonusSalvaging += utilities.Where(m => m.BonusSalvaging > 0 && m.IsEnabled).Sum(m => m.BonusSalvaging);
        return bonusSalvaging;
    }

    public void AddBlock(float value)
    {
        BlockPoints += value;
    }

    public void ApplyDamageKinetic(float amount)
    {
        ApplyDamageKinetic(amount, out bool wasReduced, out bool wasBlocked);
    }

    public void ApplyDamageThermal(float amount)
    {
        ApplyDamageThermal(amount, out bool wasAbsorbed);
    }
    public int ApplyDamageThermal(float amount, out bool wasAbsorbed, bool calculationOnly = false)
    {
        wasAbsorbed = false;
        var realAmount = amount;

        //apply block modifier
        if (ShieldPoints > 0)
        {
            float pointsAbsorbed = ShieldPoints > realAmount ? realAmount : ShieldPoints;
            if (!calculationOnly)
            {
                onDamageAbsorbed?.Invoke(this, new RoboInstanceDataEventArgs<float>(pointsAbsorbed));
                ShieldPoints -= pointsAbsorbed;
            }
            realAmount -= pointsAbsorbed;
            wasAbsorbed = true;
        }
        if (realAmount < 0)
            return 0;

        if (realAmount > 0)
        {
            if (!calculationOnly)
            {
                CurrentPower = (int)Mathf.Clamp(CurrentPower - realAmount, 0, CurrentPower);
                onDamageKinetic?.Invoke(this, new RoboInstanceDataEventArgs<float>(realAmount));
                onPowerChanged?.Invoke(this, new RoboInstanceDataEventArgs<float>(realAmount * -1f));
                CheckIsAlive();
            }
            return Mathf.FloorToInt(realAmount);
        }
        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="wasReduced"></param>
    /// <param name="wasBlocked"></param>
    /// <returns>value of applied damage, (pure without reduced/blocked part)</returns>
    public int ApplyDamageKinetic(float amount, out bool wasReduced, out bool wasBlocked, bool calculationOnly = false)
    {
        wasReduced = false;
        wasBlocked = false;
        var realAmount = amount;

        //apply block modifier
        if (BlockPoints > 0)
        {
            float pointsBlocked = BlockPoints > realAmount ? realAmount : BlockPoints;
            if (!calculationOnly)
            {
                onDamageBlocked?.Invoke(this, new RoboInstanceDataEventArgs<float>(pointsBlocked));
                BlockPoints -= pointsBlocked;
            }
            realAmount -= pointsBlocked;
            wasBlocked = true;
        }
        if (realAmount < 0)
            return 0;

        //apply armor modifier
        if (armor > 0)
        {
            if (!calculationOnly)
            {
                onDamageReducedByArmor?.Invoke(this, new RoboInstanceDataEventArgs<float>(armor > realAmount ? realAmount : armor));
            }
            realAmount -= armor;
            wasReduced = true;
        }
        if (realAmount < 0)
            return 0;

        if (realAmount > 0)
        {
            if (!calculationOnly)
            {
                CurrentPower = (int)Mathf.Clamp(CurrentPower - realAmount, 0, CurrentPower);
                onDamageKinetic?.Invoke(this, new RoboInstanceDataEventArgs<float>(realAmount));
                onPowerChanged?.Invoke(this, new RoboInstanceDataEventArgs<float>(realAmount * -1f));
                CheckIsAlive();
            }
            return Mathf.FloorToInt(realAmount);
        }
        return 0;
    }

    public void ApplyPowerCost(float amount)
    {
        CurrentPower = Mathf.FloorToInt(Mathf.Clamp(CurrentPower - amount, 0, CurrentPower));
        onPowerCost?.Invoke(this, new RoboInstanceDataEventArgs<float>(amount));
        onPowerChanged?.Invoke(this, new RoboInstanceDataEventArgs<float>(amount * -1f));
        CheckIsAlive();
    }


    void CheckIsAlive()
    {
        if (CurrentPower > 0)
        {
            //alive
        }
        else
        {
            onKilled?.Invoke(this);
        }
    }

    public void Heal(float amount)
    {
        var realHeal = MaxPower - CurrentPower > amount ? amount : MaxPower - CurrentPower;
        CurrentPower += Mathf.FloorToInt(realHeal);
        onHeal?.Invoke(this, new RoboInstanceDataEventArgs<float>(realHeal));
        onPowerChanged?.Invoke(this, new RoboInstanceDataEventArgs<float>(realHeal));
    }


    public bool CanUseModule(Guid moduleId, System.Guid targetRobotId)
    {
        var module = AllEquippedModules.FirstOrDefault(m => m.Id == moduleId);
        if (module == null)
            Debug.LogError($"Module {moduleId} not exists for robot {Id}");

        var target = gameState.battleData.AllFighters.FirstOrDefault(r => r.Id == targetRobotId);

        //Commented to allow suicide
        //if (module.PowerCost > CurrentPower)
        //{
        //    //not enough power
        //    return false;
        //}

        if (GameState.instance.battleData.phaseOwnerId != Id)
        {
            //not his phase to move
        }
        if (GameState.instance.battleData.ActivePhase && !module.IsActiveEffectModule)
        {
            //trying to use passive module in active phase
            return false;
        }
        
        if(module.HpRegen>0)
        {
            if (module.AllowedTargets.Any(t => t == AllowedTargets.Self) && this.Id == target.Id)
                return true;
            if (module.AllowedTargets.Any(t => t == AllowedTargets.Friendly)
                && ((this.IsPlayer && target.IsAlly) || this.IsEnemy && target.IsEnemy))//player on allies, enemies on other enemies
                return true;
            else
            {
                //allow casting on invalid target, gameLogic will replace it to self
            }
        }


        return true;
    }

    public ModuleInstance GetModuleById(System.Guid moduleId)
    {
        return AllEquippedModules.FirstOrDefault(m => m.Id == moduleId);
    }

    /// <summary>
    /// with bonuses
    /// </summary>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    public int GetTotalKineticDamage(System.Guid moduleId)
    {
        var module = AllEquippedModules.FirstOrDefault(m => m.Id == moduleId);
        if (module == null)
            throw new Exception($"Robot does not have this module! {moduleId} , {nameof(AllEquippedModules)} does not contain this module!Check the robot setup!");
        return module.DamageKinetic + (module.DamageKinetic > 0 ? this.GetBonusKineticDMG() : 0);
    }
    /// <summary>
    /// with bonuses
    /// </summary>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    public int GetTotalThermalDamage(System.Guid moduleId)
    {
        var module = AllEquippedModules.FirstOrDefault(m => m.Id == moduleId);
        return module.DamageThermal + (module.DamageThermal > 0 ? this.GetBonusThermalDMG() : 0);
    }
}
