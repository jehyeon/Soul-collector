using UnityEngine;

public class Shield : AttackedPassiveSkill
{
    protected override void Start()
    {
        base.Start();
        percent = 0.2f;
        time = 2f;
        activated = false;
    }

    public override void Activate()
    {
        if (activated)
        {
            // 실드가 활성화된 상태면 발동 안함
            return;
        }
        activated = true;
        player.invincibility = true;
        EffectManager.Instance.EffectShield(time, player.transform);
        Invoke("Done", time);
    }
    
    private void Done()
    {
        activated = false;
        player.invincibility = false;
    }
}
