using UI;

public class PlayerScoreContainer : LabelContainer<PlayerScoreLabel, PlayerScoreContainer>
{
    public override PlayerScoreContainer GetInstance()
    {
        return this;
    }
}