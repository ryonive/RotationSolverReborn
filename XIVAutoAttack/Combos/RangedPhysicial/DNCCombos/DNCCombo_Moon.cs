using Dalamud.Game.ClientState.JobGauge.Types;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedPhysicial.DNCCombos.DNCCombo_Moon;

namespace XIVAutoAttack.Combos.RangedPhysicial.DNCCombos;

internal sealed class DNCCombo_Moon : DNCCombo_Base<CommandType>
{
    public override string Author => "ϫ��Moon";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.��Χ����, $"{ShieldSamba}"},
        {DescType.��Χ����, $"{CuringWaltz}, {Improvisation}"},
        {DescType.�ƶ�����, $"{EnAvant}"},
    };

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (EnAvant.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (CuringWaltz.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (Improvisation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (ShieldSamba.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        //����״̬��ֹʹ��
        if (IsDancing) return false;

        //����֮̽��
        if (Devilment.ShouldUse(out act, emptyOrSkipCombo: true))
        {
            if (SettingBreak && !TechnicalStep.EnoughLevel) return true;

            if (Player.HasStatus(true, StatusID.TechnicalFinish)) return true;
        }

        //Ӧ�������
        if (UseClosedPosition(out act)) return true;
      
        //�ٻ�
        if (Flourish.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //���衤��
        if (FanDance3.ShouldUse(out act, skipBuff: true)) return true;

        if (Player.HasStatus(true, StatusID.Devilment) || Feathers > 3 || !TechnicalStep.EnoughLevel)
        {
            //���衤��
            if (FanDance2.ShouldUse(out act)) return true;
            //���衤��
            if (FanDance.ShouldUse(out act)) return true;
        }

        //���衤��
        if (FanDance4.ShouldUse(out act, skipBuff: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //�����
        if (!InCombat && !Player.HasStatus(true, StatusID.ClosedPosition1) && ClosedPosition.ShouldUse(out act)) return true;

        //�����貽
        if (FinishStepGCD(out act)) return true;

        //ִ���貽
        if (ExcutionStepGCD(out act)) return true;

        //�����貽
        if (SettingBreak && InCombat && TechnicalStep.ShouldUse(out act, skipBuff: true)) return true;

        //����GCD
        if (AttackGCD(out act, Player.HasStatus(true, StatusID.Devilment))) return true;

        return false;
    }

    /// <summary>
    /// ����GCD
    /// </summary>
    /// <param name="act"></param>
    /// <param name="breaking"></param>
    /// <returns></returns>
    private bool AttackGCD(out IAction act, bool breaking)
    {
        act = null;
        //����״̬��ֹʹ��
        if (IsDancing) return false;

        //����
        if ((breaking || Esprit >= 85) && SaberDance.ShouldUse(out act, skipBuff: true)) return true;

        //������
        if (Tillana.ShouldUse(out act, skipBuff: true)) return true;

        //������
        if (StarfallDance.ShouldUse(out act, skipBuff: true)) return true;

        //ʹ�ñ�׼�貽
        if (UseStandardStep(out act)) return true;

        //����AOE
        if (Bloodshower.ShouldUse(out act)) return true;
        if (Fountainfall.ShouldUse(out act)) return true;
        //��������
        if (RisingWindmill.ShouldUse(out act)) return true;
        if (ReverseCascade.ShouldUse(out act)) return true;

        //����AOE
        if (Bladeshower.ShouldUse(out act)) return true;
        if (Windmill.ShouldUse(out act)) return true;
        //��������
        if (Fountain.ShouldUse(out act)) return true;
        if (Cascade.ShouldUse(out act)) return true;

        return false;
    }
     
    /// <summary>
    /// ʹ�ñ�׼�貽
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseStandardStep(out IAction act)
    {
        if (!StandardStep.ShouldUse(out act, skipBuff: true)) return false;

        //�ȼ��������̫�಻����,��ֱ�����˻���ɶ��
        if (Level - Target.Level > 10) return false;

        //��Χû�е��˲�����
        if (!HaveHostilesInRange) return false;

        //�����貽״̬�Ϳ���ȴ��ʱ���ͷ�
        if (TechnicalStep.EnoughLevel && (Player.HasStatus(true, StatusID.TechnicalFinish) || TechnicalStep.IsCoolDown && TechnicalStep.WillHaveOneChargeGCD(2))) return false;

        return true;
    }

    /// <summary>
    /// �����貽
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseFinishStepGCD(out IAction act)
    {
        if (!FinishStepGCD(out act)) return false;

        if (Target.IsBoss()) return true;

        if (Windmill.ShouldUse(out _)) return true;

        if (TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 25).Length >= 3) return false;

        return false;
    }

    /// <summary>
    /// Ӧ�������
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseClosedPosition(out IAction act)
    { 
        if (!ClosedPosition.ShouldUse(out act)) return false;

        //Ӧ�������
        if (InCombat && Player.HasStatus(true, StatusID.ClosedPosition1))
        {
            foreach (var friend in TargetUpdater.PartyMembers)
            {
                if (friend.HasStatus(true, StatusID.ClosedPosition2))
                {
                    if (ClosedPosition.Target != friend) return true;
                    break;
                }
            }
        }
        else if (ClosedPosition.ShouldUse(out act)) return true;

        act = null;
        return false;
    }
}
