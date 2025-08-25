namespace Violee;

public static class Mediator
{
    public static void Mediate()
    {
        GameState.PlayingState
            .OnUpdate(dt =>
            {
                var curPoint = PlayerMono.PlayerCurPoint.Value;
                MapManager.GetPlayerVisit(PlayerMono.GetPos(), ref curPoint);
                PlayerMono.PlayerCurPoint.Value = curPoint;
                if (!WindowManager.HasPaused)
                    MapManager.OnPlaying(dt);
                PlayerMono.TickOnPlaying(WindowManager.HasWindow);
            });
        
        MapManager.GenerateStream
            .Where(_ => GameState.IsTitle)
            .OnBegin(_ => GameState.EnterGeneratingMap());
        MapManager.DijkstraStream
            .OnEnd(param =>
            {
                PlayerMono.OnDijkstraEnd(BoxHelper.Pos2DTo3DPoint(param.StartPos, param.StartDir));
                MainItemMono.OnDijkstraEnd();
            });
        MapManager.DijkstraStream.Continue(_ => GameState.EnterPlaying());
    }
}