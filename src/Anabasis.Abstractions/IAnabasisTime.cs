namespace Anabasis.Platform.Abstractions;

public interface IAnabasisTime
{
    public void Update(double timeSinceLastUpdate);
    public void Render(double timeSinceLastRender);
}