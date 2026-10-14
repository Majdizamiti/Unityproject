using UnityEngine;
using UnityEngine.UI;   // CHANGED: for Image

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 5;
    [SerializeField] float invulnerableTime = 1f;
    [SerializeField] GameOverScreen gameOverScreen;
    [SerializeField] TMPro.TMP_Text healthText;       // optional
    [SerializeField] AudioSource hitSound;            // optional

    [SerializeField] Image healthFill;                // CHANGED: the HealthFill image
    [SerializeField] float fillSpeed = 3f;            // CHANGED: how fast the bar moves

    int currentHealth;
    float lastHitTime = -999f;
    bool dead;
    float targetFill = 1f;                            // CHANGED

    void Start()
{
    currentHealth = maxHealth;
    UpdateUI();
    if (healthFill != null) healthFill.fillAmount = 1f;

    if (hitSound != null)
    {
        hitSound.playOnAwake = false;   // CHANGED: never play by itself
        hitSound.Stop();                // CHANGED: cut it if it already started
    }
    
}
    void Update()   // CHANGED: smooth bar animation
    {
        if (healthFill == null) return;

        healthFill.fillAmount = Mathf.MoveTowards(
            healthFill.fillAmount, targetFill, fillSpeed * Time.unscaledDeltaTime);

        healthFill.color = Color.Lerp(Color.red, Color.green, healthFill.fillAmount);
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("TakeDamage at t=" + Time.time.ToString("F2") + " amount=" + amount +
          "\n" + System.Environment.StackTrace);
        Debug.Log("TakeDamage called, amount = " + amount + ", health before = " + currentHealth);
        if (dead || Time.time < lastHitTime + invulnerableTime) return;

        lastHitTime = Time.time;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (hitSound != null) hitSound.Play();
        UpdateUI();

        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)   // CHANGED: for health pickups, if you add some later
    {
        if (dead) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
    }

    void Die()
    {
        dead = true;

        var controls = GetComponent<PlayerControls>();
        if (controls != null) controls.enabled = false;

        if (gameOverScreen != null)
            gameOverScreen.Show();
        else
            Debug.LogError("PlayerHealth: Game Over Screen is not assigned!", this);
    }

    void UpdateUI()
    {
        targetFill = (float)currentHealth / maxHealth;   // CHANGED
        if (healthText != null) healthText.text = "HEALTH: " + currentHealth;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
{
    TrapDamage trap = hit.collider.GetComponentInParent<TrapDamage>();
    if (trap != null)
    {
        Debug.Log("Remy hit a TRAP: " + hit.collider.name);
        TakeDamage(trap.Damage);
    }
}

}