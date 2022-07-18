public interface IShotable
{
    void Damage(int damage);
    void Die();

    bool IsAlive { get; }
}