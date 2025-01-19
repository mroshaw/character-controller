namespace DaftAppleGames.TpCharacterController.Spawning
{
    public interface ISpawnable
    {
        public Spawner Spawner { get; set; }

        public abstract void PreSpawn();
        public abstract void Spawn();
        public abstract void Despawn();
    }
}