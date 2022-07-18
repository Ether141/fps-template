using UnityEngine;

public class Target : MonoBehaviour, IShotable
{
    [SerializeField] private int startHealth = 3;
    [SerializeField] private float deactiveStateTime = 5f;

    private int currentHealth;
    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            SetMeshColor();
        }
    }
    public bool IsAlive => CurrentHealth > 0;

    private Collider col;
    private Animator anim;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        CurrentHealth = startHealth;
        col = GetComponent<Collider>();
        anim = transform.parent.GetComponent<Animator>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Damage(int damage)
    {
        if (CurrentHealth - damage > 0)
        {
            CurrentHealth -= damage;
        }
        else
        {
            Die();
        }
    }

    public void Die()
    {
        CurrentHealth = 0;
        anim.SetBool("isActive", false);
        col.enabled = false;

        this.WaitAndDo(() =>
        {
            col.enabled = true;
            CurrentHealth = startHealth;
            anim.SetBool("isActive", true);
        }, deactiveStateTime);
    }

    private void SetMeshColor()
    {
        if (meshRenderer != null)
        {
            float val = 1f - (float)currentHealth / startHealth;
            meshRenderer.material.color = IsAlive ? new Color(1f, val, val, 1f) : Color.white;
        }
    }
}
