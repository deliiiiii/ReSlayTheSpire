using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace Violee;

[Serializable]
public class BingChaJi
{
    [ShowInInspector] public int SetCount => Math.Max(con, Find().con);
    BingChaJi f = null!;
    public BoxPointData PointData = null!;
    public HashSet<BingChaJi> ConnectedSet = null!;
    int con => ConnectedSet.Count;

    public void Init(BoxPointData pointData)
    {
        f = this;
        PointData = pointData;
        ConnectedSet = [this];
    }
    
    public void Merge(BingChaJi other)
    {
        var thisF = Find();
        var otherF = other.Find();
        if (thisF == otherF)
            return;
        ConnectedSet.AddRange(otherF.ConnectedSet);
        otherF.ConnectedSet = ConnectedSet;
        // MyDebug.LogWarning(
        //     $"CurPoint {pointData.BelongBox.Pos2D}.{pointData.Dir} OtherPoint {other.pointData.BelongBox.Pos2D}.{other.pointData.Dir}");
        // MyDebug.LogWarning($"Merge {thisF.pointData.BelongBox.Pos2D}.{thisF.pointData.Dir} {thisF.Con} , {otherF.pointData.BelongBox.Pos2D}.{otherF.pointData.Dir} {otherF.Con} {GetHashCode() > otherF.GetHashCode()}");
        if (thisF.GetHashCode() > otherF.GetHashCode())
        {
            otherF.f = thisF;
        }
        else
        {
            thisF.f = otherF;
        }
    }

    BingChaJi Find()
    {
        return f == this ? f : f = f.Find();
    }
}

