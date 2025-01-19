namespace DaftAppleGames.TpCharacterController
{
    public interface INoiseMaker
    {
        public void MakeNoise(float noiseHeard);
        public float GetNoiseLevel();
    }
}