namespace Game.Domain.GameAggregate;

public class GameMoveConfig
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<int> Beats { get; set; }
}