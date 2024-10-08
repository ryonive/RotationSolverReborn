﻿using Dalamud.Interface.Windowing;

namespace RotationSolver.UI;

internal abstract class CtrlWindow(string name) : Window(name, BaseFlags)
{
    public const ImGuiWindowFlags BaseFlags = ImGuiWindowFlags.NoScrollbar
                        | ImGuiWindowFlags.NoCollapse
                        | ImGuiWindowFlags.NoTitleBar
                        | ImGuiWindowFlags.NoNav
                        | ImGuiWindowFlags.NoScrollWithMouse;

    public override void PreDraw()
    {
        Vector4 bgColor = Service.Config.IsControlWindowLock
            ? Service.Config.ControlWindowLockBg
            : Service.Config.ControlWindowUnlockBg;
        ImGui.PushStyleColor(ImGuiCol.WindowBg, bgColor);

        Flags = BaseFlags;
        if (Service.Config.IsControlWindowLock)
        {
            Flags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
        }

        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);

        try
        {
            base.PreDraw();
        }
        catch (Exception)
        {

        }
    }

    public override void PostDraw()
    {
        try
        {
            base.PostDraw();
        }
        catch (Exception)
        {

        }
        finally
        {
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
        }
    }
}