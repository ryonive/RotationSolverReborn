﻿using ECommons.DalamudServices;

namespace RotationSolver.Basic.Configuration.Timeline;

[Description("Move Time line")]
internal class MoveTimelineItem : BaseTimelineItem
{
    public List<Vector3> Points { get; set; } = [];
    public override bool InPeriod(TimelineItem item)
    {
        var time = item.Time - DataCenter.RaidTimeRaw;

        if (time < 0) return false;

        if (time > Time || Time - time > 3) return false;
        return true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        var ipc = Svc.PluginInterface.GetIpcSubscriber<List<Vector3>, bool, object>("vnavmesh.Path.MoveTo");

        if(ipc == null)
        {
            Svc.Log.Error("Can't find the vnavmesh to move.");
            return;
        }
        ipc.InvokeAction(Points, false);
    }
}