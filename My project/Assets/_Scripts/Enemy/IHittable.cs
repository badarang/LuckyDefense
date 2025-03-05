public interface IHittable
{
    void TakeDamage(float damage, bool isAttackerRight);
    bool IsDead { get; }
}