namespace Violee;

public static class Mediator
{
    public static void Mediate()
    {
        GameManager.PlayingState
            .OnUpdate(dt =>
            {
                var curPoint = PlayerMono.PlayerCurPoint.Value;
                MapManager.GetPlayerVisit(PlayerMono.GetPos(), ref curPoint);
                PlayerMono.PlayerCurPoint.Value = curPoint;
                if (!WindowManager.HasPaused)
                    MapManager.OnPlaying(dt);
                PlayerMono.TickOnPlaying(WindowManager.HasWindow);
            });
    }
}